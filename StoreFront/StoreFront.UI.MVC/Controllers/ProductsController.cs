using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using StoreFront.UI.MVC.Utilities;
using X.PagedList;

namespace StoreFront.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly BeautySupplyContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(BeautySupplyContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        [Authorize]
        [AllowAnonymous]
        
        public async Task<IActionResult> Index()
        {
            var beautySupplyContext = _context.Products.Include(p => p.Category).Include(p => p.ProductType).Include(p => p.ProductStatus);
            return View(await beautySupplyContext.ToListAsync());
        }

        //Write the AjaxDelete Action
        [AcceptVerbs("POST")]
        public JsonResult AjaxDelete(int id)
        {
            
            Product product = _context.Products.Find(id);
            _context.Products.Remove(product);
            _context.SaveChanges();

            string confirmMessage = $"Deleted the product {product.ProductName} from the database!";
            return Json(new { id = id, message = confirmMessage });
        }

        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> TiledProducts(string searchTerm, int categoryId, int page = 1)
        {
            int pageSize = 8;

            var products = _context.Products.Where(p => p.StatusId == 1)
                .Include(p => p.Category)
                .Include(p => p.OrderProducts).ToList();

            #region Optional Category Filter

            //Create a ViewData object to send a list of categories to the view
            //This is similar to what gets scaffolded in Products/Create()
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Category = 0;

            //At this point we need to add int categoryId as a parameter to the TiledProducts() Action
            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
                ViewBag.Category = categoryId;
            }
            #endregion

            #region Optional Search Filter

            if (!String.IsNullOrEmpty(searchTerm))
            {
                //In these scopes, ther *is* a search term
                products = products.Where(p =>
                p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                p.Category.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                p.ProductDescription.ToLower().Contains(searchTerm.ToLower())).ToList();

                //ViewBag for total # of results
                ViewBag.NbrResults = products.Count;
                //ViewBag to repeat the user's search term back to them
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                ViewBag.NbrResults = null;
                ViewBag.SearchTerm = null;
            }
            #endregion

            return View(products.ToPagedList(page, pageSize));
        }
        [Authorize]
        [AllowAnonymous]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductType)
                .Include(p => p.ProductStatus)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName");
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,CategoryId,SupplierId,ProductImage,OuncesPerUnit,MillilitersPerUnit,StatusId,Tool,ProductTypeId,Image")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Image != null)
                {
                    //Check the file type
                    //retrieve the extension of the uploaded file
                    string ext = Path.GetExtension(product.Image.FileName);

                    //Create a list of valid extensions to check against
                    string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    //Verify the uploaded file has an extension matching one of the extensions in the list above
                    //AND verify the file size will work within our .NET app.
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)//underscores make the int more readable
                    {
                        //Generate a unique filename
                        product.ProductImage = Guid.NewGuid() + ext;

                        //Save the file to the web server (our wwwroot/images folder)
                        //To access wwwroot, add a property to the controller for the _webHostEnvironment
                        //(see the top of this Controller for the example)
                        //Retrieve the path to wwwroot
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        //make a varaible for the full image path
                        string fullImagePath = webRootPath + "/assets/img/";

                        //Create a MemoryStream to read the image into the server memory
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);//transfer file from the POST action to server memory
                            using (var img = Image.FromStream(memoryStream))//relies on using statement for System.Drawing
                            {
                                //Now we send the Image to ImageUtility for resizing and thumbnail creation
                                //We need 5 parameters for the ImageUtility.Resize():
                                //1) (int) maximum image size
                                //2) (int) maximum thumbnail size
                                //3) (string) full path where the file will be saved
                                //4) (Image) the actual Image file
                                //5) (string) filename
                                int maxImageSize = 500;//size is set in pixels
                                int maxThumbSize = 100;

                                //Below relies on the using statement for our UI layer .Utilities (GadgetStore.UI.MVC.Utilities)
                                ImageUtility.ResizeImage(fullImagePath, product.ProductImage, img, maxImageSize, maxThumbSize);
                                //myFile.Save("path/to/folder/", "fileName"); - how we save a NON-image
                            }
                        }
                    }
                }
                else
                {
                    //If no image was uploaded, assign a default filename
                    //We also need to add a default image and name it "noimage.png" then copy it to our wwwroot/images folder
                    product.ProductImage = "noimage.png";
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status", product.StatusId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status", product.StatusId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,CategoryId,SupplierId,ProductImage,OuncesPerUnit,MillilitersPerUnit,StatusId,Tool,ProductTypeId,Image")] Product product)
        {
            string oldImageName = product.ProductImage;

            //Check if the user uploaded a file
            if (product.Image != null)
            {
                //get the file's extension
                string ext = Path.GetExtension(product.Image.FileName);

                //list valid extensions
                string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

                //check the file's extension against the list of valid extensions
                if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                {
                    //generate a new unique file name
                    product.ProductImage = Guid.NewGuid() + ext;
                    //put together our file path to save the image
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string fullPath = webRootPath + "/assets/img/";

                    //Delete the old image if one existed
                    if (oldImageName != "noimage.png")
                    {
                        ImageUtility.Delete(fullPath, oldImageName);
                    }

                    //Save the new image to our server (wwwroot/images/)
                    using (var memoryStream = new MemoryStream())
                    {
                        await product.Image.CopyToAsync(memoryStream);
                        using (var img = Image.FromStream(memoryStream))
                        {
                            int maxImageSize = 500;
                            int maxThumbSize = 100;
                            ImageUtility.ResizeImage(fullPath, product.ProductImage, img, maxImageSize, maxThumbSize);
                        }
                    }
                }
            }

            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            ViewData["StatusId"] = new SelectList(_context.ProductStatuses, "StatusId", "Status", product.StatusId);
            return View(product);
        }

        //// GET: Products/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Products == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.ProductType)
        //        .Include(p => p.ProductStatus)
        //        .FirstOrDefaultAsync(m => m.ProductId == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Products == null)
        //    {
        //        return Problem("Entity set 'BeautySupplyContext.Products'  is null.");
        //    }
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        _context.Products.Remove(product);
        //    }
            
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}

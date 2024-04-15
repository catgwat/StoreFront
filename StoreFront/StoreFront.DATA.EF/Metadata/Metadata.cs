using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StoreFront.DATA.EF.Models//Metadata
{
    //internal class Metadata
    //{
    //}

    public class CategoryMetadata 
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required!")]
        [StringLength(50)]
        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;
        
        [StringLength(500)]
        [Display(Name = "Description")]
        public string? CategoryDescription { get; set; }
    }


    public class OrderMetadata 
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; } = null!;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Display(Name = "Order Date")]
        [Required]
        public DateTime OrderDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Ship To")]
        [Required]
        public string ShipToName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "City")]
        [Required]
        public string ShipCity { get; set; } = null!;

        [StringLength(2)]
        [Display(Name = "State")]
        public string? ShipState { get; set; }

        [StringLength(5)]
        [Display(Name = "Zip")]
        [Required]
        [DataType(DataType.PostalCode)]
        public string? ShipZip { get; set; }
    }


    public class ProductMetadata 
    {
        public int ProductId { get; set; }

        [StringLength(200)]
        [Display(Name = "Product")]
        [Required]
        public string ProductName { get; set; } = null!;

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        [Display(Name = "Price")]
        [Range(0, (double)decimal.MaxValue)]
        [Required]
        public decimal ProductPrice { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        [UIHint("MultilineText")]
        [DataType(DataType.MultilineText)]
        public string? ProductDescription { get; set; }

        [Required]
        [Range(0, short.MaxValue)]
        [Display(Name = "In Stock")]
        public short UnitsInStock { get; set; }

        [Required]
        [Range(0, short.MaxValue)]
        [Display(Name = "On Order")]
        public short UnitsOnOrder { get; set; }

        [Required]
        public int CategoryId { get; set; }


        public int? SupplierId { get; set; }

        [StringLength(75)]
        [Display(Name = "Image")]
        public string? ProductImage { get; set; }

        [Range(0, short.MaxValue)]
        [Display(Name = "Ounces Per Unit")]
        public int? OuncesPerUnit { get; set; }

        [Range(0, short.MaxValue)]
        [Display(Name = "Milliliters Per Unit")]
        public int? MillilitersPerUnit { get; set; }

        
        public int StatusId { get; set; }

        [Display(Name ="Is this product a tool?")]
        public bool Tool { get; set; }

        public int ProductTypeId { get; set; }
    }


    public class CustomerDetailMetadata 
    {
        public string CustomerId { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [StringLength(150)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        [StringLength(5)]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }

        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }
    }

    public class ProductTypeMetadata
    {
        public int ProductTypeId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Product Type")]
        public string ProductTypeName { get; set; } = null!;

        [StringLength(500)]
        [Display(Name = "Type Description")]
        public string? ProductTypeDescription { get; set; }

    }

    public class ProductStatusMetadata
    {
        public int StatusId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = null!;
    }

    
}

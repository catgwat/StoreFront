using StoreFront.DATA.EF.Models;

namespace StoreFront.UI.MVC.Models
{
    public class CartItemViewModel
    {
        public int Qty { get; set; }

        public Product Product { get; set; }
        //Above is an example of a concept called "Containment"
        //This is a use of a complex datatype as a field/property in a Class.

        //Primitive datatype: Any Class that stores a single property/value (int, bool, char, decimal, string, etc.)
        //Complex datatype: Any Class with multiple properties/values (DateTime, Product, ContactViewModel, Console, etc.)

        //Build the constructor 
        public CartItemViewModel(int qty, Product product)
        {
            //Assignment
            Qty = qty;
            Product = product;
        }
    }
}

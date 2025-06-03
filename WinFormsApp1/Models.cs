using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }  
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Barcode { get; set; }

        public string ImagePath { get; set; } 
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CartItem
    {
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set; }
        public decimal Total => Product.Price * Quantity;
    }
}

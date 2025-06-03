/* 
 * using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    internal class ShoppingCart
    {

        private List<CartItem> _items = new List<CartItem>();
        private readonly ProductCatalog _catalog;

        public List<CartItem> Items => _items;

        public ShoppingCart(ProductCatalog catalog)
        {
            _catalog = catalog;
        }

        public void AddItem(int productId, int quantity)
        {
            var product = _catalog.GetProduct(productId);
            if (product == null)
                throw new Exception($"Product with ID {productId} not found");

            var existingItem = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem(product, quantity));
            }
        }

        public void RemoveItem(int productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    _items.Remove(item);
                else
                    item.Quantity = quantity;
            }
        }

        public double CalculateTotal()
        {
            return _items.Sum(item => item.Subtotal);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
*/

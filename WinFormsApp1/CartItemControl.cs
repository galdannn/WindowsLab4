using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class CartItemControl : UserControl
    {

        private CartItem _cartItem;

        // Events to communicate back to the parent form
        public event EventHandler<CartItem> PlusClicked;
        public event EventHandler<CartItem> MinusClicked;
        public event EventHandler<CartItem> DeleteClicked;
        public CartItemControl()
        {
            InitializeComponent();
        }
        public CartItem CartItem
        {
            get => _cartItem;
            set
            {
                _cartItem = value;
                UpdateDisplay();
            }
        }
        private void UpdateDisplay()
        {
            if (_cartItem != null)
            {
                nameLabel.Text = _cartItem.Product.Name;
                priceLabel.Text = $"${_cartItem.Product.Price:F2}";
                totalLabel.Text = $"${_cartItem.Total:F2}";
                qtyLabel.Text = _cartItem.Quantity.ToString();
            }
        }

        private void plusBtn_Click(object sender, EventArgs e)
        {
            PlusClicked?.Invoke(this, _cartItem);
        }

        private void minusBtn_Click(object sender, EventArgs e)
        {
            MinusClicked?.Invoke(this, _cartItem);
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            DeleteClicked?.Invoke(this, _cartItem);
        }
    }
}

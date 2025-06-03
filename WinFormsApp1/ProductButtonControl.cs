using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ProductButtonControl : UserControl
    {
        private Product _product;
        private PictureBox picProductImage;
        private Label lblProductName;
        private Label lblProductPrice;
        private Label lblProductStock;

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                UpdateDisplay();
            }
        }

        public event EventHandler<Product> ProductClicked;

        public ProductButtonControl()
        {
            
            InitializeCustomComponent();
        }

        private void InitializeCustomComponent()
        {
            this.SuspendLayout();

            // Set up the UserControl
            this.Size = new Size(180, 220);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(5);

            // Product Image
            picProductImage = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(160, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None,
                BackColor = Color.LightGray
            };

            // Product Name
            lblProductName = new Label
            {
                Location = new Point(10, 140),
                Size = new Size(160, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Product Name"
            };

            // Product Price
            lblProductPrice = new Label
            {
                Location = new Point(10, 170),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "$0.00"
            };

            // Product Stock
            lblProductStock = new Label
            {
                Location = new Point(10, 190),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Stock: 0"
            };

            // Add controls to the UserControl
            this.Controls.AddRange(new Control[] { picProductImage, lblProductName, lblProductPrice, lblProductStock });

            // Add click events to all controls
            this.Click += ProductButtonControl_Click;
            picProductImage.Click += ProductButtonControl_Click;
            lblProductName.Click += ProductButtonControl_Click;
            lblProductPrice.Click += ProductButtonControl_Click;
            lblProductStock.Click += ProductButtonControl_Click;

            // Add hover effects
            this.MouseEnter += ProductButtonControl_MouseEnter;
            this.MouseLeave += ProductButtonControl_MouseLeave;
            picProductImage.MouseEnter += ProductButtonControl_MouseEnter;
            picProductImage.MouseLeave += ProductButtonControl_MouseLeave;
            lblProductName.MouseEnter += ProductButtonControl_MouseEnter;
            lblProductName.MouseLeave += ProductButtonControl_MouseLeave;
            lblProductPrice.MouseEnter += ProductButtonControl_MouseEnter;
            lblProductPrice.MouseLeave += ProductButtonControl_MouseLeave;
            lblProductStock.MouseEnter += ProductButtonControl_MouseEnter;
            lblProductStock.MouseLeave += ProductButtonControl_MouseLeave;

            this.ResumeLayout(false);
        }

        private void UpdateDisplay()
        {
            if (_product == null) return;

            lblProductName.Text = _product.Name;
            lblProductPrice.Text = _product.Price.ToString("C");
            lblProductStock.Text = $"Stock: {_product.Quantity}";

            // Update stock color based on quantity
            if (_product.Quantity == 0)
            {
                lblProductStock.ForeColor = Color.Red;
                lblProductStock.Text = "Out of Stock";
            }
            else if (_product.Quantity < 10)
            {
                lblProductStock.ForeColor = Color.Orange;
            }
            else
            {
                lblProductStock.ForeColor = Color.Green;
            }

            // Load product image
            LoadProductImage();
        }

        private void LoadProductImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(_product.ImagePath) && File.Exists(_product.ImagePath))
                {
                    // Dispose of the previous image to avoid memory leaks
                    if (picProductImage.Image != null)
                    {
                        picProductImage.Image.Dispose();
                    }
                    picProductImage.Image = Image.FromFile(_product.ImagePath);
                }
                else
                {
                    // Use a default image or placeholder
                    if (picProductImage.Image != null)
                    {
                        picProductImage.Image.Dispose();
                    }
                    picProductImage.Image = CreateDefaultImage();
                }
            }
            catch (Exception)
            {
                // If there's an error loading the image, use default
                if (picProductImage.Image != null)
                {
                    picProductImage.Image.Dispose();
                }
                picProductImage.Image = CreateDefaultImage();
            }
        }

        private Image CreateDefaultImage()
        {
            // Create a simple default image
            Bitmap defaultImage = new Bitmap(160, 120);
            using (Graphics g = Graphics.FromImage(defaultImage))
            {
                g.Clear(Color.LightGray);
                g.DrawString("No Image", new Font("Segoe UI", 12), Brushes.Gray,
                    new RectangleF(0, 0, 160, 120),
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
            return defaultImage;
        }

        private void ProductButtonControl_Click(object sender, EventArgs e)
        {
            ProductClicked?.Invoke(this, _product);
        }

        private void ProductButtonControl_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void ProductButtonControl_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }

        
    }
}
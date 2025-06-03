using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System.Reflection;
using System.IO; // For Path and File operations
using System.Diagnostics; // For Process.Start
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Syncfusion.Pdf.Graphics;


namespace WinFormsApp1
{
    public partial class MainForm : Form
    {
        private User currentUser;
        private List<CartItem> cart = new List<CartItem>();
        private ContextMenuStrip userContextMenu;

        //private Product? selectedProduct;

        public MainForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            InitializeCustomComponent();
            InitializeUserMenu();
            LoadCart();
            LoadCategories();
            LoadProducts(); // Add this line to load products on startup
        }

        private void InitializeCustomComponent()
        {
            this.Size = new Size(1000, 700);
            this.Text = $"POS System - Welcome {currentUser.Username} ({currentUser.Role})";
            this.StartPosition = FormStartPosition.CenterScreen;
            lblTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            ConfigureMenuBasedOnRole();
        }

        private void InitializeUserMenu()
        {
            btnUser.Text = $"{currentUser.Username}";



            btnUser.ForeColor = Color.White;


            btnUser.Cursor = Cursors.Hand;
            btnUser.Click += BtnUser_Click;
            btnUser.TextAlign = ContentAlignment.MiddleCenter;


            // Create context menu based on user role
            CreateUserContextMenu();
        }



        private void CreateUserContextMenu()
        {
            userContextMenu = new ContextMenuStrip();
            userContextMenu.BackColor = Color.White;
            userContextMenu.Font = new Font("Segoe UI", 9);

            // Common menu items for all users
            ToolStripMenuItem userInfoItem = new ToolStripMenuItem();
            userInfoItem.Text = $"User: {currentUser.Username}";
            userInfoItem.Enabled = false;
            userInfoItem.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            userContextMenu.Items.Add(userInfoItem);

            ToolStripMenuItem roleItem = new ToolStripMenuItem();
            roleItem.Text = $"Role: {currentUser.Role}";
            roleItem.Enabled = false;
            roleItem.ForeColor = Color.Gray;
            userContextMenu.Items.Add(roleItem);

            userContextMenu.Items.Add(new ToolStripSeparator());

            // Role-specific menu items
            if (currentUser.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                // Manager-specific items
                ToolStripMenuItem addAccountItem = new ToolStripMenuItem();
                addAccountItem.Text = "Add Account";
                //addAccountItem.Image = Properties.Resources.add_user_icon; // Optional: add icon
                addAccountItem.Click += AddAccount_Click;
                userContextMenu.Items.Add(addAccountItem);

                ToolStripMenuItem updateEmployeeItem = new ToolStripMenuItem();
                updateEmployeeItem.Text = "Update Employee Info";
                //updateEmployeeItem.Image = Properties.Resources.edit_user_icon; // Optional: add icon
                updateEmployeeItem.Click += UpdateEmployee_Click;
                userContextMenu.Items.Add(updateEmployeeItem);

                userContextMenu.Items.Add(new ToolStripSeparator());
            }

            // Logout item (common for all users)
            ToolStripMenuItem logoutItem = new ToolStripMenuItem();
            logoutItem.Text = "Logout";
            //logoutItem.Image = Properties.Resources.logout_icon; // Optional: add icon
            logoutItem.ForeColor = Color.Red;
            logoutItem.Click += Logout_Click;
            userContextMenu.Items.Add(logoutItem);
        }

        private void BtnUser_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Point location = btn.PointToScreen(new Point(0, btn.Height));
            userContextMenu.Show(location);
        }

        private void AddAccount_Click(object sender, EventArgs e)
        {
            // Open Add Account dialog
            AddAccountForm addAccountForm = new AddAccountForm();
            if (addAccountForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Account added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateEmployee_Click(object sender, EventArgs e)
        {
            // Open Update Employee dialog
            UpdateEmployeeForm updateEmployeeForm = new UpdateEmployeeForm();
            if (updateEmployeeForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Employee information updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void TxtProductId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PerformSearch();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                // If search is empty, show all products
                LoadProducts();
                return;
            }

            // Search and display matching products
            var searchResults = SearchProducts(searchText);
            DisplayProducts(searchResults);
        }

        // Handle form resize to keep button in top right
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (btnUser != null)
            {
                btnUser.Location = new Point(this.Width - 140, 10);
            }
        }

        private void ConfigureMenuBasedOnRole()
        {
            // Hide manager-only menu items for non-managers
            if (currentUser.Role != "Manager")
            {
                productsToolStripMenuItem.Visible = false;
                categoriesStripMenuItem.Visible = false;
            }
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProductManagement();
        }

        private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCategoryManagement();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        // NEW METHOD: Load and display all products
        private void LoadProducts()
        {
            var products = GetAllProducts();
            DisplayProducts(products);
        }

        // NEW METHOD: Get all products from database
        private List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT p.*, c.Name as CategoryName 
                        FROM Products p 
                        LEFT JOIN Categories c ON p.CategoryId = c.Id 
                        ORDER BY p.Name";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                Quantity = reader.GetInt32("Quantity"),
                                CategoryId = reader.GetInt32("CategoryId"),
                                CategoryName = reader.IsDBNull("CategoryName") ? "" : reader.GetString("CategoryName"),
                                Barcode = reader.IsDBNull("Barcode") ? "" : reader.GetString("Barcode"),
                                ImagePath = reader.IsDBNull("ImagePath") ? "" : reader.GetString("ImagePath")
                            });
                        }
                    }
                }
            }
            return products;
        }

        // NEW METHOD: Display products in flowLayoutPanel2
        private void DisplayProducts(List<Product> products)
        {
            // Clear existing controls
            flowLayoutPanel2.Controls.Clear();

            foreach (var product in products)
            {
                var productButton = new ProductButtonControl();
                productButton.Product = product;

                // Handle product click
                productButton.ProductClicked += (sender, clickedProduct) =>
                {
                    if (clickedProduct != null)
                    {
                        AddProductToCart(clickedProduct);
                    }
                };

                flowLayoutPanel2.Controls.Add(productButton);
            }
        }

        private void AddProductToCart(Product productToAdd)
        {
            if (productToAdd == null) return;

            if (productToAdd.Quantity <= 0) // Check if product is out of stock before adding
            {
                MessageBox.Show($"'{productToAdd.Name}' is out of stock.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existingItem = cart.FirstOrDefault(c => c.Product.Id == productToAdd.Id);
            if (existingItem != null)
            {
                // Check if adding another one exceeds available stock
                if (existingItem.Quantity < productToAdd.Quantity)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    MessageBox.Show($"Cannot add more of '{productToAdd.Name}'. Stock limit reached in cart.", "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Stop if stock limit is reached
                }
            }
            else
            {
                // Product is not in cart, add as new item (quantity 1)
                // We already checked productToAdd.Quantity > 0 at the beginning of the method.
                cart.Add(new CartItem { Product = productToAdd, Quantity = 1 });
            }

            LoadCart(); // Refreshes cart display and updates total price
        }




        private void LoadCart()
        {
            RefreshCartDisplay();
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            decimal total = cart.Sum(c => c.Total);
            lblTotal.Text = $"Total: {total:C}";
        }

        private void BtnPay_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Cart is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = cart.Sum(c => c.Total);
            PaymentForm paymentForm = new PaymentForm(total);
            if (paymentForm.ShowDialog() == DialogResult.OK)
            {
                // Get payment details from PaymentForm
                decimal amountPaid = paymentForm.AmountPaid;
                decimal changeGiven = paymentForm.ChangeGiven;

                // Create a copy of the cart for the receipt before clearing it
                List<CartItem> receiptCartItems = new List<CartItem>(cart.Select(ci =>
                    new CartItem { Product = ci.Product, Quantity = ci.Quantity } // Create a shallow copy or deep if Product can change
                ));

                // Generate Receipt
                GenerateReceipt(receiptCartItems, total, amountPaid, changeGiven, currentUser);

                UpdateStock();
                cart.Clear();
                RefreshCartDisplay();
                UpdateTotalPrice();
                LoadProducts(); // Refresh product display to show updated stock
                MessageBox.Show("Payment completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateStock()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                foreach (var item in cart)
                {
                    // Ensure we don't try to reduce stock below zero if something went wrong with previous checks
                    // Though the logic in AddProductToCart should prevent quantity in cart exceeding stock
                    string query = "UPDATE Products SET Quantity = Quantity - @quantity WHERE Id = @id AND Quantity >= @quantity";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@quantity", item.Quantity);
                        command.Parameters.AddWithValue("@id", item.Product.Id);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            // This could happen if stock was updated by another process or a mismatch.
                            // Log this or handle as an error. For now, we'll assume it worked.
                            // You might want to re-query actual stock here.
                            MessageBox.Show($"Warning: Stock for {item.Product.Name} might not have been updated correctly or was insufficient.", "Stock Update Issue", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private Product? SearchProduct(string searchText)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                // First try to parse as ID (if it's a number)
                if (int.TryParse(searchText, out int productId))
                {
                    // Search by ID first
                    string idQuery = @"SELECT p.*, c.Name as CategoryName 
                              FROM Products p 
                              LEFT JOIN Categories c ON p.CategoryId = c.Id 
                              WHERE p.Id = @id";
                    using (var command = new SqliteCommand(idQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", productId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Product
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader.GetString("Name"),
                                    Price = reader.GetDecimal("Price"),
                                    Quantity = reader.GetInt32("Quantity"),
                                    CategoryId = reader.GetInt32("CategoryId"),
                                    CategoryName = reader.IsDBNull("CategoryName") ? "" : reader.GetString("CategoryName"),
                                    Barcode = reader.IsDBNull("Barcode") ? "" : reader.GetString("Barcode"),
                                    ImagePath = reader.IsDBNull("ImagePath") ? "" : reader.GetString("ImagePath")
                                };
                            }
                        }
                    }
                }

                // If not found by ID or not a number, search by name (case-insensitive)
                string nameQuery = @"SELECT p.*, c.Name as CategoryName 
                            FROM Products p 
                            LEFT JOIN Categories c ON p.CategoryId = c.Id 
                            WHERE LOWER(p.Name) LIKE LOWER(@name)";
                using (var command = new SqliteCommand(nameQuery, connection))
                {
                    command.Parameters.AddWithValue("@name", $"%{searchText}%");
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                Quantity = reader.GetInt32("Quantity"),
                                CategoryId = reader.GetInt32("CategoryId"),
                                CategoryName = reader.IsDBNull("CategoryName") ? "" : reader.GetString("CategoryName"),
                                Barcode = reader.IsDBNull("Barcode") ? "" : reader.GetString("Barcode"),
                                ImagePath = reader.IsDBNull("ImagePath") ? "" : reader.GetString("ImagePath")
                            };
                        }
                    }
                }
            }
            return null;
        }

        private List<Product> SearchProducts(string searchText)
        {
            List<Product> products = new List<Product>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT p.*, c.Name as CategoryName 
                        FROM Products p 
                        LEFT JOIN Categories c ON p.CategoryId = c.Id 
                        WHERE LOWER(p.Name) LIKE LOWER(@name) 
                        OR p.Id = @id
                        OR LOWER(p.Barcode) LIKE LOWER(@barcode)
                        ORDER BY p.Name";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", $"%{searchText}%");
                    command.Parameters.AddWithValue("@barcode", $"%{searchText}%");
                    // Try to parse as ID, if fails use -1 (which won't match any ID)
                    int.TryParse(searchText, out int productId);
                    command.Parameters.AddWithValue("@id", productId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                Quantity = reader.GetInt32("Quantity"),
                                CategoryId = reader.GetInt32("CategoryId"),
                                CategoryName = reader.IsDBNull("CategoryName") ? "" : reader.GetString("CategoryName"),
                                Barcode = reader.IsDBNull("Barcode") ? "" : reader.GetString("Barcode"),
                                ImagePath = reader.IsDBNull("ImagePath") ? "" : reader.GetString("ImagePath")
                            });
                        }
                    }
                }
            }
            return products;
        }

        private void OpenProductManagement()
        {
            ProductManagementForm form = new ProductManagementForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Refresh products when product management form is closed
                LoadProducts();
            }
        }

        private void OpenCategoryManagement()
        {
            CategoryManagementForm form = new CategoryManagementForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Refresh products when product management form is closed
                LoadCategories();
            }
        }

        private void ShowHelp()
        {
            string helpText = currentUser.Role == "Manager"
                ? "Manager Help:\n- Use Products menu to manage products\n- Use Categories menu to manage categories\n- Click on product buttons to select them\n- Search products using the search box\n- Add products to cart and process payments"
                : "Cashier Help:\n- Click on product buttons to select them\n- Search products using the search box\n- Add products to cart\n- Use +/- buttons to adjust quantities\n- Process payments using the Pay button";

            MessageBox.Show(helpText, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private void RefreshCartDisplay()
        {
            cartflowLayoutPanel.Controls.Clear();
            foreach (var item in cart)
                AddCartItemToUI(item);
        }

        private void AddCartItemToUI(CartItem item)
        {
            // Create your custom CartItemControl
            var cartControl = new CartItemControl();
            cartControl.CartItem = item;

            // Subscribe to the events from your CartItemControl
            cartControl.PlusClicked += (sender, cartItem) =>
            {
                // Check stock limit before adding
                if (cartItem.Quantity < cartItem.Product.Quantity)
                {
                    cartItem.Quantity++;
                    RefreshCartDisplay();
                    UpdateTotalPrice();
                }
                else
                {
                    MessageBox.Show("Cannot add more items. Stock limit reached.", "Stock Limit",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            cartControl.MinusClicked += (sender, cartItem) =>
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;

                }
                else
                {
                    // Remove item if quantity becomes 0
                    cart.Remove(cartItem);

                }
                RefreshCartDisplay();
                UpdateTotalPrice();
            };

            cartControl.DeleteClicked += (sender, cartItem) =>
            {
                // Remove item from cart
                cart.Remove(cartItem);
                RefreshCartDisplay();
                UpdateTotalPrice();
            };

            // Add the control to your FlowLayoutPanel
            cartflowLayoutPanel.Controls.Add(cartControl);
        }
        private List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id, Name FROM Categories ORDER BY Name";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name")
                            });
                        }
                    }
                }
            }
            return categories;
        }
        private void LoadCategories()
        {
            if (flowLayoutPanel3 == null)
            {
                MessageBox.Show("flowLayoutPanel3 for categories is not initialized. Please add it in the MainForm designer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            flowLayoutPanel3.SuspendLayout();
            flowLayoutPanel3.Controls.Clear();
            flowLayoutPanel3.WrapContents = true; // Or false, depending on desired layout

            // Add an "All Products" button
            var allCategoryButton = new CategoryButtonControl
            {
                Category = new Category { Id = 0, Name = "All Products" } // Special ID for "All"
            };
            allCategoryButton.CategoryClicked += CategoryButton_Clicked;
            flowLayoutPanel3.Controls.Add(allCategoryButton);

            List<Category> categories = GetAllCategories();
            foreach (var category in categories)
            {
                var categoryButton = new CategoryButtonControl
                {
                    Category = category
                };
                categoryButton.CategoryClicked += CategoryButton_Clicked;
                flowLayoutPanel3.Controls.Add(categoryButton);
            }
            flowLayoutPanel3.ResumeLayout(true);
        }

        private void CategoryButton_Clicked(object sender, Category category)
        {
            if (category.Id == 0) // "All Products" was clicked
            {
                LoadProducts(); // Load all products
            }
            else
            {
                LoadProductsByCategoryId(category.Id);
            }
            txtSearch.Clear(); // Optional: Clear search text when a category is selected
        }
        private void LoadProductsByCategoryId(int categoryId)
        {
            List<Product> products = GetProductsByCategoryId(categoryId);
            DisplayProducts(products);
        }

        private List<Product> GetProductsByCategoryId(int categoryId)
        {
            List<Product> products = new List<Product>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                // Ensure CategoryName is selected, similar to GetAllProducts
                string query = @"SELECT p.*, c.Name as CategoryName 
                                 FROM Products p 
                                 LEFT JOIN Categories c ON p.CategoryId = c.Id 
                                 WHERE p.CategoryId = @CategoryId 
                                 ORDER BY p.Name";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryId", categoryId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Price = reader.GetDecimal("Price"),
                                Quantity = reader.GetInt32("Quantity"),
                                CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "" : reader.GetString(reader.GetOrdinal("CategoryName")),
                                Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode")) ? "" : reader.GetString(reader.GetOrdinal("Barcode")),
                                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString(reader.GetOrdinal("ImagePath"))
                            });
                        }
                    }
                }
            }
            return products;
        }
        private void GenerateReceipt(List<CartItem> cartItems, decimal totalAmount, decimal amountPaid, decimal changeGiven, User cashier)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "POS Receipt";
            document.Info.Author = "Your POS System";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Define fonts using PdfFontFamily for default PDF fonts
            XFont fontTitle = new XFont("Helvetica", 18); // Bold for title
            XFont fontHeader = new XFont("Helvetica", 12); // Bold for headers
            XFont fontRegular = new XFont("Helvetica", 10);
            XFont fontSmall = new XFont("Helvetica", 8);

            double yPosition = 40; // Starting Y position
            double leftMargin = 40;
            double rightMargin = page.Width - 40;
            double lineHeight = 15;
            double itemIndent = leftMargin + 10;
            double priceIndent = rightMargin - 100;
            double qtyIndent = rightMargin - 150;
            double lineTotalIndent = rightMargin - 50;

            // --- Header ---
            gfx.DrawString("RECEIPT", fontTitle, XBrushes.Black, new XPoint(page.Width / 2, yPosition), XStringFormats.TopCenter);
            yPosition += lineHeight * 2;

            gfx.DrawString($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", fontRegular, XBrushes.Black, new XPoint(leftMargin, yPosition));
            yPosition += lineHeight;
            gfx.DrawString($"Cashier: {cashier.Username}", fontRegular, XBrushes.Black, new XPoint(leftMargin, yPosition));
            yPosition += lineHeight * 1.5;

            // --- Items Header ---
            gfx.DrawLine(XPens.Black, leftMargin, yPosition, rightMargin, yPosition); // Separator
            yPosition += 10;
            gfx.DrawString("Item", fontHeader, XBrushes.Black, new XPoint(itemIndent, yPosition));
            gfx.DrawString("Qty", fontHeader, XBrushes.Black, new XPoint(qtyIndent, yPosition));
            gfx.DrawString("Price", fontHeader, XBrushes.Black, new XPoint(priceIndent, yPosition));
            gfx.DrawString("Total", fontHeader, XBrushes.Black, new XPoint(lineTotalIndent, yPosition));
            yPosition += lineHeight + 5;
            gfx.DrawLine(XPens.Black, leftMargin, yPosition, rightMargin, yPosition); // Separator
            yPosition += lineHeight;

            // --- Cart Items ---
            foreach (var item in cartItems)
            {
                gfx.DrawString(item.Product.Name, fontRegular, XBrushes.Black, new XPoint(itemIndent, yPosition));
                gfx.DrawString(item.Quantity.ToString(), fontRegular, XBrushes.Black, new XPoint(qtyIndent, yPosition));
                gfx.DrawString($"{item.Product.Price:C}", fontRegular, XBrushes.Black, new XPoint(priceIndent, yPosition));
                gfx.DrawString($"{item.Total:C}", fontRegular, XBrushes.Black, new XPoint(lineTotalIndent, yPosition));
                yPosition += lineHeight;
            }
            yPosition += 5;
            gfx.DrawLine(XPens.Black, leftMargin, yPosition, rightMargin, yPosition); // Separator
            yPosition += lineHeight;

            // --- Totals ---
            // Note: XStringFormat is not strictly necessary for simple left-aligned strings,
            // but useful if you need precise alignment. For your current usage, it's fine.
            XStringFormat rightAlignFormat = new XStringFormat { Alignment = XStringAlignment.Far };

            gfx.DrawString("Total:", fontHeader, XBrushes.Black, new XPoint(priceIndent - 30, yPosition));
            gfx.DrawString($"{totalAmount:C}", fontHeader, XBrushes.Black, new XPoint(lineTotalIndent, yPosition));
            yPosition += lineHeight;

            gfx.DrawString("Amount Paid:", fontRegular, XBrushes.Black, new XPoint(priceIndent - 30, yPosition));
            gfx.DrawString($"{amountPaid:C}", fontRegular, XBrushes.Black, new XPoint(lineTotalIndent, yPosition));
            yPosition += lineHeight;

            gfx.DrawString("Change Given:", fontRegular, XBrushes.Black, new XPoint(priceIndent - 30, yPosition));
            gfx.DrawString($"{changeGiven:C}", fontRegular, XBrushes.Black, new XPoint(lineTotalIndent, yPosition));
            yPosition += lineHeight * 2;

            // --- Footer ---
            gfx.DrawString("Thank you for your purchase!", fontHeader, XBrushes.Black, new XPoint(page.Width / 2, yPosition), XStringFormats.TopCenter);
            yPosition += lineHeight;
            gfx.DrawString("Retail Supermarket", fontSmall, XBrushes.Gray, new XPoint(page.Width / 2, yPosition), XStringFormats.TopCenter);

            // --- Save the PDF ---
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Save Receipt",
                FileName = $"Receipt_{DateTime.Now:yyyyMMddHHmmss}.pdf", // Default filename
                DefaultExt = "pdf",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    document.Save(saveFileDialog.FileName);
                    MessageBox.Show($"Receipt saved to {saveFileDialog.FileName}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally, open the PDF after saving
                    DialogResult dr = MessageBox.Show("Do you want to open the receipt?", "Open Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving PDF: {ex.Message}", "PDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
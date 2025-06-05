// ProductManagementForm.cs - .NET 8.0 Version with Image Support
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using System.IO;

namespace WinFormsApp1
{
    public partial class ProductManagementForm : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtName, txtPrice, txtQuantity;
        private ComboBox cmbCategory;
        private Button btnAdd, btnUpdate, btnDelete, btnSelectImage;
        private PictureBox picProductImage;
        private Label lblImagePath;
        private List<Product> products = new List<Product>();
        private List<Category> categories = new List<Category>();
        private string selectedImagePath = "";

        public ProductManagementForm()
        {
            InitializeComponent();
            InitializeCustomComponent();
            LoadData();
        }

        private void InitializeCustomComponent()
        {
            this.Size = new Size(900, 700);
            this.Text = "Product Management";
            this.StartPosition = FormStartPosition.CenterParent;

            
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(850, 300),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            SetupProductGrid();
            dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;

            
            GroupBox gbInput = new GroupBox
            {
                Text = "Product Details",
                Location = new Point(20, 340),
                Size = new Size(850, 280)
            };

            Label lblName = new Label { Text = "Name:", Location = new Point(20, 30), Size = new Size(60, 23) };
            txtName = new TextBox { Location = new Point(90, 30), Size = new Size(200, 23) };

            Label lblPrice = new Label { Text = "Price:", Location = new Point(20, 60), Size = new Size(60, 23) };
            txtPrice = new TextBox { Location = new Point(90, 60), Size = new Size(100, 23) };

            Label lblQuantity = new Label { Text = "Quantity:", Location = new Point(20, 90), Size = new Size(60, 23) };
            txtQuantity = new TextBox { Location = new Point(90, 90), Size = new Size(100, 23) };

            Label lblCategory = new Label { Text = "Category:", Location = new Point(20, 120), Size = new Size(60, 23) };
            cmbCategory = new ComboBox { Location = new Point(90, 120), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };

           
            Label lblImage = new Label { Text = "Image:", Location = new Point(20, 150), Size = new Size(60, 23) };
            btnSelectImage = new Button { Text = "Select Image", Location = new Point(90, 150), Size = new Size(100, 30), BackColor = Color.LightYellow };
            btnSelectImage.Click += BtnSelectImage_Click;

            lblImagePath = new Label
            {
                Text = "No image selected",
                Location = new Point(200, 155),
                Size = new Size(200, 20),
                ForeColor = Color.Gray
            };

            
            picProductImage = new PictureBox
            {
                Location = new Point(450, 30),
                Size = new Size(150, 150),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnAdd = new Button { Text = "Add", Location = new Point(90, 190), Size = new Size(80, 30), BackColor = Color.LightGreen };
            btnUpdate = new Button { Text = "Update", Location = new Point(180, 190), Size = new Size(80, 30), BackColor = Color.LightBlue, Enabled = false };
            btnDelete = new Button { Text = "Delete", Location = new Point(270, 190), Size = new Size(80, 30), BackColor = Color.LightCoral, Enabled = false };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            gbInput.Controls.AddRange(new Control[] {
                lblName, txtName, lblPrice, txtPrice, lblQuantity, txtQuantity,
                lblCategory, cmbCategory, lblImage, btnSelectImage, lblImagePath,
                picProductImage, btnAdd, btnUpdate, btnDelete
            });

            this.Controls.AddRange(new Control[] { dgvProducts, gbInput });
        }

        private void SetupProductGrid()
        {
            dgvProducts.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, DataPropertyName = "Id" },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", Width = 200, DataPropertyName = "Name" },
                new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Price", Width = 100, DataPropertyName = "Price" },
                new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Quantity", Width = 100, DataPropertyName = "Quantity" },
                new DataGridViewTextBoxColumn { Name = "CategoryName", HeaderText = "Category", Width = 150, DataPropertyName = "CategoryName" },
                new DataGridViewTextBoxColumn { Name = "ImagePath", HeaderText = "Image", Width = 200, DataPropertyName = "ImagePath" }
            });
        }

        private void LoadData()
        {
            LoadProducts();
            LoadCategories();
        }

        private void LoadProducts()
        {
            products.Clear();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = @"SELECT p.*, c.Name as CategoryName 
                               FROM Products p 
                               LEFT JOIN Categories c ON p.CategoryId = c.Id";
                using (var command = new SqliteCommand(query, connection))
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
                            ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath")
                        });
                    }
                }
            }
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = products;
        }

        private void LoadCategories()
        {
            categories.Clear();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Categories";
                using (var command = new SqliteCommand(query, connection))
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

            cmbCategory.DataSource = null;
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select Product Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;
                    lblImagePath.Text = Path.GetFileName(selectedImagePath);
                    lblImagePath.ForeColor = Color.Black;

                    
                    try
                    {
                        picProductImage.Image = Image.FromFile(selectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        selectedImagePath = "";
                        lblImagePath.Text = "No image selected";
                        lblImagePath.ForeColor = Color.Gray;
                        picProductImage.Image = null;
                    }
                }
            }
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                txtName.Text = selectedProduct.Name;
                txtPrice.Text = selectedProduct.Price.ToString("F2");
                txtQuantity.Text = selectedProduct.Quantity.ToString();
                cmbCategory.SelectedValue = selectedProduct.CategoryId;

                
                if (!string.IsNullOrEmpty(selectedProduct.ImagePath) && File.Exists(selectedProduct.ImagePath))
                {
                    try
                    {
                        picProductImage.Image = Image.FromFile(selectedProduct.ImagePath);
                        lblImagePath.Text = Path.GetFileName(selectedProduct.ImagePath);
                        lblImagePath.ForeColor = Color.Black;
                        selectedImagePath = selectedProduct.ImagePath;
                    }
                    catch
                    {
                        picProductImage.Image = null;
                        lblImagePath.Text = "Image not found";
                        lblImagePath.ForeColor = Color.Red;
                        selectedImagePath = "";
                    }
                }
                else
                {
                    picProductImage.Image = null;
                    lblImagePath.Text = "No image";
                    lblImagePath.ForeColor = Color.Gray;
                    selectedImagePath = "";
                }

                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                ClearInputs();
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Products (Name, Price, Quantity, CategoryId) VALUES (@name, @price, @quantity, @categoryId)";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        command.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                        command.Parameters.AddWithValue("@quantity", int.Parse(txtQuantity.Text));
                        command.Parameters.AddWithValue("@categoryId", cmbCategory.SelectedValue);

                        command.ExecuteNonQuery();

                        
                        var getIdCommand = new SqliteCommand("SELECT last_insert_rowid()", connection);
                        long productId = (long)getIdCommand.ExecuteScalar();

                        
                        string savedImagePath = null;
                        if (!string.IsNullOrEmpty(selectedImagePath))
                        {
                            savedImagePath = DatabaseHelper.SaveProductImage(selectedImagePath, (int)productId);
                        }

                        
                        if (savedImagePath != null)
                        {
                            string updateQuery = "UPDATE Products SET ImagePath = @imagePath WHERE Id = @id";
                            using (var updateCommand = new SqliteCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@imagePath", savedImagePath);
                                updateCommand.Parameters.AddWithValue("@id", productId);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                LoadProducts();
                ClearInputs();
                MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; 
                
            }
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0 && ValidateInputs())
            {
                var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();

                    
                    string savedImagePath = selectedProduct.ImagePath; 
                    if (!string.IsNullOrEmpty(selectedImagePath) && selectedImagePath != selectedProduct.ImagePath)
                    {
                        // Delete old image
                        DatabaseHelper.DeleteProductImage(selectedProduct.ImagePath);
                        // Save new image
                        savedImagePath = DatabaseHelper.SaveProductImage(selectedImagePath, selectedProduct.Id);
                    }

                    string query = "UPDATE Products SET Name = @name, Price = @price, Quantity = @quantity, CategoryId = @categoryId, ImagePath = @imagePath WHERE Id = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        command.Parameters.AddWithValue("@price", decimal.Parse(txtPrice.Text));
                        command.Parameters.AddWithValue("@quantity", int.Parse(txtQuantity.Text));
                        command.Parameters.AddWithValue("@categoryId", cmbCategory.SelectedValue);
                        command.Parameters.AddWithValue("@imagePath", savedImagePath);
                        command.Parameters.AddWithValue("@id", selectedProduct.Id);
                        command.ExecuteNonQuery();
                    }
                }
                LoadProducts();
                ClearInputs();
                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; 

            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var selectedProduct = (Product)dgvProducts.SelectedRows[0].DataBoundItem;
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedProduct.Name}'?",
                                           "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using (var connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string query = "DELETE FROM Products WHERE Id = @id";
                        using (var command = new SqliteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", selectedProduct.Id);
                            command.ExecuteNonQuery();
                        }
                    }

                    
                    DatabaseHelper.DeleteProductImage(selectedProduct.ImagePath);

                    LoadProducts();
                    ClearInputs();
                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; 

                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a product name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid quantity (0 or greater).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return false;
            }

            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            cmbCategory.SelectedIndex = -1;
            selectedImagePath = "";
            lblImagePath.Text = "No image selected";
            lblImagePath.ForeColor = Color.Gray;
            picProductImage.Image = null;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
           
            if (picProductImage.Image != null)
            {
                picProductImage.Image.Dispose();
            }
            base.OnFormClosed(e);
        }
    }
}
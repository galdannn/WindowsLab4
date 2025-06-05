// CategoryManagementForm.cs - .NET 8.0 Version
using System;
using System.Collections.Generic;

using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using WinFormsApp1;

namespace WinFormsApp1
{
    public partial class CategoryManagementForm : Form
    {
        private DataGridView dgvCategories;
        private TextBox txtCategoryName;
        private Button btnAdd, btnUpdate, btnDelete;
        private List<Category> categories = new List<Category>();

        public CategoryManagementForm()
        {
            InitializeComponent();
            InitializeCustomComponent();
            LoadCategories();
        }

        private void InitializeCustomComponent()
        {
            this.Size = new Size(600, 500);
            this.Text = "Category Management";
            this.StartPosition = FormStartPosition.CenterParent;

            
            dgvCategories = new DataGridView
            {
                Location = new Point(20, 20),
                Size = new Size(550, 300),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvCategories.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 80, DataPropertyName = "Id" },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Category Name", Width = 400, DataPropertyName = "Name" }
            });

            dgvCategories.SelectionChanged += DgvCategories_SelectionChanged;

            
            GroupBox gbInput = new GroupBox
            {
                Text = "Category Details",
                Location = new Point(20, 340),
                Size = new Size(550, 120)
            };

            Label lblName = new Label
            {
                Text = "Category Name:",
                Location = new Point(20, 30),
                Size = new Size(100, 23)
            };

            txtCategoryName = new TextBox
            {
                Location = new Point(130, 30),
                Size = new Size(200, 23)
            };

            btnAdd = new Button
            {
                Text = "Add",
                Location = new Point(350, 30),
                Size = new Size(80, 30),
                BackColor = Color.LightGreen
            };
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = new Button
            {
                Text = "Update",
                Location = new Point(350, 70),
                Size = new Size(80, 30),
                BackColor = Color.LightBlue,
                Enabled = false
            };
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(440, 70),
                Size = new Size(80, 30),
                BackColor = Color.LightCoral,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;

            gbInput.Controls.AddRange(new Control[] { lblName, txtCategoryName, btnAdd, btnUpdate, btnDelete });

            this.Controls.AddRange(new Control[] { dgvCategories, gbInput });
        }

        private void LoadCategories()
        {
            categories.Clear();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Categories ORDER BY Name";
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
            dgvCategories.DataSource = null;
            dgvCategories.DataSource = categories;
        }

        private void DgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
                txtCategoryName.Text = selectedCategory.Name;

                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                txtCategoryName.Clear();
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Categories (Name) VALUES (@name)";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", txtCategoryName.Text.Trim());
                        try
                        {
                            command.ExecuteNonQuery();
                            LoadCategories();
                            txtCategoryName.Clear();
                            MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (SqliteException ex)
                        {
                            
                            if (ex.SqliteExtendedErrorCode == 2067 || 
                                ex.Message.Contains("UNIQUE constraint failed"))
                            {
                                MessageBox.Show("Category name already exists! Please choose a different name.",
                                               "Duplicate Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show($"Error adding category: {ex.Message}",
                                               "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0 && ValidateInput())
            {
                var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE Categories SET Name = @name WHERE Id = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", txtCategoryName.Text.Trim());
                        command.Parameters.AddWithValue("@id", selectedCategory.Id);
                        try
                        {
                            command.ExecuteNonQuery();
                            LoadCategories();
                            txtCategoryName.Clear();
                            MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (SqliteException ex)
                        {
                            
                            if (ex.SqliteExtendedErrorCode == 2067 || // SQLITE_CONSTRAINT_UNIQUE
                                ex.Message.Contains("UNIQUE constraint failed"))
                            {
                                MessageBox.Show("Category name already exists! Please choose a different name.",
                                               "Duplicate Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show($"Error updating category: {ex.Message}",
                                               "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;

                
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE CategoryId = @id";
                    using (var command = new SqliteCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", selectedCategory.Id);
                        long productCount = (long)command.ExecuteScalar()!;

                        if (productCount > 0)
                        {
                            MessageBox.Show($"Cannot delete category '{selectedCategory.Name}' because it is being used by {productCount} product(s).\n\nPlease remove or reassign the products first.",
                                          "Cannot Delete Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                var result = MessageBox.Show($"Are you sure you want to delete the category '{selectedCategory.Name}'?\n\nThis action cannot be undone.",
                                           "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using (var connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string query = "DELETE FROM Categories WHERE Id = @id";
                        using (var command = new SqliteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", selectedCategory.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                    LoadCategories();
                    txtCategoryName.Clear();
                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Please enter a category name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return false;
            }

            if (txtCategoryName.Text.Trim().Length < 2)
            {
                MessageBox.Show("Category name must be at least 2 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return false;
            }

            return true;
        }
    }
}
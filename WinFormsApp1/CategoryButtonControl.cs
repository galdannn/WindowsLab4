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
    public partial class CategoryButtonControl : UserControl
    {
        private Category _category;
        private Label lblCategoryName;

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                UpdateDisplay();
            }
        }

        public event EventHandler<Category> CategoryClicked;

        public CategoryButtonControl()
        {
            InitializeCustomComponent();
        }

        private void InitializeCustomComponent()
        {
            this.SuspendLayout();

            // Set up the UserControl
            this.Size = new Size(150, 40); // You can adjust this size
            this.BackColor = Color.LightSteelBlue; // A distinct color for category buttons
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(5);

            // Category Name Label
            lblCategoryName = new Label
            {
                Location = new Point(5, 5), // Padding within the button
                Size = new Size(this.Width - 10, this.Height - 10), // Fill most of the button
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Category Name",
                AutoSize = false // Important for TextAlign to work correctly with specified Size
            };
            // Alternatively, to make the label fill the control:
            // lblCategoryName.Dock = DockStyle.Fill;


            // Add controls to the UserControl
            this.Controls.Add(lblCategoryName);

            // Add click events to the UserControl and its Label
            this.Click += OnCategoryButtonClick;
            lblCategoryName.Click += OnCategoryButtonClick; // Propagate click from label

            // Add hover effects
            this.MouseEnter += CategoryButtonControl_MouseEnter;
            this.MouseLeave += CategoryButtonControl_MouseLeave;
            lblCategoryName.MouseEnter += CategoryButtonControl_MouseEnter; // Ensure label hover affects parent
            lblCategoryName.MouseLeave += CategoryButtonControl_MouseLeave;

            this.ResumeLayout(false);
        }

        private void UpdateDisplay()
        {
            if (_category != null)
            {
                lblCategoryName.Text = _category.Name;
            }
            else
            {
                lblCategoryName.Text = "N/A";
            }
        }

        private void OnCategoryButtonClick(object sender, EventArgs e)
        {
            // Raise the event, passing the associated Category object
            CategoryClicked?.Invoke(this, _category);
        }

        private void CategoryButtonControl_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.CornflowerBlue; // Darker shade on hover
            lblCategoryName.ForeColor = Color.White;
        }

        private void CategoryButtonControl_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.LightSteelBlue; // Original color
            lblCategoryName.ForeColor = Color.Black;
        }
    }
}

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

            
            this.Size = new Size(150, 40); 
            this.BackColor = Color.LightSteelBlue; 
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(5);

            
            lblCategoryName = new Label
            {
                Location = new Point(5, 5), 
                Size = new Size(this.Width - 10, this.Height - 10), 
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Category Name",
                AutoSize = false 
            };
           


            
            this.Controls.Add(lblCategoryName);

            
            this.Click += OnCategoryButtonClick;
            lblCategoryName.Click += OnCategoryButtonClick; 

            
            this.MouseEnter += CategoryButtonControl_MouseEnter;
            this.MouseLeave += CategoryButtonControl_MouseLeave;
            lblCategoryName.MouseEnter += CategoryButtonControl_MouseEnter; 
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
            
            CategoryClicked?.Invoke(this, _category);
        }

        private void CategoryButtonControl_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.CornflowerBlue; 
            lblCategoryName.ForeColor = Color.White;
        }

        private void CategoryButtonControl_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.LightSteelBlue; 
            lblCategoryName.ForeColor = Color.Black;
        }
    }
}

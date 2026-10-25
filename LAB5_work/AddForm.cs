using System;
using System.Windows.Forms;

namespace LAB5_work
{
    // Requirement 4: separate form for adding products.
    // existingProduct = false -> new product (all fields)
    // existingProduct = true  -> only Id + quantity to add to an existing product
    public partial class AddForm : Form
    {
        private bool existingProduct;

        // results read by Form1 after ShowDialog() returns OK
        public Product NewProduct { get; private set; }
        public int ProductId { get { return (int)numId.Value; } }
        public int Quantity { get { return (int)numQuantity.Value; } }

        //addForm constructor receives a boolean indicating whether the user wants to add a new product or add quantity to an existing product
        public AddForm(bool existingProduct)
        {
            InitializeComponent();
            this.existingProduct = existingProduct;
            // check if product exists in the database if existingProduct is true, and adjust the form accordingly
            if (existingProduct)
            {
                Text = "Add quantity to existing product";
                // hide the fields of a new product, keep only Quantity + Id
                lblName.Visible = txtName.Visible = false;
                lblDescription.Visible = txtDescription.Visible = false;
                lblEntryDate.Visible = dtpEntryDate.Visible = false;
                lblExpiration.Visible = dtpExpiration.Visible = false;
                btnOK.Top = btnCancel.Top = 80;
                ClientSize = new System.Drawing.Size(ClientSize.Width, 115);
            }
            else
            {
                Text = "New product";
                lblId.Visible = numId.Visible = false;
                dtpExpiration.Value = DateTime.Today.AddMonths(1);
            }
        }

        //ok button click event handler, validates the input and creates a new Product object if necessary
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!existingProduct)
            {
                // simple validation before creating the object
                if (txtName.Text.Trim() == "")
                {
                    MessageBox.Show("The name is required.");
                    return;
                }
                if (dtpExpiration.Value.Date < dtpEntryDate.Value.Date)
                {
                    MessageBox.Show("The expiration date can't be before the entry date.");
                    return;
                }

                NewProduct = new Product
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    EntryDate = dtpEntryDate.Value.Date,
                    ExpirationDate = dtpExpiration.Value.Date,
                    Quantity = Quantity
                };
            }

            DialogResult = DialogResult.OK; // closes the dialog
        }
    }
}

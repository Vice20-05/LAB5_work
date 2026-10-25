using System;
using System.Windows.Forms;

namespace LAB5_work
{
    // Requirement 5: separate form for selling, opened by double click on a grid row
    public partial class SellForm : Form
    {
        // how many pieces the user wants to sell (read by Form1 after OK)
        public int SoldQuantity { get { return (int)numSell.Value; } }

        // the constructor receives the details of the selected record
        public SellForm(Product product)
        {
            InitializeComponent();

            lblDetails.Text =
                "Id: " + product.Id + "\n" +
                "Name: " + product.Name + "\n" +
                "Description: " + product.Description + "\n" +
                "Entry date: " + product.EntryDate.ToShortDateString() + "\n" +
                "Expiration date: " + product.ExpirationDate.ToShortDateString() + "\n" +
                "Available: " + product.Quantity;

            // can't sell more than what is available
            numSell.Minimum = 1;
            numSell.Maximum = Math.Max(1, product.Quantity);
            numSell.Value = 1;
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}

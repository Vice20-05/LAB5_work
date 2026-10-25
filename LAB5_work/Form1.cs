using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAB5_work
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // load all products when the window opens
        private void Form1_Load(object sender, EventArgs e)
        {
            RunDbTask(db => "Products loaded");
        }

        // Requirement 6: every database operation runs on the Thread Pool (Task.Run),
        // so the window does not freeze (the first connection to LocalDB is slow).
        // When it is done, Invoke() sends the result back to the GUI thread.
        private void RunDbTask(Func<ProductDbContext, string> dbWork)
        {
            string filter = txtSearch.Text.Trim(); // read the UI here, before leaving the GUI thread
            lblStatus.Text = "Working...";

            Task.Run(() =>
            {
                string message;
                List<Product> products = null;
                try
                {
                    using (ProductDbContext db = new ProductDbContext())
                    {
                        message = dbWork(db);           // the actual operation (add / sell / nothing)
                        products = LoadProducts(db, filter); // then reload the grid content
                    }
                }
                catch (Exception ex)
                {
                    message = "Error: " + ex.Message;
                }

                // back to the GUI thread with the result
                if (!IsDisposed)
                    Invoke((Action)(() => ShowResult(products, message)));
            });
        }

        // Requirement 3: search = filtering what the grid shows (by name or description), with LINQ
        private List<Product> LoadProducts(ProductDbContext db, string filter)
        {
            var query = from p in db.Products
                        where filter == "" || p.Name.Contains(filter) || p.Description.Contains(filter)
                        orderby p.Id
                        select p;
            return query.ToList();
        }

        // runs on the GUI thread (called through Invoke)
        private void ShowResult(List<Product> products, string message)
        {
            if (products != null)
                dataGridView1.DataSource = products;
            lblStatus.Text = message;
        }

        // Requirement 2: Refresh button, clears the search and shows everything
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RunDbTask(db => "Refreshed");
        }

        // Menu: Search products -> applies the text from the search box as a filter
        private void searchProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDbTask(db => "Search: \"" + txtSearch.Text + "\"");
        }

        // ENTER in the search box does the same as the menu item
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                searchProductsToolStripMenuItem_Click(sender, e);
            }
        }

        // Requirement 4: Add products -> New product
        private void newProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddForm form = new AddForm(false);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Product p = form.NewProduct;
            RunDbTask(db =>
            {
                db.Products.Add(p);
                db.SaveChanges();
                return "Added: " + p.Name;
            });
        }

        // Requirement 4: Add products -> Existing product (the user types the Id seen in the grid)
        private void existingProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddForm form = new AddForm(true);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            int id = form.ProductId;
            int quantity = form.Quantity;
            RunDbTask(db =>
            {
                Product p = db.Products.Find(id);
                if (p == null)
                    return "No product with Id " + id;

                p.Quantity += quantity;
                db.SaveChanges();
                return p.Name + " quantity is now " + p.Quantity;
            });
        }

        // Requirement 5: double click on a row opens the sell form for that product
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return; // header was clicked

            Product selected = (Product)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            SellProduct(selected);
        }

        // Menu: Sell products -> sells the currently selected row
        private void sellProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Select a product in the table first (or double click it).");
                return;
            }
            SellProduct((Product)dataGridView1.CurrentRow.DataBoundItem);
        }

        private void SellProduct(Product selected)
        {
            SellForm form = new SellForm(selected);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            int id = selected.Id;
            int sold = form.SoldQuantity;
            RunDbTask(db =>
            {
                Product p = db.Products.Find(id);
                if (p == null)
                    return "Product no longer exists";
                if (sold > p.Quantity)
                    return "Only " + p.Quantity + " left";

                p.Quantity -= sold;
                string message = "Sold " + sold + " x " + p.Name;

                // quantity reached 0 -> delete the product from the database
                if (p.Quantity == 0)
                {
                    db.Products.Remove(p);
                    message += " (sold out, deleted)";
                }
                db.SaveChanges();
                return message;
            });
        }
    }
}

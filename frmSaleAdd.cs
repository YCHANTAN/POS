// Make sure to add this to your using directives if not already there
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_System.Model
{
    public partial class frmSaleAdd : Sample
    {
        public frmSaleAdd()
        {
            InitializeComponent();
        }

        public int id = 0;
        public int cusID = 0;

        private void label6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSaleAdd_Load(object sender, EventArgs e)
        {
            string qry = @"SELECT cusID 'id', cusName 'name' FROM Customer";
            MainClass.CBFill(qry, cbCustomer);

            if (cusID > 0)
            {
                cbCustomer.SelectedValue = cusID;
                LoadForEdit();
                GrandTotal();
            }

            LoadProductsFromDatabase();
        }

        public void AddItems(string id, string name, string priceStr, Image pimage, string cost)
        {
            var w = new ucProduct()
            {
                PName = name,
                Price = priceStr,
                PImage = pimage,
                PCost = cost,
                id = Convert.ToInt32(id),
            };

            flowLayoutPanel1.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;
                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    if (Convert.ToInt32(item.Cells["dgvproid"].Value) == wdg.id)
                    {
                        int qty = Convert.ToInt32(item.Cells["dgvqty"].Value) + 1;
                        int unitPrice = Convert.ToInt32(wdg.Price);
                        item.Cells["dgvqty"].Value = qty;
                        item.Cells["dgvAmount"].Value = qty * unitPrice;
                        GrandTotal();
                        return;
                    }
                }

                guna2DataGridView1.Rows.Add(new object[] { 0, wdg.id, wdg.PName, 1, wdg.Price, 1 * Convert.ToInt32(wdg.Price), wdg.PCost });
                GrandTotal();
            };
        }

        // ✅ NEW: SubTotal method
        private double SubTotal()
        {
            double subtotal = 0;
            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            {
                subtotal += Convert.ToDouble(item.Cells["dgvAmount"].Value);
            }

            subTotal.Text = subtotal.ToString("N2"); // Add lblSubTotal to the form
            return subtotal;
        }

        // ✅ NEW: Discount rule
        private int GetDiscount(double subtotal)
        {
            if (subtotal >= 500)
                return 20;
            else if (subtotal >= 200)
                return 15;
            else if (subtotal >= 100)
                return 10;
            else
                return 0;
        }

        // ✅ UPDATED: GrandTotal method with discount
        private void GrandTotal()
        {
            double subtotal = SubTotal(); // raw total
            int discountPercent = GetDiscount(subtotal);
            double discountAmount = subtotal * discountPercent / 100.0;
            double grandTotal = subtotal - discountAmount;

            lblDiscount.Text = discountPercent + "%"; // Add lblDiscount to the form
            lblTotal.Text = grandTotal.ToString("N2"); // This is now the GRAND TOTAL
        }

        private void LoadProductsFromDatabase()
        {
            string qry = "SELECT * FROM Product";
            SqlCommand cmd = new SqlCommand(qry, MainClass.GetConnection());
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    byte[] imageArray = (byte[])row["PImage"];
                    AddItems(row["proID"].ToString(), row["pName"].ToString(), row["pPrice"].ToString(),
                        Image.FromStream(new MemoryStream(imageArray)), row["pCost"].ToString());
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            guna2DataGridView1.Rows.Clear();
            txtDate.Value = DateTime.Now;
            cbCustomer.SelectedIndex = -1;
            subTotal.Text = "0.00";
            lblDiscount.Text = "0%";
            lblTotal.Text = "0.00";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in flowLayoutPanel1.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.Trim().ToLower());
            }
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string qry = "SELECT * FROM Product WHERE pBarcode LIKE '" + txtBarcode.Text + "'";
                using (SqlConnection con = MainClass.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(qry, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                        {
                            if (Convert.ToInt32(item.Cells["dgvproid"].Value) == int.Parse(row["proID"].ToString()))
                            {
                                int qty = Convert.ToInt32(item.Cells["dgvqty"].Value) + 1;
                                int unitPrice = Convert.ToInt32(row["pPrice"]);
                                item.Cells["dgvqty"].Value = qty;
                                item.Cells["dgvAmount"].Value = qty * unitPrice;
                                GrandTotal();
                                txtBarcode.Text = "";
                                return;
                            }
                        }

                        guna2DataGridView1.Rows.Add(new object[]
                        {
                            0,
                            row["proID"].ToString(),
                            row["pName"].ToString(),
                            1,
                            row["pPrice"].ToString(),
                            Convert.ToInt32(row["pPrice"]),
                            row["pCost"].ToString()
                        });
                        txtBarcode.Text = "";
                        GrandTotal();
                    }
                }
            }
        }

        private int TryParseCell(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                return 1;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!MainClass.Validation(this))
            {
                MessageBox.Show("Remove error occurred. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string qry1 = id == 0
                ? "INSERT INTO tblMian VALUES (@date, @type, @supID); SELECT SCOPE_IDENTITY();"
                : "UPDATE tblMian SET mdate = @date, myType = @type, mSupCusID = @supID WHERE MainID = @id;";

            using (SqlConnection con = MainClass.GetConnection())
            {
                SqlCommand cmd1 = new SqlCommand(qry1, con);
                cmd1.Parameters.AddWithValue("@id", id);
                cmd1.Parameters.AddWithValue("@date", txtDate.Value.Date);
                cmd1.Parameters.AddWithValue("@type", "SAL");
                cmd1.Parameters.AddWithValue("@supID", Convert.ToInt32(cbCustomer.SelectedValue));

                if (con.State == ConnectionState.Closed)
                    con.Open();

                if (id == 0)
                    id = Convert.ToInt32(cmd1.ExecuteScalar());
                else
                    cmd1.ExecuteNonQuery();

                int record = 0;
                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    int did = TryParseCell(row.Cells["dgvid"].Value);
                    int proID = TryParseCell(row.Cells["dgvproid"].Value);
                    int qty = TryParseCell(row.Cells["dgvqty"].Value);
                    int unitPrice = TryParseCell(row.Cells["dgvPrice"].Value);
                    int cost = TryParseCell(row.Cells["dgvCost"].Value);
                    int amount = TryParseCell(row.Cells["dgvAmount"].Value);

                    string qry2 = did == 0
                        ? "INSERT INTO tblDetails VALUES (@mID, @proID, @qty, @price, @amount, @cost);"
                        : "UPDATE tblDetails SET dMainID = @mID, productID = @proID, qty = @qty, price = @price, amount = @amount, cost = @cost WHERE detailID = @id;";

                    SqlCommand cmd2 = new SqlCommand(qry2, con);
                    cmd2.Parameters.AddWithValue("@id", did);
                    cmd2.Parameters.AddWithValue("@mID", id);
                    cmd2.Parameters.AddWithValue("@proID", proID);
                    cmd2.Parameters.AddWithValue("@qty", qty);
                    cmd2.Parameters.AddWithValue("@price", unitPrice);
                    cmd2.Parameters.AddWithValue("@amount", amount);
                    cmd2.Parameters.AddWithValue("@cost", cost);

                    record += cmd2.ExecuteNonQuery();
                }

                if (record > 0)
                {
                    MessageBox.Show("Data saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    id = 0;
                    cusID = 0;
                    txtDate.Value = DateTime.Now;
                    cbCustomer.SelectedIndex = -1;
                    guna2DataGridView1.Rows.Clear();
                    subTotal.Text = "0.00";
                    lblDiscount.Text = "0%";
                    lblTotal.Text = "0.00";
                }
            }
        }

        private void LoadForEdit()
        {
            string qry = "SELECT * FROM tblDetails INNER JOIN Product ON proID = productID WHERE dMainID = " + id;
            using (SqlConnection con = MainClass.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    guna2DataGridView1.Rows.Add(
                        row["detailID"].ToString(),
                        row["proID"].ToString(),
                        row["pName"].ToString(),
                        row["qty"].ToString(),
                        row["price"].ToString(),
                        row["amount"].ToString(),
                        row["cost"].ToString()
                    );
                }
            }

            GrandTotal();
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int rowIndex = guna2DataGridView1.CurrentCell.RowIndex;
                    int delID = Convert.ToInt32(guna2DataGridView1.Rows[rowIndex].Cells["dgvid"].Value);

                    guna2DataGridView1.Rows.RemoveAt(rowIndex);

                    string qry1 = "DELETE FROM tblDetails WHERE detailID = " + delID;
                    Hashtable ht = new Hashtable();
                    MainClass.SQl(qry1, ht);

                    GrandTotal();
                }
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}

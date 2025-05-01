using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace POS_System.Model
{
    public partial class frmPurchaseAdd : SampleAdd
    {
        public frmPurchaseAdd()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        public int mainID = 0;
        public int supID = 0;

        private void frmPurchaseAdd_Load(object sender, EventArgs e)
        {
            cbProduct.SelectedIndexChanged -= new EventHandler(cbProduct_SelectedIndexChanged);
            //Stop before product load from database 
            string qry = "SELECT proID 'id' , pName 'name' FROM Product";
            string qry2 = "SELECT supID 'id' , supName 'name' FROM Supplier";

            MainClass.CBFill(qry, cbProduct);
            MainClass.CBFill(qry2, cbSupplier);

            if (supID > 0)
            {
                cbSupplier.SelectedValue = supID;
                LoadForEdit();
            }
            txtBarcode.Focus();

            //Re enable it
            //at load we need to stop product selection change event
            cbProduct.SelectedIndexChanged -= new EventHandler(cbProduct_SelectedIndexChanged); // Unsubscribe from the event
        }

        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProduct.SelectedIndex != 1)
            {
                txtQty.Text = "";
                GetDetails();
            }
        }

        private void GetDetails()
        {
            string qry = "SELECT * FROM Product WHERE proID = " + Convert.ToInt32(cbProduct.SelectedValue) + "";
            using (SqlConnection con = MainClass.GetConnection()) // Use MainClass.GetConnection() to get the connection
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    txtCost.Text = dt.Rows[0]["pCost"].ToString();
                    Calculate();
                }
            }
        }

        private void Calculate()
        {
            double qty = 0;
            double cost = 0;
            double amt = 0;

            double.TryParse(txtQty.Text, out qty);
            double.TryParse(txtCost.Text, out cost);

            amt = qty * cost;
            txtAmount.Text = amt.ToString();
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string qry = "SELECT * FROM Product WHERE pBarcode LIKE '" + txtBarcode.Text + "";
                using (SqlConnection con = MainClass.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(qry, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        cbProduct.SelectedValue = Convert.ToInt32(dt.Rows[0]["proID"].ToString());
                        Calculate();
                        txtBarcode.Text = "";
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string pid;
            string pname;
            string qty;
            string cost;
            string amt;

            pname = cbProduct.Text;
            pid = cbProduct.SelectedValue.ToString();
            qty = txtQty.Text;
            cost = txtCost.Text;
            amt = txtAmount.Text;

            //0 for serial and id 
            guna2DataGridView1.Rows.Add(0, pid, pname, qty, cost, amt);
            cbProduct.SelectedIndex = 0;
            cbProduct.SelectedIndex = -1;
            txtQty.Text = "";
            txtCost.Text = "";
            txtAmount.Text = "";

        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int count = 0;
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }
        private int TryParseCell(object value)
        {
            if (value == null || value == DBNull.Value) return 0;

            int result;
            if (int.TryParse(value.ToString(), out result))
                return result;

            return 0; // default fallback if not parsable
        }


        public override void btnSave_Click(object sender, EventArgs e)
        {
            if (MainClass.Validation(this) == false)
            {
                MessageBox.Show("Remove error occurred. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string qry1 = ""; // for main table
            string qry2 = ""; // for details table
            int record = 0;

            if (mainID == 0)
            {
                qry1 = "INSERT INTO tblMian VALUES (@date, @type, @supID); SELECT SCOPE_IDENTITY();";
            }
            else
            {
                qry1 = "UPDATE tblMian SET mdate = @date, myType = @type, mSupCusID = @supID WHERE MainID = @id;";
            }

            using (SqlConnection con = MainClass.GetConnection()) // Use MainClass.GetConnection() to get the connection
            {
                SqlCommand cmd1 = new SqlCommand(qry1, con);
                cmd1.Parameters.AddWithValue("@id", mainID);
                cmd1.Parameters.AddWithValue("@date", Convert.ToDateTime(txtDate.Value).Date);
                cmd1.Parameters.AddWithValue("@type", "PUR");
                cmd1.Parameters.AddWithValue("@supID", Convert.ToInt32(cbSupplier.SelectedValue));
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                if (mainID == 0)
                {
                    mainID = Convert.ToInt32(cmd1.ExecuteScalar());
                }
                else
                {
                    cmd1.ExecuteNonQuery();
                }

                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    int did = TryParseCell(row.Cells["dgvid"].Value);
                    int proID = TryParseCell(row.Cells["dgvproid"].Value);
                    int qty = TryParseCell(row.Cells["dgvqty"].Value);
                    int cost = TryParseCell(row.Cells["dgvCost"].Value);
                    int amount = TryParseCell(row.Cells["dgvAmount"].Value);

                    if (did == 0)
                    {
                        qry2 = "INSERT INTO tblDetails VALUES (@mID, @proID, @qty, @price, @amount, @cost);";
                    }
                    else
                    {
                        qry2 = "UPDATE tblDetails SET dMainID = @mID, productID = @proID, qty = @qty, price = @price, amount = @amount, cost = @cost WHERE detailID = @id;";
                    }

                    SqlCommand cmd2 = new SqlCommand(qry2, con);
                    cmd2.Parameters.AddWithValue("@id", did);
                    cmd2.Parameters.AddWithValue("@mID", mainID);
                    cmd2.Parameters.AddWithValue("@proID", proID);
                    cmd2.Parameters.AddWithValue("@qty", qty);
                    cmd2.Parameters.AddWithValue("@price", cost);
                    cmd2.Parameters.AddWithValue("@amount", amount);
                    cmd2.Parameters.AddWithValue("@cost", cost);

                    record += cmd2.ExecuteNonQuery();
                }

                if (record > 0)
                {
                    MessageBox.Show("Data saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    mainID = 0;
                    supID = 0;
                    txtDate.Value = DateTime.Now;
                    cbSupplier.SelectedIndex = 0;
                    cbSupplier.SelectedIndex = -1;
                    guna2DataGridView1.Rows.Clear();
                }
            }
        }
        private void LoadForEdit()
        {
            string qry = "SELECT * FROM tblDetails inner join product on proID = productID WHERE dMainID = " + mainID + " ";
            using (SqlConnection con = MainClass.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(qry, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string did;
                    string pid;
                    string pname;
                    string qty;
                    string cost;
                    string amt;

                    did = row["detailID"].ToString();
                    pname = row["pName"].ToString();
                    pid = row["productID"].ToString();
                    qty = row["qty"].ToString();
                    cost = row["price"].ToString();
                    amt = row["amount"].ToString();

                    //0 for serial and id 
                    guna2DataGridView1.Rows.Add(0, did, pid, pname, qty, cost, amt);
                }
            }
        }
    }
}
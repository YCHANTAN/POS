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
    public partial class frmProductAdd : SampleAdd
    {
        public frmProductAdd()
        {
            InitializeComponent();
        }

        public int id = 0;
        public int catID = 0;
        public string filePath = "";
        Byte[] imageByteArray;

        private void frmProductAdd_Load(object sender, EventArgs e)
        {
            string qry = "SELECT catID 'id' , catName 'name' FROM Category";
            MainClass.CBFill(qry, cbCategory);

            if (id > 0)
            {
                cbCategory.SelectedValue = catID;
                LoadImage();
            }
        }

        public override void btnSave_Click(object sender, EventArgs e)
        {
            if (MainClass.Validation(this) == false)
            {
                MessageBox.Show("Validation Failed");
                return;
            }

            string qry = "";
            if (id == 0) // Insert
            {
                qry = @"INSERT INTO Product (pName, pCatID, pBarcode, pCost, pPrice, pQty, pImage) 
                        VALUES (@name, @pCatID, @barcode, @cost, @saleprice, @qty, @image)";
            }
            else // Update
            {
                qry = @"UPDATE Product SET 
                        pName = @name,
                        pCatID = @pCatID,
                        pBarcode = @barcode,
                        pCost = @cost,
                        pPrice = @saleprice,
                        pQty = @qty,
                        pImage = @image
                        WHERE proID = @id";
            }

            Image temp = new Bitmap(txtPic.Image);
            MemoryStream ms = new MemoryStream();
            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            imageByteArray = ms.ToArray();

            Hashtable ht = new Hashtable();
            ht.Add("@id", id);
            ht.Add("@name", txtName.Text);
            ht.Add("@pCatID", Convert.ToInt32(cbCategory.SelectedValue));
            ht.Add("@barcode", txtBarcode.Text);
            ht.Add("@cost", Convert.ToDouble(txtCost.Text));
            ht.Add("@saleprice", Convert.ToDouble(txtSalePrice.Text)); // Ensure you have a sale price textbox
            ht.Add("@qty", Convert.ToInt32(txtSalePrice.Text)); // 👈 Quantity added
            ht.Add("@image", imageByteArray);

            if (MainClass.SQl(qry, ht) > 0)
            {
                MessageBox.Show("Product saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
        }

        private void ClearForm()
        {
            id = 0;
            txtName.Text = "";
            txtBarcode.Text = "";
            cbCategory.SelectedIndex = -1;
            txtCost.Text = "";
            txtSalePrice.Text = ""; // Ensure it exists
            txtSalePrice.Text = "";
            txtPic.Image = POS_System.Properties.Resources.user;
            txtName.Focus();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images(.jpg, .png)|*.png; *.jpg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                txtPic.Image = new Bitmap(filePath);
            }
        }

        private void LoadImage()
        {
            string qry = @"SELECT pImage FROM Product WHERE proID = " + id;
            using (SqlConnection connection = new SqlConnection(MainClass.con_string))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(qry, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Byte[] imageArray = (byte[])dt.Rows[0]["pImage"];
                    txtPic.Image = Image.FromStream(new MemoryStream(imageArray));
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            // Placeholder event
        }
    }
}

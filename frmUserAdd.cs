using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Reflection;


namespace POS_System.Model
{
    public partial class frmUserAdd : SampleAdd
    {
        public frmUserAdd()
        {
            InitializeComponent();
        }

        public int id = 0;

        public override void btnSave_Click(object sender, EventArgs e)
        {
            // Before save, we need validation
            if (MainClass.Validation(this) == false)
            {
                MessageBox.Show("Validation Failed");
                return;
            }
            else
            {
                string qry = "";
                if (id == 0) // Insert
                {
                    qry = @"INSERT INTO USERS VALUES(@name, @username, @pass, @phone, @image)";
                }
                else // Update
                {
                    qry = @"UPDATE USERS SET uName = @name,
                                          uUsername = @username,
                                          uPass = @pass,
                                          uPhone = @phone,
                                          uImage = @image
                                          WHERE userID = @id";
                }


                Image temp = new Bitmap(txtPic.Image);
                MemoryStream ms = new MemoryStream();
                temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageByteArray = ms.ToArray();

                Hashtable ht = new Hashtable();
                ht.Add("@id", id);
                ht.Add("@name", txtName.Text);
                ht.Add("@username", txtUser.Text);
                ht.Add("@pass", txtPass.Text);
                ht.Add("@phone", txtPhone.Text);
                ht.Add("@image", imageByteArray);

                if (MainClass.SQl(qry, ht) > 0)
                {
                    MessageBox.Show("User Added Successfully");
                    id = 0;
                    txtName.Text = "";
                    txtUser.Text = "";
                    txtPass.Text = "";
                    txtPhone.Text = "";
                    txtPic.Image = POS_System.Properties.Resources.user;
                    txtName.Focus();
                }
            }
        }

        public string filePath = "";
        Byte[] imageByteArray;

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images(.jpg, .png)|*.png; *jpg "; 

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                txtPic.Image = new Bitmap(filePath);
            }
        }

        private static SqlConnection con = new SqlConnection(MainClass.con_string);

        private void LoadImage()
        {
            string qry = @"SELECT uImage FROM users WHERE userID = " + id + "";
            using (SqlConnection connection = new SqlConnection(MainClass.con_string))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(qry, connection);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Byte[] imageArray = (byte[])dt.Rows[0]["uImage"];
                    byte[] imageByteArray = imageArray;
                    txtPic.Image = Image.FromStream(new MemoryStream(imageArray));
                }
            }
        }

        private void frmUserAdd_Load(object sender, EventArgs e)
        {
            if (id > 0)
            {
                LoadImage();
            }
        }
    }
}

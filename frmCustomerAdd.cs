using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_System.Model
{
    public partial class frmCustomerAdd : SampleAdd
    {
        public frmCustomerAdd()
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
                    qry = @"INSERT INTO Customer VALUES(@name, @phone, @Email)";
                }
                else // Update
                {
                    qry = @"UPDATE Customer SET cusName = @name,
                                          cusPhone = @phone,
                                          cusEmail = @Email
                                          WHERE cusID = @id";
                }

                Hashtable ht = new Hashtable();
                ht.Add("@id", id);
                ht.Add("@name", txtName.Text);
                ht.Add("@phone", txtPhone.Text);
                ht.Add("@Email", txtEmail.Text);

                if (MainClass.SQl(qry, ht) > 0)
                {
                    MessageBox.Show("User Added Successfully");
                    id = 0;
                    txtName.Text = "";
                    txtPhone.Text = "";
                    txtEmail.Text = "";
                    txtName.Focus();
                }
            }
        }
    }
}

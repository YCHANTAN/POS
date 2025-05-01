using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS_System.Model;

namespace POS_System.View
{
    public partial class frmSupplierView : SampleView
    {
        public frmSupplierView()
        {
            InitializeComponent();
        }

        private void frmSupplierView_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public override void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmSupplierAdd());
            LoadData();
        }

        public override void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvname);
            lb.Items.Add(dgvPhone);
            lb.Items.Add(dgvemail);

            string qry = @"SELECT *
                           FROM Supplier 
                           WHERE supName LIKE '%" + txtSearch.Text.Trim() + @"%' 
                           ORDER BY supID DESC";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Update
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
            {
                frmSupplierAdd frm = new frmSupplierAdd();
                frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvname"].Value);
                frm.txtPhone.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvPhone"].Value);
                frm.txtEmail.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvemail"].Value);

                MainClass.BlurBackground(frm);
                LoadData();
            }

            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvDel")
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this user?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                    string qry = "DELETE FROM Supplier WHERE supID = " + id + "";
                    Hashtable ht = new Hashtable();

                    if (MainClass.SQl(qry, ht) > 0)
                    {
                        MessageBox.Show("User Deleted Successfully");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Error Deleting User");
                    }
                }
            }
        }
    }
}

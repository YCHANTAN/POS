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
    public partial class frmProductView : SampleView
    {
        public frmProductView()
        {
            InitializeComponent();
        }
        public override void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmProductAdd());
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
            lb.Items.Add(dgvcatID);
            lb.Items.Add(dgvCategory);
            lb.Items.Add(dgvbarcode);
            lb.Items.Add(dgvCost);
            lb.Items.Add(dgvSale);

            string qry = @"SELECT proID, pName, pCatID, catName, pBarcode, pCost, pPrice 
                           FROM Product INNER JOIN category on catID = pCatID 
                           WHERE pName LIKE '%" + txtSearch.Text.Trim() + @"%' 
                           ORDER BY proID DESC";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Update
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
            {
                frmProductAdd frm = new frmProductAdd();
                frm.id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.catID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvcatID"].Value); 
                frm.txtName.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvname"].Value);
                frm.txtBarcode.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvbarcode"].Value);
                frm.txtCost.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvCost"].Value);
                frm.txtSalePrice.Text = Convert.ToString(guna2DataGridView1.CurrentRow.Cells["dgvSale"].Value);

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
                    string qry = "DELETE FROM Product WHERE proID = " + id + "";
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

        private void frmProductView_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}

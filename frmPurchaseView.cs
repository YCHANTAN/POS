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
    public partial class frmPurchaseView : SampleView
    {
        public frmPurchaseView()
        {
            InitializeComponent();
        }

        private void frmPurchaseView_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public override void btnAdd_Click(object sender, EventArgs e)
        {
            MainClass.BlurBackground(new frmSaleAdd());
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
            lb.Items.Add(dgvdate);
            lb.Items.Add(dgvSupID);
            lb.Items.Add(dgvSupplier);
            lb.Items.Add(dgvAmount);

            string qry = @"SELECT dMainID, mdate, m.mSupCusID, c.cusName, SUM(d.amount) FROM tblMian m
                           inner join tblDetails d on d.dMainID = m.MainID
                           inner join Customer c on c.cusID = m.mSupCusID
                           WHERE m.myType = 'SAL' and cusName like '%" + txtSearch.Text + "%' GROUP BY dMainID, mdate, m.mSupCusID, c.cusName";

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Update
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvEdit")
            {
                frmPurchaseAdd frm = new frmPurchaseAdd();
                frm.mainID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                frm.supID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvSupID"].Value);

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
                    string qry = "DELETE FROM tblMian WHERE MainID = " + id + "";
                    string qry2 = "DELETE FROM tblDetails WHERE dMainID = " + id + "";

                    Hashtable ht = new Hashtable();
                    if (MainClass.SQl(qry, ht) > 0)
                    {
                        if (MainClass.SQl(qry2, ht) > 0)
                        {
                            MessageBox.Show("User Deleted Successfully");
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Error Deleting User");
                        }
                    }
                    if (MainClass.SQl(qry2, ht) > 0)
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

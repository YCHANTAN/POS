using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS_System.View;

namespace POS_System
{
    public partial class frmMain : Sample
    {
        static frmMain _obj;
        public static frmMain Instance
        {
            get
            {
                if (_obj == null)
                {
                    _obj = new frmMain();
                }
                return _obj;
            }
        }
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _obj = this;

            lblUser.Text = MainClass.USER;
            pictureBox1.Image = MainClass.IMG;

            btnHome.PerformClick();

        }

        public void AddControls(Form F)
        {
            this.CenterPanel.Controls.Clear();
            F.Dock = DockStyle.Fill;
            F.TopLevel = false;
            CenterPanel.Controls.Add(F);
            F.Show();
        }
        private void label4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CenterPanel_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnHome_Click_Click(object sender, EventArgs e)
        {
            AddControls(new frmDashboard());
        }

        private void btnProducts_Click_Click(object sender, EventArgs e)
        {
            AddControls(new frmProductView());
        }

        private void btnUsers_Click_Click(object sender, EventArgs e)
        {
            AddControls(new frmUserView());
        }

        private void btnSales_Click_Click(object sender, EventArgs e)
        {
            AddControls(new frmSaleView());
        }

        private void btnCustomers_Click_Click(object sender, EventArgs e)
        {
            AddControls(new frmCustomerView());
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            AddControls(new frmCategoryView());
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            AddControls(new frmSupplierView());
        }

        private void btnPurchases_Click(object sender, EventArgs e)
        {
            AddControls(new frmPurchaseView());
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                    "Are you sure you want to exit application?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

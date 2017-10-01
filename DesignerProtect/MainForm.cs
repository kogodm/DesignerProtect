using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PhotoShopBackUpC;

namespace DesignerProtect
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Form1 f;
        private void MainForm_Load(object sender, EventArgs e)
        {
            f = new Form1(); //创建一个新窗体
            this.Size = f.Size;
            this.FormBorderStyle = FormBorderStyle.None;
            f.FormBorderStyle = FormBorderStyle.None;

//            this.WindowState = FormWindowState.Maximized;
//            f.WindowState = FormWindowState.Maximized;

            f.ShowInTaskbar = false;
            f.StartPosition = FormStartPosition.CenterParent;
            f.parent = this;
            f.TopLevel = true;
            f.Show(this);
            f.Location = Location;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                Location = mousePos;
                f.Location = mousePos;
            }
        }
        private Point mouse_offset;
    }
}

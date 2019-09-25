using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication2.Presenters;
using WindowsFormsApplication2.Views;

namespace WindowsFormsApplication2
{
    public partial class MainForm : Form
    {
        IPresenter startChild = new IPresenter(null);
        ConfigurationManager source = ConfigurationManager.Source;
        
        public MainForm()
        {
            InitializeComponent();
            вводСигналаToolStripMenuItem.Click += new System.EventHandler(toolStripButton1_Click);
            выводСигналаToolStripMenuItem.Click += new System.EventHandler(toolStripButton2_Click);
            настройкиToolStripMenuItem.Click += new System.EventHandler(toolStripButton3_Click);
        }
        protected override void OnClosed(EventArgs e)
        {
            Application.Exit();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnClosed(e);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            startChild.childForm(this, 1);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            startChild.childForm(this, 2);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            startChild.childForm(this, 3);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
        }
    }
}

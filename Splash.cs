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
    public partial class Splash : Form, IView
    {
        public Splash()
        {
            InitializeComponent();
        }
        string IView.pass
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPresenter present = new IPresenter(this);
            present.Transfer(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

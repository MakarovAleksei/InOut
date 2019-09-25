using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication2.Models;
using WindowsFormsApplication2.Views;

namespace WindowsFormsApplication2.Presenters
{
    public class IPresenter
    {
        IView interfeic;

        public IPresenter(IView view)
        {
            interfeic = view;
        }

        public void Transfer(Form splash)
        {
            IModel trans = new IModel();
            trans.passText = interfeic.pass;
            MainForm main = new MainForm();
            if (trans.passText == "123")
            {
                splash.Hide();
                main.ShowDialog();
            }
            /*
            Settings F3 = new Settings();
            F3.Show();
            F3.Hide();
             */ 
        }


        public void childForm(Form parent, int switcher)
        {
            if (Application.OpenForms["InputForm"] == null && switcher == 1)
            {
                InputForm F1 = new InputForm();
                F1.MdiParent = parent;
                F1.Show();
            }
            if (Application.OpenForms["OutputForm"] == null && switcher == 2)
            {
                OutputForm F2 = new OutputForm();
                F2.MdiParent = parent;
                F2.Show(); 
            }
            if (Application.OpenForms["Settings"] == null && switcher == 3)
            {
                Settings F3 = new Settings();
                F3.ShowDialog();
            }
        }
    }
}

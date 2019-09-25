using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace WindowsFormsApplication2
{
    public partial class Settings : Form
    {
        WaveOutput.WavePlayer wavePlayer = new WaveOutput.WavePlayer();

        ConfigurationManager source = ConfigurationManager.Source;

        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            string[] outlist = wavePlayer.GetOutDevicesList();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Default device");
            foreach (var dev in outlist)
            {
                comboBox1.Items.Add(dev);
            }
            string[] inlist = wavePlayer.GetInDevicesList();
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Default device");
            foreach (var dev in inlist)
            {
                comboBox2.Items.Add(dev);
            }
            comboBox1.SelectedIndex = source.OutputDevice;
            comboBox2.SelectedIndex = source.InputDevice;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            source.OutputDevice = comboBox1.SelectedIndex;
            source.InputDevice = comboBox2.SelectedIndex;

            comboBox1.SelectedIndex = source.OutputDevice;
            comboBox2.SelectedIndex = source.InputDevice;

            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

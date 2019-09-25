using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Timers;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace WindowsFormsApplication2
{
    public partial class OutputForm : Form
    {
        WaveFileWriter write;
        WaveOutput.WavePlayer wavePlayer = new WaveOutput.WavePlayer();
        ConfigurationManager source = ConfigurationManager.Source;
//конструктор формы
        public OutputForm()
        {
            InitializeComponent();
            wavePlayer.PlaybackStopped += Stopped;
        }
//при закрытии формы        
        protected override void OnClosed(EventArgs e)
        {
            if (openFileDialog1.FileName != null)
            {
                openFileDialog1.FileName = "";
                wavePlayer.Close();
            }

        }
        delegate void NoArg();
      
        public int N_Output { get; set; }
        private bool dont_do_event = false;


//обработчик события "окончание воспроизведения"       
        private void Stopped() //обработчик события "окончание воспроизведения"
        {
            Invoke((NoArg)delegate()  //другой поток создавал этот объект
            {
                toolStripStatusLabel1.Text = "Остановлено";
                toolStripStatusLabel1.BackColor = Color.Yellow;
                timer1.Stop();
                toolStripButton5.Enabled = true;
                открытьФайлToolStripMenuItem.Enabled = true;
                toolStripButton2.Enabled = true;
                стартToolStripMenuItem.Enabled = true;
               
            });
        }
//?
        private void OutputForm_Shown(object sender, EventArgs e)
        {
            menuStrip1.Visible = false;
        }

//нажатие кнопки Стоп
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            wavePlayer.StopPlayback();
            toolStripButton5.Enabled = true;
            открытьФайлToolStripMenuItem.Enabled = true;
            toolStripButton2.Enabled = true;
            стартToolStripMenuItem.Enabled = true;
           
        }

        
        
        //нажатие кнопки Старт
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (wavePlayer.LoadWave(openFileDialog1.FileName, false) == 0)
            {
                Settings settings = new Settings();
                toolStripStatusLabel1.Visible = true;
                toolStripSeparator2.Visible = true;
                toolStripStatusLabel1.Text = "Вывод сигнала запущен";
                toolStripStatusLabel1.BackColor = Color.LightGreen;

                wavePlayer.AutoRewind = toolStripButton1.Checked;
                /*
                if(wavePlayer.AutoRewind == true)
                {
                    int Time = Convert.ToInt32(wavePlayer.TotalTime);
                    toolStripProgressBar1.Maximum = Time;
                    toolStripProgressBar1.Value = 0;

                   
                }
                 */
 
                wavePlayer.StartPlayback(source.OutputDevice);

                 
                //int totalbytes = Convert.ToInt32(wavePlayer.TotalTime * wavePlayer.SampleRate * wavePlayer.BitsPerSample / 8 * wavePlayer.Channels);
                //toolStripProgressBar1.Maximum = totalbytes;
                toolStripButton5.Enabled = false;
                открытьФайлToolStripMenuItem.Enabled = false;
                toolStripButton2.Enabled = false;
                стартToolStripMenuItem.Enabled = false;
                start_timer();
            }
            
        }
//запускаем таймер воспроизведения для индикации
        private void start_timer()
        {
            
            timer1.Start();
            
        }

//таймер
        private void timer1_Tick(object sender, EventArgs e)
        {

            long ttt = wavePlayer.Position;
            if (ttt <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = Convert.ToInt32(ttt);
            else toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
            if (ttt == toolStripProgressBar1.Maximum) Stopped();

        }

//нажатие кнопки Автоповтор
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            
            if (toolStripButton1.CheckState != System.Windows.Forms.CheckState.Checked)
            {
                toolStripButton1.CheckState = System.Windows.Forms.CheckState.Checked;
            }
            else
            {
                toolStripButton1.CheckState = System.Windows.Forms.CheckState.Unchecked;
            }
              
        }

//выбор нового значения длины отображения из Combobox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (Convert.ToInt32(comboBox1.Text) + hScrollBar1.Value > numericUpDown2.Maximum)
            {
                comboBox1.SelectedIndex = 0;
           //???
             //   comboBox1.SelectedIndex = Convert.ToInt32(Math.Floor(Math.Log(Convert.ToDouble(numericUpDown2.Maximum - hScrollBar1.Value), 2))) - 3;
            }
            */
/*
            hScrollBar2.Value = Convert.ToInt32(comboBox1.SelectedItem);

            comboBox1.Text = Convert.ToString(hScrollBar2.Value);
 * 
 * 
 */
            if (dont_do_event) return;

            if (Convert.ToInt32(comboBox1.Text) + hScrollBar1.Value > numericUpDown2.Maximum)
            {

                comboBox1.SelectedIndex = 0;
                comboBox1.SelectedIndex = Convert.ToInt32(Math.Floor(Math.Log(Convert.ToDouble(numericUpDown2.Maximum - hScrollBar1.Value), 2))) - 3;

            }

            hScrollBar2.Value = Convert.ToInt32(comboBox1.SelectedItem);


        }

//работа с Combobox
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }
//изменение положения Scrollbar для начала отображения
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (dont_do_event) return;
            numericUpDown2.Value = hScrollBar1.Value;
        }
//изменение положения Scrollbar для начала отображения
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (hScrollBar1.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            {
                hScrollBar1.Value = Convert.ToInt32(numericUpDown2.Maximum - hScrollBar2.Value);
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(hScrollBar1.Value + 1, hScrollBar2.Value - 1 + hScrollBar1.Value + 1);
        }
//изменение положения Scrollbar для длины отображения
        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            if (dont_do_event) return;
            comboBox1.Text = hScrollBar2.Value.ToString();
        }
//изменение положения Scrollbar для длины отображения
        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (hScrollBar1.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            {
                hScrollBar2.Value = Convert.ToInt32(numericUpDown2.Maximum - hScrollBar1.Value);
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(hScrollBar1.Value + 1, hScrollBar2.Value - 1 + hScrollBar1.Value + 1);
        }
//изменение положения числового поля для начала отображения
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (numericUpDown2.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            { numericUpDown2.Value = numericUpDown2.Maximum - hScrollBar2.Value; }

            hScrollBar1.Value = Convert.ToInt32(numericUpDown2.Value);
        }

//работа с числовым полем
        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void numericUpDown2_KeyUp(object sender, KeyEventArgs e)
        {
            if (numericUpDown2.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            { numericUpDown2.Value = numericUpDown2.Maximum - hScrollBar2.Value; }
            hScrollBar1.Value = Convert.ToInt32(numericUpDown2.Value);
        }

//открытие нового файла
        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (wavePlayer.LoadWave(openFileDialog1.FileName, false) == 0)
                {
                    dont_do_event = true;
                    textBox1.Text = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                    textBox2.Text = wavePlayer.TotalTime.ToString();
                    textBox3.Text = wavePlayer.SampleRate.ToString();
                    textBox4.Text = wavePlayer.Channels.ToString();
                    textBox5.Text = wavePlayer.BitsPerSample.ToString();
                    toolStripParameters();
                    toolStripButton1.Enabled = true;
                    toolStripButton2.Enabled = true;
                    toolStripButton3.Enabled = true;
                    hScrollBar2.Enabled = true;
                    сохранитьКакИзображениеToolStripMenuItem.Enabled = true;
                    сохранитьВыделенныйФрагментToolStripMenuItem.Enabled = true;
                    toolStripProgressBar1.Value = 0;
                    numericUpDown2.Value = 0;



                    String extension = System.IO.Path.GetExtension(openFileDialog1.FileName).ToLower();
                    switch (extension)
                    {
                        case ".mp3": bildGraf(wavePlayer.Channels); plottATR_mp3(openFileDialog1.FileName); break;
                        case ".wav": bildGraf(wavePlayer.Channels); plottATR_wav(openFileDialog1.FileName); break;
                        default: break;
                    }
                    
                    dont_do_event = false;


                }
            }
        }


        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

            if (comboBox1.Text != "")
            {
                if (Convert.ToInt32(comboBox1.Text) <= hScrollBar2.Maximum + 1 - hScrollBar2.LargeChange - numericUpDown2.Value && Convert.ToInt32(comboBox1.Text) >= hScrollBar2.Minimum)
                    hScrollBar2.Value = Convert.ToInt32(comboBox1.Text);
                else
                {
                    if (Convert.ToInt32(comboBox1.Text) > hScrollBar2.Maximum + 1 - hScrollBar2.LargeChange - numericUpDown2.Value)
                        comboBox1.Text = Convert.ToString(hScrollBar2.Maximum + 1 - hScrollBar2.LargeChange - numericUpDown2.Value);
                    else
                        comboBox1.Text = Convert.ToString(hScrollBar2.Minimum);

                }

            }



        }

        private void OutputForm_Load(object sender, EventArgs e)
        {

        }

//Работа с элементами прокрутки
        private void _ScrollBar(int N)
        {
            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = N;
          
           // numericUpDown2.Value = 0;
            hScrollBar2.Maximum = 2 * N;
            hScrollBar2.Minimum = 1;
            hScrollBar2.LargeChange = (N / 100 > 10) ? (N / 100) : 10;
            hScrollBar2.SmallChange = (N / 1000 > 1) ? (N / 1000) : 1;
            hScrollBar2.Maximum = N + hScrollBar2.LargeChange - 1;// -hScrollBar2.SmallChange;
            hScrollBar2.Value = N;
            comboBox1.Text = hScrollBar2.Value.ToString();
            comboBox1.Items.Clear();
            for (int i = 3; i <= Math.Log(N_Output, 2); i++)
                comboBox1.Items.Add(Convert.ToInt32(Math.Pow(2, i)).ToString());
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = hScrollBar2.Maximum;
            hScrollBar1.Value = 0;
            hScrollBar1.LargeChange = hScrollBar2.LargeChange;
            hScrollBar1.SmallChange = hScrollBar2.SmallChange;



            hScrollBar2.Enabled = true;
            comboBox1.Enabled = true;


            int totalbytes = Convert.ToInt32(N * wavePlayer.BitsPerSample / 8 * wavePlayer.Channels);
            toolStripProgressBar1.Maximum = totalbytes;


        }

//Создание графика сигнала
        public void plottATR_wav(String FileName)
        {
            WaveStream wave = new WaveFileReader(FileName);
            int sampleSize = wavePlayer.BitsPerSample;
            var bufferSize = Convert.ToInt32(wave.Length);
            var buffer = new byte[wave.Length];
            var read = wave.Read(buffer, 0, bufferSize);
            if (wavePlayer.WaveFormat != "IeeeFloat")
            {
                switch (sampleSize)
                {
                    case 8:
                        if (wavePlayer.Channels == 2)
                        {
                            for (int i = 0; i < bufferSize; i += 2)
                            {
                                uint sampleChen1 = buffer[i];
                                float sample8Chen1 = (sampleChen1 - 128f)/256f ;
                                chart1.Series[0].Points.Add(sample8Chen1);

                                uint sampleChen2 = buffer[i + 1];
                                float sample8Chen2 = (sampleChen2 - 128f)/256f;
                                chart1.Series[1].Points.Add(sample8Chen2);
                            }
                            N_Output = bufferSize / 2;
                            _ScrollBar(N_Output);
                        }
                        else
                        {
                            for (int i = 0; i < bufferSize; i++)
                            {
                                ushort sample = (buffer[i]);
                                float sample8 = (sample - 128)/ 256f;
                                chart1.Series[0].Points.Add(sample8);
                            }
                            N_Output = bufferSize;
                            _ScrollBar(N_Output); break;
                        } break;
                    case 16:
                        if (wavePlayer.Channels == 2)
                        {
                            for (int i = 0; i < bufferSize; i += 4)
                            {
                                long sampleChen1 = (short)((buffer[i + 1] << 8) | buffer[i]);
                                float sample16Chen1 = sampleChen1 / 32768f;
                                chart1.Series[0].Points.Add(sample16Chen1);

                                long sampleChen2 = (short)((buffer[i + 3] << 8) | buffer[i + 2]);
                                float sample16Chen2 = sampleChen2 / 32768f;
                                chart1.Series[1].Points.Add(sample16Chen2);
                            }
                            N_Output = bufferSize / 4;
                            _ScrollBar(N_Output);
                        }
                        else
                        {
                            for (int i = 0; i < bufferSize; i += 2)
                            {
                                short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                                float sample16 = sample / 32768f;
                                chart1.Series[0].Points.Add(sample16);
                            }
                            N_Output = bufferSize / 2;
                            _ScrollBar(N_Output);
                        } break;
                    case 24:
                        if (wavePlayer.Channels == 2)
                        {
                            for (int i = 0; i < bufferSize; i += 6)
                            {
                                int sampleChen1 = (((buffer[i + 2] << 12) | buffer[i + 1]) << 12 | buffer[i + 0]);
                                float sample24Chen1 = sampleChen1 / 2147483648f;
                                chart1.Series[0].Points.Add(sample24Chen1);

                                int sampleChen2 = (((buffer[i + 5] << 12) | buffer[i + 4]) << 12 | buffer[i + 3]);
                                float sample24Chen2 = sampleChen2 / 2147483648f;
                                chart1.Series[1].Points.Add(sample24Chen2);
                            }
                            N_Output = bufferSize / 6;
                            _ScrollBar(N_Output);
                        }
                        else
                        {
                            for (int i = 0; i < bufferSize; i += 3)
                            {
                                long sample = (((buffer[i + 2] << 12) | buffer[i + 1]) << 12 | buffer[i]);
                                float sample24 = sample / 2147483648f;
                                chart1.Series[0].Points.Add(sample24);
                            }
                            N_Output = bufferSize / 3;
                            _ScrollBar(N_Output);
                        } break;
                    case 32:
                        if (wavePlayer.Channels == 2)
                        {
                            for (int i = 0; i < bufferSize; i += 8)
                            {
                                int sampleChen1 = ((((buffer[i + 3] << 8) | buffer[i + 2]) << 8 | buffer[i + 1]) << 8 | buffer[i + 0]);
                                float sample32Chen1 = sampleChen1 / 2147483648f;
                                chart1.Series[0].Points.Add(sample32Chen1);

                                int sampleChen2 = ((((buffer[i + 7] << 8) | buffer[i + 6]) << 8 | buffer[i + 5]) << 8 | buffer[i + 4]);
                                float sample32Chen2 = sampleChen2 / 2147483648f;
                                chart1.Series[1].Points.Add(sample32Chen2);
                            }
                            N_Output = bufferSize / 8;
                            _ScrollBar(N_Output);
                        }
                        else
                        {
                            for (int i = 0; i < bufferSize; i += 4)
                            {
                                int sample = ((((buffer[i + 3] << 8) | buffer[i + 2]) << 8 | buffer[i + 1]) << 8 | buffer[i]);
                                float sample32 = sample / 2147483648f;
                                chart1.Series[0].Points.Add(sample32);
                            }
                            N_Output = bufferSize / 4;
                            _ScrollBar(N_Output);
                        } break;
                    default: break;
                }
            }
            else
            {
                if (wavePlayer.Channels == 2)
                {
                    for (int i = 0; i < bufferSize / sampleSize; i++)
                    {
                        var intSampleValue = BitConverter.ToSingle(buffer, i * wavePlayer.BitsPerSample);
                        chart1.Series[0].Points.Add(intSampleValue);
                    }
                    for (int i = 1; i < bufferSize / sampleSize; i++)
                    {
                        var intSampleValue = BitConverter.ToSingle(buffer, i * wavePlayer.BitsPerSample - 4);
                        chart1.Series[1].Points.Add(intSampleValue);
                    }
                    N_Output = bufferSize / sampleSize;
                    _ScrollBar(N_Output);
                }
                else
                {
                    for (int i = 0; i < bufferSize / sampleSize; i++)
                    {
                        var intSampleValue = BitConverter.ToSingle(buffer, i * sampleSize);
                        chart1.Series[0].Points.Add(intSampleValue);
                    }
                    N_Output = bufferSize / sampleSize;
                    _ScrollBar(N_Output);
                }
            }
        }

//создание графика сигнала
        private void plottATR_mp3(String FileName)
        {
            Mp3FileReader mp3 = new Mp3FileReader(FileName);
            int sampleSize = wavePlayer.BitsPerSample;
            var bufferSize = Convert.ToInt32(mp3.Length);
            var buffer = new byte[mp3.Length];
            int read = 0;
            read = mp3.Read(buffer, 0, bufferSize);
            switch (sampleSize)
            {
                case 8:
                    if (wavePlayer.Channels == 2)
                    {
                        for (int i = 0; i < bufferSize; i += 2)
                        {
                            uint sampleChen1 = buffer[i];
                            float sample8Chen1 = sampleChen1 / 256f;
                            chart1.Series[0].Points.Add(sampleChen1);

                            uint sampleChen2 = buffer[i + 1];
                            float sample8Chen2 = sampleChen2 / 256f;
                            chart1.Series[1].Points.Add(sampleChen2);
                        }
                        N_Output = bufferSize / 2;
                        _ScrollBar(N_Output);
                    }
                    else
                    {
                        for (int i = 0; i < bufferSize; i++)
                        {
                            ushort sample = (buffer[i]);
                            float sample8 = sample / 256f;
                            chart1.Series[0].Points.Add(sample8);
                        }
                        N_Output = bufferSize;
                        _ScrollBar(N_Output); break;
                    } break;
                case 16:
                    if (wavePlayer.Channels == 2)
                    {
                        for (int i = 0; i < bufferSize; i += 4)
                        {
                            long sampleChen1 = (short)((buffer[i + 1] << 8) | buffer[i]);
                            float sample16Chen1 = sampleChen1 / 32768f;
                            chart1.Series[0].Points.Add(sample16Chen1);

                            long sampleChen2 = (short)((buffer[i + 3] << 8) | buffer[i + 2]);
                            float sample16Chen2 = sampleChen2 / 32768f;
                            chart1.Series[1].Points.Add(sample16Chen2);
                        }
                        N_Output = bufferSize / 4;
                        _ScrollBar(N_Output);
                    }
                    else
                    {
                        for (int i = 0; i < bufferSize; i += 2)
                        {
                            short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                            float sample16 = sample / 32768f;
                            chart1.Series[0].Points.Add(sample16);
                        }
                        N_Output = bufferSize / 2;
                        _ScrollBar(N_Output);
                    } break;
                case 24:
                    if (wavePlayer.Channels == 2)
                    {
                        for (int i = 0; i < bufferSize; i += 6)
                        {
                            int sampleChen1 = (((buffer[i + 2] << 12) | buffer[i + 1]) << 12 | buffer[i + 0]);
                            float sample24Chen1 = sampleChen1 / 2147483648f;
                            chart1.Series[0].Points.Add(sample24Chen1);

                            int sampleChen2 = (((buffer[i + 5] << 12) | buffer[i + 4]) << 12 | buffer[i + 3]);
                            float sample24Chen2 = sampleChen2 / 2147483648f;
                            chart1.Series[1].Points.Add(sample24Chen2);
                        }
                        N_Output = bufferSize / 6;
                        _ScrollBar(N_Output);
                    }
                    else
                    {
                        for (int i = 0; i < bufferSize; i += 3)
                        {
                            long sample = (((buffer[i + 2] << 12) | buffer[i + 1]) << 12 | buffer[i]);
                            float sample24 = sample / 2147483648f;
                            chart1.Series[0].Points.Add(sample24);
                        }
                        N_Output = bufferSize / 3;
                        _ScrollBar(N_Output);
                    } break;
                case 32:
                    if (wavePlayer.Channels == 2)
                    {
                        for (int i = 0; i < bufferSize; i += 8)
                        {
                            int sampleChen1 = ((((buffer[i + 3] << 8) | buffer[i + 2]) << 8 | buffer[i + 1]) << 8 | buffer[i + 0]);
                            float sample32Chen1 = sampleChen1 / 2147483648f;
                            chart1.Series[0].Points.Add(sample32Chen1);

                            int sampleChen2 = ((((buffer[i + 7] << 8) | buffer[i + 6]) << 8 | buffer[i + 5]) << 8 | buffer[i + 4]);
                            float sample32Chen2 = sampleChen2 / 2147483648f;
                            chart1.Series[1].Points.Add(sample32Chen2);
                        }
                        N_Output = bufferSize / 8;
                        _ScrollBar(N_Output);
                    }
                    else
                    {
                        for (int i = 0; i < bufferSize; i += 4)
                        {
                            int sample = ((((buffer[i + 3] << 8) | buffer[i + 2]) << 8 | buffer[i + 1]) << 8 | buffer[i]);
                            float sample32 = sample / 2147483648f;
                            chart1.Series[0].Points.Add(sample32);
                        }
                        N_Output = bufferSize / 4;
                        _ScrollBar(N_Output);
                    } break;
                default: break;
            }
        }


//инициализация графика перед построением графика
        private void bildGraf(int channel)
        {
            if (channel == 2)
            {
                chart1.Series.Dispose();
                chart1.ChartAreas.Dispose();
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.ChartAreas.Add("ChartArea1");
                chart1.Series.Add("Канал 1");
                chart1.Series.Add("Канал 2");
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                chart1.Series[0].ChartArea = "ChartArea1";
            }
            else
            {
                chart1.Series.Dispose();
                chart1.ChartAreas.Dispose();
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.ChartAreas.Add("ChartArea1");
                chart1.Series.Add("Канал 1");
                chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                chart1.Series[0].ChartArea = "ChartArea1";
            }
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
        }
        
//Выбрано действие сохранить график        
        private void сохранитьКакИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Изображение Jpeg|*.jpeg|Изображение Bmp|*.bmp";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1: this.chart1.SaveImage(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                    case 2: this.chart1.SaveImage(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp); break;
                }
            }
        }


        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
        }

//выдача параметров сигнала в нижнее поле окна
        private void toolStripParameters()
        {
            toolStripSeparator3.Visible = true;
            toolStripSeparator4.Visible = true;
            toolStripStatusLabel2.Visible = true;
            toolStripStatusLabel3.Visible = true;
            toolStripStatusLabel4.Visible = true;
            toolStripStatusLabel2.Text = "Имя файла: " + Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            toolStripStatusLabel3.Text = "Формат: " + wavePlayer.WaveFormat;
            toolStripStatusLabel4.Text = "Длительность: " + wavePlayer.TotalTime.ToString();
        }

//сохранить выделенный фрагмент
        private void сохранитьВыделенныйФрагментToolStripMenuItem_Click(object sender, EventArgs e) // сохранение фрагмента файла
        {
            String extension = System.IO.Path.GetExtension(openFileDialog1.FileName).ToLower();
            if (extension == ".wav")
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog(); // окно выбора пути сохранения
                saveFileDialog1.Filter = "WAV files (*.wav)|*.wav"; // фильтр
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TrimWav(openFileDialog1.FileName, saveFileDialog1.FileName, hScrollBar1.Value, hScrollBar2.Value);
                }
            }
            if (extension == ".mp3")
            {
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp3"; // создание временного файла
                Mp3FileReader mp3 = new Mp3FileReader(openFileDialog1.FileName); // открытие файла в виде потока байтов
                Mp3WaveFormat Format = mp3.Mp3WaveFormat; // создание переменной формата с указанием параметров
                var buffer = new byte[mp3.Length]; // создание массива байтов с длиной равной длине файла
                byte[] samples = new byte[buffer.Length];
                int _bufferSize = Convert.ToInt32(mp3.Length); // переменная, которая содержит длину файла
                var read = 0;
                read = mp3.Read(buffer, 0, buffer.Length); // чтение файла
                write = new WaveFileWriter(fileName, Format); // создание файла с указанием пути и формата
                
                string strMP3Folder = openFileDialog1.FileName;
                int count = 1;

                using (Mp3FileReader reader = new Mp3FileReader(strMP3Folder))
                {
                    Mp3Frame mp3Frame = reader.ReadNextFrame();
                    while (mp3Frame != null)
                    {
                        if (count > 250) //retrieve a sample of 500 frames
                            return;

                        write.Write(mp3Frame.RawData, 0, mp3Frame.RawData.Length);
                        count = count + 1;
                        mp3Frame = reader.ReadNextFrame();
                    }
                }
                
                write.Close(); // закрытие потока записи

                SaveFileDialog saveFileDialog1 = new SaveFileDialog(); // окно выбора пути сохранения
                saveFileDialog1.Filter = "MP3 files (*.mp3)|*.mp3"; // фильтр
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.Copy(fileName, saveFileDialog1.FileName, true); // копирование временного файла в место указанное пользователем
                    File.Delete(fileName); // удаление временного файла
                }
            }
        }

//вырезать фрагмент сигнала
        private void TrimWav(String OpenFileName, String TrimFileName, int StartSample, int EndSample) // метод сохранения фрагмента файла
        {
            string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".wav"; // создание временного файла
            WaveStream wave = new WaveFileReader(OpenFileName); // открытие файла в виде потока байтов
            WaveFormat Format = wave.WaveFormat; // создание делегата класса для указания формата нового файла
            write = new WaveFileWriter(fileName, Format); // создание файла с указанием пути и формата
            var buffer = new byte[wave.Length]; // создание массива байтов с длиной равной длине файла 
            int _bufferSize = Convert.ToInt32(wave.Length); // переменная, которая содержит длину файла
            var read = 0;
            read = wave.Read(buffer, 0, _bufferSize); // чтение файла

            if (wavePlayer.WaveFormat == "IeeeFloat") // если способ кодировки файла IEEE Float
                write.Write(buffer, hScrollBar1.Value * 32, hScrollBar2.Value * 32); // запись выбранных байтов в файл
            else // если файл PCM
            {
                switch (wavePlayer.BitsPerSample)
                {
                    case 8:
                        if (wavePlayer.Channels == 1)
                            write.Write(buffer, StartSample, EndSample); // запись выбранных байтов в файл
                        else
                            write.Write(buffer, StartSample * 2, EndSample * 2); // запись выбранных байтов в файл
                        break;
                    case 16:
                        if (wavePlayer.Channels == 1)
                            write.Write(buffer, StartSample * 2, EndSample * 2); // запись выбранных байтов в файл
                        else
                            write.Write(buffer, StartSample * 4, EndSample * 4); // запись выбранных байтов в файл
                        break;
                    case 24:
                        if (wavePlayer.Channels == 1)
                            write.Write(buffer, StartSample * 3, EndSample * 3); // запись выбранных байтов в файл
                        else
                            write.Write(buffer, StartSample * 6, EndSample * 6); // запись выбранных байтов в файл
                        break;
                    case 32:
                        if (wavePlayer.Channels == 1)
                            write.Write(buffer, StartSample * 4, EndSample * 4); // запись выбранных байтов в файл
                        else
                            write.Write(buffer, StartSample * 8, EndSample * 8); // запись выбранных байтов в файл
                        break;
                }
            }
            write.Close(); // закрытие потока
            File.Copy(fileName, TrimFileName, true); // копирование временного файла в место указанное пользователем
            File.Delete(fileName); // удаление временного файла
        }

//изменилось отображение  графика
        private void chart1_AxisViewChanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e)
        {
            int start = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;

            if (double.IsNaN(chart1.ChartAreas[0].AxisX.ScaleView.Size))
            {
                comboBox1.Text = N_Output.ToString();
            }
            else
            {
                comboBox1.Text = (chart1.ChartAreas[0].AxisX.ScaleView.Size + 1).ToString();
                hScrollBar2.Value = (int)chart1.ChartAreas[0].AxisX.ScaleView.Size + 1;
                hScrollBar1.Value = start;
                numericUpDown2.Value = start;
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
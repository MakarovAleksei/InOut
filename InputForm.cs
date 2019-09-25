using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace WindowsFormsApplication2
{
    public partial class InputForm : Form
    {
        WaveIn waveIn;
        WaveFileWriter writer;
        
        
        MainForm mainForm = new MainForm();
        string fileName;

        private bool dont_do_event = false;
       
        ConfigurationManager source = ConfigurationManager.Source;

        public float correctionChen1 { get; set; }
        public float correctionChen2 { get; set; }
        public int N_Input { get; set; }
        public double sek { get; set; }
        public float sample { get; set; }


        public InputForm()
        {
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            if(waveIn != null)
            {
                waveIn.StopRecording();
            }
            if(fileName != null)
            {
                File.Delete(fileName);
            }
        }

        private void InputForm_Shown(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;
            toolStripComboBox3.SelectedIndex = 0;
            menuStrip1.Visible = false;
        }
        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if(waveIn == null)
            {
                if (fileName != null)
                {
                    File.Delete(fileName);
                }

                StartRec();
            }
        }

        //данные получены при записи онлайн
        void waveIn_DataAvailable(object sender, WaveInEventArgs e) // запись байтов в файл и построение графика
        {
            int Channels = writer.WaveFormat.Channels;
            byte[] buffer = e.Buffer; // запись байтов с микрофона в массив байтов
            writer.Write(e.Buffer, 0, e.BytesRecorded); // запись байтов в файл
            writer.Flush(); // установка правильных заголовков файла
            int bufferSize = Convert.ToInt32(writer.Length); // получение длины записи


            
            switch (writer.WaveFormat.BitsPerSample) // построение графика происходит аналогичном как и для вывода сигнала
            {
                case 8:
                    if (writer.WaveFormat.Channels == 2)
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i += 2)
                        {
                            float sampleChen1 = e.Buffer[i] / 256f;
                            chart1.Series[0].Points.Add(sampleChen1 - correctionChen1);
                        
                            float sampleChen2 = e.Buffer[i + 1] / 256f;
                            chart1.Series[1].Points.Add(sampleChen2 - correctionChen2);
                        }
                         */ 
                       
                        N_Input = bufferSize / 2;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    }
                    else
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i++)
                        {
                            sample = e.Buffer[i] / 256f;
                            chart1.Series[0].Points.Add(sample - correctionChen1);
                        }
                         */ 
                       
                        N_Input = bufferSize;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    } 
                    break;
                case 16:
                    if (writer.WaveFormat.Channels == 2)
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i += 4)
                        {
                            float sampleChen1 = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]) / 32768f;
                            chart1.Series[0].Points.Add(sampleChen1 - correctionChen1);

                            float sampleChen2 = (short)((e.Buffer[i + 3] << 8) | e.Buffer[i + 2]) / 32768f;
                            chart1.Series[1].Points.Add(sampleChen2 - correctionChen2);
                        }
                         */ 
                        N_Input = bufferSize / 4;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    }
                    else
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i += 2)
                        {
                            float sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]) / 32768f;
                            chart1.Series[0].Points.Add(sample - correctionChen1);
                        }
                         */ 

                        N_Input = bufferSize / 2;
                        
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    } 
                    break;
                case 24:
                    if (writer.WaveFormat.Channels == 2)
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i += 6)
                        {
                            float sampleChen1 = ((((e.Buffer[i + 2] << 8) | e.Buffer[i + 1]) << 8 | e.Buffer[i]) << 8 ) / 2147483648f;
                            chart1.Series[0].Points.Add(sampleChen1 - correctionChen1);

                            float sampleChen2 = ((((e.Buffer[i + 5] << 8) | e.Buffer[i + 4]) << 8 | e.Buffer[i + 3]) << 8) / 2147483648f;
                            chart1.Series[1].Points.Add(sampleChen2 - correctionChen2);
                        }
                         */
 
                        N_Input = bufferSize / 6;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    }
                    else
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i += 3)
                        {
                            float sample = ((((e.Buffer[i + 2] << 8) | e.Buffer[i + 1]) << 8 | e.Buffer[i]) << 8) / 2147483648f;
                            chart1.Series[0].Points.Add(sample - correctionChen1);
                        }
                         */ 
                        N_Input = bufferSize / 3;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    } 
                    break;
                case 32:
                    if (writer.WaveFormat.Channels == 2)
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded ; i += 8)
                        {
                            float sampleChen1 = ((((e.Buffer[i + 3] << 8) | e.Buffer[i + 2]) << 8 | e.Buffer[i + 1]) << 8 | e.Buffer[i + 0]) / 2147483648f;
                            chart1.Series[0].Points.Add(sampleChen1 - correctionChen1);

                            float sampleChen2 = ((((e.Buffer[i + 7] << 8) | e.Buffer[i + 6]) << 8 | e.Buffer[i + 5]) << 8 | e.Buffer[i + 4]) / 2147483648f;
                            chart1.Series[1].Points.Add(sampleChen2 - correctionChen2);
                        }
                         */ 
                        N_Input = bufferSize / 8;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    }
                    else
                    {
                        /*
                        for (int i = 0; i < e.BytesRecorded; i += 4)
                        {
                            float sample = ((((e.Buffer[i + 3] << 8) | e.Buffer[i + 2]) << 8 | e.Buffer[i + 1]) << 8 | e.Buffer[i]) / 2147483648f;
                            chart1.Series[0].Points.Add(sample - correctionChen1);
                        }
                         */ 
                        N_Input = bufferSize / 4;
                        sek = Math.Round(Math.Pow(writer.WaveFormat.SampleRate, -1) * N_Input, 1);
                        toolStripLabel2.Text = sek.ToString();
                    }
                    break;
                default: break;
            }


            

            if (sek >= 10)
            {
                if(waveIn != null)
                {
                    waveIn.StopRecording(); // остановка записи
                    enabledButton(true);
                   // _ScrollBar(N_Input);
                    toolStripStatusLabel1.Text = "Остановлено";
                    toolStripStatusLabel1.BackColor = Color.LightGreen;
                }
            }

           // _ScrollBar(N_Input);

        }

        //запись окончена
        void waveIn_RecordingStopped(object sender, EventArgs e) // событие для остановки записи
        {
            if (this.InvokeRequired)
            {
                this.Refresh();
                this.BeginInvoke(new EventHandler(waveIn_RecordingStopped), sender, e);
            }
            else
            {
                if (waveIn != null)
                {
                    this.Refresh();
                    waveIn.Dispose(); // освобождение
                    waveIn = null;    // памяти
                    writer.Close();   // после 
                    writer = null;    // записи в буфер

                    dont_do_event = true;
                    plottATR_wav(fileName);
                    dont_do_event = false;
                    
                }
            }
           
        }

        //старт записи
        void StartRec() // метод ввода сигнала
        {
            if (waveIn == null)
            {
                N_Input = 0;
                int sampleRate = Convert.ToInt32(toolStripComboBox1.Text); // установка
                int Bits = Convert.ToInt32(toolStripComboBox2.Text);       // параметров
                int Channels = Convert.ToInt32(toolStripComboBox3.Text);   // ввода сигнала
                dont_do_event = true;
                enabledButton(false);
                bildGraf(Channels); // настройка элемента отображения графика chart
                toolStripStatusLabel1.Text = "Ввод сигнала запущен";
                toolStripStatusLabel1.BackColor = Color.LightCoral;
                fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".wav"; // создание имени временного файла
                waveIn = new WaveIn(); // подготовка устройства ввода сигнала
                waveIn.DeviceNumber = source.InputDevice; // выбор устройства
                waveIn.DataAvailable += waveIn_DataAvailable; // событие при котором происходит запись байтов в файл и построение графика
                waveIn.RecordingStopped += new EventHandler<NAudio.Wave.StoppedEventArgs>(waveIn_RecordingStopped); // событие указывающее о том, что все байты получены. Необходимо для остановки записи
                waveIn.WaveFormat = new WaveFormat(sampleRate, Bits, Channels); // установка параметров файла
                waveIn.BufferMilliseconds = 250; // время записи байтов в буфер
                writer = new WaveFileWriter(fileName, waveIn.WaveFormat); // создание файла с указанием формата и пути
                waveIn.StartRecording(); // начало записи
                dont_do_event = false;

            }
        }
      
        //остановлена запись
        void StopRec()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording(); // остановка записи
                enabledButton(true);
              //  _ScrollBar(N_Input);
                toolStripStatusLabel1.Text = "Остановлено";
                toolStripStatusLabel1.BackColor = Color.LightGreen;

                /*
                writer.Close();
                writer.Dispose();
                writer = null;
                */

                

            }
        }
        
        //нажатие кнопки остановки записи
        private void toolStripButton3_Click(object sender, EventArgs e) // метод остановки записи по кнопке
        {
            StopRec();
        }

        //нажатие кнопки показать панель
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
        }

        //изменение индекса выранного элемента в Длине отображения
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (Convert.ToInt32(comboBox1.Text) + hScrollBar1.Value > numericUpDown2.Maximum)
            {
                comboBox1.SelectedIndex = 0;
                comboBox1.SelectedIndex = Convert.ToInt32(Math.Floor(Math.Log(Convert.ToDouble(numericUpDown2.Maximum - hScrollBar1.Value), 2))) - 3;
            }
            hScrollBar2.Value = Convert.ToInt32(comboBox1.SelectedItem);
        }
        
        //изменение текста в поле Длина отображения
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
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
        
        //нажатие на кнопку в поле Длина отображения
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }
        
        //скроллирование ползунка Начало отображения
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (dont_do_event) return;
            numericUpDown2.Value = hScrollBar1.Value;
        }
        
        //изменение значения движка Начало отображения
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (hScrollBar1.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            {
                hScrollBar1.Value = Convert.ToInt32(numericUpDown2.Maximum - hScrollBar2.Value);
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(hScrollBar1.Value + 1, hScrollBar2.Value - 1 + hScrollBar1.Value + 1);
        }
        
        //изменение значения в числовом поле Начало отображения
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (numericUpDown2.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            { numericUpDown2.Value = numericUpDown2.Maximum - hScrollBar2.Value; }

            hScrollBar1.Value = Convert.ToInt32(numericUpDown2.Value);
        }

        //скроллирование ползунка Длина отображения
        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            if (dont_do_event) return;
            comboBox1.Text = hScrollBar2.Value.ToString();
        }

        //изменение значения движка Длина отображения
        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            if (dont_do_event) return;
            if (hScrollBar1.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            {
                hScrollBar2.Value = Convert.ToInt32(numericUpDown2.Maximum - hScrollBar1.Value);
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(hScrollBar1.Value + 1, hScrollBar2.Value - 1 + hScrollBar1.Value + 1);
        }
        
        //кликанье в поле Длина отображения
        private void numericUpDown2_KeyUp(object sender, KeyEventArgs e)
        {
            if (dont_do_event) return;
            if (numericUpDown2.Value + hScrollBar2.Value > numericUpDown2.Maximum)
            { numericUpDown2.Value = numericUpDown2.Maximum - hScrollBar2.Value; }

            hScrollBar1.Value = Convert.ToInt32(numericUpDown2.Value);
        }

        //активация кнопок управления, изначально неактивных
        private void enabledButton(bool flag)
        {
            hScrollBar1.Enabled = flag;
            hScrollBar2.Enabled = flag;
            comboBox1.Enabled = flag;
            numericUpDown2.Enabled = flag;
            if(!flag)
            {
                comboBox1.Text = null;
                comboBox1.Items.Clear();
            }
            сохранитьВесьСигналToolStripMenuItem.Enabled = flag;
            сохранитьВыделенныйФрагментToolStripMenuItem.Enabled = flag;
            сохранитьКакИзображениеToolStripMenuItem.Enabled = flag;
        }

        //инициализация поля графика
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

        //отображение сигнала на графике
        public void plottATR_wav(String FileName)
        {
            int N_Output=0;
            //WaveOutput.WavePlayer wavePlayer = new WaveOutput.WavePlayer();
            
            WaveStream wave = new WaveFileReader(FileName);
            
            //wavePlayer.LoadWave(FileName, false);


            int sampleRate = Convert.ToInt32(toolStripComboBox1.Text); // установка
            
            int Channels = Convert.ToInt32(toolStripComboBox3.Text);   
            

            //int sampleSize = wavePlayer.BitsPerSample;
            int sampleSize = Convert.ToInt32(toolStripComboBox2.Text);       


            var bufferSize = Convert.ToInt32(wave.Length);
            
            var buffer = new byte[wave.Length];
            var read = wave.Read(buffer, 0, bufferSize);
            if (true)
              //  if (wavePlayer.WaveFormat != "IeeeFloat")
            {
                switch (sampleSize)
                {
                    case 8:
                        if (Channels == 2)
                        {
                            for (int i = 0; i < bufferSize; i += 2)
                            {
                                uint sampleChen1 = buffer[i];
                                float sample8Chen1 = (sampleChen1 - 128f) / 256f;
                                chart1.Series[0].Points.Add(sample8Chen1);

                                uint sampleChen2 = buffer[i + 1];
                                float sample8Chen2 = (sampleChen2 - 128f) / 256f;
                                chart1.Series[1].Points.Add(sample8Chen2);
                            }
                            N_Output = bufferSize / 2;
                           // _ScrollBar(N_Output);
                        }
                        else
                        {
                            for (int i = 0; i < bufferSize; i++)
                            {
                                ushort sample = (buffer[i]);
                                float sample8 = (sample - 128) / 256f;
                                chart1.Series[0].Points.Add(sample8);
                            }
                            N_Output = bufferSize;
                            //_ScrollBar(N_Output); 
                            break;
                        } break;
                    case 16:
                        if (Channels == 2)
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
                            //_ScrollBar(N_Output);
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
                            //_ScrollBar(N_Output);
                        } break;
                    case 24:
                        if (Channels == 2)
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
                            //_ScrollBar(N_Output);
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
                            //_ScrollBar(N_Output);
                        } break;
                    case 32:
                        if (Channels == 2)
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
                            //_ScrollBar(N_Output);
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
                            //_ScrollBar(N_Output);
                        } break;
                    default: break;
                }
            }
            else
            {
                if (Channels == 2)
                {
                    for (int i = 0; i < bufferSize / sampleSize; i++)
                    {
                        var intSampleValue = BitConverter.ToSingle(buffer, i * sampleSize);
                        chart1.Series[0].Points.Add(intSampleValue);
                    }
                    for (int i = 1; i < bufferSize / sampleSize; i++)
                    {
                        var intSampleValue = BitConverter.ToSingle(buffer, i * sampleSize - 4);
                        chart1.Series[1].Points.Add(intSampleValue);
                    }
                    N_Output = bufferSize / sampleSize;
                    //_ScrollBar(N_Output);
                }
                else
                {
                    for (int i = 0; i < bufferSize / sampleSize; i++)
                    {
                        var intSampleValue = BitConverter.ToSingle(buffer, i * sampleSize);
                        chart1.Series[0].Points.Add(intSampleValue);
                    }
                    N_Output = bufferSize / sampleSize;
                    //_ScrollBar(N_Output);
                }
            }

            _ScrollBar(N_Output);

            wave.Close();
            wave.Dispose();
            wave = null;
          
            /*
            wavePlayer.Close();
            wavePlayer = null;
             */

        }

        //активировано действие Сохранить как изображение
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

        //активировано действие Сохранить весь сигнал
        private void сохранитьВесьСигналToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "WAV files (*.wav)|*.wav|MP3 files (*.mp3)|*.mp3";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(fileName, saveFileDialog1.FileName, true);
            }
        }

        //активировано действие Сохранить выделенный фрагмент
        private void сохранитьВыделенныйФрагментToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WaveFileWriter write;
            string newFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".wav";
            WaveStream wave = new WaveFileReader(fileName);

            WaveFormat Format = wave.WaveFormat;
            var buffer = new byte[wave.Length];
            int _bufferSize = Convert.ToInt32(wave.Length);
            var read = 0;
            read = wave.Read(buffer, 0, _bufferSize);
            write = new WaveFileWriter(newFileName, Format);
            switch (Format.BitsPerSample)
            {
                case 8:
                if(Format.Channels == 1)
                    write.Write(buffer, hScrollBar1.Value, hScrollBar2.Value);
                else
                    write.Write(buffer, hScrollBar1.Value * 2, hScrollBar2.Value * 2);
                break;
                case 16:
                if (Format.Channels == 1)
                    write.Write(buffer, hScrollBar1.Value * 2, hScrollBar2.Value * 2);
                else
                    write.Write(buffer, hScrollBar1.Value * 4, hScrollBar2.Value * 4);
                break;
                case 24:
                if (Format.Channels == 1)
                    write.Write(buffer, hScrollBar1.Value * 3, hScrollBar2.Value * 3);
                else
                    write.Write(buffer, hScrollBar1.Value * 6, hScrollBar2.Value * 6);
                break;
                case 32:
                if (Format.Channels == 1)
                    write.Write(buffer, hScrollBar1.Value * 4, hScrollBar2.Value * 4);
                else
                    write.Write(buffer, hScrollBar1.Value * 8 , hScrollBar2.Value * 8);
                break;
            }
            write.Close();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "WAV файлы (*.wav)|*.wav";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(newFileName, saveFileDialog1.FileName, true);
                File.Delete(newFileName);
            }
        }
        

        //установка скроллеров и полей Начала и Длины в начальные значения
        
        private void _ScrollBar(int N)
        {
            numericUpDown2.Value = 0;
            hScrollBar2.Maximum = 2 * N;
            hScrollBar2.Minimum = 1;
            hScrollBar2.LargeChange = (N / 100 > 10) ? (N / 100) : 10;
            hScrollBar2.SmallChange = (N / 1000 > 1) ? (N / 1000) : 1;
            hScrollBar2.Maximum = N + hScrollBar2.LargeChange - 1;// -hScrollBar2.SmallChange;
            hScrollBar2.Value = N;
            comboBox1.Text = hScrollBar2.Value.ToString();
            comboBox1.Items.Clear();
            for (int i = 3; i < Math.Log(N, 2); i++)
                comboBox1.Items.Add(Convert.ToInt32(Math.Pow(2, i)).ToString());
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = hScrollBar2.Maximum;
            hScrollBar1.Value = 0;
            hScrollBar1.LargeChange = hScrollBar2.LargeChange;
            hScrollBar1.SmallChange = hScrollBar2.SmallChange;
            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = N;
        }

          
         
         
        //изменился масштаб отображения на графике
        private void chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            int start = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;

            if (double.IsNaN(chart1.ChartAreas[0].AxisX.ScaleView.Size))
            {
                comboBox1.Text = N_Input.ToString();
            }
            else
            {
                comboBox1.Text = (chart1.ChartAreas[0].AxisX.ScaleView.Size + 1).ToString();
                hScrollBar2.Value = (int)chart1.ChartAreas[0].AxisX.ScaleView.Size + 1;
                hScrollBar1.Value = start;
                numericUpDown2.Value = start;
            }
        }
        
        //нажата кнопка корректировки уровня записи
        private void button1_Click(object sender, EventArgs e)
        {
            correctionChen1 = (float)numericUpDown3.Value;
            correctionChen2 = (float)numericUpDown4.Value;
        }

     
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void вывестиСигналотладкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }
    }
}

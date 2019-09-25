using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace WaveOutput
{
    // Объявить тип делегата для события,
    public delegate void MyEventHandler();
    /// <summary>
    /// Stream for looping playback
    /// </summary>
    class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;

            //sourceStream.
        }
        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }
        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }
        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }
        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
    public class WavePlayer
    {
        public event MyEventHandler PlaybackStopped;
        public bool AutoRewind = true;
        private WaveOut waveOut = new WaveOut();
        private WaveStream reader;
        private string waveFormat;
        private double totalTime;
        private int sampleRate, bitsPerSample, channels, blockAlign;

        // Этот метод вызывается для запуска события
        private void OnPlaybackStopped()
        {
            if (PlaybackStopped != null) PlaybackStopped();
        }
        public void Close()
        {
            
            waveOut.Dispose();
            waveOut = null;
        }
        public String WaveFormat
        {
            get
            {
                return waveFormat;
            }
        }
        public double TotalTime
        {
            get
            {
                return totalTime;
            }
        }
        public int SampleRate
        {
            get
            {
                return sampleRate;
            }
        }
        
        public long Position
        {
            get
            {
                return reader.Position;
            }
        }


        public int BitsPerSample
        {
            get
            {
                return bitsPerSample;
            }
        }
        public int Channels
        {
            get
            {
                return channels;
            }
        }
        public int BlockAlign
        {
            get
            {
                return blockAlign;
            }
        }
        public string State
        {
            get
            {
                return waveOut.PlaybackState.ToString();
            }
        }
        public int _Lenght
        {
            get
            {
                return (int)reader.Length;
            }
        }
        public string[] GetOutDevicesList()
        {
            var enumerator = new MMDeviceEnumerator();
            string[] array = new string[0];
            int i = 1;
                foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    Array.Resize(ref array, i); array[i - 1] = endpoint.FriendlyName; i++;
                }
            return array;
        }
        public string[] GetInDevicesList()
        {
            var enumerator = new MMDeviceEnumerator();
            string[] array = new string[0];
            int i = 1;
            foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                Array.Resize(ref array, i); array[i - 1] = endpoint.FriendlyName; i++;
            }
            return array;
        }
        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Console.WriteLine("base object stopped");
            this.OnPlaybackStopped();
        }
        public WavePlayer()
        {
            waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
          

        }
        public int LoadWave(String fileName, bool needclose) // метод получения заголовка
        {
            StopPlayback();
            int result = 1;
            if (System.IO.File.Exists(fileName))   
            {
                String extension = System.IO.Path.GetExtension(fileName).ToLower(); // получение пути к файлу

                switch (extension) // чтение файла в зависимости от расширения
                {
                    case ".mp3": reader = new NAudio.Wave.Mp3FileReader(fileName); result = 0; break; // чтение mp3 файла
                    case ".wav": reader = new NAudio.Wave.WaveFileReader(fileName); result = 0; break; // чтение wav файла
                    default: break;
                }
            }
            if (reader != null)
            {
                waveFormat = reader.WaveFormat.ToString(); // получение формата файла
                blockAlign = reader.BlockAlign; // получение размера блока выравнивания
                totalTime = reader.TotalTime.TotalSeconds; // получение длительности файла в секундах
                sampleRate = reader.WaveFormat.SampleRate; // получение частоты дискретизации
                bitsPerSample = reader.WaveFormat.BitsPerSample; // получение битового разрешения
                channels = reader.WaveFormat.Channels; // получение числа каналов
                
                if (needclose) { reader.Close(); reader.Dispose(); } // завершение чтение и освобождение памяти
            }
            return result;
        }
        public int[,] Read
        {
            get
            {
                byte[] data = new byte[reader.Length];
                long N = reader.Length / channels / (bitsPerSample / 8);
                int read = reader.Read(data, 0, data.Length);
                int[,] outarray = new int[2,N];
          
                if (bitsPerSample == 8 && channels == 1)
                {
                    for (int i = 0; i < reader.Length; i++ )
                    {
                        outarray[0, i] = data[i]-128;
                    }
                }
                if (bitsPerSample == 16 && channels == 1)
                {
                    for (int i = 0; i < reader.Length; i=i+2)
                    {
                     if (data[i+1] > 127)
                        outarray[0, i/2] = data[i]-(255-data[i+1])*256;
                     else outarray[0, i / 2] = data[i] + data[i + 1] * 256;
                    }
                }
                if (bitsPerSample == 8 && channels == 2)
                {
                    for (int i = 0; i < reader.Length; i=i+2)
                    {
                        outarray[0, i/2] = data[i] - 128;
                        outarray[1, i / 2] = data[i+1] - 128;
                    }
                }
                if (bitsPerSample == 16 && channels == 2)
                {
                    for (int i = 0; i < reader.Length; i = i + 4)
                    {
                        if (data[i + 1] > 127)
                            outarray[0, i / 4] = data[i] - (255 - data[i + 1]) * 256;
                        else outarray[0, i / 4] = data[i] + data[i + 1] * 256;
                        if (data[i + 3] > 127)
                            outarray[1, i / 4] = data[i + 2] - (255 - data[i + 3]) * 256;
                        else outarray[1, i / 4] = data[i + 2] + data[i + 3] * 256;
                    }
                }
                return outarray;
            }
        }
        public void StartPlayback(int devnum) // метод воспроизведения файла
        {
            if (reader != null)
            {
                if (waveOut.PlaybackState != PlaybackState.Playing)
                {
                    if (AutoRewind) // проверка запуска автоповтора. Если try
                    {
                        LoopStream loop = new LoopStream(reader); // экземпляр класса для осуществления автоповтора из открытого WaveStream
                        waveOut = new WaveOut(); // подготовка устройства вывода для воспроизведения
                        if (devnum >= 0)
                            waveOut.DeviceNumber = devnum; // выбор устройства для воспроизведения
                        waveOut.Init(loop); // инициализация воспроизведения с автоповтором
                        waveOut.Play(); // запуск воспроизведения
                    }
                    else // если false
                    {
                        waveOut.Init(reader); //инициализация воспроизведения открытого WaveStream
                        waveOut.Play();// запуск воспроизведения
                    }
                }
            }
        }
        public void StopPlayback()
        {
            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Stop();
                if (AutoRewind) OnPlaybackStopped();
            }

            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
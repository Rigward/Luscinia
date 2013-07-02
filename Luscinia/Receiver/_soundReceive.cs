using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;
namespace AudioCloud
{
    class _soundReceive
    {
        public Thread Thrd;

        private BufferedWaveProvider waveProvider;
        private IWavePlayer waveOut;
        private IWaveIn waveIn;
        private UdpClient udpListener;
        public _soundReceive()
        {
            waveOut = new WaveOut();
            waveIn = new WasapiLoopbackCapture();                           //this is only to get proper format of recording
            try
            {
                Thrd = new Thread(this.Receiver);
                Thrd.IsBackground = true;
                Thrd.Name = "SoundReceive";
                Thrd.Start();
            }
            catch { };
        }

        public void Receiver()
        {
            udpListener = new UdpClient();
            udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 810);

            waveProvider = new BufferedWaveProvider(waveIn.WaveFormat);     //getting proper format from wavein
            waveProvider.DiscardOnBufferOverflow = true;                  //With true value buffer ignores new packages, if it is full
            byte[] b;                                                       //buffer for data recieving
            waveOut.Init(waveProvider);                
            udpListener.Client.Bind(endPoint);
            waveOut.Play();

            do
            {
                b = udpListener.Receive(ref endPoint);                      //receiving message
                //Console.WriteLine("{0}", b.Length+"     "+waveProvider.BufferLength+"        "+waveProvider.BufferedBytes);
                waveProvider.AddSamples(b, 0, b.Length);                    //adding data to buffer
                Thread.CurrentThread.Join(0);
            }
            while (b.Length != 0);
            waveOut.Dispose();
            waveOut.Stop();
            
        }

        ~_soundReceive()
        {
            udpListener.Close();
            waveIn.Dispose();

        }
    }
}


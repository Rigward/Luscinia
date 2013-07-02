using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioCloud.Sender
{
    /// <summary>
    /// Interaction logic for SenderController.xaml
    /// </summary>
    public partial class SenderController : Page
    {
        private Thread Thrd;
        private _passCheckSender passCheckSender;
        private _sizes Sizes;
        private string IP;     //for forwarding value to next page
 
        public SenderController(string _IP)
        {
            InitializeComponent();

            Sizes = new _sizes();

            _Grid.Height = Sizes.Height;
            _Grid.Width = Sizes.Width;

            IP = _IP;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            passCheckSender = new _passCheckSender(IP, Password.GetLineText(0));
            Thrd = new Thread(verifyingCompleted);
            Thrd.Name = "verifyingCheck";
            Thrd.Start();

        }

        private void verifyingCompleted()
        {
            bool Flag = false;
            Thread.Sleep(100);
            while (!Flag)
            {
                if (!passCheckSender.Thrd.IsAlive)
                    Flag = true;
                Thread.CurrentThread.Join(0);
            }

            if (passCheckSender.checkFlag)
            {
                Dispatcher.BeginInvoke((Action)( () =>
                 {
                     AudioCloud.Sender.SenderCapture _SenderCapture = new AudioCloud.Sender.SenderCapture(IP, passCheckSender.Volume);
                     this.NavigationService.Navigate(_SenderCapture);
                 } ) 
                    );
                
            }

            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Help.Text = "Wrong password, try again:";
                        Help.Foreground = Brushes.Tomato;
                        Password.Clear();
                    })
                    );
                
            }
        }

    }
}

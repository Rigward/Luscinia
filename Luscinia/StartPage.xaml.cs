using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Luscinia
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
            _Grid.Height = Sizes.Height;
            _Grid.Width = Sizes.Width;
            _Receiver.Height = Sizes.Height/8;
            _Receiver.Width = Sizes.Width / 3;
            _Sender.Height = Sizes.Height/8;
            _Sender.Width = Sizes.Width / 3;
        } 

        private void buttonSender(object sender, RoutedEventArgs e)
        {
            SenderMain _sndr = new SenderMain();
            this.NavigationService.Navigate(_sndr);
        }

        private void buttonReceiver(object sender, RoutedEventArgs e)
        {
            receiver _rcvr = new receiver();
            this.NavigationService.Navigate(_rcvr);
        }
    }
}

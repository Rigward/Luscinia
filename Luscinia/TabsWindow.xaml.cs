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
	/// Interaction logic for TabsWindow.xaml
	/// </summary>
	public partial class TabsWindow : Page
	{
		public TabsWindow()
		{
			InitializeComponent();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GetNavigationService(this).Navigate(new SenderMain());
			MessageBox.Show("You clicked!");
		}
	}
}

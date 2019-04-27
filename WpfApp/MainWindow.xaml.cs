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

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Page
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("确认选择简单难度？", "确认", MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new Uri("game.xaml", UriKind.Relative));
                Window1.level = 0;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("确认选择中等难度？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new Uri("game.xaml", UriKind.Relative));
                Window1.level = 4;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("确认选择恶魔难度？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new Uri("game.xaml", UriKind.Relative));
                Window1.level = 10;
            }
        }
    }
}

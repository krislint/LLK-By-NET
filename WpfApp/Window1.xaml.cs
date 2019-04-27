using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : NavigationWindow
    {
        public static int level;
        public Window1()
        {
            InitializeComponent();
            ShowsNavigationUI = false;
            var cur=(Bitmap)Images.ResourceManager.GetObject("cursor");
            this.Cursor = MouseManager.CreateCursor(cur, 0, 0);
        }
        

    }
}

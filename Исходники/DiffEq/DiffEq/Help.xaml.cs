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
using System.Windows.Shapes;
using System.Configuration;
using System.Xml;

namespace DiffEq
{
    /// <summary>
    /// Логика взаимодействия для Help.xaml
    /// </summary>
    public partial class Help : Window
    {



        public Help()
        {
            InitializeComponent();

        }

        private void Desc_click(object sender, RoutedEventArgs e)
        {
            TBDescription.Visibility = System.Windows.Visibility.Visible;
            TBMath.Visibility = System.Windows.Visibility.Hidden;
            TBButtons.Visibility = System.Windows.Visibility.Hidden;
            TBAuthor.Visibility = System.Windows.Visibility.Hidden;
            TBAlgorithm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Math_click(object sender, RoutedEventArgs e)
        {
            TBDescription.Visibility = System.Windows.Visibility.Hidden;
            TBMath.Visibility = System.Windows.Visibility.Visible;
            TBButtons.Visibility = System.Windows.Visibility.Hidden;
            TBAuthor.Visibility = System.Windows.Visibility.Hidden;
            TBAlgorithm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void But_click(object sender, RoutedEventArgs e)
        {
            TBDescription.Visibility = System.Windows.Visibility.Hidden;
            TBMath.Visibility = System.Windows.Visibility.Hidden;
            TBButtons.Visibility = System.Windows.Visibility.Visible;
            TBAuthor.Visibility = System.Windows.Visibility.Hidden;
            TBAlgorithm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Aut_click(object sender, RoutedEventArgs e)
        {
            TBDescription.Visibility = System.Windows.Visibility.Hidden;
            TBMath.Visibility = System.Windows.Visibility.Hidden;
            TBButtons.Visibility = System.Windows.Visibility.Hidden;
            TBAuthor.Visibility = System.Windows.Visibility.Visible;
            TBAlgorithm.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Alg_click(object sender, RoutedEventArgs e)
        {
            TBDescription.Visibility = System.Windows.Visibility.Hidden;
            TBMath.Visibility = System.Windows.Visibility.Hidden;
            TBButtons.Visibility = System.Windows.Visibility.Hidden;
            TBAuthor.Visibility = System.Windows.Visibility.Hidden;
            TBAlgorithm.Visibility = System.Windows.Visibility.Visible;
        }

        private void alg_link(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://mathfaculty.fullerton.edu/mathews//n2003/rungekuttafehlberg/RungeKuttaFehlbergProof.pdf");
        }
    }
}

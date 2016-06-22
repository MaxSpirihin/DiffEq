using DiffEq.Helpers;
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

namespace DiffEq
{
    /// <summary>
    /// Логика взаимодействия для PrefWindow.xaml
    /// </summary>
    public partial class PrefWindow : Window
    {
        const double maxEdge = 1000;
        const double maxAbs = 0.01;
        const int minStep = 10;
        const int maxStep = 10000;

        public PrefWindow()
        {
            InitializeComponent();

        }


        private void SaveValues()
        {
            if (CheckException()!=null)
            {
                MessageBox.Show(CheckException().Message,"Ошибка");
                return;
            }


            ((MainWindow)Owner).UpdatePreferences(Convert.ToDouble(TBXLeft.Text.Replace('.', ',')),
                Convert.ToDouble(TBXRight.Text.Replace('.', ',')), Convert.ToDouble(TBYTop.Text.Replace('.', ',')),
                Convert.ToDouble(TBYBottom.Text.Replace('.', ',')), Convert.ToInt32(TBCountOfStep.Text), 
                CBEnableAxes.IsChecked.Value);

            Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveValues();
            
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonDefault_Click(object sender, RoutedEventArgs e)
        {
            TBXLeft.Text = DefaultPreferences.LEFT_X.ToString();
            TBXRight.Text = DefaultPreferences.RIGHT_X.ToString();
            TBYBottom.Text = DefaultPreferences.BOTTOM_Y.ToString();
            TBYTop.Text = DefaultPreferences.TOP_Y.ToString();
            TBCountOfStep.Text = DefaultPreferences.COUNT_OF_STEPS.ToString();
            CBEnableAxes.IsChecked = DefaultPreferences.ENABLE_AXIS;

        }

        private void Window_Activated(object sender, EventArgs e)
        {
            TBXLeft.Text = ((MainWindow)Owner).getPref().LeftX.ToString();
            TBXRight.Text = ((MainWindow)Owner).getPref().RightX.ToString();
            TBYBottom.Text = ((MainWindow)Owner).getPref().BottomY.ToString();
            TBYTop.Text = ((MainWindow)Owner).getPref().TopY.ToString();
            TBCountOfStep.Text = ((MainWindow)Owner).getPref().CountOfSteps.ToString();
            CBEnableAxes.IsChecked = ((MainWindow)Owner).getPref().EnableAxis;
        }


        private Exception CheckException()
        {
            double test;

            //ввод не чисел
            try { test = Convert.ToDouble(TBXLeft.Text.Replace('.', ',')); }
            catch (Exception) { return new Exception("Минимум х должен быть десятичным числом."); }
            try { test = Convert.ToDouble(TBXRight.Text.Replace('.', ',')); }
            catch (Exception) { return new Exception("Максимум х должен быть десятичным числом."); }
            try { test = Convert.ToDouble(TBYBottom.Text.Replace('.', ',')); }
            catch (Exception) { return new Exception("Минимум y должен быть десятичным числом."); }
            try { test = Convert.ToDouble(TBYTop.Text.Replace('.', ',')); }
            catch (Exception) { return new Exception("Максимум y должен быть десятичным числом."); }
            try { test = Convert.ToInt32(TBCountOfStep.Text); }
            catch (Exception) { return new Exception("Количество шагов должно быть целым числом."); }

            //ввод вне границ
            if (Math.Abs(Convert.ToDouble(TBXLeft.Text.Replace('.', ','))) > maxEdge)
                return new Exception(String.Format("Минимум Х должен по модулю быть меньше {1}",maxEdge));
            if (Math.Abs(Convert.ToDouble(TBXRight.Text.Replace('.', ','))) > maxEdge)
                return new Exception(String.Format("Максимум Х должен по модулю быть меньше {1}", maxEdge));
            if (Math.Abs(Convert.ToDouble(TBYBottom.Text.Replace('.', ','))) > maxEdge)
                return new Exception(String.Format("Минимум Y должен по модулю быть меньше {1}", maxEdge));
            if (Math.Abs(Convert.ToDouble(TBYTop.Text.Replace('.', ','))) > maxEdge)
                return new Exception(String.Format("Максимум Y должен по модулю быть меньше {1}", maxEdge));
            if (Math.Abs(Convert.ToInt32(TBCountOfStep.Text)) > maxStep ||
                            Math.Abs(Convert.ToInt32(TBCountOfStep.Text)) < minStep)
                return new Exception(String.Format("Количество шагов должно быть от {0} до {1}", minStep, maxStep));


            //разница
            if (Convert.ToDouble(TBXRight.Text.Replace('.', ',')) - Convert.ToDouble(TBXLeft.Text.Replace('.', ',')) < maxAbs)
                return new Exception(String.Format("Максимум x должен быть больше минимума х на {0}", maxAbs));
            if (Convert.ToDouble(TBYTop.Text.Replace('.', ',')) - Convert.ToDouble(TBYBottom.Text.Replace('.', ',')) < maxAbs)
                return new Exception(String.Format("Максимум y должен быть больше минимума y на {0}", maxAbs));

            return null;
        }



    }
}

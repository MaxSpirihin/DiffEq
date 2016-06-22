using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System.Reflection;
using DiffEq.Helpers;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Win32;

namespace DiffEq
{
    public partial class MainWindow : Window
    {
        ProgramState programState = new ProgramState();


        #region Вспомогательные переменные
        //список графиков на экране
        List<LineGraph> graphs = new List<LineGraph>();
        //оси
        LineGraph axes;
        //координаты мыши в момент нажатия на полотно правой
        double mouseX, mouseY;
        //для сохранения
        string fileName;
        StateSaver saver;
        #endregion




        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainContainer_Loaded);
            programState = new ProgramState();
            PrepareWorkPlace();
            WriteToConsole("Начало работы системы", false);
            saver = new StateSaver();
            UpdateWorkPlace();
        }


        private void PrepareWorkPlace()
        {
            SetFieldEdges(programState.pref.LeftX, programState.pref.RightX, programState.pref.BottomY, programState.pref.TopY);
            CPFunction.SetColor((Color)ColorConverter.ConvertFromString(programState.pref.FunctionColor));
            CPEquation.SetColor((Color)ColorConverter.ConvertFromString(programState.pref.EquationColor));
            SLFuncThinkness.Value = programState.pref.FuncThinnkness;
            SLEquationThinkness.Value = programState.pref.EqThinnkness;
            TBFunction.Text = programState.pref.Function;
            TBEquation.Text = programState.pref.Equation;
            TBX0.Text = programState.pref.x0;
            TBY0.Text = programState.pref.y0;

            ClearWorkPlace();

            
        }


        #region Работа с данными


        /// <summary>
        /// удаляет из данных все графики
        /// </summary>
        private void ClearAllGraphs()
        {
            programState.graphInfos = new List<GraphInfo>();
            saver.UpdateState(programState.graphInfos);
            UpdateWorkPlace();
            WriteToConsole("Полотно очищено");
        }

        /// <summary>
        /// добавляет к данным график
        /// </summary>
        private void AddGraph(bool IsEquation, double x0, double y0)
        {
            if (programState.graphInfos.Count >= programState.pref.MaxCountOfGraphs)
            {
                WriteToConsole("Построено максимальное количество графиков. Очистите полотно.", true);
                return;
            }

            try
            {
                programState.graphInfos.Add(new GraphInfo()
                {
                    color = (IsEquation ? CPEquation.getColor() : CPFunction.getColor()).ToString(),
                    Thinkness = IsEquation ? Convert.ToInt32(TBEquationThinkness.Text) :
                        Convert.ToInt32(TBFuncThinkness.Text),
                    Function = IsEquation ? TBEquation.Text : TBFunction.Text,
                    IsEquation = IsEquation,
                    x0 = x0,
                    y0 = y0
                });

                string extra = "";
                if (IsEquation)
                {
                    double? limitL;
                    double? limitR;
                    GraphBuilder.GetLimits(out limitL, out limitR, programState.graphInfos.Last().Function,
                        programState.pref.LeftX, programState.pref.RightX, programState.pref.CountOfSteps,
                        programState.graphInfos.Last().x0, programState.graphInfos.Last().y0);

                    if (limitL != null && limitR == null)
                        extra = String.Format(" Решение построено от точки x={0:0.00} т.к. y'(x,y)->∞", limitL.Value);
                    else if (limitL == null && limitR != null)
                        extra = String.Format(" Решение построено до точки x={0:0.00} т.к. y'(x,y)->∞", limitR.Value);
                    else if (limitL != null && limitR != null)
                        extra = String.Format(" Решение построено для x∈({0:0.00},{1:0.00}) т.к. y'(x,y)->∞", limitL.Value, limitR.Value);
                   
                }

                saver.UpdateState(programState.graphInfos);
                UpdateWorkPlace();

                string info = IsEquation ?
                    String.Format("Построено  решение дифференциального уравнения y'(x,y) = {0}; y({1:0.00}) = {2:0.00}." + extra+"\n",
                    TBEquation.Text, x0, y0) :
                    String.Format("Построен график функции y = {0:0.00}.\n", TBFunction.Text);

                WriteToConsole(info);
            }
            catch (Exception ex)
            {
                programState.graphInfos.RemoveAt(programState.graphInfos.Count - 1);
                WriteToConsole(ex.Message, true);
            }
        }

        /// <summary>
        /// добавляет к данным график
        /// </summary>
        private void AddGraph(bool IsEquation)
        {
            double x0 = Convert.ToDouble(TBX0.Text);
            double y0 = Convert.ToDouble(TBY0.Text);
            AddGraph(IsEquation, x0, y0);
        }

        /// <summary>
        /// открыть окно настроек
        /// </summary>
        private void OpenPref()
        {
            PrefWindow pw = new PrefWindow();
            pw.Owner = this;
            pw.ShowDialog();
        }


        private void CancelAction()
        {
            if (saver.AllowCancel())
            {
                programState.graphInfos = saver.Cancel();
                UpdateWorkPlace();

                WriteToConsole("Действие отменено.");
            }
        }

        private void ReturnAction()
        {
            if (saver.AllowReturn())
            {
                programState.graphInfos = saver.Return();
                UpdateWorkPlace();

                WriteToConsole("Действие возвращено.");
            }
        }


        /// <summary>
        /// обновить настройки
        /// </summary>
        public void UpdatePreferences(double leftX, double rightX, double topY, double bottomY, int countOfSteps, bool enableAxis)
        {
            programState.pref.LeftX = leftX;
            programState.pref.RightX = rightX;
            programState.pref.TopY = topY;
            programState.pref.BottomY = bottomY;
            programState.pref.CountOfSteps = countOfSteps;
            programState.pref.EnableAxis = enableAxis;

            //проходимся по графикам и удаляем те гре x0 уже не в пределах
            List<GraphInfo> deleteInfos = new List<GraphInfo>();
            foreach (GraphInfo gi in programState.graphInfos.Where(x => x.IsEquation &&
                (x.x0 < programState.pref.LeftX || x.x0 > programState.pref.RightX)))
                deleteInfos.Add(gi);

            foreach (GraphInfo dgi in deleteInfos)
                programState.graphInfos.Remove(dgi);

            WriteToConsole("Изменены параметры полотна");
            if (deleteInfos.Count() > 0)
                WriteToConsole(String.Format("Часть решений ({0}) дифф. уравнений было удалено т.к. точки x0 из их задачи Коши не попадают в пределы", deleteInfos.Count.ToString()));

            saver.ChangePrefs(programState.pref.LeftX, programState.pref.RightX);

            UpdateWorkPlace();
        }

        public Preferences getPref()
        {
            return programState.pref;
        }

        private void OpenHelp()
        {
            Help help = new Help();
            help.ShowDialog();
        }

        #endregion

        #region Отображение



        /// <summary>
        /// устанавливает грани для поля
        /// </summary>
        /// <param name="leftX">левая граница</param>
        /// <param name="rightX">правая граница</param>
        /// <param name="bottomY">нижняя граница</param>
        /// <param name="topY">верхняя граница</param>
        private void SetFieldEdges(double leftX, double rightX, double bottomY, double topY)
        {
            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.YRange = new DisplayRange(bottomY, topY);
            restr.XRange = new DisplayRange(leftX, rightX);
            Field.Viewport.Restrictions.Clear();
            Field.Viewport.Restrictions.Add(restr);
        }


        private void UpdateWorkPlace()
        {
            MenuCancel.IsEnabled = saver.AllowCancel();
            MenuReturn.IsEnabled = saver.AllowReturn();

            SetFieldEdges(programState.pref.LeftX, programState.pref.RightX, programState.pref.BottomY, programState.pref.TopY);

            ClearWorkPlace();

            foreach (GraphInfo info in programState.graphInfos)
            {
                BuildGraph(info);
            }
        }





        private void BuildGraph(GraphInfo info)
        {
            //достаем параметры
            double leftx = Field.Visible.Left;
            double rightx = Field.Visible.Right;
            double yExtraSize = Math.Abs(Field.Visible.Top - Field.Visible.Bottom);
            double yMax = Field.Visible.Bottom + yExtraSize;
            double yMin = Field.Visible.Top - yExtraSize;
            int N = programState.pref.CountOfSteps;
            CompositeDataSource source = info.IsEquation ?
                GraphBuilder.BuildSolution(info.Function, leftx, rightx, N, info.x0, info.y0, yMax, yMin) :
                GraphBuilder.BuildFunction(info.Function, leftx, rightx, N, yMax, yMin);

            //строим график
            string graphDescription = info.IsEquation ? "y'(x,y) = " + info.Function : "y(x) = " + info.Function;
            graphs.Add(Field.AddLineGraph(source, (Color)ColorConverter.ConvertFromString(info.color), info.Thinkness, graphDescription));
        }




        /// <summary>
        /// удаляет все графики с полотна
        /// </summary>
        private void ClearWorkPlace()
        {
            foreach (LineGraph gr in graphs)
                Field.Children.Remove(gr);
            graphs = new List<LineGraph>();

            if (axes != null)
            {
                Field.Children.Remove(axes);
                axes = null;
            }

            List<double> y;
            List<double> x;


            //рисуем оси
            if (programState.pref.EnableAxis)
            {
                y = new List<double> { 0, 0, Field.Visible.Bottom, Field.Visible.Bottom, Field.Visible.Top };
                x = new List<double> { Field.Visible.Left, Field.Visible.Right, Field.Visible.Right, 0, 0 };
            }
            else
            {
                y = new List<double> { 0, 0 };
                x = new List<double> { Field.Visible.Left, Field.Visible.Left };
            }


            axes = Field.AddLineGraph(x.AsXDataSource().Join(y.AsYDataSource()), Colors.Black, 1, "Графики:");



        }



        #endregion

        #region Обработчики

        private void ButtonClearConsole_Click(object sender, RoutedEventArgs e)
        {
            ClearConsole();
        }

        private void SLFuncThinkness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TBFuncThinkness.Text = SLFuncThinkness.Value.ToString();
        }

        private void ButtonFunc_Click(object sender, RoutedEventArgs e)
        {
            AddGraph(false, 0, 0);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAllGraphs();

        }

        private void ButtonDiffEq_Click(object sender, RoutedEventArgs e)
        {
            AddGraph(true);
        }

        private void SLEquationThinkness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TBEquationThinkness.Text = SLEquationThinkness.Value.ToString();
        }


        private void BuildSilutionAtPoint_Click(object sender, RoutedEventArgs e)
        {


            AddGraph(true, mouseX, mouseY);
        }


        private void SavePicture_Click(object sender, RoutedEventArgs e)
        {
            SavePicture();
        }

        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            SaveLogFile();
        }

        private void Field_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var transform = Field.Transform;
            var mouseScreenPosition = Mouse.GetPosition(Field.CentralGrid);
            var mousePositionInData = mouseScreenPosition.ScreenToViewport(transform);

            mouseX = mousePositionInData.X;
            mouseY = mousePositionInData.Y;
        }


        private void PrefOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenPref();
        }

        private void CancelAction_Click(object sender, RoutedEventArgs e)
        {
            CancelAction();
        }

        private void ReturnAction_Click(object sender, RoutedEventArgs e)
        {
            ReturnAction();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            OpenHelp();
        }

        void MainContainer_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (Application.Current.Properties["ArbitraryArgName"] != null)
            {
                string fname = Application.Current.Properties["ArbitraryArgName"].ToString();
                string text = System.IO.File.ReadAllText(fname);

                try
                {
                    ProgramState ps = ProgramState.DeSerialize(text);
                    programState = ps;
                    saver = new StateSaver();
                    PrepareWorkPlace();
                    UpdateWorkPlace();
                    fileName = fname;
                    WriteToConsole("Начало работы системы", false);
                    WriteToConsole("Состояние системы загружено из файла \"" + fname + "\"");
                }
                catch(Exception ex) {
                    WriteToConsole("Ошибка при загрузке файла.", true); }
            }
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                New();
            }

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save();
                e.Handled = true;
            }
   
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Open();
            }


            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Open();
            }


            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CancelAction();
            }

            if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ReturnAction();
            }

            if (e.Key == Key.F1)
            {
                OpenHelp();
                e.Handled = true;
            }


        }

        #endregion

        #region Файлы

        /// <summary>
        /// сохраняет картинку с графиками
        /// </summary>
        private void SavePicture()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    Field.SaveScreenshot(saveFileDialog.FileName);
                    WriteToConsole("Изображение графиков сохранено в файл \"" + saveFileDialog.FileName + "\"");
                }
                catch (Exception)
                {
                    WriteToConsole("Не удалось сохранить изображение. Неверный формат файла", true);
                }
            }
        }

        /// <summary>
        /// сохраняет в файл лог из консоли
        /// </summary>
        private void SaveLogFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName,
                    new TextRange(TBConsole.Document.ContentStart, TBConsole.Document.ContentEnd).Text);
                WriteToConsole("Лог сохранен в файл \"" + saveFileDialog.FileName + "\"");
            }
        }

        /// <summary>
        /// сохраняет состояние системы
        /// </summary>
        private void Save()
        {
            //доставнем все параметры
            programState.pref.Equation = TBEquation.Text;
            programState.pref.EquationColor = CPEquation.getColor().ToString();
            programState.pref.Function = TBFunction.Text;
            programState.pref.FunctionColor = CPFunction.getColor().ToString();
            programState.pref.FuncThinnkness = (int) SLFuncThinkness.Value;
            programState.pref.EqThinnkness = (int)SLEquationThinkness.Value;
            programState.pref.x0 = TBX0.Text;
            programState.pref.y0 = TBY0.Text;



            if (String.IsNullOrEmpty(fileName))
            {
                SaveAs();
                return;
            }

            System.IO.File.WriteAllText(fileName,
                    programState.Serialize());
            WriteToConsole("Состояние системы сохранено в файл.");

        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DiffEq File|*.dfq";
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName,
                    programState.Serialize());
                fileName = saveFileDialog.FileName;
                WriteToConsole("Состояние системы сохранено в файл \"" + saveFileDialog.FileName + "\"");
            }
        }


        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DiffEq File|*.dfq";
            if (openFileDialog.ShowDialog() == true)
            {
                string text = System.IO.File.ReadAllText(openFileDialog.FileName);

                try
                {
                    ProgramState ps = ProgramState.DeSerialize(text);
                    programState = ps;
                    saver = new StateSaver();
                    PrepareWorkPlace();
                    UpdateWorkPlace();
                    fileName = openFileDialog.FileName;
                    WriteToConsole("Состояние системы загружено из файла \"" + openFileDialog.FileName + "\"");
                }
                catch { WriteToConsole("Ошибка при загрузке файла", true); }
            }
        }

        private void New()
        {
            fileName = null;
            programState = new ProgramState();
            PrepareWorkPlace();
            saver = new StateSaver();
            UpdateWorkPlace();
        }

        #endregion

        #region Консоль


        private void WriteToConsole(string text)
        {
            WriteToConsole(text, false);
        }

        private void WriteToConsole(string text, bool IsError)
        {
            Color color = IsError ? Colors.Red : Colors.Black;
            TBConsole.AppendText(DateTime.Now.ToString("HH:mm:ss") + (IsError ? " ERROR: " : " ") + text, color.ToString());
            SVConsole.ScrollToBottom();
        }

        private void ClearConsole()
        {
            TBConsole.Document.Blocks.Clear();
        }


        #endregion

        



        

        

        


        

        



    }


}

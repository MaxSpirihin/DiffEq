using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiffEq.Helpers
{

   public class DefaultPreferences
    {
        public const int COUNT_OF_STEPS = 1000;
        public const double LEFT_X = -10;
        public const double RIGHT_X = 10;
        public const double BOTTOM_Y = -5;
        public const double TOP_Y = 5;
        public const int MAX_COUNT_OF_GRAPHS = 10;
        public const bool ENABLE_AXIS = true;
        public const string FUNCTION = "Sin(x)";
        public const string EQUATION = "x + 1";
        public const string X0 = "0";
        public const string Y0 = "0";
        public const int THINKNESS = 2;
        public static readonly string FUNCTION_COLOR = Colors.Blue.ToString();
        public static readonly string EQUATION_COLOR = Colors.Red.ToString();
    }


    /// <summary>
    /// содержит все настройки приложения
    /// </summary>
    public class Preferences
    {

        public int CountOfSteps { get; set; }
        public double LeftX { get; set; }
        public double RightX { get; set; }
        public double BottomY { get; set; }
        public double TopY { get; set; }
        public int MaxCountOfGraphs { get; set; }
        public bool EnableAxis { get; set; }
        public string Function { get; set; }
        public string Equation { get; set; }
        public string x0 { get; set; }
        public string y0 { get; set; }
        public int FuncThinnkness { get; set; }
        public int EqThinnkness { get; set; }
        public string FunctionColor { get; set; }
        public string EquationColor { get; set; }


        public Preferences()
        {
            CountOfSteps = DefaultPreferences.COUNT_OF_STEPS;
            LeftX = DefaultPreferences.LEFT_X;
            RightX = DefaultPreferences.RIGHT_X;
            BottomY = DefaultPreferences.BOTTOM_Y;
            TopY = DefaultPreferences.TOP_Y;
            MaxCountOfGraphs = DefaultPreferences.MAX_COUNT_OF_GRAPHS;
            EnableAxis = DefaultPreferences.ENABLE_AXIS;
            Function = DefaultPreferences.FUNCTION;
            Equation = DefaultPreferences.EQUATION;
            x0 = DefaultPreferences.X0;
            y0 = DefaultPreferences.Y0;
            FunctionColor = DefaultPreferences.FUNCTION_COLOR;
            EquationColor = DefaultPreferences.EQUATION_COLOR;
            FuncThinnkness = DefaultPreferences.THINKNESS;
            EqThinnkness = DefaultPreferences.THINKNESS;
        }
    }
}

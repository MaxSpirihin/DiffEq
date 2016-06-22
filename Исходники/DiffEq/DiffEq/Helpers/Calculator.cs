using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffEq.Helpers
{
    /// <summary>
    /// позволяет вычислять функции по строковому представлению
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// функция типа f(x)
        /// </summary>
        public static double Fx(string function, double x)
        {
            Expression expression = new Expression(function);
            expression.Parameters.Add("x", x);
            expression.Parameters.Add("X", x);
            return Convert.ToDouble(expression.Evaluate());
        }

        /// <summary>
        /// функция типа F(x,y)
        /// </summary>
        public static double Fxy(string function, double x, double y)
        {
            Expression expression = new Expression(function);
            expression.Parameters.Add("x", x);
            expression.Parameters.Add("X", x);
            expression.Parameters.Add("y", y);
            expression.Parameters.Add("Y", y);
            return Convert.ToDouble(expression.Evaluate());
        }

    }
}

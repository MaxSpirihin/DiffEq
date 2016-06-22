using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffEq.Helpers
{
    /// <summary>
    /// создает функцию из строкового представления и позволяет вычислять значения
    /// </summary>
    public class Function
    {
        Expression expression;
        

        public Function(string function)
        {
            function = Update(function);
            expression = new Expression(function);
        }

        /// <summary>
        /// функция типа f(x)
        /// </summary>
        public double Compute(double x)
        {
            
            expression.Parameters.Clear();
            expression.Parameters.Add("x", x);
            expression.Parameters.Add("X", x);
            return Convert.ToDouble(expression.Evaluate());
        }

        /// <summary>
        /// функция типа F(x,y)
        /// </summary>
        public double Compute(double x, double y)
        {
            expression.Parameters.Clear();
            expression.Parameters.Add("x", x);
            expression.Parameters.Add("X", x);
            expression.Parameters.Add("y", y);
            expression.Parameters.Add("Y", y);
            return Convert.ToDouble(expression.Evaluate());
        }


        private string Update(string s)
        {
            s = s.Replace("sin", "Sin");
            s = s.Replace("cos", "Cos");
            s = s.Replace("tan", "Tan");
            s = s.Replace("asin", "Asin");
            s = s.Replace("acos", "Acos");
            s = s.Replace("atan", "Atan");
            s = s.Replace("tg", "Tan");
            s = s.Replace("exp", "Exp");
            s = s.Replace("pow", "Pow");
            s = s.Replace("abs", "Abs");
            s = s.Replace("log", "Log");
            s = s.Replace("EXP", "Exp(1)");
            s = s.Replace("PI", "3.1415");
           return s;
        }
    }
}

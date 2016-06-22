using Microsoft.Research.DynamicDataDisplay.DataSources;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace DiffEq.Helpers
{
    /// <summary>
    /// класс ответственен за построение точек для графика
    /// </summary>
    class GraphBuilder
    {

        const int INFINITY = 100000;
        const double MAXDER = 2;

        private static double Rk4(Function func, double x1, double y1, double h)
        {
            double k1, k2, k3, k4;
            k1 = func.Compute(x1, y1);
            k2 = func.Compute(x1 + h / 2, y1 + h / 2);
            k3 = func.Compute(x1 + h / 2, y1 + h * k2 / 2);
            k4 = func.Compute(x1 + h, y1 + h * k3);
            return y1 + h * (k1 + 2 * k2 + 2 * k3 + k4) / 6;
        }


        private static double Rk5(Function func, double x1, double y1, double h)
        {
            double k1, k2, k3, k4, k5, k6;
            k1 = h * func.Compute(x1, y1);
            k2 = h * func.Compute(x1 + h / 4, y1 + k1 / 4);
            k3 = h * func.Compute(x1 + 3 * h / 8, y1 + 3 * k1 / 32 + 9 * k2 / 32);
            k4 = h * func.Compute(x1 + 12 * h / 13, y1 + 1932 * k1 / 2197 - 7200 * k2 / 2197 + 7296 * k3 / 2197);
            k5 = h * func.Compute(x1 + h, y1 + 439 * k1 / 216 - 8 * k2 + 3680 * k3 / 513 - 845 * k4 / 4104);
            k6 = h * func.Compute(x1 + h / 2, y1 - 8 * k1 / 27 + 2 * k2 - 3544 * k3 / 2565 + 1859 * k4 / 4104 - 11 * k5 / 40);

            double y4 = y1 + 25 * k1 / 216 + 1408 * k3 / 2565 + 2197 * k4 / 4101 - k5 / 5;
            double y5 = y1 + 16 * k1 / 135 + 6656 * k3 / 12825 + 28561 * k4 / 56430 - 9 * k5 / 50 + 2 * k6 / 55;

            return y5;
        }

        private static double Rk5(Function func, double x1, double y1, double h, out bool derivativeIsPositive)
        {
            derivativeIsPositive = func.Compute(x1, y1) > 0;
            return Rk5(func, x1, y1, h);
        }


        private static double Rkf45(Function func, double x1, double y1, double h, out double nextH)
        {
            double k1, k2, k3, k4, k5, k6;
            k1 = h * func.Compute(x1, y1);
            k2 = h * func.Compute(x1 + h / 4, y1 + k1 / 4);
            k3 = h * func.Compute(x1 + 3 * h / 8, y1 + 3 * k1 / 32 + 9 * k2 / 32);
            k4 = h * func.Compute(x1 + 12 * h / 13, y1 + 1932 * k1 / 2197 - 7200 * k2 / 2197 + 7296 * k3 / 2197);
            k5 = h * func.Compute(x1 + h, y1 + 439 * k1 / 216 - 8 * k2 + 3680 * k3 / 513 - 845 * k4 / 4104);
            k6 = h * func.Compute(x1 + h / 2, y1 - 8 * k1 / 27 + 2 * k2 - 3544 * k3 / 2565 + 1859 * k4 / 4104 - 11 * k5 / 40);

            double y4 = y1 + 25 * k1 / 216 + 1408 * k3 / 2565 + 2197 * k4 / 4101 - k5 / 5;
            double y5 = y1 + 16 * k1 / 135 + 6656 * k3 / 12825 + 28561 * k4 / 56430 - 9 * k5 / 50 + 2 * k6 / 55;


            double s = Math.Pow(0.001/ (2 * Math.Abs(y5 - y4)), 0.25);

            nextH = s * h;

            return y5;
        }



        /// <summary>
        /// Возвращает массив точек для графика
        /// </summary>
        /// <param name="function">Строковое представление ф-ии y(x)=</param>
        /// <param name="leftX">Левая граница по х</param>
        /// <param name="rightX">Правая граница по х</param>
        /// <param name="N">Кол-во шагов</param>
        /// <param name="yMax">Максимальное значение y коор-ты (Точки с y больше yMax не будут добавлены)</param>
        /// <param name="yMin">Минимальное значение y коор-ты (Точки с y меньше yMin не будут добавлены)</param>
        /// <returns></returns>
        public static CompositeDataSource BuildFunction(string function, double leftX, double rightX, int N, double yMax, double yMin)
        {
            Function func = new Function(function);

            List<double> x = new List<double>();
            List<double> y = new List<double>();

            double step = (rightX - leftX) / N;

            //вычисление и добавление точек
            for (int i = 0; i <=N; i++)
            {
                double currentX = leftX + i * step;
                double currentY = func.Compute(currentX);

                if (currentY > yMin*5 && currentY < yMax*5)
                {
                    x.Add(currentX);
                    y.Add(currentY);
                }

            }

            // Перевод в данные:
            var xDataSource = x.AsXDataSource();
            var yDataSource = y.AsYDataSource();

            return xDataSource.Join(yDataSource);

        }


        /// <summary>
        /// Возвращает массив точек для решения дифф.ур-ия с задачей Коши
        /// </summary>
        /// <param name="function">Строковое представление ф-ии y'(x)=</param>
        /// <param name="leftX">Левая граница по х</param>
        /// <param name="rightX">Правая граница по х</param>
        /// <param name="N">Кол-во шагов</param>
        /// <param name="x0">x-координата точки для задачи Коши. Должна быть в пределах [leftX; rightX]</param>
        /// <param name="y0">y-координата точки для задачи Коши</param>
        /// <param name="yMax">Максимальное значение y коор-ты (Точки с y больше yMax не будут добавлены)</param>
        /// <param name="yMin">Минимальное значение y коор-ты (Точки с y меньше yMin не будут добавлены)</param>
        /// <returns></returns>
        public static CompositeDataSource BuildSolution(string function, double leftX, double rightX, int N, double x0, double y0, double yMax, double yMin)
        {

            //отлавливаем ошибки
            if (x0 < leftX || x0 > rightX)
                throw new Exception("x0 должет быть в пределах [ " + leftX.ToString() + " ; " + rightX.ToString() + " ] ");


            Function func = new Function(function);
            double currentX, currentY, lastD, nextD;
            double yForAdd;
            bool derivativeIsPositive;
            double step = (rightX - leftX) / N;

            //идем от исходной точки вправо
            List<double> xRight = new List<double>();
            List<double> yRight = new List<double>();
            currentX = x0;
            currentY = y0;
            lastD = func.Compute(x0, y0);

            while (currentX < rightX)
            {
                xRight.Add(currentX);
                yForAdd = currentY < yMin ? yMin : currentY > yMax ? yMax : currentY;
                yRight.Add(yForAdd);

                currentX += step;
                currentY = Rk5(func, currentX, currentY, step, out derivativeIsPositive);
                nextD = func.Compute(currentX, currentY);

                if ((Double.IsNaN(currentY) || Math.Abs(currentY) > INFINITY) || DerCompare(lastD, nextD))
                {
                    //добавим предельную точку и брейк
                    xRight.Add(currentX);
                    yForAdd = lastD > 0 ? yMax : yMin;
                    yRight.Add(yForAdd);
                    break;
                }

                lastD = nextD;
            }

            /*
            for (int i = 0; i < 1000; i++ )
            {
                if (currentY > yMin && currentY < yMax)
                {
                    xRight.Add(currentX);
                    yRight.Add(currentY);
                }
                currentX += step;

                currentY = Rkf45(func, currentX, currentY, step,out step);
                
            }*/


            //идем от исходной точки влево
            List<double> xLeft = new List<double>();
            List<double> yLeft = new List<double>();
            currentX = x0;
            currentY = y0;
            lastD = func.Compute(x0, y0);
            while (currentX > leftX)
            {
                xLeft.Add(currentX);
                yForAdd = currentY < yMin ? yMin : currentY > yMax ? yMax : currentY;
                yLeft.Add(yForAdd);

                currentX -= step;
                currentY = Rk5(func, currentX, currentY, -step, out derivativeIsPositive);
                nextD = func.Compute(currentX, currentY);

                if ((Double.IsNaN(currentY) || Math.Abs(currentY) > INFINITY) || DerCompare(lastD, nextD))
                {
                    //добавим предельную точку и брейк
                    xLeft.Add(currentX);
                    yForAdd = lastD > 0 ? yMin : yMax;
                    yLeft.Add(yForAdd);
                    break;
                }
                lastD = nextD;
            }


            //Объединение и перевод в данные:
            xLeft.Reverse();
            yLeft.Reverse();
            xLeft.AddRange(xRight);
            yLeft.AddRange(yRight);
            var xDataSource = xLeft.AsXDataSource();
            var yDataSource = yLeft.AsYDataSource();

            return xDataSource.Join(yDataSource);

        }


        /// <summary>
        /// возвращает точки по х, где апппроксимация перестает работать т.к. ф-ия уходит в бесконечность
        /// </summary>
        /// <param name="limitL"></param>
        /// <param name="limitR"></param>
        /// <param name="function"></param>
        /// <param name="leftX"></param>
        /// <param name="rightX"></param>
        /// <param name="N"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        public static void GetLimits(out double? limitL, out double? limitR,string function, double leftX, double rightX, int N, double x0, double y0)
        {
            //отлавливаем ошибки
            if (x0 < leftX || x0 > rightX)
                throw new Exception("x0 должет быть в пределах [ " + leftX.ToString() + " ; " + rightX.ToString() + " ] ");

            Function func = new Function(function);
            double currentX, currentY,lastD, nextD;
            double step = (rightX - leftX) / N;
            limitL = null;
            limitR = null;

            //идем от исходной точки вправо
            currentX = x0;
            currentY = y0;
            lastD = func.Compute(x0, y0);
            while (currentX < rightX)
            {
                currentX += step;
                currentY = Rk5(func, currentX, currentY, step);
                nextD = func.Compute(currentX, currentY);
                if ((Double.IsNaN(currentY) || Math.Abs(currentY) > INFINITY) || DerCompare(lastD, nextD))
                {
                    //запоминаем точку
                    limitR = currentX;
                    break;
                }
                lastD = nextD;
            }


            //идем от исходной точки влево
            currentX = x0;
            currentY = y0;
            lastD = func.Compute(x0, y0);
            while (currentX > leftX)
            {
                currentX -= step;
                currentY = Rk5(func, currentX, currentY, -step);
                nextD = func.Compute(currentX, currentY);

                if ((Double.IsNaN(currentY) || Math.Abs(currentY) > INFINITY) || DerCompare(lastD,  nextD))
                {
                    //запоминаем точку
                    limitL = currentX;
                    break;
                }
                lastD = nextD;
            }
        }

        private static bool DerCompare(double lastD, double nextD)
        {
            if (Math.Sign(lastD) == Math.Sign(nextD)) return false;
            return (Math.Abs(nextD - lastD) > MAXDER);

        }

    }
}

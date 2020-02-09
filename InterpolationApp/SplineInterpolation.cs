using System;
using System.Collections.Generic;
using System.Text;

namespace Spline_Test
{
    //Класс интерполяции кубическим сплайном. Конструктор с векторами x и y, и (необязательно)
    //значение первой производной на конце, вызываем interp для интерполяции.
    class SplineInterpolation : BaseInterpolation
    {
        List<double> Y2;
        public SplineInterpolation( List<double> xv, List<double> yv, double yp1 = 1e+99, double ypn = 1e+99 )
            : base( xv,yv, 2)
        {
            Y2 = new List<double>();
            foreach (var i in xv) Y2.Add(0);
            
            Set_y2( yp1, ypn );
        }
        //Этот метод хранит массив Y2[0..n-1] с вторыми производными интерполяционной функции в заданных массивами X, y точках.
        //Если yp1 и/или ypn равны 1x10^99 или больше, тогда алгоритм устанавливает граничные условия для натурального сплайна,
        //с нулевыми вторыми производными на этих границах, иначе производные на концах будут первого порядка.
        private void Set_y2(double yp1, double ypn)
        {
            List<double> u = new List<double>();
            for (int j = 0; j < Y2.Count - 1; j++) u.Add(0);
            int n = u.Count;

            if( yp1 > 0.99e99 ) //Нижнее граничное условие устанавливается или в натуральное или в указанное значение первой производной.
                Y2[0] = u[0] = 0.0;
            else //Краевые условия 1-го типа
            {
                Y2[0] = -0.5;
                u[0] = ( 3.0 / ( X[1] - X[0] ) ) * ( ( Y[1] - Y[0] ) / ( X[1] - X[0] ) - yp1 );
            }
            for(int i = 1; i < n; i++ )//Прогонка, прямой ход. Y2 используется для временного хранения разложения.
            { 
                double sig = ( X[i] - X[i - 1] ) / ( X[i + 1] - X[i - 1] );
                double p = 2.0 - sig * Y2[i - 1]; //sig * Y2[i-1] + 2.0
                Y2[i] = ( sig - 1.0 ) / p;
                double d = 6.0 * (( Y[i + 1] - Y[i] ) / ( X[i + 1] - X[i] ) - ( Y[i] - Y[i - 1] ) / ( X[i] - X[i - 1] )) / ( X[i + 1] - X[i - 1] );
                u[i] = ( d - sig * u[i - 1] ) / p;
            }
            double qn, un;
            if( ypn > 0.99e99 ) //Верхнее граничное условие устанавливается или в натуральное или в указанное значение первой производной.
                qn = un = 0.0;
            else //Краевые условия 1-го типа
            {
                qn = 0.5;
                un = ( 3.0 / ( X[n - 1] - X[n - 2] ) ) * ( ypn - ( Y[n - 1] - Y[n - 2] ) / ( X[n - 1] - X[n - 2] ) );
            }
            Y2[n - 1] = ( un - qn * u[n - 2] ) / ( qn * Y2[n - 2] + 1.0 );
            for(int k = n - 1; k >= 0; k-- )//Прогонка, обратный ход.
                Y2[k] = Y2[k] * Y2[k + 1] + u[k];
        }

        //Подается значение x, и используются указатели на данные xx и yy, и хранимый вектор вторых производных Y2,
        //возвращает интерполированное кубическим сплайном значение y. 
        public override double RawInterpolation( int j, double x )
        {
            double h = X[j+1] - X[j];
            if( h == 0.0 )
                throw new Exception("Bad input to routine splint" ); //Значения x_j+1 и x_j должны быть различными.

            double a = ( X[j+1] - x ) / h; //A         = (x_j+1 - x) / (x_j+1 - x_j)
            double b = 1 - a;              //B = 1 - A = (x - x_j) / (x_j+1 - x_j) 
                                           //C = 1/6 (A^3 - A) (x_j+1 - x_j)^2 
                                           //D = 1/6 (B^3 - B) (x_j+1 - x_j)^2
                                           //              h = (x_j+1 - x_j)
            //y = Ay + By + Cy'' + Dy''
            //      j    j+1  j      j+1
            return a * Y[j] + b * Y[j+1] + ( ( a * a * a - a ) * Y2[j] + ( b * b * b - b ) * Y2[j + 1] ) * ( h * h ) / 6.0;
                                                                                            //1/6 и h выносим за скобки
        }

    }

};



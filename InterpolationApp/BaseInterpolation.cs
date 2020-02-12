using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Spline_Test
{
    abstract class BaseInterpolation
    {
        private readonly int _m;

        protected List<double> X;
        protected List<double> Y;

        protected BaseInterpolation( List<double> x, List<double> y, int m)
        {
            _m = m;                                                   //Это размер интервала который мы берем.
            X = x;
            Y = y;
        }

        // Только эту функцию вызывает пользователь.
        public double Interpolate(double x)
        {
            return RawInterpolation(Locate(x), x); //первый параметр отступ слева //RawInterpolation( jLeftOffset, x );
        }

        private int Locate(double x)
        {
            if( X.Count < 2 || _m < 2 || _m > X.Count )
                throw new Exception( "locate size error" );
            bool @ascending = (X[X.Count-1] >= X[0]);   //Локальная перменная По возрастанию = true

            int jLower = 0;                             //Инициализируем верхний предел
            int jUpper = X.Count - 1;                   //Инициализируем нижний предел
            while ( jUpper - jLower > 1 )               //Вычисление средней точки
            {
                int jMiddle = ( jUpper + jLower ) >> 1; //Деление на 2 остаток отсекается.
                if( x >= X[jMiddle] == @ascending )
                    jLower = jMiddle;             
                else
                    jUpper = jMiddle;
            }
            //_correlation = Math.Abs( jLower - _jsav ) <= _dj; //Корреляция
            //_jsav = jLower;
            //
            return jLower;//Math.Max( 0, Math.Min( X.Count - _m, jLower - ( ( _m - 1 ) >> 1 ) ) );
        }

        //Наследуемый класс используется в качестве фактического метода интерполяции.
        public abstract double RawInterpolation( int jlo, double x );
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
//Remastered
namespace Spline_Test
{
    abstract class BaseInterpolation
    {
        private readonly int _m;
        //private int _jsav;
        //private bool _correlation;
        //private readonly int _dj;

        protected List<double> X;
        protected List<double> Y;

        protected BaseInterpolation( List<double> x, List<double> y, int m)
        {
            _m = m;                                                   //Это размер интервала который мы берем.
            //_jsav = 0;
            //_correlation = false;
            X = x;
            Y = y;
            //_dj = Math.Min( 1, (int)Math.Pow( (double)X.Count, 0.25 ) );   //?? Max
        }
        // Только эту функцию вызывает пользователь.
        public double Interpolate(double x)
        {
            //int jLeftOffset = _correlation ? Hunt(x) : Locate(x);   //Отуступ слева
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

        //private int Hunt(double x)
        //{
        //    int jl = _jsav, jm, ju, inc = 1;
        //    if( X.Count < 2 || _m < 2 || _m > X.Count )
        //        throw new Exception( "hunt size error" );
        //    bool ascnd = (X[X.Count-1] >= X[0]);
        //    if (jl < 0 || jl > X.Count - 1)
        //    {
        //        jl = 0;
        //        ju = X.Count - 1;
        //    }
        //    else
        //    {
        //        if (x >= X[jl] == ascnd)
        //        {
        //            for (;;)
        //            {
        //                ju = jl + inc;
        //                if (ju >= X.Count - 1)
        //                {
        //                    ju = X.Count - 1; 
        //                    break; 
        //                }
        //                else if (x < X[ju] == ascnd)
        //                    break;
        //                else
        //                {
        //                    jl = ju;
        //                    inc += inc;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ju = jl;
        //            for (;;)
        //            {
        //                jl = jl - inc;
        //                if( jl <= 0 )
        //                {
        //                    jl = 0;
        //                    break;
        //                }
        //                else if( x >= X[jl] == ascnd )
        //                    break;
        //                else
        //                {
        //                    ju = jl;
        //                    inc += inc;
        //                }
        //            }
        //        }
        //    }

        //    while (ju-jl > 1)
        //    {
        //        jm = ( ju + jl ) >> 1;
        //        if( x >= X[jm] == ascnd )
        //            jl = jm;
        //        else
        //        {
        //            ju = jm;
        //        }
        //    }
        //    _correlation = Math.Abs( jl - _jsav ) <= _dj;
        //    _jsav = jl;
        //    return Math.Max( 0, Math.Min( X.Count - _m, jl - ( ( _m - 2 ) >> 1 ) ) );
        //}
        //Наследуемый класс используется в качестве фактического метода интерполяции.
        public abstract double RawInterpolation( int jlo, double x );
    }
}

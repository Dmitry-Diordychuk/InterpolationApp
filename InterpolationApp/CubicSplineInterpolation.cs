using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Xml.XPath;
using Spline_Test;

namespace InterpolationApp
{
    class CubicSplineInterpolation : BaseInterpolation
    {
        List<double> CoefA;
        List<double> CoefB;
        List<double> CoefC;
        List<double> CoefD;
        public CubicSplineInterpolation( List<double> xv, List<double> yv, double yp1 = 1e+99, double ypn = 1e+99 )
            : base( xv, yv, 2 )
        {
            CoefA = new List<double>();
            CoefB = new List<double>();
            CoefC = new List<double>();
            CoefD = new List<double>();

            Set();//yp1, ypn );
        }

        List<double> TridagSolve(List<double> a, List<double> b, List<double> c, List<double> d)
        {
            int n = X.Count;
            List<double> x = new List<double>(new double[n]);
            List<double> alfa = new List<double>(new double[n-1]);
            List<double> beta = new List<double>(new double[n-1]);

            for (int i = 0; i < n-1; i++)
            {
                if( i == 0 )
                {
                    alfa[0] = ( -c[0] ) / b[0];   //0 индекс в дальнейшем не нужен.
                    beta[0] = d[0] / b[0];
                }
                else
                {
                    alfa[i] = -c[i] / ( a[i] * alfa[i-1] + b[i] );
                    beta[i] = ( d[i] - a[i] * beta[i-1] ) / ( a[i] * alfa[i-1] + b[i] );
                }
            }

            x[n - 1] = ( d[n - 1] - a[n - 1] * beta[n - 2] ) / ( b[n - 1] + a[n - 1] * alfa[n - 2] );
            for (int i = n-2; i > -1; i--)
            {
                x[i] = alfa[i] * x[i + 1] + beta[i];
            }

            return x;
        }

        private void Set( )//double yp1, double ypn )
        {
            int n = X.Count;

            double[] hi = new double[n-1];
            for (int i = 0; i < n-1; i++)
            {
                hi[i] = X[i + 1] - X[i];
            }

            List<double> tridiagA = new List<double>();
            for (int i = 0; i < n-1; i++)                   
            {
                tridiagA.Add(hi[i]);
            }
            tridiagA.Add( 0 );

            List<double> tridiagB = new List<double>(){1};
            for (int i = 0; i < n-2; i++)
            {
                tridiagB.Add(2*(hi[i]+hi[i+1]));
            }
            tridiagB.Add(1);

            List<double> tridiagC = new List<double>(){0};
            for (int i = 0; i < n-1; i++)
            {
                tridiagC.Add(hi[i]);
            }

            List<double> tridiagD = new List<double>{0};
            for (int i = 1; i < n-1; i++)
            {
                tridiagD.Add(3*( (Y[i+1]-Y[i])/(X[i+1]-X[i]) - (Y[i]-Y[i-1])/(X[i]-X[i-1]) ));
            }
            tridiagD.Add(0);

            List<double> c  = TridagSolve(tridiagA, tridiagB, tridiagC, tridiagD);

            List<double> a = Y;

            List<double> b = new List<double>();
            for( int i = 0; i < n - 1; i++ )
            {
                b.Add( ( Y[i + 1] - Y[i] ) / hi[i] - ( c[i + 1] + 2 * c[i] ) * ( hi[i] ) / 3 );
            }

            List<double> d = new List<double>();
            for( int i = 0; i < n-1; i++ )
            {
                d.Add( ( c[i + 1] - c[i] ) / ( 3 * hi[i] ) );
            }

            for( int i = 0; i < n-1; i++ )
            {
                CoefA.Add(a[i]);
                CoefB.Add(b[i]);
                CoefC.Add(c[i]);
                CoefD.Add(d[i]);
            }
        }

        public override double RawInterpolation( int i, double x )
        {
            double h = x - X[i];
            //if( h == 0.0 )
                //return x;
              //  throw new Exception( "В таблице есть точки с одинаковым значение абсцисс!" );
            //(3)-(4)
            
            return CoefA[i] + CoefB[i] * h + CoefC[i] * ( h * h ) + CoefD[i] * ( h * h * h );
        }
    }
}

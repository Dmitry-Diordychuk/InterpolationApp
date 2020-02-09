using System;
using System.Collections.Generic;
using System.Text;

namespace InterpolationApp
{
    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public Point( double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static double PointToX( Point point )
        {
            return point.x;
        }

        public static double PointToY( Point point )
        {
            return point.y;
        }

        public override string ToString()
        {
            return $"x:{Math.Round(x,2)}\ny:{Math.Round(y,2)}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace InterpolationApp
{
    public static class File
    {
        private static string path = @"data\table.tb";
        /*
         * Разделителем в файле выступает символ |. Левое значение x правое y.
         * Пример:
         *        0|0
         *        1|5
         *        3|7  
         */
        public static List<Point> LoadFile(string _path = @"data\table.tb")
        {
            try
            {
                List<Point> result = new List<Point>();
                using (StreamReader sr = new StreamReader(_path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] separatedLine = line.Split( '|' );
                        double x, y;
                        if (Double.TryParse(separatedLine[0].Replace('.',','), out x) && 
                            Double.TryParse(separatedLine[1].Replace( '.', ',' ), out y))
                        {
                            result.Add( new Point( x, y ) );
                        }
                        else
                        {
                            MessageBox.Show( "Ошибка чтения файла!" );
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show( e.Message );
                throw;
            }
        }

        public static void SaveFile( List<Point> points, string _path = @"data\table.tb")
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_path))
                {
                    foreach (var p in points)
                    {
                        sw.WriteLine($"{p.x}|{p.y}");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show( e.Message );
                throw;
            }
        }
    }
}

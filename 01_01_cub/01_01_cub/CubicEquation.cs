using System; 
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;


namespace _01_01_cub
{
    static class CubicEquation
    {
        public static Complex[] Kordan(double a, double b, double c, double d)
        {
            if (a.CompareTo(0) == 0)
            {
                throw new Exception("Not cubic Equation: param a = 0");
            }

            Complex[] roots = new Complex[3];

            double p = (3 * a * c - b * b) / (3 * a * a);
            double q = (2 * Math.Pow(b, 3) - 9 * a * b * c + 27 * a * a * d) / (27 * Math.Pow(a, 3));

            double Q = Math.Pow((p / 3), 3) + Math.Pow((q / 2), 2);

            Complex y1;
            Complex y2;
            Complex y3;

            double param = b / (3 * a);

            if (Q > 0)
            {
                double sumtmp = Sqrt3(-q / 2 + Math.Sqrt(Q)) + Sqrt3(-q / 2 - Math.Sqrt(Q));
                double diftmp = Sqrt3(-q / 2 + Math.Sqrt(Q)) - Sqrt3(-q / 2 - Math.Sqrt(Q));
                y1 = sumtmp -param;
                y2 = new Complex(-1.0 / 2 * sumtmp - param, Math.Sqrt(3) / 2 * diftmp);
                y3 = new Complex(-1.0 / 2 * sumtmp - param, -Math.Sqrt(3) / 2 * diftmp);
                roots[0] = y1;
                roots[1] = y2;
                roots[2] = y3;
            }
            else if (Q < 0)
            { 
            
                double phi = 0;

                if (q < 0)
                {
                    phi = Math.Atan(Math.Sqrt(-Q) / (-q / 2));
                }
                else if (q > 0)
                {
                    phi = Math.Atan(Math.Sqrt(-Q) / (-q / 2)) + 3.14;
                }
                else if (q.CompareTo(0) == 0)
                {
                    phi = Math.PI / 2;
                }

                y1 = 2 * Math.Sqrt(-p / 3) * Math.Cos(phi / 3);
                y2 = 2 * Math.Sqrt(-p / 3) * Math.Cos(phi / 3 + 2 * 3.14 / 3);
                y3 = 2 * Math.Sqrt(-p / 3) * Math.Cos(phi / 3 + 4 * 3.14 / 3);

                roots[0] = y1 - param;
                roots[1] = y2 - param;
                roots[2] = y3 - param;
            }
            else if (Q.CompareTo(0) == 0)
            {
                roots[0] = 2 * Sqrt3(-q / 2) - param;
                roots[1] = -Sqrt3(-q / 2) - param;
                roots[2] = -Sqrt3(-q / 2) - param;
            }

            return roots;
        }

        public static Complex[] Vieta(double a, double b, double c, double d)
        {
           /* if (a.CompareTo(0) == 0)
                throw new Exception("#Error: It is not cubical equation a = 0");*/

            b /= a;
            c /= a;
            d /= a;

            Complex[] roots = new Complex[3];

            double Q = (b * b - 3 * c) / 9;
            double R = (2 * Math.Pow(b, 3) - 9 * b * c + 27 * d) / 54;

            double S = Math.Pow(Q, 3) - Math.Pow(R, 2);

            double phi;

            if (S > 0)
            {
                phi = Math.Acos(R / Math.Pow(Q, 3.0 / 2)) / 3 ;
                roots[0] = -2 * Math.Sqrt(Q) * Math.Cos(phi) - b / 3;
                roots[1] = -2 * Math.Sqrt(Q) * Math.Cos(phi + 2 * 3.14 / 3) - b / 3;
                roots[2] = -2 * Math.Sqrt(Q) * Math.Cos(phi - 2 * 3.14 / 3) - b / 3;
            }
            else if (S < 0)
            {
                double tmpx = Math.Abs(R) / Math.Pow(Math.Abs(Q), 3.0 / 2);
                double T;

                if (Q > 0)
                {
                    phi = Math.Log(tmpx + Math.Sqrt(tmpx * tmpx - 1)) / 3;
                    T   = Math.Sign(R) * Math.Sqrt(Q) * Math.Cosh(phi);
                    
                    roots[0] = -2 * T - b / 3;
                    roots[1] = new Complex(T - b / 3, Math.Sqrt(3) * Math.Sqrt(Q) * Math.Sinh(phi));
                    roots[2] = new Complex(T - b / 3, -Math.Sqrt(3) * Math.Sqrt(Q) * Math.Sinh(phi));
                }
                else if (Q < 0)
                {
                    phi = Math.Log(tmpx + Math.Sqrt(tmpx * tmpx + 1)) / 3;
                    T = Math.Sign(R) * Math.Sqrt(Q) * Math.Sinh(phi);

                    roots[0] = -2 * T - b / 3;
                    roots[1] = new Complex(T - b / 3, Math.Sqrt(3) * Math.Sqrt(Math.Abs(Q)) * Math.Cosh(phi));
                    roots[2] = new Complex(T - b / 3, -Math.Sqrt(3) * Math.Sqrt(Math.Abs(Q)) * Math.Cosh(phi));
                }
                else if (Q.CompareTo(0) == 0)
                {
                    T = -Math.Pow(d - b * b * b / 27, 1.0 / 3) - b / 3;
                    roots[0] = T;
                    roots[1] = new Complex(-(b + T) / 2, 1.0 / 2 * Math.Sqrt(Math.Abs((b - 3 * T) * (b + T) - 4 * c)));
                    roots[2] = new Complex(-(b + T) / 2, - 1.0 / 2 * Math.Sqrt(Math.Abs((b - 3 * T) * (b + T) - 4 * c)));
                }
            }
            else if (S.CompareTo(0) == 0)
            {
                double T = Math.Sign(R) * Math.Sqrt(Q);

                roots[0] = -2 * T - b / 3;
                roots[1] = T - b / 3;
                roots[2] = T - b / 3;
            }

            return roots;
        }

        private static double Sqrt3(double value)
        {

            return Math.Sign(value) * Math.Pow(Math.Abs(value), 1.0 / 3);
        }
    }
}

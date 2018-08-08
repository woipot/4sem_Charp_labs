using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07_01_PolynomMatrix
{
    public interface IArithmetic<T>
    {

        T Addition(T obj1);

        T Subtraction(T obj1);

        T Multiplication(T obj1);

        T Division(T obj1);

        T Negative();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07_01_PolynomMatrix
{
    class MatrixException : Exception
    {
        public MatrixException() { }

        public MatrixException(string message) : base(message) { }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace _07_01_PolynomMatrix
{
    public sealed class MyMatrix : IArithmetic<MyMatrix>, ICloneable, IEnumerable
    {

        #region Constructors

        public MyMatrix(uint size)
        {
            if (size == 0)
                throw new MatrixException("Incorrect size (size != 0)");

            _size = size;
            _matrix = new double[size, size];
        }

        public MyMatrix(MyMatrix obj) : this(obj.Size, obj.Matrix)
        {
        }

        public MyMatrix(uint size, double[,] arr)
        {
            if (size == 0)
                throw new MatrixException("Incorrect size (size != 0)");

            _size = size;

            var tmpMatrArr = new double[size, size];

            for (var row = 0; row < size; ++row)
            {
                for (var column = 0; column < size; ++column)
                {
                    tmpMatrArr[row, column] = arr[row, column];
                }
            }

            _matrix = tmpMatrArr;
        }

        public MyMatrix(uint size, double value)
        {
            if (size == 0)
                throw new MatrixException("Incorrect size (size != 0)");

            _size = size;

            var tmpMatrArr = new double[size, size];

            for (var row = 0; row < size; ++row)
            {
                for (var column = 0; column < size; ++column)
                {
                    tmpMatrArr[row, column] = value;
                }
            }

            _matrix = tmpMatrArr;
        }

        #endregion

    

        #region Properties

        public uint Size => _size;

        public double[,] Matrix => _matrix;

        #endregion



        #region IArithmetic

        public MyMatrix Addition(MyMatrix obj1)
        {
            var isEqualSize = obj1.Size == Size;
            if(!isEqualSize) 
                throw new MatrixException("Different Matrix Size");

            var result = new MyMatrix(Size);
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    result._matrix[i, j] = _matrix[i, j] + obj1._matrix[i, j];
                }
            }
            return result;
        }

        public MyMatrix Subtraction(MyMatrix obj1)
        {
            var isEqualSize = obj1.Size == Size;
            if (!isEqualSize)
                throw new MatrixException("Different Matrix Size");

            var result = new MyMatrix(Size);
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    result._matrix[i, j] = _matrix[i, j] - obj1._matrix[i, j];
                }
            }
            return result;
        }

        public MyMatrix Multiplication(MyMatrix obj1)
        {
            var isEqualSize = obj1.Size == Size;
            if (!isEqualSize)
                throw new MatrixException("Different Matrix Size");

            var result = new MyMatrix(obj1.Size);
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    for (var q= 0; q < Size; q++)
                    {
                        result._matrix[i, j] += _matrix[i, q] * obj1._matrix[q, j];
                    }
                }
            }
            return result;
        }

        public MyMatrix Division(MyMatrix obj1)
        {
            try
            {
                return Multiplication(obj1.GetInverseMatrix());
            }
            catch (Exception)
            {
                throw ;
            }
        }

        public MyMatrix Negative()
        {
            var result = new MyMatrix(Size);
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    result._matrix[i, j] = -_matrix[i, j];
                }
            }
            return result;
        }

        #endregion



        #region ICloneable

        public object Clone()
        {
            return new MyMatrix(this);
        }

        #endregion



        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return _matrix.GetEnumerator();
        }

        #endregion



        #region PublicStaticMethods
        public static MyMatrix operator +(MyMatrix obj1, MyMatrix obj2)
        {
            return obj1.Addition(obj2);
        }

        public static MyMatrix operator -(MyMatrix obj1, MyMatrix obj2)
        {
            return obj1.Subtraction(obj2);
        }

        public static MyMatrix operator *(MyMatrix obj1, MyMatrix obj2)
        {
            return obj1.Multiplication(obj2);
        }

        public static MyMatrix operator /(MyMatrix obj1, MyMatrix obj2)
        {
                return obj1.Multiplication(obj2.GetInverseMatrix());
        }


        public static MyMatrix operator +(MyMatrix obj1, double num)
        {
            var resultMatrix = new MyMatrix(obj1);

            for (var i = 0; i < obj1.Size; ++i)
            {
                for (var j = 0; j < obj1.Size; ++j)
                {
                    resultMatrix._matrix[i, j] += num;
                }
            }

            return resultMatrix;
        }

        public static MyMatrix operator - (MyMatrix obj1, double num)
        {
            var resultMatrix = new MyMatrix(obj1);

            for (var i = 0; i < obj1.Size; ++i)
            {
                for (var j = 0; j < obj1.Size; ++j)
                {
                    resultMatrix._matrix[i, j] -= num;
                }
            }

            return resultMatrix;
        }

        public static MyMatrix operator *(MyMatrix obj1, double num)
        {
            var resultMatrix = new MyMatrix(obj1);

            for (var i = 0; i < obj1.Size; ++i)
            {
                for (var j = 0; j < obj1.Size; ++j)
                {
                    resultMatrix._matrix[i, j] *= num;
                }
            }

            return resultMatrix;
        }

        public static MyMatrix operator /(MyMatrix obj1, double num)
        {
            if (num.CompareTo(0) == 0 )
                throw  new MatrixException("Division by zero");

            var resultMatrix = new MyMatrix(obj1);

            for (var i = 0; i < obj1.Size; ++i)
            {
                for (var j = 0; j < obj1.Size; ++j)
                {
                    resultMatrix._matrix[i, j] /= num;
                }
            }

            return resultMatrix;
        }
        #endregion



        #region OtherPublicMethods

        public double GetDeterminant()
        {
            try
            {
                var determinant = GetDeterminant(this);
                return determinant;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public MyMatrix Transposed()
        {
            var transposedMatrix = new MyMatrix(this);
            for (var i = 0; i < Size; i++)
                for (var j = 0; j < Size; j++)
                    transposedMatrix._matrix[j,i] = _matrix[i,j];

            return transposedMatrix;
        }

        public MyMatrix GetInverseMatrix()
        {
            var determinant = GetDeterminant();
            if (determinant.CompareTo(0) == 0)
                throw new MatrixException("Impossible to find inverse matrix. Determianant = 0.");


            var result = GetAlgebraicAddition();

            result = result.Transposed();
            var additionalFactor = 1 / determinant;
            return result * additionalFactor;
        }


        public override string ToString()
        {
            var procSb = new StringBuilder();

            procSb.Append("[{");
            var counter = 0;
            foreach (var value in Matrix)
            {
                var isBracket = counter % Size  == 0;
                if (isBracket && counter != 0)
                    procSb.Append("}{");
                procSb.Append(" " + value);
                
                ++counter;
            }
            procSb.Append("}]");

            return procSb.ToString();
        }
        #endregion



        #region Other Private Static Methods

        private static double GetDeterminant(MyMatrix matrix)
        {
            var determinant = 0.0;
            var k = 1;


            if (matrix.Size < 1)
                throw new MatrixException("The determinant can not be calculated");

            if (matrix.Size == 1)
            {
                determinant = matrix._matrix[0, 0];
                return determinant;
            }
            if (matrix.Size == 2)
            {
                determinant = matrix._matrix[0, 0] * matrix._matrix[1, 1] - matrix._matrix[1, 0] * matrix._matrix[0, 1];
                return determinant;
            }
            if (matrix.Size > 2)
            {
                for (var i = 0; i < matrix.Size; i++)
                {
                    determinant = determinant + k * matrix._matrix[0, i] * GetDeterminant(GetMinor(matrix, i, 0));
                    k = -k;
                }
            }
            return determinant;
        }

        private static MyMatrix GetMinor(MyMatrix matrix, int col, int row)
        {
            var minor = new MyMatrix(matrix.Size - 1);
            var minorI = 0;

            for (var i = 0; i < matrix.Size; i++)
            {
                var minorJ = 0;
                if (i == row)
                    continue;

                for (var j = 0; j < matrix.Size; j++)
                {
                    if (j == col)
                        continue;
                    minor._matrix[minorI, minorJ++] = matrix._matrix[i, j];
                }
                minorI++;
            }
            return minor;
        }

        #endregion



        #region other Private Metdos

        private MyMatrix GetAlgebraicAddition()
        {

            var algebraicAdditionmatrix = new MyMatrix(Size);

            var k = 1;
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    algebraicAdditionmatrix._matrix[j, i] = GetDeterminant(GetMinor(this, i, j)) * k;
                    k *= -1;
                }
            }

            return algebraicAdditionmatrix;

        }

        #endregion



        #region PrivateVariables

        private double[,] _matrix;
        private readonly uint _size;

        #endregion

    }
}

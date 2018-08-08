using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07_01_PolynomMatrix
{
    class Program
    {
        static void Main(string[] args)
        {

            var firstMatrixArr = new double[3,3];
            var secondMatrixArr = new double[3,3];

            for(var i = 0; i < 3 ; ++ i)
            for (var k = 0; k < 3; ++k)
            {
                firstMatrixArr[i, k] = i + 2 * k;
                secondMatrixArr[i, k] = i + 10 - k + firstMatrixArr[i, k];
            }
            firstMatrixArr[2, 2] = 100;
            secondMatrixArr[2, 1] = -50;

            var firstMatrix = new MyMatrix(3, firstMatrixArr);
            var secondMatrix = new MyMatrix(3, secondMatrixArr);
            Console.WriteLine($"first  Matrix = {firstMatrix}");
            Console.WriteLine($"second Matrix = {secondMatrix}");


            var copyFirstMatrix = firstMatrix.Clone();
            Console.Write("Copy forech martrix");
            foreach (var token in (IEnumerable) copyFirstMatrix)
            {
                Console.Write($"{token}  ");
            }
            Console.WriteLine();

            var firstMatdet = firstMatrix.GetDeterminant();
            var secondMatDet = secondMatrix.GetDeterminant();
            Console.WriteLine($"first det = {firstMatdet};\tsecond det = {secondMatDet}\n");


            var firsTransposedMatrix = firstMatrix.Transposed();
            var secondTransportedMatrix = secondMatrix.Transposed();
            Console.WriteLine($"first  Transposed Matrix = {firsTransposedMatrix}");
            Console.WriteLine($"second Transposed Matrix = {secondTransportedMatrix}\n");

            var firstInverseMat = firstMatrix.GetInverseMatrix();
            Console.WriteLine($"First inverse = {firstInverseMat}\n");

            var sumMatrix = firstMatrix + secondMatrix;
            Console.WriteLine($"firs + second = {sumMatrix}\n");

            var multMatrix = firstMatrix * secondMatrix;
            Console.WriteLine($"firs * second = {multMatrix}\n");

            var delMatrix = firstMatrix / secondMatrix;
            Console.WriteLine($"firs / second = {delMatrix}\n");





            var firstPolynom = new MyPolynom<MyMatrix>();
            var secondPolynom = new MyPolynom<MyMatrix>();

            firstPolynom.Add(1, firstMatrix);
            secondPolynom.Add(3, secondMatrix);
            firstPolynom.Add(1, firstMatrix);
            secondPolynom.Add(3, secondMatrix);

            firstPolynom.Add(2, firstMatrix + secondMatrix);
            secondPolynom.Add(1, secondMatrix * firstMatrix);
            Console.WriteLine($"\nPolynoms\n\nfirst  = {firstPolynom}\n");
            Console.WriteLine($"second = {secondPolynom}\n");

            var sumPolynom = firstPolynom + secondPolynom;
            Console.WriteLine($"pol sum = {sumPolynom}\n");

            var subPolynom1 = firstPolynom - secondPolynom;
            var subPolynom2 = firstPolynom - firstPolynom;
            Console.WriteLine($"pol substruction first - second = {subPolynom1}");
            Console.WriteLine($"pol substruction first - first = {subPolynom2}\n");

            var multPol = firstPolynom * secondPolynom;
            Console.WriteLine($"pol multiplication = {multPol}\n");

            var div = secondPolynom / firstPolynom;
            Console.WriteLine($"pol division = {div}\n");

            var superpos = firstPolynom.GetSuperpos(secondPolynom);
            Console.WriteLine($"superPos first(second) = {superpos}\n");

            var point = firstPolynom.InPoint(firstMatrix);
            Console.WriteLine($"first Polunom in point{firstMatrix} = {point}\n");


            Console.ReadKey();
            //{{1, 2, 3}, {1, 2, 3}, {1, 2, 3}};
        }
    }
}

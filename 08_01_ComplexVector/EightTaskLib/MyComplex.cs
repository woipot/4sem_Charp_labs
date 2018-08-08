using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EightTaskLib
{
    public sealed class MyComplex : IArithmetic<MyComplex>
    {
        //public delegate void ComplexDelegate(object sender, MyEvent m);   
        public static event EventHandler<MyEvent> DeleteEvent;                                   

        
        #region properties

        private double Real { get; }

        private double Imagenary { get; }
        #endregion

        

        #region Constructors
        public MyComplex()
        {
            Real = 0;
            Imagenary = 0;
        }

        public MyComplex(double real, double imaginary)
        {
            Real = real;
            Imagenary = imaginary;
        }

        public MyComplex(double real)
        {
            Real = real;
        }
        #endregion



        #region Operators

        public static MyComplex operator +(MyComplex obj1, MyComplex obj2)
        {
            var sumTwoReal = obj1.Real + obj2.Real;
            var sumTwoImaginary = obj1.Imagenary + obj2.Imagenary;
            var result = new MyComplex(sumTwoReal, sumTwoImaginary);
            return result;

        }

        public static MyComplex operator -(MyComplex obj1, MyComplex obj2)
        {
            var subTwoReal = obj1.Real - obj2.Real;
            var subTwoImaginary = obj1.Imagenary - obj2.Imagenary;
            var result = new MyComplex(subTwoReal, subTwoImaginary);
            return result;
        }

        public static MyComplex operator *(MyComplex obj1, MyComplex obj2)
        {
            var resultReal = obj1.Real * obj2.Real - obj1.Imagenary * obj2.Imagenary;
            var resultImaginary = obj1.Real * obj2.Imagenary + obj1.Imagenary * obj2.Real;
            var result = new MyComplex(resultReal, resultImaginary);
            return result;
        }

        public static MyComplex operator /(MyComplex obj1, MyComplex obj2)
        {
            var division = obj2.Real * obj2.Real + obj2.Imagenary * obj2.Imagenary;
            MyComplex result;

            if (division.CompareTo(0) == 0)
            {
                DeleteEvent?.Invoke(obj1, new MyEvent("#Error: Division by zero"));
                result = new MyComplex(0, 0);
            }
            else
            {
                var resultReal = (obj1.Real * obj2.Real + obj1.Imagenary * obj2.Imagenary) / division;
                var resultImaginary = (obj1.Imagenary * obj2.Real - obj1.Real * obj2.Imagenary) / division;
                result = new MyComplex(resultReal, resultImaginary);
            }
            return result;
        }

        #endregion



        #region OtherPublicMethods

        public double GetModule()
        {
            var doubleReal = Real * Real;
            var doubleImagenary = Imagenary * Imagenary;
            var realAndImagSum = doubleReal + doubleImagenary;
            var result = Math.Sqrt(realAndImagSum);

            return result;
        }

        public double GetAngle()
        {
            return Math.Atan2(Imagenary, Real);
        }

        public MyComplex GetPow(double degree)
        {
            var modulInDegree = Math.Pow(GetModule(), degree); 
            var angleWithCoef = degree * GetAngle(); 
            var resultReal = modulInDegree * Math.Cos(angleWithCoef);
            var resultImaginary = modulInDegree * Math.Sin(angleWithCoef);
            var result = new MyComplex(resultReal, resultImaginary);
            return result;
        }
    
        public IEnumerable<MyComplex> GetRoot(int rootDegree)  
        {
            var modulInDegree = Math.Pow(GetModule(), 1.0 / rootDegree);
            var angleWithCoef = GetAngle() / rootDegree; 
            const double oneRadianInDegree = 57.2958;
            var result = new MyComplex[rootDegree];
            for (var i = 0; i < rootDegree; i++)
            {
                result[i] = new MyComplex();
            }
            for (var i = 0; i < rootDegree; i++)
            {
                var degree = 360 * i / (rootDegree * oneRadianInDegree);

                var newReal = modulInDegree * Math.Cos(angleWithCoef + degree);
                var newImagenary = modulInDegree * Math.Sin(angleWithCoef + degree);

                result[i] = new MyComplex(newReal, newImagenary);
            }
            return result;

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var signInt = Math.Sign(Imagenary);
            var signStr = "+";

            if (signInt == -1)
                signStr = "-";

            var absoluteImagnary = Math.Abs(Imagenary);

            sb.Append($"{Real} {signStr} {absoluteImagnary}i");

            return sb.ToString();
        }
        #endregion



        #region IArithmetic

        public MyComplex Addition(MyComplex obj1)
        {
            return this + obj1;
        }

        public MyComplex Subtraction(MyComplex obj1)
        {
            return this - obj1;
        }

        public MyComplex Multiplication(MyComplex obj1)
        {
            return obj1 * this;
        }

        public MyComplex Division(MyComplex obj1)
        {
            return this / obj1;
        }

        public MyComplex SQrt()
        {
            return GetRoot(2).First();
        }

        public MyComplex Pow(int degree)
        {
            return GetPow(degree);
        }

        #endregion
    }
}

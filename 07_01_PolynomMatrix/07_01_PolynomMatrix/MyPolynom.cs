using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _07_01_PolynomMatrix
{
    class MyPolynom<T> : IEnumerable, ICloneable where T: IArithmetic<T>, ICloneable
    {
        #region Constructors

        public MyPolynom()
        {
            _monomsTree = new SortedDictionary<int, T>();
        }

        public MyPolynom(MyPolynom<T> obj1)
        {
            _monomsTree = new SortedDictionary<int, T>();
            foreach (var monom in obj1._monomsTree)
            {
                _monomsTree[monom.Key] = (T) monom.Value.Clone();
            }
        }

        public MyPolynom(Dictionary<int, T> obj1)
        {
            _monomsTree = new SortedDictionary<int, T>();
            foreach (var monom in obj1)
            {
                _monomsTree[monom.Key] = (T)monom.Value.Clone();
            }
        }
        #endregion



        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            return _monomsTree.GetEnumerator();
        }
        #endregion



        #region ICloneable
        public object Clone()
        {
            return new MyPolynom<T>(this);
        }
        #endregion



        #region Operators
        public void Add(int degree, T coeff)
        {
            var isContainsMonom = _monomsTree.Keys.Contains(degree);
            if (isContainsMonom)
            {
                _monomsTree[degree] = _monomsTree[degree].Addition(coeff);
                return;
            }

             _monomsTree[degree] = coeff;
        }

        public static MyPolynom<T> operator +(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            var result = new MyPolynom<T>(obj1);

            foreach (var monom in obj2._monomsTree)
                result.Add(monom.Key, monom.Value);

            return result;
        }

        public static MyPolynom<T> operator -(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            var result = new MyPolynom<T>(obj1);

            foreach (var monom in obj2._monomsTree)
                result.Add(monom.Key, monom.Value.Negative());

            return result;
        }

        public static MyPolynom<T> operator *(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            var result = new MyPolynom<T>();

            foreach (var firstMonom in obj1._monomsTree)
            {
                foreach (var secondMonom in obj2._monomsTree)
                {
                    var coeff = firstMonom.Value.Multiplication(secondMonom.Value);
                    var degree = firstMonom.Key + secondMonom.Key;
                    result.Add(degree, coeff);
                }
            }
            return result;
        }

        public static MyPolynom<T> operator /(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            var result = new MyPolynom<T>();
            var number = new MyPolynom<T>(obj1);

            var isEmpty = obj1._monomsTree.Count == 0 || obj2._monomsTree.Count == 0;
            if (isEmpty)
                throw new Exception("Empty Polynoms");
            while (number >= obj2)
            {
                var numberMaxDegree = number._monomsTree.Keys.Max();
                var obj2MaxDegree = obj2._monomsTree.Keys.Max();

                var degree = numberMaxDegree - obj2MaxDegree;
                var coeff = number._monomsTree[numberMaxDegree].Division(obj2._monomsTree[obj2MaxDegree]); 
                result.Add(degree, coeff);
                var pair = new KeyValuePair<int, T>(degree, coeff);

                var tmp = obj2 * pair;
                number = number - tmp;
                number._monomsTree.Remove(numberMaxDegree);
            }
            return result;
        }

        public static MyPolynom<T> operator +(MyPolynom<T> obj1, KeyValuePair<int,T> degreeValuePair)
        {
            var result = new MyPolynom<T>(obj1) {{degreeValuePair.Key, degreeValuePair.Value}};
            return result;
        }

        public static MyPolynom<T> operator -(MyPolynom<T> obj1, KeyValuePair<int, T> degreeValuePair)
        {
            var result = new MyPolynom<T>(obj1) { { degreeValuePair.Key, degreeValuePair.Value.Negative() } };
            return result;
        }

        public static MyPolynom<T> operator *(MyPolynom<T> obj1, KeyValuePair<int, T> degreeValuePair)
        {
            var result = new MyPolynom<T>();
            foreach (var pair in obj1._monomsTree)
            {
                var degree = pair.Key + degreeValuePair.Key;
                var coeff = pair.Value.Multiplication(degreeValuePair.Value);
                result.Add(degree, coeff);
            }
            return result;
        }

        public static MyPolynom<T> operator /(MyPolynom<T> obj1, KeyValuePair<int, T> degreeValuePair)
        {
            var result = new MyPolynom<T>();
            foreach (var pair in obj1._monomsTree)
            {
                var degree = pair.Key - degreeValuePair.Key;
                try
                {
                    var coeff = pair.Value.Division(degreeValuePair.Value);
                    result.Add(degree, coeff);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        public static bool operator ==(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            var isEqualSize = obj1._monomsTree.Count == obj2._monomsTree.Count;
            if (!isEqualSize)
                return false;

            var obj1Keys = obj1._monomsTree.Keys.ToList();
            var obj2Keys = obj2._monomsTree.Keys.ToList();

            for (var index = 0; index < obj1Keys.Count; ++index)
            {
                if (obj1Keys[index] != obj2Keys[index])
                    return false;
            }


            return true;

        }

        public static bool operator >(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            var isBiggerSize = obj1._monomsTree.Count > obj2._monomsTree.Count;
            if (isBiggerSize)
                return true;

            var obj1Max = obj1._monomsTree.Keys.Max();
            var obj2Max = obj2._monomsTree.Keys.Max();

            return obj1Max > obj2Max;
        }

        public static bool operator !=(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            return !(obj1 == obj2);
        }

        public static bool operator >=(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            return (obj1 > obj2 || obj1 == obj2);
        }

        public static bool operator <(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            return !(obj1 >= obj2);
        }

        public static bool operator <=(MyPolynom<T> obj1, MyPolynom<T> obj2)
        {
            return !(obj1 < obj2);
        }

        #endregion



        #region PublicMethods

        public T InPoint(T obj1)
        {
            if (_monomsTree.Count == 0)
                throw new Exception("Пустой полином");

            var result = obj1.Subtraction(obj1);
            foreach (var pair in _monomsTree)
            {
                var newNode = Pow(obj1, pair.Key).Multiplication(pair.Value);
                result = result.Addition(newNode);
            }
            return result;
        }

        public MyPolynom<T> GetSuperpos(MyPolynom<T> obj1)
        {

            var result = new MyPolynom<T>();

            foreach (var pair in _monomsTree)
            {
                var newB = new MyPolynom<T>(obj1);
                for (var j = 1; j < pair.Key; j++)
                    newB = newB * obj1;

                var dopPair = new KeyValuePair<int, T>(0, pair.Value);
                result = result + newB * dopPair;
            }
            return result;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            var firstCounter = false;
            foreach (var pair in _monomsTree)
            {
                if (firstCounter)
                {
                    sb.Append(" + ");
                }
                sb.Append(pair.Value + "x^" + pair.Key);

                if(firstCounter == false)
                    firstCounter = true;
            }
            return sb.ToString();
        }
        #endregion



        #region PrivateStaticMethods
        private static T Pow(T num, int pow)
        {
            var result = (T)num.Clone();
            for (var i = 0; i < pow; ++i)
                result = result.Multiplication(result);
            return result;
        }
        #endregion



        #region PrivateVariables

        private SortedDictionary<int,T> _monomsTree;

        #endregion
    }
}

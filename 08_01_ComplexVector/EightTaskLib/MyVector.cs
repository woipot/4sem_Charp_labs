using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EightTaskLib
{
    public sealed class MyVector<T> : ICollection<T> 
        where T : IArithmetic<T>, new()
    {
        #region Constructors

        public MyVector()
        {
            _vector = new List<T>();
        }

        public MyVector(IEnumerable<T> collection)
        {
            _vector = new List<T>(collection);
        }

        public MyVector(int size)
        {
            _vector = new List<T>(size);

        }

        #endregion

        

        #region Opearators
        public static MyVector<T> operator +(MyVector<T> obj1, MyVector<T> obj2)
        {
            var result = new MyVector<T>();

            for (var i = 0; i < obj1.Count; i++)
            {
                var sum = obj1[i].Addition(obj2[i]);

                result.Add(sum);
            }

            return result;
        }

        public static MyVector<T> operator -(MyVector<T> obj1, MyVector<T> obj2)
        {
            var result = new MyVector<T>();

            for (var i = 0; i < obj1.Count; i++)
            {
                var subLeftAndRight = obj1[i].Subtraction(obj2[i]);
                result.Add(subLeftAndRight);
            }
            return result;
        }

        public T this[int index]
        {
            get => _vector[index];
            set => _vector[index] = value;
        }

        #endregion

        

        #region PublicMethods

        public T GetModule()
        {
            var sumAllNumber = new T();
            foreach (var token in _vector)
            {
                var numberInSqrt = token.Pow(2);
                sumAllNumber = sumAllNumber.Addition( numberInSqrt);
            }

            var modul = sumAllNumber.SQrt();
            return modul;
        }

        public T GetScalarMult(MyVector<T> obj1)
        {
            if (_vector.Count != obj1.Count)
                throw new ArithmeticException();

            var skal = new T();
            for (var i = 0; i < _vector.Count; i++)
            {
                var multTwoNumber = _vector[i].Multiplication(obj1[i]);
                skal = skal.Addition(multTwoNumber);
            }

            return skal;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            foreach (var token in _vector)
            {
                sb.Append($"{token},  ");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static List<MyVector<T>> Ortogonalization(List<MyVector<T>> vectorList)
        {
            var copyOfVectors = new List<MyVector<T>>(vectorList);
            var vectorsSimilarSize = new List<MyVector<T>>();

            for (var index = 0; index < vectorList.Count; index++)
            {
                var result = new MyVector<T>(copyOfVectors[index].Count);
                if (index == 0)
                {
                    vectorsSimilarSize.Insert(index, copyOfVectors[index]);
                }
                else if (index != 0)
                {
                    var copyIndex = index;
                    while (copyIndex != 0)
                    {
                        try
                        {
                            var vector = vectorsSimilarSize[copyIndex - 1];
                            if (result.Count == 0)
                                result = Multiplication(copyOfVectors[index], vector);
                            else if (result.Count != 0)
                                result = result + (Multiplication(copyOfVectors[index], vector));
                            copyIndex--;
                        }
                        catch (ArithmeticException ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }
                    }
                    vectorsSimilarSize.Insert(index, (copyOfVectors[index] - result));
                }
            }
            return vectorsSimilarSize;
        }

        public T[] ToArray()
        {
            var massivOfVector = _vector.ToArray();
            return massivOfVector;
        }

        #endregion



        #region ICollection

        public IEnumerator<T> GetEnumerator()
        {
            return _vector.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _vector.Add(item);
        }

        public void Clear()
        {
            _vector.Clear();
        }

        public bool Contains(T item)
        {
            return _vector.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _vector.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _vector.Remove(item);
        }

        public int Count => _vector.Count;
        public bool IsReadOnly => false;

        #endregion



        #region PrivateMethods

        private static MyVector<T> Multiplication(MyVector<T> obj1, MyVector<T> obj2)
        {
            var scalrObj1AndObj2 = obj1.GetScalarMult(obj2);
            var scalrObj2AndObj2 = obj2.GetScalarMult(obj2);
            var divisionRes = scalrObj1AndObj2.Division(scalrObj2AndObj2);
            var result = obj2.MultiplicateOnT(divisionRes);
            return result;
        }

        private MyVector<T> MultiplicateOnT(T coeff)
        {
            var result = new MyVector<T>();

            for (var i = 0; i < Count; i++)
            {
                var multLeftAndRight = _vector[i].Multiplication(coeff);
                result.Add(multLeftAndRight);
            }

            return result;
        }
        #endregion



        #region PrivateVariabes
        private readonly List<T> _vector;
        #endregion
    }
}

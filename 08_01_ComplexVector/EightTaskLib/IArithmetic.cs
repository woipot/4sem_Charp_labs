namespace EightTaskLib
{
    public interface IArithmetic<T>
    {
        T Addition(T obj1);

        T Subtraction(T obj1);

        T Multiplication(T obj1);

        T Division(T obj1);

        T SQrt();

        T Pow(int degree);

    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace KR_02_01
{
    public class Typer
    {
        private Type _type;


        public  Typer(string typeName, bool needException = false, bool checkRegister = true)
        {
            _type = Type.GetType(typeName, needException, checkRegister);
        }

        public string GetInfo()
        {
            var sb = new StringBuilder();

            sb.Append(_type.FullName + "\n");

        sb.Append("Constructors:\n");

            var constructors = _type.GetConstructors();
            var counter = 1;
            foreach (var constructor in constructors)
            {
                sb.Append(counter + ") " + constructor + "\n");
                ++counter;
            }

            sb.Append("\nFields:\n");
            var fields = _type.GetFields();
            counter = 1;
            foreach (var field in fields)
            {
                sb.Append(counter + ") " + field + "\n");
                ++counter;
            }

            sb.Append("\nMethods:\n");
            var methods = _type.GetMethods();
            counter = 1;
            foreach (var method in methods)
            {
                sb.Append(counter + ") " + method + "\n");
                ++counter;
            }

            sb.Append("\nPropertys:\n");
            var properties = _type.GetProperties();
            counter = 1;
            foreach (var property in properties)
            {
                sb.Append(counter + ") " + property + "\n");
                ++counter;
            }
            return sb.ToString();
        }

        public object Invoke(string methodName, object[] args = null)
        {
            var methods = _type.GetMethods();

            var result = new object();
            foreach (var method in methods)
            {
                if (method.Name.Contains(methodName))
                {
                    result = Activator.CreateInstance(_type);
                    if (method.GetParameters().Length == args.Count())
                    return method.Invoke(result, args.ToArray());
                }

            }
            return result;
        }

        public object Invoke(string methodName, string[] args = null)
        {
            var methods = _type.GetMethods();

            var result = new object();
            foreach (var method in methods)
            {
                if (!method.Name.Contains(methodName)) continue;

                var methodParams = method.GetParameters();
                if (methodParams.Length != args.Count()) continue;

                result = Activator.CreateInstance(_type);

                var objParams = new List<object>();
                for (var i = 0; i < methodParams.Length; i++)
                {
                    var convertedObj = Convert.ChangeType(args[i], methodParams[i].ParameterType);
                    objParams.Add(convertedObj);
                }

                return method.Invoke(result, objParams.ToArray());
            }
            return result;
        }


        public static object Create(string typeName)
        {
            var type = new Typer(typeName, true);

            var newObj = Activator.CreateInstance(type._type);

            return newObj;
        }

    }
}

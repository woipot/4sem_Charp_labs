using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_02_01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter type: ");
            var typeName = Console.ReadLine();

            Console.Write("Need Exception y/n: ");
            var userChEx = Console.ReadLine();
            var needException = userChEx == "y";

            Console.Write("Need check reg y/n: ");
            var userChReg = Console.ReadLine();
            var needReeg = userChReg == "n";

            Typer tt;
            try
            {
                tt = new Typer(typeName, needException, needReeg);
                var info = tt.GetInfo();
                Console.WriteLine(info);

                Console.WriteLine("What method need run: ");
                var methodName = Console.ReadLine();

                Console.Write("Enter params:");
                var inputParams = Console.ReadLine();
                var separators = new[] {" "};
                var paramArr = inputParams.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                var result = tt.Invoke(methodName, paramArr);
                
                Console.WriteLine($"Result: {result}");
                


                Console.Write("Need create new y/n: ");
                var userCreate = Console.ReadLine();
                var isCreate = userCreate == "y";

                if (isCreate)
                {
                    var nObj = Typer.Create(typeName);
                    Console.WriteLine("new obj is created successfuly");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.ReadKey();
        }
    }
}

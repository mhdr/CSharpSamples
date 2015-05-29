using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClearLastLine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("-");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("=");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("=-");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("==");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("==-");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("===");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("===-");
            Thread.Sleep(1000 * 3);
            Console.Write("\r");

            Console.Write("====");

            Console.ReadKey();
        }
    }
}

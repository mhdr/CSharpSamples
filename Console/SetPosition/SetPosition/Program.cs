using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetPosition
{
    class Program
    {
        static void Main(string[] args)
        {
            // current column
            Console.Write(Console.CursorLeft);

            // current row
            Console.Write(Console.CursorTop);

            Console.SetCursorPosition(0,0);

            Console.WriteLine("Hello World");

            Console.ReadKey();
        }
    }
}

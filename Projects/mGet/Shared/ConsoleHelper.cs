using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ConsoleHelper
    {
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void ClearConsoleLine(int fromRow,int toRow)
        {
            for (int i = fromRow; i <= toRow; i++)
            {
                Console.SetCursorPosition(0,i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0,fromRow);
        }

        public static void Write(string txt, ConsoleColor color)
        {
            var defaultColor=Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(txt);
            Console.ForegroundColor = defaultColor;
        }
    }
}

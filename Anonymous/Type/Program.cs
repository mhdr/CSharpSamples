using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Type
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculation calculation = new Calculation();

            Thread thread = new Thread(() =>
            {
                calculation.DoWork(1, 10, v =>
                {
                    string msg = string.Format("Thread 1 => Progress : {0} , Estimated : {1}",v.Progress,v.Estimated);
                    Console.WriteLine(msg);
                });
            });

            thread.Start();
        }
    }
}

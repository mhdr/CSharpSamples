using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Callback
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculation calculation = new Calculation();

            Thread thread=new Thread(() =>
            {
                calculation.DoWork(1, 10, p =>
                {
                    string msg = string.Format("Thread 1 : {0}", p);
                    Console.WriteLine(msg);
                });
            });

            Thread thread2 = new Thread(() =>
            {
                calculation.DoWork(7, 24, p =>
                {
                    string msg = string.Format("Thread 2 : {0}", p);
                    Console.WriteLine(msg);
                });
            });

            Thread thread3=new Thread(() =>
            {
                calculation.DoWork2(4,14, (progress, estimated) =>
                {
                    string msg = string.Format("Thread 3 : {0} , Estimtaed : {1}", progress,estimated);
                    Console.WriteLine(msg);
                });
            });

            thread.Start();
            thread2.Start();
            thread3.Start();

            Console.ReadKey();
        }
    }
}

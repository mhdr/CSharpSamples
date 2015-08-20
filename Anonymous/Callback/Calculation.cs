using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Callback
{
    public class Calculation
    {
        public delegate void CalculationProgress(int progress);
        public delegate void CalculationProgress2(int progress,int estimated);

        // 21 : 2 numver of parameter , 1 : version
        public delegate void Func21(int progress, int estimated);
        public Calculation()
        {
            
        }

        public void DoWork(int start, int end,CalculationProgress progress)
        {
            for (int i = start; i < end; i++)
            {
                progress(i);
                Thread.Sleep(1000);
            }
        }

        public void DoWork2(int start, int end, CalculationProgress2 callback)
        {
            for (int i = start; i < end; i++)
            {
                callback(i, end - i);
                Thread.Sleep(1000);
            }
        }

        public void DoWork3(int start, int end, Func21 callback)
        {
            for (int i = start; i < end; i++)
            {
                callback(i, end - i);
                Thread.Sleep(1000);
            }
        }
    }
}

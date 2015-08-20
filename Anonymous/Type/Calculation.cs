using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Type
{
    public class Calculation
    {
        public delegate void Func11(dynamic state);
        public Calculation()
        {
            
        }

        public void DoWork(int start, int end,Func11 callback)
        {
            for (int i = start; i < end; i++)
            {
                // anonymous type
                var v = new {Progress=i,Estimated=end-i};
                callback(v);
                Thread.Sleep(1000);
            }
        }
    }
}

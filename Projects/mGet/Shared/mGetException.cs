using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class MGetException : Exception
    {
        private int _code;
        public MGetException() : base()
        {
            
        }

        public MGetException(string message) : base(message)
        {
            
        }

        public MGetException(int code,string message) : base(message)
        {
            this.Code = code;
        }

        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }
    }
}

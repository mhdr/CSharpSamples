using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    [Serializable]
    public class Greeting
    {
        public string _msg;

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
    }
}

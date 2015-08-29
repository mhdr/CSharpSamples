using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mGet
{
    [Serializable]
    public class Config
    {
        private int _numberOfConnections=8;

        public int NumberOfConnections
        {
            get { return _numberOfConnections; }
            set { _numberOfConnections = value; }
        }

        public void SetParamaterValue(string name, string value)
        {
            if (name == "NumberOfConnections")
            {
                this.NumberOfConnections = int.Parse(value);
            }
        }

        public object GetParameterValue(string name)
        {
            if (name == "NumberOfConnections")
            {
                return this.NumberOfConnections;
            }

            return null;
        }
    }
}

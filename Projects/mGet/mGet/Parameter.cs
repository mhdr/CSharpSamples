using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mGet
{
    public class Parameter
    {
        private int _id;
        private string _name;
        private string _description;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public static List<Parameter> GetParameters()
        {
            List<Parameter> parameters=new List<Parameter>();

            parameters.Add(new Parameter()
            {
                Id =1,
                Name = "NumberOfConnections",
                Description ="set or get the number of connections established per download"
            });

            return parameters;
        }

        public static List<Parameter> GetParameters(Func<Parameter,bool> predicate)
        {
            return Parameter.GetParameters().Where(p => predicate(p)).ToList();
        }

        public static Parameter GetParameter(Func<Parameter, bool> predicate)
        {
            return Parameter.GetParameters().FirstOrDefault(p => predicate(p));
        }
    }
}

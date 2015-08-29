using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class UriHelper
    {
        public static bool IsValid(this Uri uri)
        {
            try
            {
                if (uri.Scheme.ToLower() == "http".ToLower())
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}

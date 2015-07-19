using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mGetLibrary
{
    public class HttpResponse
    {
        private string _statusCode;
        private string _statusMessage;
        private string _httpVersion;
        private Dictionary<string, string> _headers;

        public string StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; }
        }

        public string HttpVersion
        {
            get { return _httpVersion; }
            set { _httpVersion = value; }
        }

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        public static HttpResponse Parse(byte[] headerBytes)
        {
            if (headerBytes == null)
            {
                return null;
            }

            if (headerBytes.Length == 0)
            {
                return null;
            }

            string headerStr = Encoding.Default.GetString(headerBytes);

            string[] headersArray = headerStr.Split(new string[]{"\r\n"},StringSplitOptions.None);
            List<string> headers=new List<string>();

            foreach (string header in headersArray)
            {
                if (!string.IsNullOrEmpty(header))
                {
                    headers.Add(header);
                }
            }

            Dictionary<string,string> headersDic=new Dictionary<string, string>();

            for (int i = 1; i < headers.Count; i++)
            {
                string[] h = headers[i].Split(new string[] {":"}, StringSplitOptions.None);
                string key = h[0].Trim();
                string value = h[1].Trim();
                headersDic.Add(key,value);
            }

            string[] status = headers[0].Split(new string[] {" "}, StringSplitOptions.None);
            string httpVersion = status[0];
            string statusCode = status[1];
            string statusMessage = status[2];

            HttpResponse httpResponse=new HttpResponse();
            httpResponse.HttpVersion = httpVersion;
            httpResponse.Headers = headersDic;
            httpResponse.StatusCode = statusCode;
            httpResponse.StatusMessage = statusMessage;

            return httpResponse;
        }
    }
}

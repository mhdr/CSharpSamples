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

                try
                {
                    // in case of duplicate key for example

                    headersDic.Add(key, value);
                }
                catch (ArgumentException ex)
                {
                }
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

        public static Tuple<long, long, long> ExtractContentRange(HttpResponse response)
        {
            // example Content-Range: bytes 595883-1191764/1191765

            if (response.Headers.ContainsKey("Content-Range"))
            {
                var line = String.Format("{0}:{1}", "Content-Range", response.Headers["Content-Range"]);

                var result = ExtractContentRange(line);
                return result;
            }

            return null;
        }

        public static Tuple<long, long, long> ExtractContentRange(string line)
        {
            // example Content-Range: bytes 0-595882/1191765

            string value1 = line.Split(new string[] { ":" }, StringSplitOptions.None)[1].Trim();
            string value2 = value1.Split(new string[] { " " }, StringSplitOptions.None)[1].Trim();
            string length = value2.Split(new string[] { "/" }, StringSplitOptions.None)[1].Trim();
            string value3 = value2.Split(new string[] { "/" }, StringSplitOptions.None)[0].Trim();
            string start = value3.Split(new string[] { "-" }, StringSplitOptions.None)[0].Trim();
            string end = value3.Split(new string[] { "-" }, StringSplitOptions.None)[1].Trim();

            long lengthValue = long.Parse(length);
            long startValue = long.Parse(start);
            long endValue = long.Parse(end);

            Tuple<long, long, long> result = new Tuple<long, long, long>(startValue, endValue, lengthValue);

            return result;
        }
    }
}

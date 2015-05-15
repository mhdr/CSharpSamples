using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DownloadFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "127.0.0.1";
            var port = 8080;
            var fileName = "npp.exe";

            var uri = string.Format("http://{0}:{1}/{2}", host, port, fileName);

            var request = WebRequest.CreateHttp(uri);
            var response = request.GetResponse();

            var responseStream = response.GetResponseStream();

            var output = JsonConvert.SerializeObject(request.Headers, Formatting.Indented);
            Console.WriteLine(output);

            var output1 = JsonConvert.SerializeObject(response.Headers, Formatting.Indented);
            Console.WriteLine(output1);

            var length = response.ContentLength;

            Console.WriteLine(length);

            var newFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),fileName);

            var writer=new FileStream(newFilePath,FileMode.Create);
            writer.Flush();
            writer.Close();

            writer=new FileStream(newFilePath,FileMode.Append);

            var buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead=responseStream.Read(buffer,0,buffer.Length))>0)
            {
                writer.Write(buffer,0,bytesRead);
            }

            Console.WriteLine(writer.Length);
            writer.Flush();
            writer.Close();

            Console.ReadKey();
        }
    }
}

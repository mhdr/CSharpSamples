﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetTextFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "192.168.1.3";
            var port = 80;
            var path = "hello.txt";

            var uri = string.Format("http://{0}:{1}/{2}", host, port, path);

            var request = WebRequest.CreateHttp(uri);
            var response = request.GetResponse();

            var responseData = response.GetResponseStream();

            var output = JsonConvert.SerializeObject(request.Headers, Formatting.Indented);
            Console.WriteLine(output);

            var output1 = JsonConvert.SerializeObject(response.Headers, Formatting.Indented);
            Console.WriteLine(output1);

            var reader=new StreamReader(responseData);
            var data = reader.ReadToEnd();

            var output2 = JsonConvert.SerializeObject(data, Formatting.Indented);
            Console.WriteLine(output2);

            Console.ReadKey();
        }
    }
}

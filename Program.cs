using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using Amazon.CloudSearch;
using System.Diagnostics;

namespace Tizaro.CloudSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudSearchExecutive cs = new CloudSearchExecutive();
            cs.RunCloudSearch();         
            //wait();
        }

        [Conditional("DEBUG")]
        static void wait()
        {
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}

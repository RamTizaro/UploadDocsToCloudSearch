using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;


namespace Tizaro.CloudSearch
{
    internal class TizaroConfiguration
    {
        static readonly string CONNECTION_STRING_KEY = "DbConnection";

        internal static string Connectionstring
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[CONNECTION_STRING_KEY].ConnectionString;
            }
        }
             
        internal static string LogPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogPath"];
            }
        }

        internal static string SMTP
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTP"];
            }
        }

        internal static string SMTPUser
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPUser"];
            }
        }

        internal static string SMTPPass
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPPass"];
            }
        }

        internal static string AdminEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["AdminEmail"];
            }
        }

        internal static string TechSupportEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["TechSupportEmail"];
            }
        }
       
        internal static string AmazonDocEndpoint
        {
            get
            {
                return ConfigurationManager.AppSettings["AmazonDocEndPoint"];
            }
        }

        internal static string NoOfDocsToFetch
        {
            get
            {
                return ConfigurationManager.AppSettings["NoOfDocsToFetch"];
            }
        }

    }

}

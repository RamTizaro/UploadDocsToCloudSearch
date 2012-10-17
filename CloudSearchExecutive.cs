using System;
using System.IO;
using System.Data;
using System.Web;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Collections.Generic;
using Amazon.CloudSearch;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;

namespace Tizaro.CloudSearch
{
    public class CloudSearchExecutive
    {
        SqlConnection con = new SqlConnection(TizaroConfiguration.Connectionstring);

        public void RunCloudSearch()
        {
            string sSql = "SELECT Top " + TizaroConfiguration.NoOfDocsToFetch + " [id],[documentcontent] FROM SearchCorpusDocumentExport where submitteddt is null and searchcorpusid=2 order by id;";

            using (con)
            {
                SqlCommand command = new SqlCommand(sSql, con);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                StringBuilder batchBuilder = new StringBuilder();               
                batchBuilder.AppendLine("<batch>");
                string query = null;
                string queryids = "";
                query= "Update SearchCorpusDocumentExport set [submitteddt] = getdate()  where id in (";
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        batchBuilder.AppendLine(HandleSpecialChars(reader.GetString(1).Trim()));
                        queryids += ((queryids.Length > 0) ? " ," : "") + reader.GetInt32(0);
                    }
                }
                query += queryids + ")";
                batchBuilder.AppendLine("</batch>");
                Logger.StartLog(DateTime.Now);
                //Logger.LogMessage(batchBuilder.Length.ToString());
                //Logger.LogMessage(batchBuilder.MaxCapacity.ToString());
               // Logger.LogMessage(batchBuilder.ToString());
                try
                {
                    string result = (PostData(TizaroConfiguration.AmazonDocEndpoint, StrToByteArray(batchBuilder.ToString())));
                    if (Success(result))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        try
                        {
                            adapter.UpdateCommand = con.CreateCommand();
                            adapter.UpdateCommand.CommandText = query;
                            adapter.UpdateCommand.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex.ToString());
                        }

                    }
                    else
                    {
                       Logger.LogMessage("Loading the Documents to Amazon has Failed ");
                    }
                }
                catch (AmazonCloudSearchException ex)
                {
                    Logger.LogError(ex.ToString());
                }
                Logger.CloseLog();
                command.Dispose();
                reader.Close();
                con.Close();
                con.Dispose();
            }
        }

        public bool Success(string result)
        {
            bool isSuccess;
            if (result.Contains("status=\"success\"") == true)
            {
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }
            return isSuccess;
        }      
        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private string PostData(string url, byte[] postData)
        {
            HttpWebRequest request = null;
            Uri uri = new Uri(url);
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.ContentLength = postData.Length;
            request.KeepAlive = true;           

            using (Stream writeStream = request.GetRequestStream())
            {
                writeStream.Write(postData, 0, postData.Length);
            }
            string result = string.Empty;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            result = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch(WebException ex)
            {
                Logger.LogError(ex.ToString());                
            }
            return result;
        }

        private byte[] ReadXMLFile(string filePath)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(filePath);

            FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;                
                while ((count = fStream.Read(buffer, sum, length - sum)) > 0)
                sum += count;
            }
            finally
            {
                fStream.Close();
            }

            return buffer;
        }

        public string HandleSpecialChars(string xmlString)
        {
            if (!string.IsNullOrEmpty(xmlString))
            {
                xmlString = xmlString.Replace("#", "");
                xmlString = xmlString.Replace("&gt;", "|");
                xmlString = xmlString.Replace("&lt;", "|");
                xmlString = xmlString.Replace("specdescription-specialfeatures", "specdescriptionspecialfeatures");
                xmlString = xmlString.Replace("tagbest-seller", "tagbestseller");
                xmlString = xmlString.Replace("\">", "\"><![CDATA[");
                xmlString = xmlString.Replace("</field>", "]]></field>");
                xmlString = xmlString.Replace("lang=\"en\"><![CDATA[", "lang=\"en\">");
                xmlString = xmlString.Replace("\"", "&quot;");
                xmlString = xmlString.Replace("=&quot; ", "=\"");
                xmlString = xmlString.Replace("&quot;> ", "\">");
                xmlString = xmlString.Replace("&quot; lang=", "\" lang=");
                xmlString = xmlString.Replace("&quot; version=", "\" version=");
                xmlString = xmlString.Replace(" version=&quot;", " version=\"");
                xmlString = xmlString.Replace(" lang=&quot;", " lang=\"");
                xmlString = xmlString.Replace(" id=&quot;", " id=\"");
                xmlString = xmlString.Replace(" lang=\"en&quot;", " lang=\"en\"");
                xmlString = xmlString.Replace(" name=&quot;", " name=\"");
                xmlString = xmlString.Replace("&quot;><!", "\"><!");
                xmlString = xmlString.Replace("&quot;/>", "\"/>");
                
            }
            return xmlString;
        }

        private string ReplaceSpecialCharacters(string Input)
        {
            Input = Regex.Replace(Input, "&amp;", "&");
            Input = Regex.Replace(Input, "&apos;", "'");
            Input = Regex.Replace(Input, "&#x22;", "\"");
            Input = Regex.Replace(Input, "&#x2039;", "<");
            Input = Regex.Replace(Input, "&#x203A;", ">");
            Input = Regex.Replace(Input, "&gt;", ">=");
            Input = Regex.Replace(Input, "&lt;", "<=");

            return Input;
        }

        private string GetTextFromXMLFile(string file)
        {
            StreamReader reader = new StreamReader(file);
            string ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        public static byte[] ConvertToBytes(XmlDocument doc)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] docAsBytes = encoding.GetBytes(doc.OuterXml);
            return docAsBytes;
        }
     }
}

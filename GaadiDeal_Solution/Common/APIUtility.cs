using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Common
{
    public class APIUtility
    {
        public enum Method { GET, POST };

        public static string NikoAPIKey { get { return "3peitBjp"; } }


        public static T HttpRequestMethod<T>(string url, Method method = Method.GET, object postData = null, string APIKey = "")
        {
            string data = "";
            if (postData != null)
            {
                data = JsonConvert.SerializeObject(postData);
            }
            string strJSon = HttpRequestMethod(url, method, data, APIKey);

            T obj = JsonConvert.DeserializeObject<T>(strJSon);
            return obj;
        }

        public static string HttpRequestMethod(string url, Method method = Method.GET,string postData = "", string APIKey="")
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";


            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "PSAPIRequest";
            webRequest.Timeout = Int32.MaxValue;
            

            if (method == Method.POST)
            {
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(postData);
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = byteData.Length;
                webRequest.Headers.Add("APIKEY", APIKey);

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }



        static string WebResponseGet(HttpWebRequest webRequest)
        {
            string responseData = "";
            try
            {
                var response = (HttpWebResponse)webRequest.GetResponse();
                
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseData = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
            }

            return responseData;
        }

        public static HttpWebRequest RESTRequest(string url, Method method = Method.GET,string postData = "", string APIKey="")
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "PSAPIRequest";
            webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(postData);
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = byteData.Length;
                webRequest.Headers.Add("APIKEY", APIKey);

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            return webRequest;
        }

        public static string RESTResponse(HttpWebResponse response)
        {
            string responseData = "";
            try
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseData = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return responseData;
        }

        
    }
}

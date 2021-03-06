﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Dex.Utilities
{
    public static class Web
    {
        /// <summary>
        /// Creates an HTTP Request of type POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static T DoPost<T>(string url, Dictionary<string, string> paramters)
        {
            var populatedEndPoint = CreateFormattedPostRequest(paramters);
            byte[] bytes = Encoding.UTF8.GetBytes(populatedEndPoint);

            HttpWebRequest request = CreateWebRequest(url, bytes.Length);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                else
                {
                    Encoding enc = System.Text.Encoding.GetEncoding(1252);
                    StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc);

                    string Response = loResponseStream.ReadToEnd();

                    loResponseStream.Close();


                    if (typeof(T) == typeof(string))
                    {
                        return (T)((object)Response);
                    }

                    JavaScriptSerializer jSer = new JavaScriptSerializer();
                    return jSer.Deserialize<T>(Response);
                }
            }
        }

        /// <summary>
        /// Creates a web request
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="contentLength"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateWebRequest(string endPoint, Int32 contentLength)
        {
            var request = (HttpWebRequest)WebRequest.Create(endPoint);

            request.Method = "POST";
            request.ContentLength = contentLength;
            request.ContentType = "application/x-www-form-urlencoded";

            return request;
        }

        /// <summary>
        /// Formats the parameters of the request. 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string CreateFormattedPostRequest(Dictionary<string, string> values)
        {
            StringBuilder paramterBuilder = new StringBuilder();
            int counter = 0;

            foreach (var value in values)
            {
                paramterBuilder.AppendFormat("{0}={1}", value.Key, HttpUtility.UrlEncode(value.Value.ToString()));

                if (counter != values.Count - 1)
                {
                    paramterBuilder.Append("&");
                }

                counter++;
            }

            return paramterBuilder.ToString();
        }
    
    }
}

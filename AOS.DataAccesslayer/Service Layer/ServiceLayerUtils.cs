using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AOS.DataAccessLayer
{
    public class ServiceLayerUtils
    {
        /// <summary>
        /// Check SO service version
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string GetSOServiceVersion(string url)
        {
            return "so75";

            //if (url.ToLower().Contains("https"))
            //    return "so75ssl";
            //else
            //    return "so75";
            //bool httpsEnabled = url.IndexOf("https", StringComparison.OrdinalIgnoreCase) >= 0;

            //// SO75 constants
            //var SO75URLSuffix = "/SoPrincipal.svc";
            //var SO75SearchFor = "SuperOffice.Services75.WcfService.WcfSoPrincipalService";

            //// SO82 constants
            //var SO82URLSuffix = "/SoPrincipal.svc";
            //var SO82SearchFor = "SuperOffice.Services82.WcfService.WcfSoPrincipalService";

            //url = url.TrimEnd('/');

            //// What is the service version
            //using (WebClient client = new WebClient())
            //{
            //    //manipulate request headers (optional)
            //    client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            //    string testStr = string.Empty;

            //    // Test if it's SO75
            //    try
            //    {
            //        testStr = client.DownloadString(string.Concat(url, SO75URLSuffix));

            //        if (testStr.ToLower().Contains(SO75SearchFor))
            //            return "so75";
            //    }
            //    catch (Exception ex)
            //    {
            //    }

            //    // Test if it's SO82
            //    try
            //    {
            //        testStr = client.DownloadString(string.Concat(url, SO82URLSuffix));

            //        return (httpsEnabled) ? "so82ssl" : "so82";
            //    }
            //    catch (ArgumentNullException ex)
            //    {
            //        return "ArgumentNullException occurred";
            //    }
            //    catch (WebException)
            //    {
            //        return "WebException";
            //    }
            //    catch (NotSupportedException)
            //    {
            //        return "NotSupportException";
            //    }

            //    return "unknown";
            //}
        }
    }
}

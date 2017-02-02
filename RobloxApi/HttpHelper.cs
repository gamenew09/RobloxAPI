using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    internal static class HttpHelper
    {

        static bool _Inited = false;

        static void InitHttpHelper()
        {
            if (_Inited)
                return;

            _Inited = true;

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((send, cert, chain, sslPolicyErrors) =>
            {
                return true; // Allways use a server certificate.
            });
        }

        /// <summary>
        /// Gets a string response from the url provided.
        /// </summary>
        /// <param name="url">The url to request a string from.</param>
        /// <returns>The string requested.</returns>
        public static async Task<string> GetStringFromURL(string url)
        {
            InitHttpHelper();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.UserAgent = "CSharp.RobloxAPI";
            WebResponse resp = await req.GetResponseAsync();
            
            string data;
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                data = await reader.ReadToEndAsync();

            return data;
        }

    }
}

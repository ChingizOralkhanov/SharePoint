using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePoint
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteURL = "https://epam.sharepoint.com/sites/EPAMNikeTest";
            string site1URL = "https://epam-my.sharepoint.com/personal/serik_seidigalimov_epam_com";
            SecureString secureString = new SecureString();
            foreach (char c in "Diebold2018".ToCharArray())
            {
                secureString.AppendChar(c);
            }
            ICredentials credentials = new SharePointOnlineCredentials("Shyngyskhan_Oralkhanov@epam.com", secureString);
            DownloadFileViaRestAPI(siteURL, credentials, "Documents", "Coverage Report.xlsx", "C:\\buildabot\\Sharepoint");
            Console.WriteLine("Success");
            Console.ReadLine();
        }

        public static void DownloadFileViaRestAPI(string webUrl, ICredentials credentials, string documentLibName, string fileName, string path)
        {
            webUrl = webUrl.EndsWith("/") ? webUrl.Substring(0, webUrl.Length - 1) : webUrl;
            string webRelativeUrl = null;
            if (webUrl.Split('/').Length > 3)
            {
                webRelativeUrl = "/" + webUrl.Split(new char[] { '/' }, 4)[3];
            }
            else
            {
                webRelativeUrl = "";
            }
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                client.Credentials = credentials;
                FileStream outputStream = new FileStream(path + fileName, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.None);
                try
                {
                    Uri endpointUri = new Uri(webUrl + "/_api/web/GetFileByServerRelativeUrl('" + webRelativeUrl + "/" + documentLibName + "/" + fileName + "')/$value");
                    byte[] data = client.DownloadData(endpointUri);
                    outputStream.Write(data, 0, data.Length);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    outputStream.Flush(true);
                    outputStream.Close();
                }

            }
        }
    }
}

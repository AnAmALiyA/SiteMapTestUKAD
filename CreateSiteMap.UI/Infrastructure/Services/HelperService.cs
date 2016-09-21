using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CreateSiteMap.UI.Services
{
    public class HelperService
    {
        public async Task<bool> IsUrlExistAsync(string url)
        {          
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }
                  
            bool isCorrectly = Uri.IsWellFormedUriString(url, UriKind.Absolute);
            if (!isCorrectly)
            {
                return false;
            }

            bool isExist = false;
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(url), Method = HttpMethod.Head })
                {
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        isExist = response.IsSuccessStatusCode;
                    }
                }

                return isExist;
            }
            catch (Exception ex)
            {
                string except = ex.Message;
                return false;
            }
        }
               
        public string GetHostFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL can not be null");
            }

           try
            {
                Uri uri = new Uri(url);
                return uri.Scheme + "://" + uri.Host;
            }
            catch
            {
                return null;
            }
        }
    }
}

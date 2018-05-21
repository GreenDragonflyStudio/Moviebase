using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Moviebase.Services.Helpers
{
    public static class HttpClientHelpers
    {
        public static async Task<T> GetRequestBody<T>(this HttpClient client, string uri, int maxRetry = 0)
        {
            try
            {
                string responseBody = null;
                var tryTimes = 0;
                do
                {
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseByte = await response.Content.ReadAsByteArrayAsync();
                        responseBody = Encoding.UTF8.GetString(responseByte);
                        break;
                    }
                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        var retryAfter = response.Headers.RetryAfter?.Delta;
                        if (retryAfter.HasValue && retryAfter.Value.TotalSeconds > 0)
                        {
                            await Task.Delay(retryAfter.Value);
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                    }

                    ++tryTimes;
                } while (tryTimes < maxRetry);

                return responseBody == null ? default(T) : JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception e)
            {
                Debug.Print("Error requesting resource: " + e.Message);
                return default(T);
            }
        }

        public static async Task DownloadFile(this HttpClient client, string uri, string filePath)
        {
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            if (File.Exists(filePath)) File.Delete(filePath);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        public static string BuildFullUri(string endpoint, string path, NameValueCollection col)
        {
            var uri = new UriBuilder
            {
                Host = endpoint,
                Scheme = Uri.UriSchemeHttps,
                Path = path,
                Query = BuildQueryString(col)
            };

            return uri.ToString();
        }

        public static string BuildQueryString(NameValueCollection col)
        {
            if (col == null) return String.Empty;
            var sb = new StringBuilder();

            for (int i = 0; i < col.Count; i++)
            {
                var key = col.GetKey(i);
                if (i > 0) sb.Append("&");
                sb.AppendFormat("{0}={1}", key, col.Get(key));
            }
            return sb.ToString();
        }
    }
}

// TODO Bart: further clean up webclient, fix post/put, add exception for failed calls
using Kliva.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kliva.Services
{
    /// <summary>
    /// Class to create web requests and receive a response from the server.
    /// Somewhat finetuned to the Strava api.
    /// </summary>
    public class StravaWebClient
    {
        public HttpClient StravaHttpClient;

        /// <summary>
        /// The Response Code that was received on the last request.
        /// </summary>
        private HttpStatusCode _lastResponseCode;

        public StravaWebClient()
        {
            InitializeHttpClient();
        }

        /// <summary>
        /// AsyncResponseReceived is raised when an asynchronous response is received from the server.
        /// </summary>
        public event EventHandler<AsyncResponseReceivedEventArgs> AsyncResponseReceived;

        /// <summary>
        /// Sends a GET request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public Task<string> GetAsync(Uri uri)
        {
            if (uri == null)
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");

            return HandleStravaApiCallAsync(StravaHttpClient.GetAsync(uri));
        }

        /// <summary>
        /// Sends a POST request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public Task<string> SendPostAsync(Uri uri)
        {
            if (uri == null)
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");

            return HandleStravaApiCallAsync(StravaHttpClient.PostAsync(uri, null));
        }

        public Task<string> SendPostAsync(Uri uri, string fileName)
        {
            if (uri == null)
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");

            if(string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Please provide a valid file name");

            FileInfo info = new FileInfo(fileName);
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(File.ReadAllBytes(info.FullName)), "file", info.Name);

            return HandleStravaApiCallAsync(StravaHttpClient.PostAsync(uri, content));
        }

        /// <summary>
        /// Sends a PUT request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public Task<string> SendPutAsync(Uri uri)
        {
            if (uri == null)
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");

            return HandleStravaApiCallAsync(StravaHttpClient.PutAsync(uri, null));
        }

        /// <summary>
        /// Sends a DELETE request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public Task<string> SendDeleteAsync(Uri uri)
        {
            if (uri == null)
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");

            return HandleStravaApiCallAsync(StravaHttpClient.DeleteAsync(uri));
        }

        private void InitializeHttpClient()
        {
            HttpMessageHandler handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }; // support gzip,deflate
            StravaHttpClient = new HttpClient(handler);

            //foreach (KeyValuePair<string, string> headerData in this.HttpHeaders)
            //{
            //    _stravaHttpClient.DefaultRequestHeaders.Add(headerData.Key, headerData.Value);
            //}
            StravaHttpClient.MaxResponseContentBufferSize = 10000000; // 10mb
        }

        // TODO Bart: debug, check and see if we can add logging for this
        private static void CheckStravaApiUsage(HttpResponseMessage response)
        {
            //Getting the Strava API usage data.
            KeyValuePair<string, IEnumerable<string>> usage =
                response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Usage"));

            if (usage.Value != null)
            {
                //Setting the related Properties in the Limits-class.
                Limits.Usage = new Usage(int.Parse(usage.Value.ElementAt(0).Split(',')[0]),
                    int.Parse(usage.Value.ElementAt(0).Split(',')[1]));
            }

            //Getting the Strava API limits
            KeyValuePair<string, IEnumerable<string>> limit =
                response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Limit"));

            if (limit.Value != null)
            {
                //Setting the related Properties in the Limits-class.
                Limits.Limit = new Limit(int.Parse(limit.Value.ElementAt(0).Split(',')[0]),
                    int.Parse(limit.Value.ElementAt(0).Split(',')[1]));
            }
        }

        private async Task<string> HandleStravaApiCallAsync(Task<HttpResponseMessage> asyncHttpCall)
        {
            using (HttpResponseMessage response = await asyncHttpCall)
            {
                if (response != null)
                {
                    AsyncResponseReceived?.Invoke(null, new AsyncResponseReceivedEventArgs(response));
                    CheckStravaApiUsage(response);

                    _lastResponseCode = response.StatusCode;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }

            return string.Empty;
        }
    }
}

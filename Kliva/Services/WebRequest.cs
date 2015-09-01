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
    /// This class holds information about a received web response.
    /// </summary>
    public class ResponseReceivedEventArgs
    {
        /// <summary>
        /// The HttpWebResponse object that was received from the server.
        /// </summary>
        public HttpWebResponse Response { get; set; }

        /// <summary>
        /// Initializes a new instance of the ResponseReceivedEventArgs class.
        /// </summary>
        /// <param name="response">The HttpWebResponse received from the server.</param>
        public ResponseReceivedEventArgs(HttpWebResponse response)
        {
            Response = response;
        }
    }

    /// <summary>
    /// This class holds information about a asynchronously received server response.
    /// </summary>
    public class AsyncResponseReceivedEventArgs
    {
        /// <summary>
        /// The HttpResponseMessage that was received from the server.
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Initializes a new instance of the AsyncResponseReceivedEventArgs class.
        /// </summary>
        /// <param name="response">The HttpResponseMessage object received from the server.</param>
        public AsyncResponseReceivedEventArgs(HttpResponseMessage response)
        {
            Response = response;
        }
    }

    /// <summary>
    /// Class to create web requests and receive a response from the server.
    /// </summary>
    public static class WebRequest
    {
        /// <summary>
        /// The Response Code that was received on the last request.
        /// </summary>
        public static HttpStatusCode LastResponseCode { get; set; }
        /// <summary>
        /// AsyncResponseReceived is raised when an asynchronous response is received from the server.
        /// </summary>
        public static event EventHandler<AsyncResponseReceivedEventArgs> AsyncResponseReceived;

        /// <summary>
        /// ResponseReceived is raised when a response is received from the server.
        /// </summary>
        public static event EventHandler<ResponseReceivedEventArgs> ResponseReceived;

        /// <summary>
        /// Sends a GET request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public static async Task<String> SendGetAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");
            }

            using (var httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(uri))
                {
                    if (response != null)
                    {
                        if (AsyncResponseReceived != null)
                        {
                            AsyncResponseReceived(null, new AsyncResponseReceivedEventArgs(response));
                        }

                        //Request was successful
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            //Getting the Strava API usage data.
                            KeyValuePair<String, IEnumerable<String>> usage = response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Usage"));

                            if (usage.Value != null)
                            {
                                //Setting the related Properties in the Limits-class.
                                Limits.Usage = new Usage(Int32.Parse(usage.Value.ElementAt(0).Split(',')[0]),
                                    Int32.Parse(usage.Value.ElementAt(0).Split(',')[1]));
                            }

                            //Getting the Strava API limits
                            KeyValuePair<String, IEnumerable<String>> limit = response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Limit"));

                            if (limit.Value != null)
                            {
                                //Setting the related Properties in the Limits-class.
                                Limits.Limit = new Limit(Int32.Parse(limit.Value.ElementAt(0).Split(',')[0]),
                                    Int32.Parse(limit.Value.ElementAt(0).Split(',')[1]));
                            }

                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Sends a POST request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public static async Task<String> SendPostAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(uri, null))
                {
                    if (response != null)
                    {
                        if (AsyncResponseReceived != null)
                        {
                            AsyncResponseReceived(null, new AsyncResponseReceivedEventArgs(response));
                        }

                        //Getting the Strava API usage data.
                        KeyValuePair<String, IEnumerable<String>> usage =
                            response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Usage"));

                        if (usage.Value != null)
                        {
                            //Setting the related Properties in the Limits-class.
                            Limits.Usage = new Usage(Int32.Parse(usage.Value.ElementAt(0).Split(',')[0]),
                                Int32.Parse(usage.Value.ElementAt(0).Split(',')[1]));
                        }

                        //Getting the Strava API limits
                        KeyValuePair<String, IEnumerable<String>> limit =
                            response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Limit"));

                        if (limit.Value != null)
                        {
                            //Setting the related Properties in the Limits-class.
                            Limits.Limit = new Limit(Int32.Parse(limit.Value.ElementAt(0).Split(',')[0]),
                                Int32.Parse(limit.Value.ElementAt(0).Split(',')[1]));
                        }

                        //Request was successful
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Sends a PUT request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public static async Task<String> SendPutAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");
            }

            using (var httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsync(uri, null))
                {
                    if (response != null)
                    {
                        if (AsyncResponseReceived != null)
                        {
                            AsyncResponseReceived(null, new AsyncResponseReceivedEventArgs(response));
                        }

                        //Getting the Strava API usage data.
                        KeyValuePair<String, IEnumerable<String>> usage = response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Usage"));

                        if (usage.Value != null)
                        {
                            //Setting the related Properties in the Limits-class.
                            Limits.Usage = new Usage(Int32.Parse(usage.Value.ElementAt(0).Split(',')[0]),
                                Int32.Parse(usage.Value.ElementAt(0).Split(',')[1]));
                        }

                        //Getting the Strava API limits
                        KeyValuePair<String, IEnumerable<String>> limit = response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Limit"));

                        if (limit.Value != null)
                        {
                            //Setting the related Properties in the Limits-class.
                            Limits.Limit = new Limit(Int32.Parse(limit.Value.ElementAt(0).Split(',')[0]),
                                Int32.Parse(limit.Value.ElementAt(0).Split(',')[1]));
                        }

                        LastResponseCode = response.StatusCode;
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Sends a DELETE request to the server asynchronously.
        /// </summary>
        /// <param name="uri">The Uri where the request will be sent.</param>
        /// <returns>The server's response.</returns>
        public static async Task<String> SendDeleteAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("Parameter uri must not be null. Please commit a valid Uri object.");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = await httpClient.DeleteAsync(uri))
                {

                    if (response != null)
                    {
                        if (AsyncResponseReceived != null)
                        {
                            AsyncResponseReceived(null, new AsyncResponseReceivedEventArgs(response));
                        }

                        //Getting the Strava API usage data.
                        KeyValuePair<String, IEnumerable<String>> usage = response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Usage"));

                        if (usage.Value != null)
                        {
                            //Setting the related Properties in the Limits-class.
                            Limits.Usage = new Usage(Int32.Parse(usage.Value.ElementAt(0).Split(',')[0]),
                                Int32.Parse(usage.Value.ElementAt(0).Split(',')[1]));
                        }

                        //Getting the Strava API limits
                        KeyValuePair<String, IEnumerable<String>> limit = response.Headers.ToList().Find(x => x.Key.Equals("X-RateLimit-Limit"));

                        if (limit.Value != null)
                        {
                            //Setting the related Properties in the Limits-class.
                            Limits.Limit = new Limit(Int32.Parse(limit.Value.ElementAt(0).Split(',')[0]),
                                Int32.Parse(limit.Value.ElementAt(0).Split(',')[1]));
                        }

                        LastResponseCode = response.StatusCode;
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }

            return String.Empty;
        }
    }
}

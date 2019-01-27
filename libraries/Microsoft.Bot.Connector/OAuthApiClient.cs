﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Bot.Connector
{
    /// <summary>
    /// Service client to handle requests to the Bot Framework API service.
    /// </summary>
    public class OAuthClient : ServiceClient<OAuthClient>
    {
        private readonly ConnectorClient _client;
        private readonly string _uri;

        /// <summary>
        /// The default endpoint that is used for API requests.
        /// </summary>
        public static string OAuthEndpoint { get; set; } = AuthenticationConstants.OAuthUrl;


        /// <summary>
        /// When using the Emulator, whether to emulate the OAuthCard behavior or use connected flows
        /// </summary>
        public static bool EmulateOAuthCards { get; set; } = false;

        /// <summary>
        /// Initializes an new instance of the <see cref="OAuthClient"/> class.
        /// </summary>
        /// <param name="client">The Bot Connector REST client to use.</param>
        /// <param name="uri">The URL to use to get a token.</param>
        public OAuthClient(ConnectorClient client, string uri)
        {
            if (!(Uri.TryCreate(uri, UriKind.Absolute, out var uriResult)))
                throw new ArgumentException("Please supply a valid https uri");
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _uri = uri;
        }

        /// <summary>
        /// Gets a user token for a given user and connection.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="connectionName">Name of the auth connection to use.</param>
        /// <param name="magicCode">The user entered code to validate.</param>
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task completes successfully, the <see cref="TokenResponse"/> contains the user token.</remarks>
        public async Task<TokenResponse> GetUserTokenAsync(string userId, string connectionName, string magicCode, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("userId", userId);
                tracingParameters.Add("connectionName", connectionName);
                tracingParameters.Add("magicCode", magicCode);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "GetUserTokenAsync", tracingParameters);
            }
            // Construct URL
            var tokenUrl = new Uri(new Uri(_uri + (_uri.EndsWith("/") ? "" : "/")), "api/usertoken/GetToken?userId={userId}&connectionName={connectionName}{magicCodeParam}").ToString();
            tokenUrl = tokenUrl.Replace("{connectionName}", Uri.EscapeDataString(connectionName));
            tokenUrl = tokenUrl.Replace("{userId}", Uri.EscapeDataString(userId));
            if (!string.IsNullOrEmpty(magicCode))
            {
                tokenUrl = tokenUrl.Replace("{magicCodeParam}", $"&code={Uri.EscapeDataString(magicCode)}");
            }
            else
            {
                tokenUrl = tokenUrl.Replace("{magicCodeParam}", String.Empty);
            }

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("GET");
            httpRequest.RequestUri = new Uri(tokenUrl);

            // Set Credentials
            if (_client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            cancellationToken.ThrowIfCancellationRequested();

            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            httpResponse = await _client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }
            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            if (statusCode == HttpStatusCode.OK)
            {
                string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var tokenResponse = Rest.Serialization.SafeJsonConvert.DeserializeObject<TokenResponse>(responseContent);
                    return tokenResponse;
                }
                catch (JsonException)
                {
                    // ignore json exception and return null
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }
                    return null;
                }
            }
            else if (statusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Signs the user out of a connection.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="connectionName">Name of the auth connection to sign out of.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task completes successfully, the response indicates whether the call to
        /// sign the user out was successful.</remarks>
        public async Task<bool> SignOutUserAsync(string userId, string connectionName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("userId", userId);
                tracingParameters.Add("connectionName", connectionName);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "SignOutUserAsync", tracingParameters);
            }

            // Construct URL
            var tokenUrl = new Uri(new Uri(_uri + (_uri.EndsWith("/") ? "" : "/")), "api/usertoken/SignOut?&userId={userId}{connectionNameParam}").ToString();
            tokenUrl = tokenUrl.Replace("{connectionNameParam}", string.IsNullOrEmpty(connectionName) ? 
                String.Empty : 
                $"&connectionName={Uri.EscapeDataString(connectionName)}");
            tokenUrl = tokenUrl.Replace("{userId}", Uri.EscapeDataString(userId));
            
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("DELETE");
            httpRequest.RequestUri = new Uri(tokenUrl);

            // Set Credentials
            if (_client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            cancellationToken.ThrowIfCancellationRequested();

            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            httpResponse = await _client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode _statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            if (_statusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the raw signin link to be sent to the user for signin for a connection name.
        /// </summary>
        /// <param name="state">A serialized and encoded parameter of a TokenExchangeState parameter.</param>
        /// <param name="finalRedirect">The endpoint URL for the final page of a succesful login attempt.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task completes successfully and the call to the OAuth client is successful,
        /// the result contains the signin link.</remarks>
        public async Task<string> GetSignInLinkAsync(string state, string finalRedirect = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("state", state);
                tracingParameters.Add("finalRedirect", finalRedirect);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "GetSignInLinkAsync", tracingParameters);
            }

            // Construct URL
            var tokenUrl = new Uri(new Uri(_uri + (_uri.EndsWith("/") ? "" : "/")), "api/botsignin/getsigninurl?&state={state}{finalRedirectParam}").ToString();
            tokenUrl = tokenUrl.Replace("{state}", state);
            tokenUrl = tokenUrl.Replace("{finalRedirectParam}", string.IsNullOrEmpty(finalRedirect) ?
                String.Empty :
                $"&finalRedirect={Uri.EscapeDataString(finalRedirect)}");
            
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("GET");
            httpRequest.RequestUri = new Uri(tokenUrl);

            // Set Credentials
            if (_client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            cancellationToken.ThrowIfCancellationRequested();

            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            httpResponse = await _client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            if (statusCode == HttpStatusCode.OK)
            {
                var link = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                return link;
            }
            return String.Empty;
        }

        /// <summary>
        /// Get the status of tokens for connections for this bot for a particular user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includeFilter">A comma seperated list of connections to include. If null, then all connections are returned</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TokenStatus[]> GetTokenStatusAsync(string userId, string includeFilter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            // Tracing
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("userId", userId);
                tracingParameters.Add("includeFilter", includeFilter);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "GetTokenStatusAsync", tracingParameters);
            }

            // Construct URL
            var tokenUrl = new Uri(new Uri(_uri + (_uri.EndsWith("/") ? "" : "/")), "api/usertoken/gettokenstatus?userId={userId}{includeFilterParam}").ToString();
            tokenUrl = tokenUrl.Replace("{userId}", Uri.EscapeDataString(userId));
            if (!string.IsNullOrEmpty(includeFilter))
            {
                tokenUrl = tokenUrl.Replace("{includeFilterParam}", $"&include={Uri.EscapeDataString(includeFilter)}");
            }
            else
            {
                tokenUrl = tokenUrl.Replace("{includeFilterParam}", String.Empty);
            }
            
            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("GET");
            httpRequest.RequestUri = new Uri(tokenUrl);
            
            // Set Credentials
            if (_client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            cancellationToken.ThrowIfCancellationRequested();

            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }

            httpResponse = await _client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            if (statusCode == HttpStatusCode.OK)
            {
                string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var statuses = Rest.Serialization.SafeJsonConvert.DeserializeObject<TokenStatus[]>(responseContent);
                    return statuses;
                }
                catch (JsonException)
                {
                    // ignore json exception and return null
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }
                    return null;
                }
            }
            else if (statusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieve an Azure Active Directory token for particular AAD resources.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="connectionName">Name of the auth connection to use for AAD token exchange.</param>
        /// <param name="resourceUrls">The collection of resource URLs for which to get tokens</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task completes successfully, the response includes a collection of TokenResponse
        /// objects with the resourceUrl and its corresponding TokenResponse.</remarks>
        public async Task<Dictionary<string, TokenResponse>> GetAadTokensAsync(string userId, string connectionName, string[] resourceUrls, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            if (resourceUrls == null)
            {
                throw new ArgumentNullException(nameof(resourceUrls));
            }
            
            if (resourceUrls.Length == 0)
            {
                throw new ArgumentException("Collection cannot be empty", nameof(resourceUrls));
            }

            if (resourceUrls.Any(s => string.IsNullOrEmpty(s)))
            {
                throw new ArgumentException("Resource URLs must have a value", nameof(resourceUrls));
            }

            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("userId", userId);
                tracingParameters.Add("connectionName", connectionName);
                tracingParameters.Add("resourceUrls", resourceUrls);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(invocationId, this, "SignOutUserAsync", tracingParameters);
            }

            // Construct URL
            var tokenUrl = new Uri(new Uri(_uri + (_uri.EndsWith("/") ? "" : "/")), "api/usertoken/GetAadTokens?userId={userId}&connectionName={connectionName}").ToString();
            tokenUrl = tokenUrl.Replace("{userId}", Uri.EscapeDataString(userId));
            tokenUrl = tokenUrl.Replace("{connectionName}", Uri.EscapeDataString(connectionName));

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new Uri(tokenUrl);

            // Serialize Request
            string requestContent = Rest.Serialization.SafeJsonConvert.SerializeObject(new AadResourceUrls() { ResourceUrls = resourceUrls }, _client.SerializationSettings);
            httpRequest.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
            httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
           
            // Set Credentials
            if (_client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }
            cancellationToken.ThrowIfCancellationRequested();

            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            httpResponse = await _client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (statusCode == HttpStatusCode.OK)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        var tokens = Rest.Serialization.SafeJsonConvert.DeserializeObject<Dictionary<string, TokenResponse>>(responseContent);
                        return tokens;
                    }
                    catch (JsonException)
                    {
                        // ignore json exception and return null
                        return null;
                    }
                }
                else if (statusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    var ex = new ErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", statusCode));
                    string responseContent = null;
                    try
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        ErrorResponse errorBody = Rest.Serialization.SafeJsonConvert.DeserializeObject<ErrorResponse>(responseContent, _client.DeserializationSettings);
                        if (errorBody != null)
                        {
                            ex.Body = errorBody;
                        }
                    }
                    catch (JsonException)
                    {
                        // Ignore the exception
                    }
                    ex.Request = new HttpRequestMessageWrapper(httpRequest, requestContent);
                    ex.Response = new HttpResponseMessageWrapper(httpResponse, responseContent);
                    if (shouldTrace)
                    {
                        ServiceClientTracing.Error(invocationId, ex);
                    }
                    throw ex;
                }
            }
            finally
            {
                httpRequest.Dispose();
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }
            }
        }

        /// <summary>
        /// Send a dummy OAuth card when the bot is being used on the Emulator for testing without fetching a real token.
        /// </summary>
        /// <param name="emulateOAuthCards">Indicates whether the Emulator should emulate the OAuth card.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SendEmulateOAuthCardsAsync(bool emulateOAuthCards)
        {
            bool shouldTrace = ServiceClientTracing.IsEnabled;
            string invocationId = null;
            if (shouldTrace)
            {
                invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("emulateOAuthCards", emulateOAuthCards);
                ServiceClientTracing.Enter(invocationId, this, "SendEmulateOAuthCards", tracingParameters);
            }

            var cancellationToken = default(CancellationToken);
            // Construct URL
            var tokenUrl = new Uri(new Uri(_uri + (_uri.EndsWith("/") ? "" : "/")), "api/usertoken/emulateOAuthCards?emulate={emulate}").ToString();
            tokenUrl = tokenUrl.Replace("{emulate}", emulateOAuthCards.ToString());

            // Create HTTP transport objects
            var httpRequest = new HttpRequestMessage();
            HttpResponseMessage httpResponse = null;
            httpRequest.Method = new HttpMethod("POST");
            httpRequest.RequestUri = new Uri(tokenUrl);
            
            // Set Credentials
            if (_client.Credentials != null)
            {
                await _client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            if (shouldTrace)
            {
                ServiceClientTracing.SendRequest(invocationId, httpRequest);
            }
            httpResponse = await _client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            if (shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(invocationId, httpResponse);
            }
        }
    }
}

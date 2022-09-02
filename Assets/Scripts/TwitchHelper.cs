// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using BrainCloud.LitJson;


public class TwitchHelper
{
    public delegate void AuthorizationGranted(string accessToken, string email, string username);
    public delegate void AuthorizationDenied();

    private AuthorizationGranted m_AuthorizationGranted = null;
    private AuthorizationDenied m_AuthorizationDenied = null;
    private string m_ClientId;
    private string m_ClientSecret;
    private string m_RedirectUrl;
    private string m_AuthState;
    private string m_AccessToken;
    private string m_UserEmail;
    private string m_Username;
    private System.Threading.SynchronizationContext m_SyncContext = null;

    public string AccessToken
    {
        get { return m_AccessToken; }
    }

    public string UserEmail
    {
        get { return m_UserEmail; }
    }

    public string Username
    {
        get { return m_Username; }
    }

    public TwitchHelper(string clientId, string clientSecret, string redirectUrl)
    {
        m_ClientId = clientId;
        m_ClientSecret = clientSecret;
        m_RedirectUrl = redirectUrl;
        m_AuthState = "";
        m_AccessToken = "";
        m_UserEmail = "";
        m_Username = "";
    }

    public async Task<bool> ValidateAccessToken(string accessToken)
    {
        string apiUrl = "https://id.twitch.tv/oauth2/validate";

        HttpClient http = new HttpClient();
        http.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        HttpResponseMessage response = await http.GetAsync(apiUrl);
        return response.StatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<string> GetUser(string accessToken)
    {
        string apiUrl = "https://api.twitch.tv/helix/users";

        HttpClient http = new HttpClient();
        http.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        http.DefaultRequestHeaders.Add("Client-Id", m_ClientId);

        HttpResponseMessage response = await http.GetAsync(apiUrl);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string json = await response.Content.ReadAsStringAsync();
            return json;
        }
        else
            return null;
    }

    public async Task<string> GetAcessToken(string authCode)
    {
        string apiUrl = "https://id.twitch.tv/oauth2/token" +
                        "?client_id=" + m_ClientId +
                        "&client_secret=" + m_ClientSecret +
                        "&code=" + authCode +
                        "&grant_type=authorization_code" +
                        "&redirect_uri=" + UnityWebRequest.EscapeURL(m_RedirectUrl);

        HttpClient http = new HttpClient();
        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        httpRequest.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        HttpResponseMessage response = await http.SendAsync(httpRequest);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string json = await response.Content.ReadAsStringAsync();
            return json;
        }
        else
            return null;
    }

    public void StartLocalCallbackServer(string authState, AuthorizationGranted authorizationGranted, AuthorizationDenied authorizationDenied)
    {
        // Initialize the Twitch properties
        m_AccessToken = "";
        m_UserEmail = "";
        m_Username = "";
        m_AuthState = authState;
        m_AuthorizationGranted = authorizationGranted;
        m_AuthorizationDenied = authorizationDenied;
        m_SyncContext = System.Threading.SynchronizationContext.Current;

        // Create and start the HttpListener for the Twitch redirect callback
        HttpListener httpListener = new HttpListener();
        httpListener.Prefixes.Add(m_RedirectUrl);
        httpListener.Start();
        httpListener.BeginGetContext(new AsyncCallback(CallbackHttpRequest), httpListener);
    }

    private void CallbackHttpRequest(IAsyncResult result)
    {
        bool success = false;
        string code;
        string state;
        string error;
        HttpListener httpListener;
        HttpListenerContext httpContext;
        HttpListenerRequest httpRequest;
        HttpListenerResponse httpResponse;

        // Get the reference to the HttpListener
        httpListener = (HttpListener)result.AsyncState;

        // Fetch the context object
        httpContext = httpListener.EndGetContext(result);

        // The context object has the request object for us, that holds details about the incoming request
        httpRequest = httpContext.Request;

        code = httpRequest.QueryString.Get("code");
        state = httpRequest.QueryString.Get("state");
        error = httpRequest.QueryString.Get("error");

        // check that we got a code value and the state value matches our remembered one
        if ((error == null || error == "") && code.Length > 0 && state == m_AuthState)
        {
            Debug.Log("TwitchHelper recieved auth code: " + code);

            Task<string> responseData = GetAcessToken(code);

            while (!responseData.IsCompleted) { }

            Debug.Log("TwitchHelper recieved access token: " + responseData.Result);

            JsonData jsonData = JsonMapper.ToObject(responseData.Result);
            m_AccessToken = jsonData["access_token"].ToString();
            string tokenType = jsonData["token_type"].ToString();
            string refreshToken = jsonData["refresh_token"].ToString();
        }

        // Build a response to send an "ok" back to the browser for the user to see
        httpResponse = httpContext.Response;
        string responseString = "<html><body><b>DONE!</b><br>(You can close this tab/window now)</body></html>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

        // Send the output to the client browser
        httpResponse.ContentLength64 = buffer.Length;
        System.IO.Stream output = httpResponse.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        // The HTTP listener has served it's purpose, shut it down
        httpListener.Stop();

        // Is the access token empty or not
        if (m_AccessToken != "")
        {
            // Validate the access token with the Twitch server
            Task<bool> accessTokenValidation = ValidateAccessToken(m_AccessToken);
            while (!accessTokenValidation.IsCompleted) { }

            if (accessTokenValidation.Result)
            {
                // Get the user's data from the Twitch resource server, including the email address
                // Wait for the task to complete
                Task<string> responseData = GetUser(m_AccessToken);
                while (!responseData.IsCompleted) { }

                Debug.Log("TwitchHelper recieved user data: " + responseData.Result);

                // Parse the Json data
                JsonData jsonData = JsonMapper.ToObject(responseData.Result);
                JsonData userData = jsonData["data"];

                // Ensure there is data in the user array
                if (userData.IsArray && userData.Count > 0)
                {
                    // Parse the user's email and display name
                    m_UserEmail = userData[0]["email"].ToString();
                    m_Username = userData[0]["display_name"].ToString();
                    success = true;
                }

                // If success == true, then authentication with Twitch succeeded,
                // invoke the authorization granted delegate
                if (success)
                {
                    // This is most likely not the main thread, use the SynchronizationContext to
                    // ensure the callback is delegate is invoked on the main thread
                    m_SyncContext.Post(_ =>
                    {
                        if (m_AuthorizationGranted != null)
                            m_AuthorizationGranted(m_AccessToken, m_UserEmail, m_Username);
                    }, null);
                }
            }
        }

        // If success == false, if so authentication with Twitch failed,
        // invoke the authorization denied delegate
        if (!success)
        {
            // This is most likely not the main thread, use the SynchronizationContext to
            // ensure the callback is delegate is invoked on the main thread
            m_SyncContext.Post(_ =>
            {
                if (m_AuthorizationDenied != null)
                    m_AuthorizationDenied();
            }, null);
        }
    }
}
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Web.Mvc;

namespace Oath2.Controllers
{
    public class OAuthController : Controller
    {
        public void Callback(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                this.GetTokens(code);
            }
        }

        public ActionResult GetTokens(string code) 
        {
            var tokenFile = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\tokens.json";

            var credentialsFile = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\Credentials.json";

            var credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));

            //RestClient restClient = new RestClient();
            RestRequest request = new RestRequest();

            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("code", code);
            request.AddQueryParameter("grant_type", "authorization_code");
            request.AddQueryParameter("redirect_uri", "https://localhost:44391/OAuth/Callback");

            //restClient.BaseUrl ="https://oauth2.googleapis.com/token";
            var restClient = new RestClient(new Uri("https://oauth2.googleapis.com/token"));
            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(tokenFile, response.Content);
                return RedirectToAction("Index", "Home");
            }

            return View("Error");
        }
        public ActionResult Refreshtoken()
        {
            var tokenFile = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\tokens.json";
            var credentialsFile = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\Credentials.json";
            var credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));

            RestRequest request = new RestRequest();

            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("grant_type", "refresh_token");
            request.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());

            var restClient = new RestClient(new Uri("https://oauth2.googleapis.com/token"));
            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(tokenFile, newTokens.ToString());
                return RedirectToAction("Index","Home", new { status = "success"});
            }

            return View();
        }

        public ActionResult Revoketoken() 
        {
            var tokenFile = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));

            RestRequest request = new RestRequest();

            request.AddQueryParameter("token", tokens["access_token"].ToString());
            var restClient = new RestClient(new Uri("https://oauth2.googleapis.com/revoke"));
            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Home", new {status = "success" });
            }
            return View("Error");
        }

    }
}
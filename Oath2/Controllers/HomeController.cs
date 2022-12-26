using Facebook;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;

namespace Oath2.Controllers
{
    public class HomeController : Controller
    {
        string accessTokenFacebook;
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult GoogleSignIn()
        {
            var credentialsFile = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\Credentials.json";
            JObject credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            var clientid = credentials["client_id"];

            var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                "scope=https://www.googleapis.com/auth/userinfo.email&" +
                "access_type=offline&" +
                //"include_granted_scopes=true&" +
                "response_type=code&" +
                "state=&" +
                "redirect_uri=https://localhost:44391/OAuth/Callback&" +
                "client_id=" + clientid;

            return Redirect(redirectUrl);
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult FacebookSignIn()
        {
            //var fb = new FacebookClient();
            //var loginUrl = fb.GetLoginUrl(new
            //{
            //    client_id = "2199053333600218",
            //    client_secret = "3a97c49bc80db712cf61eb8f34593307",
            //    redirect_uri = RedirectUri.AbsoluteUri,
            //    response_type = "code",
            //    scope = "email"
            //    client_id=clientid&client_secret=ClientSecret&redirect_uri=RedirectUri&response_type=code&scope=email
            //});
            var loginUrl = "https://www.facebook.com/dialog/oauth?" +
                "client_id=2199053333600218&" +
                "redirect_uri=https://localhost:44391/Home/FacebookCallback&" +
                "response_type = code&" +
                "scope=email&";

            return Redirect(loginUrl);
        }
        public ActionResult FacebookCallback(string code)
        {
            //var credentialsFiles = "C:\\Users\\Zbkl\\source\\repos\\Oath2\\Oath2\\File\\FacebookCredentials.json";
            //var fb = new FacebookClient();
            //dynamic result = fb.Post("oauth/access_token", new
            //{
            //    client_id = "2199053333600218",
            //    client_secret = "3a97c49bc80db712cf61eb8f34593307",
            //    redirect_uri = RedirectUri.AbsoluteUri,
            //    code = code
            //});

            RestRequest requestCallback = new RestRequest();

            requestCallback.AddQueryParameter("client_id", "2199053333600218");
            requestCallback.AddQueryParameter("client_secret", "3a97c49bc80db712cf61eb8f34593307");
            requestCallback.AddQueryParameter("redirect_uri", "https://localhost:44391/Home/FacebookCallback");
            requestCallback.AddQueryParameter("code", code);

            var restClientCallback = new RestClient(new Uri("https://graph.facebook.com/v15.0/oauth/access_token?"));
            var callbackCredentials = restClientCallback.Get(requestCallback);
            JObject credentials = JObject.Parse(callbackCredentials.Content);
            //https://graph.facebook.com/me?fields=first_name,last_name,id,email + aplication Json + Authorization bearer: accestoken

            RestRequest userRequest = new RestRequest();
            userRequest.AddHeader("Authorization", "Bearer " + credentials["access_token"]);
            Session["AccessToken"] = credentials["access_token"];

            var userParam = new RestClient(new Uri("https://graph.facebook.com/me?fields=first_name,last_name,id,email"));

            var userData = userParam.Get(userRequest);
            JObject paramsUser = JObject.Parse(userData.Content);
            //accessTokenFacebook = result.access_token;

            //fb.AccessToken = accessTokenFacebook;

            //dynamic me = fb.Get("me?fields=first_name,last_name,id,email");
            //string email = me.email;
            //string firstName = me.first_name;
            //string lastName = me.last_name;
            //string userId = me.id;
            //Session["userId"] = userId;

            //FormsAuthentication.SetAuthCookie(email, false);

            return RedirectToAction("Index", "Home");

        }
        public void FacebookLogOut()
        {
            //var webBrowser = new WebBrowser();
            //webBrowser.Navigated += (o, args) =>
            //{
            //    if (args.Url.AbsoluteUri == "https://www.facebook.com/connect/login_success.html")
            //        Close();
            //};

            //string id = Session["userId"].ToString();
            //dynamic res = fb.Delete(Session["AccessToken"].ToString());
            //dynamic res = fb.Delete("me/permissions");
            RestRequest request = new RestRequest();
            request.AddQueryParameter("access_token", Session["AccessToken"].ToString());
            var userParam = new RestClient(new Uri("https://graph.facebook.com/me/permissions/email"));
            var resp = userParam.Delete(request);

            Response.Redirect("Index");
        }
    }
}

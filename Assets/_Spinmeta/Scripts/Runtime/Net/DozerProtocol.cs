using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    public class DozerProtocol
    {
        private static string _host;
        public static RestClient Client { get; private set; }
        public static Action<string, RestClient.Response> OnResponse;

        public static void Init(string host, MonoBehaviour mono)
        {
            _host = host;
            Client = new RestClient(mono);
        }

        private static string GetPath(string path)
        {
            return string.Format("{0}{1}", _host, path);
        }

        private static RestClient.Request<T> CreateRequest<T>(string path, RestClient.Method method, int retry = 3) where T : RestClient.Response
        {
            var req = new RestClient.Request<T>(GetPath(path)).SetMethod(method).SetRetry(retry).OnComplete(req =>
            {
                if (req.IsSuccess)
                {
                    OnResponse?.Invoke(req.URI, req.GetResponse());
                }
            });
            return req;
        }

        public static RestClient.Request<SignInResponse> SignIn(string email, string passward, Action<SignInResponse> onComplete = null)
        {
            var req = CreateRequest<SignInResponse>("auth/signin", RestClient.Method.POST_WITH_JSONBODY);
            req.AddBody("email", email);
            req.AddBody("passwd", passward);
            return Client.Send(req, onComplete);
        }

        public static RestClient.Request<SignUpResponse> SignUp(string email, string passward, Action<SignUpResponse> onComplete = null)
        {
            var req = CreateRequest<SignUpResponse>("auth/signup", RestClient.Method.POST_WITH_JSONBODY);
            req.AddBody("email", email);
            req.AddBody("passwd", passward);
            return Client.Send(req, onComplete);
        }

        public static RestClient.Request<UserProfileResponse> UserProfile()
        {
            var req = CreateRequest<UserProfileResponse>("user/profile", RestClient.Method.GET);
            return Client.Send<UserProfileResponse>(req);
        }

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    public static class DozerProtocolListener
    {
        public static void Init()
        {
            DozerProtocol.OnResponse += OnResponse;
        }

        private static void OnResponse(string path, RestClient.Response resp)
        {
            if (resp is SignInResponse signInResp)
            {
                User.Me.SetToken(signInResp.accessToken);
            }
            else if (resp is SignUpResponse signUpResp)
            {
                User.Me.SetToken(signUpResp.accessToken);
            }
            else if (resp is UserProfileResponse profileResp)
            {
                User.Me.SetUserID(profileResp.user_id);
                User.Me.SetNickname(profileResp.nickname);
                Debug.Log($"[DozerProtocolListener] OnResponse: {profileResp}");
            }
        }
    }
}

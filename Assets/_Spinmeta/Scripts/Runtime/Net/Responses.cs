using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    public class SignInResponse : RestClient.Response
    {
        public string userId;
        public string accessToken;
    }

    public class SignUpResponse : RestClient.Response
    {
        public string userId;
        public string accessToken;
    }

    public class UserProfileResponse : RestClient.Response
    {
        public string user_id;
        public string nickname;
        public string referral_code;
        public int? avatar;
        public string country;
        public int gender; //0 : unknown , 1 : male , 2 : female
        public string reg_dt;
        public string introduce;
        public string referrer_id;
    }
}
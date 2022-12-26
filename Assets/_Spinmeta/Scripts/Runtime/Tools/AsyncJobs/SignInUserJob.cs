using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    public class SignUserInJob : AsyncJob
    {
        private string _email;
        private string _password;

        public SignUserInJob(string email, string password, int weight = 1) : base(weight)
        {
            _email = email;
            _password = password;

            AddWork("AutoSignIn", SignIn());
            AddWork("SignUp", SignUp());
        }

        private IEnumerator SignIn()
        {
            var reqSignIn = DozerProtocol.SignIn(_email, _password);
            while (reqSignIn.IsDone == false)
            {
                SetProgress(reqSignIn.Progress);
                yield return null;
            }
            SetProgress(1f);

            if (reqSignIn.IsSuccess)
            {
                Done();
            }
            else if (reqSignIn.Error.code != Constants.ErrorCode.EMAIL_NOT_FOUND)
            {
                Fail(reqSignIn.Error);
            }
            else
            {
                //SignUp 으로 
            }

            yield break;
        }

        private IEnumerator SignUp()
        {
            var reqSignUp = DozerProtocol.SignUp(_email, _password);
            while (reqSignUp.IsDone == false)
            {
                SetProgress(0.5f + reqSignUp.Progress * 0.5f);
                yield return null;
            }

            if (reqSignUp.IsSuccess)
            {
                Done();
            }
            else
            {
                Fail(reqSignUp.Error);
            }

            yield break;
        }
    }
}
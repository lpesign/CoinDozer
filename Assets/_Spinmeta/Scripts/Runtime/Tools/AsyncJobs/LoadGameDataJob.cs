using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    public class LoadGameDataJob : AsyncJob
    {
        public LoadGameDataJob(string gameId)
        {
            AddWork("LoadUserData", LoadUserData());
        }

        private IEnumerator LoadUserData()
        {
            if (SavedData.HasUserData())
            {
                User.Me.SetData(SavedData.LoadUsersData());
            }
            else
            {
                User.Me.SetData(UserData.CreateNewUser());
                SavedData.SaveUserData(User.Me.GetUserData());
            }

            Done();

            yield break;
        }
    }
}
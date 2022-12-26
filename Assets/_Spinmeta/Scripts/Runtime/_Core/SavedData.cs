using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LP.Dozer
{
    public static class SavedData
    {
        #region AccessToken
        public static void SaveAccessToken(string token)
        {
            LocalData.Set("token", token, LocalData.SetMethod.Both);
        }

        public static string LoadAccessToken()
        {
            return LocalData.Get("token", null, LocalData.GetMethod.LocalStorageOrPlayerPrefs);
        }

        public static void DeleteAccessToken()
        {
            LocalData.DeleteKey("token", LocalData.SetMethod.Both);
        }

        public static bool HasAccessToken()
        {
            return LocalData.HasKey("token", LocalData.GetMethod.LocalStorageOrPlayerPrefs);
        }

        #endregion

        #region UserData

        public static void SaveUserData(UserData data)
        {
            Debug.Verbose($"Save UserData. coin {data.coin} / {data.maxCoin}");

            // var serializedData = SerializerUtil.Serialize(data);
            var serializedData = SerializerUtil.SerializeToBase64String(data);
            LocalData.Set("userdata", serializedData);
        }

        public static UserData LoadUsersData()
        {
            var serializedData = LocalData.Get("userdata");
            // var data = SerializerUtil.Deserialize<UserData>(serializedData);
            var data = SerializerUtil.DeserializeFromBase64String<UserData>(serializedData);
            Debug.Verbose($"Load UserData. coin {data.coin} / {data.maxCoin}");

            return data;
        }

        public static void DeleteUserData()
        {
            LocalData.DeleteKey("userdata", LocalData.SetMethod.Both);
        }

        public static bool HasUserData()
        {
            return LocalData.HasKey("userdata");
        }

        #endregion

        #region GameData

        public static void SaveGameData(string gameID, GameData gameData)
        {
            var serializedData = SerializerUtil.SerializeToBase64String(gameData);
            LocalData.Set($"gameData_{gameID}", serializedData);
        }

        public static GameData LoadGameData(string gameID)
        {
            var serializedData = LocalData.Get($"gameData_{gameID}");
            var gameData = SerializerUtil.DeserializeFromBase64String<GameData>(serializedData);
            return gameData;
        }

        public static void DeleteGameData(string gameID)
        {
            LocalData.DeleteKey($"gameData_{gameID}");
        }

        public static bool HasGameData(string gameID)
        {
            return LocalData.HasKey($"gameData_{gameID}");
        }

        #endregion

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LP.Dozer.EditorScript
{
    public class DozerEditorMenu
    {
        [MenuItem("Spinmeta/Dozer/RemoveLocalAccount", false)]
        private static void RemoveAccount()
        {
            SavedData.DeleteAccessToken();
        }

        [MenuItem("Spinmeta/Dozer/RemoveLocalUserData", false)]
        private static void RemoveUserData()
        {
            SavedData.DeleteUserData();
        }
    }
}

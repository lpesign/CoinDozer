using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{

    public class GameManager : BaseGameManager<GameManager>
    {
        #region static
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeceneLoadRuntimeMethod()
        {

            Debug.Level = Debug.LogLevel.Verbose;
#if LOG && UNITY_WEBGL && !UNITY_EDITOR
            Debug.LogPrefix = "[Unity]";
#endif
            Application.runInBackground = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoadRuntimeMethod()
        {
            CreateInstance(typeof(GameManager).Name);
        }
        #endregion

        protected override void GameManagerAwake()
        {
            base.GameManagerAwake();

            DozerProtocol.Init(Constants.Address.API, this);
            DozerProtocolListener.Init();
            
        }
    }
}
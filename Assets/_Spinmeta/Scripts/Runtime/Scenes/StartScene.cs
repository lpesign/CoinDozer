using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LP.Dozer
{
    public class StartScene : BaseScene
    {
        [SerializeField] private Button _enterButton;
        [SerializeField] private GameObject _IDAndPassword;
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private BaseLoadingScreen _loadingScreen;

        private string _email;
        private string _password;

        protected override void Awake()
        {
            base.Awake();

            _enterButton.onClick.AddListener(OnEnterClick);
            _enterButton.gameObject.SetActive(false);

            _IDAndPassword.SetActive(false);
            _emailField.onSubmit.AddListener(OnSubmit);
            _passwordField.onSubmit.AddListener(OnSubmit);
        }

        protected override void EnterScene()
        {
            base.EnterScene();

            // #if !LIVE
            //             DevTool.Instance.AddMenu("LOG", "DevToolLogView");
            // #endif

            string token = null; //todo 브라우저로부터 토큰 전달 받는 코드 필요
            if (string.IsNullOrEmpty(token))
            {
                token = SavedData.LoadAccessToken();
            }

            User.Me.SetToken(token);

            if (User.Me.IsLogin)
            {
                Debug.Log("[StartScene] login already.\ntoken: " + User.Me.Token);
                LoadGameScene();
            }
            else
            {
                Debug.Log("[StartScene] not logined. wait email and password");
                WaitEmailAndPassword();
            }


        }

        public void OnSubmit(string value)
        {
            OnEnterClick();
        }

        private void WaitEmailAndPassword()
        {
            _enterButton.gameObject.SetActive(true);
            _IDAndPassword.SetActive(true);
        }

        private void OnEnterClick()
        {
            _email = _emailField.text;
            _password = _passwordField.text;

            LoadGameScene();
        }

        private void LoadGameScene()
        {
            StartCoroutine(LoadGameSceneCoroutine());
        }

        public IEnumerator LoadGameSceneCoroutine()
        {
            Debug.Log("[StartScene] LoadGameScene");


            _IDAndPassword.SetActive(false);

            var testGameID = "1";

            List<AsyncJob> jobs = new List<AsyncJob>(){
                new AsyncJobRestRequest<UserProfileResponse>(() => DozerProtocol.UserProfile()),
                new LoadGameDataJob(testGameID)
            };

            if (!User.Me.IsLogin)
            {
                jobs.Insert(0, new SignUserInJob(_email, _password));
            }

            var wait = GameManager.Scene.LoadScene(Constants.SceneName.Game, jobs, _loadingScreen);
            yield return wait;

            Debug.Log("[StartScene] LoadGameScene done");

            if (wait.HasError)
            {
                Debug.Log($"[StartScene Login Error. code: {wait.Error.code} message: {wait.Error.message}");
                User.Me.Logout();
                _IDAndPassword.SetActive(true);
            }
            else
            {
                Debug.Log("[StartScene] LoadGameScene success");
            }

            yield break;
        }
    }
}
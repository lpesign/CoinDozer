using System.Collections;
using System.Collections.Generic;
using LP.attribute;
using UnityEngine;

namespace LP.Dozer
{
    public class GameScene : BaseScene
    {
        [Separator("Dozer")]
        [SerializeField] private string _gameId;

        [Separator("UI")]
        [SerializeField] private UITop _uiTop;
        [SerializeField] private UIUserItems _uiItems;

        [Separator("Test")]
        [SerializeField] private string _testId;

        public override bool IsSceneReady { get => User.Me.IsLogin; }

        private BaseDozer _dozer;

        protected override IEnumerator ReadyToStart()
        {
            Debug.Log("------------------ ReadyToStart 'GameScene' ... HasAccessToken: " + SavedData.HasAccessToken());

            ErrorData error = null;

            if (SavedData.HasAccessToken())
            {
                var accessToken = SavedData.LoadAccessToken();
                User.Me.SetToken(accessToken);

                List<AsyncJob> jobs = new List<AsyncJob>(){
                    new AsyncJobRestRequest<UserProfileResponse>(() => DozerProtocol.UserProfile()),
                    new LoadGameDataJob(_gameId)
                };

                var runner = new AsyncJobRunner(this);
                runner.AddJobs(jobs);
                var waitRunner = runner.Run();
                yield return waitRunner;

                if (waitRunner.HasError)
                {
                    error = waitRunner.Error;
                }
            }
            else
            {
                var wait = GameManager.Scene.LoadScene(Constants.SceneName.Start);
                yield return wait;

                if (wait.HasError)
                {
                    error = wait.Error;
                }
            }

            if (error == null)
            {
                Debug.Log("------------------ReadyToStart 'GameScene' done!");
            }
            else
            {
                Debug.LogError($"[GameScene Failed Error: {error}");
            }

            yield break;
        }



        protected override void EnterScene()
        {
            base.EnterScene();

            FindGameID();

            CameraSystem.SetCustomBlendSetting(_dozer.Factory.blenderSettings);

            _dozer.Init(_gameId);
            _uiTop.Init(_dozer);
            _uiItems.Init(_dozer);

            _dozer.StartGame();
        }

        private void FindGameID()
        {
            _dozer = GetComponentInChildren<BaseDozer>();
            if (_dozer != null)
            {
                var dozerNamespace = _dozer.GetType().Namespace;
                var gameIDFromNamespace = dozerNamespace.Split('.')[^1].Substring(1);

                if (int.TryParse(gameIDFromNamespace, out int slotIDInt))
                {
                    _gameId = slotIDInt.ToString();
                }
            }
            else if (string.IsNullOrEmpty(_testId) == false)
            {
                _gameId = _testId;
            }
        }

        public void NewUserAndGame()
        {
            _dozer.StopGame();

            User.Me.SetData(UserData.CreateNewUser());
            SavedData.SaveUserData(User.Me.GetUserData());
            SavedData.DeleteGameData(_gameId);

            _dozer.StartGame();
        }
    }
}
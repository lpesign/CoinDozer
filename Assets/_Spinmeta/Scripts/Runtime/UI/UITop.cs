using System.Collections;
using System.Collections.Generic;
using LP.attribute;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using LP.Extension;
using UnityEngine.UI;

namespace LP.Dozer
{
    public class UITop : MonoBehaviour
    {

        [Separator("Coin")]
        [SerializeField] private TextMeshProUGUI _coiText;

        [Separator("Time")]
        [SerializeField] private int _nextCoinSeconds = 3;
        [SerializeField] private TextMeshProUGUI _nextTimeText;

        [Separator("Test")]
        [SerializeField] private Button _resetButton;

        private int _secondsToLeft;
        private BaseDozer _dozer;

        private void Awake()
        {
            _resetButton.onClick.AddListener(() =>
            {
                BaseScene.GetCurrentScene<GameScene>().NewUserAndGame();
            });
        }

        private void OnEnable()
        {
            User.Me.OnCoinChanged += OnCoinChanged;
        }

        private void OnDisable()
        {
            User.Me.OnCoinChanged -= OnCoinChanged;
        }

        public void Init(BaseDozer dozer)
        {
            _dozer = dozer;
            _secondsToLeft = _nextCoinSeconds;
            UpdateCoinText(User.Me.Coin);

            _dozer.OnGameStart.AddListener( dozer =>
            {
                Run();
            });
        }

        private void OnCoinChanged(User user)
        {
            UpdateCoinText(user.Coin);
        }

        private void UpdateCoinText(int coin)
        {
            _coiText.SetNumber(coin, 0, true);
        }

        private void UpdateText()
        {
            _nextTimeText.text = TimeUtil.ConveretDigitalTime(_secondsToLeft);
        }

        public void Run()
        {
            RunAsync().Forget();
        }

        public async UniTaskVoid RunAsync()
        {
            do
            {
                _secondsToLeft--;
                UpdateText();

                await UniTask.Delay(1000, cancellationToken: _dozer.TokenSource.Token);

                if (_secondsToLeft <= 0)
                {
                    _secondsToLeft = _nextCoinSeconds;
                    User.Me.EarnCoin(1);
                }

            } while (_dozer.TokenSource.Token.IsCancellationRequested == false);
        }

    }
}
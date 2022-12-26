using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LP.Dozer
{
    public class UIUserItemIcon : MonoBehaviour
    {
        private static readonly int ANIM_RECOVERY = Animator.StringToHash("Recovery");
        public enum State
        {
            Ready,
            Active,
            CoolDown,
        }

        [SerializeField] private Animator _anim;
        [SerializeField] private Image _iconBack;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _activeObject;

        public UnityEvent<UIUserItemIcon> OnClick;

        private DozerSkill.Data _skillData;
        private UserItem _userItem;

        public DozerSkill.Data SkillData { get => _skillData; }
        public float Progress { set => _icon.fillAmount = value; }

        private State _state;

        private float _waitingTime = 0f;

        CancellationTokenSource disableCancellation;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                OnClick?.Invoke(this);
            });
        }

        private void OnEnable()
        {
            if (disableCancellation != null)
            {
                disableCancellation.Dispose();
            }
            disableCancellation = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            disableCancellation.Cancel();
        }

        public void Init(DozerSkill.Data data, UserItem userItem)
        {
            _skillData = data;
            _userItem = userItem;
            _userItem.OnValueChanged += (item) =>
            {
                SetCount(item.Count);
            };

            SetCount(userItem.Count);
            SetIcon(data.icon);
            SetState(UIUserItemIcon.State.Ready);
        }

        public DozerSkill.Data Use()
        {
            _userItem.Use();
            return _skillData;
        }

        private void SetIcon(Sprite icon)
        {
            _iconBack.sprite = icon;
            _icon.sprite = icon;
        }

        public void SetCount(int value)
        {
            _countText.text = value.ToString();

            if (_button.interactable == false && value > 0 && _state == State.Ready)
            {
                _anim.SetTrigger(ANIM_RECOVERY);
            }

            _button.interactable = value > 0;
        }

        private async UniTaskVoid WaitCooldown()
        {
            _waitingTime = 0f;

            while (_waitingTime < _skillData.cooltime && !disableCancellation.IsCancellationRequested)
            {
                _waitingTime += Time.deltaTime;
                Progress = _waitingTime / _skillData.cooltime;
                await UniTask.NextFrame(disableCancellation.Token);
            }

            _waitingTime = 0f;
            _anim.SetTrigger(ANIM_RECOVERY);

            SetState(State.Ready);
        }


        public void SetState(State state)
        {
            switch (state)
            {
                case State.Ready:
                    Progress = 1f;
                    _button.enabled = true;
                    _activeObject.SetActive(false);
                    break;

                case State.Active:
                    Progress = 1f;
                    _button.enabled = false;
                    _activeObject.SetActive(true);
                    break;

                case State.CoolDown:
                    _button.enabled = false;
                    _activeObject.SetActive(false);
                    WaitCooldown().Forget();
                    break;
            }

            _state = state;
        }
    }
}
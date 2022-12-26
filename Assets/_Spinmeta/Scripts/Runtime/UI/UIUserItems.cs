using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    public class UIUserItems : MonoBehaviour
    {
        [SerializeField] private UIUserItemIcon _iconTemplate;
        [SerializeField] private Transform _iconParent;

        private BaseDozer _dozer;
        private List<UIUserItemIcon> _icons;

        private void Awake()
        {
            _iconTemplate.gameObject.SetActive(false);
            _icons = new List<UIUserItemIcon>();
        }

        public void Init(BaseDozer dozer)
        {
            _dozer = dozer;

            _dozer.OnGameStart.AddListener(dozer =>
            {
                CreateIcons();
            });

            _dozer.OnGameStop.AddListener(dozer =>
            {
                Clear();
            });
        }

        public void Clear()
        {
            foreach (var icon in _icons)
            {
                Destroy(icon.gameObject);
            }

            _icons.Clear();
        }

        private void CreateIcons()
        {
            List<UserItem> items = User.Me.GetItems();

            var skillBox = _dozer.Factory.skillBox;
            foreach (var item in items)
            {
                DozerSkill.Data skillData = skillBox.GetSkillData(item.ID);
                if (skillData == null)
                {
                    Debug.LogWarning($"itemID: {item.ID} skillData is null");
                    continue;
                }

                UIUserItemIcon icon = Instantiate(_iconTemplate, _iconParent);

                icon.gameObject.SetActive(true);
                icon.Init(skillData, item);

                icon.OnClick.AddListener(OnIconClick);

                _icons.Add(icon);
            }
        }

        private void OnIconClick(UIUserItemIcon icon)
        {
            UseSkill(icon);
        }

        private UIUserItemIcon GetIcon(string itemID)
        {
            return _icons.Find(icon => icon.SkillData.id == itemID);
        }

        private void UseSkill(UIUserItemIcon icon)
        {
            DozerSkill.Data data = icon.SkillData;

            if (_dozer.CanUseSkill(data) == false)
            {
                return;
            }

            Debug.Log($"useskill: {data.id} sucess");
            icon.Use();
            icon.SetState(UIUserItemIcon.State.Active);
            DozerSkill skill = _dozer.Factory.CreateSkill(data).OnComplete(skill =>
            {
                icon.SetState(UIUserItemIcon.State.CoolDown);
            });
            _dozer.UseSkill(skill);
        }
    }
}
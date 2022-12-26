using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LP.attribute;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

namespace LP.Dozer
{
    public abstract class BaseDozer : MonoBehaviour
    {
        public enum CameraType
        {
            Default,
            Top,
            Back,
            FrontTop
        }

        [SerializeField, ReadOnly] private string _gameId;
        [Separator("Cameras")]
        [SerializeField] protected CinemachineVirtualCamera _default;
        [SerializeField] protected CinemachineVirtualCamera _top;
        [SerializeField] protected CinemachineVirtualCamera _back;
        [SerializeField] protected CinemachineVirtualCamera _frontTop;

        [Separator("Factory")]
        [SerializeField] protected SODozerFactory _factory;

        [Separator("Tap")]
        [SerializeField] protected float _rayMaxDistance = 100f;
        [SerializeField] protected LayerMask _rayMask = Constants.LayerMask.TAP;

        [Separator("Parts")]
        [SerializeField] protected Pusher _pusher;
        [SerializeField] protected DozerFloor _floor;

        [Separator("Coin")]
        [SerializeField] protected PreloadedCoins _preloadedCoins;
        [SerializeField] protected FallArea _fallArea;

        [SerializeField] protected CoinSpawner _spawner;
        [SerializeField] private float _saveInterval = 0.3f;

        public UnityEvent<RaycastHit> OnTap;
        public UnityEvent<CollectableItem> OnDrop;

        private Dictionary<string, CollectableItem> _items;


        private CancellationTokenSource _gameTokenSource;
        public CancellationTokenSource TokenSource => _gameTokenSource;

        private GameData _gameData;
        private HashSet<CollectableItem> _removeItems;
        private HashSet<DozerSkill> _addedSkills;

        public bool IgnoreTap { get; set; }
        public CoinSpawner Spawner { get => _spawner; }
        public Dictionary<string, CollectableItem> Items { get => _items; }
        public SODozerFactory Factory { get => _factory; }
        public DozerFloor Floor { get => _floor; }

        public UnityEvent<BaseDozer> OnGameInit;
        public UnityEvent<BaseDozer> OnGameStart;
        public UnityEvent<BaseDozer> OnGameStop;

        private void Awake()
        {
            _preloadedCoins.gameObject.SetActive(false);
            _items = new Dictionary<string, CollectableItem>();
            _gameData = new GameData();
            _removeItems = new HashSet<CollectableItem>();
            _addedSkills = new HashSet<DozerSkill>();
        }

        private void OnDestroy()
        {
            if (_gameTokenSource != null)
            {
                if (_gameTokenSource.IsCancellationRequested == false)
                {
                    _gameTokenSource.Cancel();
                }
                _gameTokenSource.Dispose();
            }
        }

        public void Init(string gameID)
        {
            _gameId = gameID;
            _spawner.Init(this);
            IgnoreTap = true;

            InitCatchers();
            InitCameras();

            OnGameInit?.Invoke(this);
        }

        private void InitCatchers()
        {
            CoinCatcher[] coinCatchers = GetComponentsInChildren<CoinCatcher>();
            foreach (var catcher in coinCatchers)
            {
                catcher.OnCatch.AddListener(OnCoinCatch);
            }
        }

        private void InitCameras()
        {
            _default.gameObject.SetActive(true);
            _top.gameObject.SetActive(false);
            _back.gameObject.SetActive(false);
            _frontTop.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            _gameTokenSource = new CancellationTokenSource();

            StartGameAsync().Forget();
        }

        private async UniTaskVoid StartGameAsync()
        {
            if (SavedData.HasGameData(_gameId))
            {
                LoadGameData(SavedData.LoadGameData(_gameId));
            }
            else
            {
                CreatePreloadedCoins();
            }


            await UniTask.Delay(100);

            IgnoreTap = false;

            StartSaveItems();

            _pusher.StartPush();

            OnGameStart?.Invoke(this);
        }

        public void StopGame()
        {
            if (_gameTokenSource != null)
            {
                _gameTokenSource.Cancel();
                _gameTokenSource.Dispose();
            }

            _pusher.StopPush(true);

            foreach (var item in _items.Values)
            {
                _spawner.Release(item);
            }

            _items.Clear();
            _gameData.Clear();
            _removeItems.Clear();
            _addedSkills.Clear();

            IgnoreTap = true;

            InitCameras();

            OnGameStop?.Invoke(this);
        }

        private void OnCoinCatch(CoinCatcher catcher, CollectableItem item)
        {
            if (item != null)
            {
                DeleteItem(item);

                if (catcher.IsCollector)
                {
                    Debug.Log($"@@@ Coin Collected '{item.name}' by {catcher.name}");
                    User.Me.EarnCoin(1);

                    SavedData.SaveUserData(User.Me.GetUserData());
                }
                else if (catcher.IsDestroyer)
                {
                    Debug.Log($"@@@ Coin Lost '{item.name}' by {catcher.name}");
                }

                SavedData.SaveGameData(_gameId, _gameData);
            }
        }

        public void LoadGameData(GameData gameData)
        {
            Debug.Log($"[BaseDozer] Load coins items: {gameData.coinDatas.Count}");

            try
            {
                foreach (CollectableItemData item in gameData.coinDatas.Values)
                {
                    CollectableItem coin = _spawner.GetItem(item);
                    AddItem(coin);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[BaseDozer] LoadGameData error: {e.Message}");
                CreatePreloadedCoins();
            }
        }


        public void CreatePreloadedCoins()
        {
            Debug.Log("[BaseDozer] create new coins");

            List<CollectableItem> preloadedCoins = _preloadedCoins.GetPreloadedCoins();
            for (var i = 0; i < preloadedCoins.Count; ++i)
            {
                CollectableItem item = preloadedCoins[i];
                var pos = item.transform.position;
                var itemType = item.ItemType;
                CollectableItem coin = _spawner.Spawn(pos, itemType);
                AddItem(coin);
            }
        }

        private void StartSaveItems()
        {
            SaveItemsAsync(_gameTokenSource.Token).Forget();
        }

        private async UniTaskVoid SaveItemsAsync(CancellationToken token)
        {
            int ms = (int)(_saveInterval * 1000);

            while (token.IsCancellationRequested == false)
            {
                // await UniTask.WaitForFixedUpdate();
                var isCancel = await UniTask.Delay(ms, cancellationToken: token).SuppressCancellationThrow();
                if (isCancel)
                {
                    break;
                }

                // await UniTask.RunOnThreadPool(() =>
                // {
                //     SavedData.SaveGameData(_gameId, _gameData);
                // });

                SavedData.SaveGameData(_gameId, _gameData);
            }
        }

        private bool CanDropCoin()
        {
            if (User.Me.Coin <= 0)
            {
                Debug.LogWarning($"코인 모자람");
                return false;
            }

            return true;
        }

        public bool TryDropCoin(Vector3 hitPoint)
        {
            if (CanDropCoin())
            {
                var dropPos = _fallArea.GetDroppablePos(hitPoint.x, hitPoint.z);
                DropCoin(dropPos);

                User.Me.PayCoin(1);
                SavedData.SaveUserData(User.Me.GetUserData());
                SavedData.SaveGameData(_gameId, _gameData);

                return true;
            }
            else
            {
                return false;
            }
        }

        public CollectableItem DropCoin(Vector3 dropPos)
        {
            CollectableItem item = _spawner.Spawn(dropPos);
            AddItem(item);

            OnDrop?.Invoke(item);
            return item;
        }

        private CollectableItem AddItem(CollectableItem item)
        {
            if (_items.ContainsKey(item.Data.id) == false)
            {
                _items.Add(item.Data.id, item);
                _gameData.AddItem(item.Data);
                return item;
            }
            else
            {
                Debug.LogWarning($"!!!!!! already added item: {item.name}");
                return null;
            }
        }

        private void DeleteItem(CollectableItem item)
        {
            if (_items.ContainsKey(item.Data.id))
            {
                _items.Remove(item.Data.id);
                _gameData.RemoveItem(item.Data);
            }

            _spawner.Release(item);
        }

        public List<CollectableItem> GetGroundedItems()
        {
            //todo 바닥에 붙어있는 코인만 반환해야함
            var result = new List<CollectableItem>();

            foreach (CollectableItem item in _items.Values)
            {
                result.Add(item);
            }

            return result;
        }


        private void Update()
        {
            (bool isHit, RaycastHit hit) = DetectTap();

            if (isHit && IgnoreTap == false)
            {
                OnTap?.Invoke(hit);
                TryDropCoin(hit.point);
            }
        }


        private void FixedUpdate()
        {
            foreach (var item in _items.Values)
            {
                if (item.Position.y < -10f)
                {
                    _removeItems.Add(item);
                }
                else
                {
                    item.UpdateData();
                }
            }

            if (_removeItems.Count > 0)
            {
                foreach (var item in _removeItems)
                {
                    DeleteItem(item);
                }

                _removeItems.Clear();
            }
        }


        public bool CanUseSkill(DozerSkill.Data data)
        {
            if (GetSkill(data.id) != null)
            {
                Debug.LogWarning($"skill '{data.id}' is using");
                return false;
            }

            DozerSkill skill = _factory.CreateSkill(data);
            if (skill == null)
            {
                Debug.LogError($"[BaseDozer] Failed to create skill: {data.id}");
                return false;
            }

            return true;
        }

        public void UseSkill(DozerSkill skill)
        {
            skill.SetToken(_gameTokenSource.Token).Execute(this);

            if (skill.HasError)
            {
                Debug.LogError($"[BaseDozer] Failed to add skill: {skill.Error}");
            }
            else
            {
                if (skill.IsDone == false)
                {
                    skill.OnComplete(skill =>
                    {
                        RemoveSkill(skill);
                    });
                    _addedSkills.Add(skill);
                }
            }

            SavedData.SaveUserData(User.Me.GetUserData());
        }

        public void RemoveSkill(DozerSkill skill)
        {
            if (_addedSkills != null && _addedSkills.Contains(skill))
            {
                _addedSkills.Remove(skill);
            }
            else
            {
                Debug.LogWarning($"[BaseDozer] Failed to remove skill: {skill.id}");
            }
        }

        public DozerSkill GetSkill(string skillID)
        {
            if (_addedSkills == null)
            {
                return null;
            }

            foreach (var skill in _addedSkills)
            {
                if (skill.id == skillID)
                {
                    return skill;
                }
            }

            return null;
        }

        private (bool isHit, RaycastHit hit) DetectTap()
        {
            var isHit = false;
            RaycastHit hit = default(RaycastHit);

            if (Input.GetMouseButtonDown(0))
            {

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, _rayMaxDistance, _rayMask))
                {
                    if (hit.collider.gameObject.layer == Constants.PhysicsLayer.TAP)
                    {
                        isHit = true;
                    }
                }
            }

            return (isHit, hit);
        }


        private CinemachineVirtualCamera GetCamera(CameraType type)
        {
            switch (type)
            {
                case CameraType.Default:
                    return _default;
                case CameraType.Top:
                    return _top;
                case CameraType.Back:
                    return _back;
                case CameraType.FrontTop:
                    return _frontTop;
                default:
                    return _default;
            }
        }

        public CinemachineVirtualCamera SetActiveCamera(CameraType type, bool active)
        {
            var cam = GetCamera(type);
            cam.gameObject.SetActive(active);
            return cam;
        }
    }
}
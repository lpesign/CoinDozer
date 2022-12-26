using System.Collections;
using System.Collections.Generic;
using System.Text;
using LP.attribute;
using UnityEngine;

namespace LP.Dozer
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private LPGameObjectPoolGroup _poolGroup;
        [SerializeField] protected Transform _coinPlatform;

        private BaseDozer _dozer;

        private void Reset()
        {
            if (_poolGroup == null)
            {
                _poolGroup = GetComponentInChildren<LPGameObjectPoolGroup>();
            }
        }

        public void Init(BaseDozer dozer)
        {
            _dozer = dozer;
            foreach (var itemInfo in dozer.Factory.itemBox.items)
            {
                _poolGroup.AddPool<CollectableItem>(itemInfo.type.ToString(), itemInfo.poolSize, () =>
                {
                    var item = Instantiate(itemInfo.prefab);
                    return item;
                });
            }
        }

        public void Release(CollectableItem item)
        {
            if (item == null)
            {
                return;
            }

            _poolGroup.Release<CollectableItem>(item.ItemType.ToString(), item);
        }

        public CollectableItem GetItem(ItemType itemType = ItemType.Coin)
        {
            var item = _poolGroup.Get<CollectableItem>(itemType.ToString(), true, _coinPlatform);
            return item;
        }

        public CollectableItem GetItem(CollectableItemData data)
        {
            var item = GetItem(data.itemType);
            item.ApplyData(data);
            return item;
        }

        public CollectableItem Spawn(CollectableItem item)
        {
            item.transform.SetParent(_coinPlatform, true);
            return item;
        }

        public CollectableItem Spawn(Vector3 pos)
        {
            ItemType itemType = _dozer.Factory.itemBox.GetRandomItemType();
            return Spawn(pos, itemType);
        }

        public CollectableItem Spawn(Vector3 pos, ItemType itemType)
        {
            var item = GetItem(itemType);
            item.transform.position = pos;

            return item;
        }
    }
}

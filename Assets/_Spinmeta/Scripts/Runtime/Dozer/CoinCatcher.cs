using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LP.Dozer
{
    [RequireComponent(typeof(BoxCollider))]
    public class CoinCatcher : MonoBehaviour
    {
        public enum Type
        {
            Collect,
            Destory
        }

        [SerializeField] protected Type _type = Type.Collect;
        public bool IsDestroyer => _type == Type.Destory;
        public bool IsCollector => _type == Type.Collect;

        public UnityEvent<CoinCatcher, CollectableItem> OnCatch;

        private void Reset()
        {
            gameObject.layer = Constants.PhysicsLayer.DEFAULT;
            var collider = GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<CollectableItem>(out CollectableItem item) == false)
            {
                item = other.gameObject.GetComponentInParent<CollectableItem>();

            }

            if (item != null)
            {
                OnCatch?.Invoke(this, item);
            }
            else
            {
                Debug.LogWarning($"Unknown object '{other.name}' Destroyed by {name}");
                Destroy(other.gameObject);

                OnCatch?.Invoke(this, null);
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    [Serializable]
    public class CollectableItemData
    {
        public string id;
        public string kind;
        public ItemType itemType;

        public float posX, posY, posZ;

        public float rotX, rotY, rotZ, rotW;

        public float vx, vy, vz;

        public float avx, avy, avz;

        public CollectableItemData(ItemType itemType)
        {
            this.id = System.Guid.NewGuid().ToString();
            this.itemType = itemType;
        }

        public Vector3 Position => new Vector3(posX, posY, posZ);
        public Quaternion Rotation => new Quaternion(rotX, rotY, rotZ, rotW);
        public Vector3 Velocity => new Vector3(vx, vy, vz);
        public Vector3 AngularVelocity => new Vector3(avx, avy, avz);
    }

    public enum ItemType
    {
        Coin,
        FancyCoin,
        Tree
    }

    [RequireComponent(typeof(Rigidbody))]
    public abstract class CollectableItem : MonoBehaviour
    {
        [SerializeField] protected ItemType _itemType;
        public ItemType ItemType => _itemType;
        [SerializeField] protected Rigidbody _rgbd;

        public CollectableItemData Data { get; private set; }
        public Rigidbody rgbd { get => _rgbd; }
        public Vector3 Position { get => _rgbd.position; }

        protected virtual void Reset()
        {
            gameObject.layer = Constants.PhysicsLayer.ITEM;
            if (_rgbd == null)
            {
                _rgbd = GetComponent<Rigidbody>();
            }

            _rgbd.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        protected virtual void Awake()
        {
            Data = new CollectableItemData(_itemType);
        }

        protected virtual void OnEnable()
        {
            _rgbd.velocity = Vector3.zero;
            _rgbd.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        public void ApplyData(CollectableItemData data)
        {
            Data.id = data.id;
            Data.kind = data.kind;

            Data.posX = data.posX;
            Data.posY = data.posX;
            Data.posZ = data.posX;

            Data.rotX = data.rotX;
            Data.rotY = data.rotY;
            Data.rotZ = data.rotZ;
            Data.rotW = data.rotW;

            transform.position = data.Position;
            transform.rotation = data.Rotation;
            // _rgbd.position = data.Position;
            // _rgbd.rotation = data.Rotation;

            _rgbd.velocity = data.Velocity;
            _rgbd.angularVelocity = data.AngularVelocity;

            UpdateData();
        }

        public void UpdateData()
        {
            // var pos = transform.position;
            // var rotation = transform.rotation;
            var pos = _rgbd.position;
            var rotation = _rgbd.rotation;
            var velocity = _rgbd.velocity;
            var angleVelocity = _rgbd.angularVelocity;

            Data.posX = pos.x;
            Data.posY = pos.y;
            Data.posZ = pos.z;

            Data.rotX = rotation.x;
            Data.rotY = rotation.y;
            Data.rotZ = rotation.z;
            Data.rotW = rotation.w;

            Data.vx = velocity.x;
            Data.vy = velocity.y;
            Data.vz = velocity.z;

            Data.avx = angleVelocity.x;
            Data.avy = angleVelocity.y;
            Data.avz = angleVelocity.z;
        }
    }

}
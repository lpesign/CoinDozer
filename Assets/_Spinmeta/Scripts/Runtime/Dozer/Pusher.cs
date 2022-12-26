using UnityEngine;
using System.Collections;


namespace LP.Dozer
{
    public class Pusher : MonoBehaviour
    {
        [SerializeField] private PathMover _mover;
        [SerializeField] private Rigidbody _rgbd;
        [SerializeField] private bool _autoStartWhenStart = false;

        private void Reset()
        {
            if (_rgbd == null)
            {
                _rgbd = GetComponentInChildren<Rigidbody>();
            }

            if (_rgbd != null)
            {
                _rgbd.gameObject.layer = Constants.PhysicsLayer.PUSHER;
                _rgbd.mass = 100;
                _rgbd.useGravity = false;
                _rgbd.isKinematic = true;
                _rgbd.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        private void Start()
        {
            if (_autoStartWhenStart)
            {
                StartPush();
            }
        }

        public void StartPush()
        {
            _mover.StartMove();
        }

        public void StopPush(bool init = false)
        {
            _mover.StopMove();
            if (init)
            {
                _mover.Init();
            }
        }

        public void StartPowerPush(Vector3 overridePos, float speedMultiplier)
        {
            _mover.SetOverrideLastLocalNode(0, overridePos);
            _mover.SetSpeedMultiplier(speedMultiplier);
        }

        public void StopPowerPush()
        {
            _mover.ResetOverrideLocalNodes();
            _mover.ResetSpeedMultiplier();
        }
    }
}

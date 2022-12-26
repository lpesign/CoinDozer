using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    public class FallArea : MonoBehaviour
    {
        [SerializeField] private BoxCollider _defaultArea;
        [SerializeField] private BoxCollider _wholeArea;

        private void Awake()
        {
            _defaultArea.gameObject.layer = Constants.PhysicsLayer.IGNORE_RAYCAST;
            _defaultArea.isTrigger = true;

            _wholeArea.gameObject.layer = Constants.PhysicsLayer.IGNORE_RAYCAST;
            _wholeArea.isTrigger = true;
        }

        public Vector3 GetDroppablePos(float x, float z)
        {
            //todo 일정한 영역 안에서만 떨어지도록 해야함
            var y = _defaultArea.transform.position.y;

            return new Vector3(x, y, z);
        }

        public Vector3 GetRandomDroppablePos(bool isWholeArea = false)
        {
            BoxCollider area = isWholeArea ? _wholeArea : _defaultArea;
            var min = area.bounds.min;
            var max = area.bounds.max;
            var x = Random.Range(min.x, max.x);
            var y = area.transform.position.y;
            var z = Random.Range(min.z, max.z);

            return new Vector3(x, y, z);
        }
    }
}
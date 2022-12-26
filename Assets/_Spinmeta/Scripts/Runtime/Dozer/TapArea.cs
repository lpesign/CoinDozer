using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{

    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(GizmoAreaDrawer))]
    public class TapArea : MonoBehaviour
    {
        [SerializeField] private BoxCollider _collider;
        private void Reset()
        {
            gameObject.layer = Constants.PhysicsLayer.TAP;
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider>();
            }

            _collider.isTrigger = true;
        }
    }
}
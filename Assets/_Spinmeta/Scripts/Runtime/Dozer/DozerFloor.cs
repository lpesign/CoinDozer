using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LP.Dozer
{
    public class DozerFloor : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        public void Impulse(float force)
        {
            _impulseSource.GenerateImpulse(force);
        }
    }
}
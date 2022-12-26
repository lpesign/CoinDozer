using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LP.Dozer
{
    public class CameraSystem : Singleton<CameraSystem>
    {
        private CinemachineBrain _brain;

        private void Awake()
        {
            _brain = GetComponent<CinemachineBrain>();

        }

        public static void SetCustomBlendSetting(CinemachineBlenderSettings settings)
        {
            Instance._brain.m_CustomBlends = settings;
        }
    }
}
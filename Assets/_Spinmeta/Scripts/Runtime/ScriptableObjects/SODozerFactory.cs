using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace LP.Dozer
{
    [CreateAssetMenu(fileName = "SODozerFactory", menuName = "Spinmeta/Dozer/Factory/SODozerFactory", order = 0)]
    public class SODozerFactory : ScriptableObject
    {
        public SODozerItemBox itemBox;
        public SODozerSkillBox skillBox;
        public CinemachineBlenderSettings blenderSettings;


        public DozerSkill CreateSkill(DozerSkill.Data data)
        {
            DozerSkill skill = null;
            switch (data.id)
            {
                case DozerSkillWall.Data.ID:
                    skill = new DozerSkillWall(data);
                    break;

                case DozerSkillEarthQuake.Data.ID:
                    skill = new DozerSkillEarthQuake(data);
                    break;

                case DozerSKillCoinShower.Data.ID:
                    skill = new DozerSKillCoinShower(data);
                    break;

                case DozerSkillPowerPush.Data.ID:
                    skill = new DozerSkillPowerPush(data);
                    break;
            }

            if (skill == null)
            {
                Debug.LogError($"[SODozerFactory] Failed to create skill: {data.id}");
            }

            return skill;
        }
    }
}
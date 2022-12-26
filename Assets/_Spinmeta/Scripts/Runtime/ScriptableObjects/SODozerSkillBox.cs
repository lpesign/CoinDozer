using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Dozer
{
    [CreateAssetMenu(fileName = "SODozerSkillBox", menuName = "Spinmeta/Dozer/Factory/SODozerSkillBox")]
    public class SODozerSkillBox : ScriptableObject
    {
        public DozerSkillWall.Data wall;
        public DozerSkillPowerPush.Data powerPush;
        public DozerSkillEarthQuake.Data earthQuake;
        public DozerSKillCoinShower.Data coinShower;

        public DozerSkill.Data GetSkillData(string id)
        {
            switch (id)
            {
                case DozerSkillWall.Data.ID:
                    return wall;
                case DozerSkillPowerPush.Data.ID:
                    return powerPush;
                case DozerSkillEarthQuake.Data.ID:
                    return earthQuake;
                case DozerSKillCoinShower.Data.ID:
                    return coinShower;
                default:
                    return null;
            }
        }
    }
}
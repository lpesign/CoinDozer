using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    public class DozerSkillEarthQuake : DozerSkill
    {
        [System.Serializable]
        public new class Data : DozerSkill.Data
        {
            public const string ID = "SKILL_EARTHQUAKE";
            public override string id => ID;

            public float forceMin;
            public float forceMax;
            public float randomPos;
            [Min(0)] public float radius;
            public float upwards;
            public ForceMode mode;
            [Min(0)] public float randomRotation;
            public float impulseForce = 1f;

            public Data()
            {
                cooltime = 10f;

                forceMin = 20f;
                forceMax = 100f;
                randomPos = 0.1f;
                radius = 1f;
                upwards = 0f;
                mode = ForceMode.Impulse;
                randomRotation = 0.1f;
            }
        }

        public DozerSkillEarthQuake(DozerSkill.Data data) : base(data) { }

        public override void Execute(BaseDozer dozer)
        {
            base.Execute(dozer);

            var data = GetData<Data>();

            var groundedItems = dozer.GetGroundedItems();

            _dozer.Floor.Impulse(data.impulseForce);

            if (groundedItems.Count == 0)
            {
                Done();
                return;
            }

            foreach (CollectableItem coin in groundedItems)
            {
                float explosionForce = RandomUtil.Range(data.forceMin, data.forceMax);
                Vector3 explosionPosition = coin.transform.position;

                explosionPosition = coin.transform.position;
                if (data.randomPos > 0)
                {
                    var rx = RandomUtil.Range(data.randomPos);
                    var rz = RandomUtil.Range(data.randomPos);
                    explosionPosition += new Vector3(rx, 0, rz);
                }

                // Debug.Log($"AddExplosionForce {coin.name} force: {explosionForce} pos: {explosionPosition} radius: {_earthRadius} upwards: {_earthUpwards} mode: {_earthMode}");
                coin.rgbd.AddExplosionForce(explosionForce, explosionPosition, data.radius, data.upwards, data.mode);

                if (data.randomRotation > 0)
                {
                    var rx = RandomUtil.Range(data.randomRotation);
                    var ry = RandomUtil.Range(data.randomRotation);
                    var rz = RandomUtil.Range(data.randomRotation);
                    coin.rgbd.AddRelativeTorque(rx, ry, rz);
                }
            }

            Done();
        }
    }
}
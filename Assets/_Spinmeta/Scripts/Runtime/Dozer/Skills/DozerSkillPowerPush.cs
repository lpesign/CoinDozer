using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace LP.Dozer
{

    public class DozerSkillPowerPush : DozerSkill
    {
        [Serializable]
        public new class Data : DozerSkill.Data
        {
            public const string ID = "SKILL_POWER_PUSH";
            public override string id => ID;

            public float speedMultiplier;
            public float duration;
            public Vector3 overridePusherPos;

            public Data()
            {
                cooltime = 10f;

                speedMultiplier = 5f;
                duration = 4f;
                overridePusherPos = new Vector3(0f, 0.25f, 0f);
            }
        }

        public DozerSkillPowerPush(DozerSkill.Data data) : base(data) { }

        public override void Execute(BaseDozer dozer)
        {
            base.Execute(dozer);

            Pusher pusher = dozer.GetComponentInChildren<Pusher>();
            if (pusher == null)
            {
                Fail("pusher not found");
                return;
            }

            ExecuteAsync(pusher).Forget();
        }

        private async UniTaskVoid ExecuteAsync(Pusher pusher)
        {
            var data = GetData<Data>();

            _dozer.SetActiveCamera(BaseDozer.CameraType.Back, true);

            pusher.StartPowerPush(data.overridePusherPos, data.speedMultiplier);
            await UniTask.Delay(System.TimeSpan.FromSeconds(data.duration), cancellationToken: _token);
            pusher.StopPowerPush();

            _dozer.SetActiveCamera(BaseDozer.CameraType.Back, false);

            Done();
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LP.Dozer
{
    public class DozerSKillCoinShower : DozerSkill
    {
        [System.Serializable]
        public new class Data : DozerSkill.Data
        {
            public const string ID = "SKILL_COIN_SHOWER";
            public override string id => ID;

            public int minCount;
            public int maxCount;
            public float interval;

            public Data()
            {
                cooltime = 10f;

                minCount = 20;
                maxCount = 50;
                interval = 0.1f;
            }
        }

        public DozerSKillCoinShower(DozerSkill.Data data) : base(data) { }

        public override void Execute(BaseDozer dozer)
        {
            base.Execute(dozer);

            FallArea fallArea = dozer.GetComponentInChildren<FallArea>();
            if (fallArea == null)
            {
                Fail("fall area not found");
                return;
            }

            ExecuteAsync(fallArea).Forget();
        }

        private async UniTaskVoid ExecuteAsync(FallArea fallArea)
        {
            var data = GetData<Data>();

            _dozer.SetActiveCamera(BaseDozer.CameraType.Top, true);

            var count = Random.Range(data.minCount, data.maxCount);

            Debug.Log($"[DozerSKillCoinShower] count: {count}");

            _dozer.IgnoreTap = true;

            System.TimeSpan time = System.TimeSpan.FromSeconds(data.interval);

            while (count-- > 0 && _token.IsCancellationRequested == false)
            {
                var randomPos = fallArea.GetRandomDroppablePos(isWholeArea: true);
                var item = _dozer.DropCoin(randomPos);
                item.rgbd.AddForce(0f, -50f, 0f, ForceMode.Force);
                await UniTask.Delay(time, cancellationToken: _token);
            }

            _dozer.IgnoreTap = false;
            _dozer.SetActiveCamera(BaseDozer.CameraType.Top, false);
            Done();
        }
    }
}
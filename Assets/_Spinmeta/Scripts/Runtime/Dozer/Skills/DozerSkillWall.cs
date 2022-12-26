using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace LP.Dozer
{
    public class DozerSkillWall : DozerSkill
    {
        [Serializable]
        public new class Data : DozerSkill.Data
        {
            public const string ID = "SKILL_WALL";
            public override string id => ID;
            public float wallDuration = 10f;

            public Data()
            {
                cooltime = 10f;

                wallDuration = 10f;
            }
        }

        public DozerSkillWall(DozerSkill.Data data) : base(data) { }

        private Animator _wallAnimator;

        public override void Execute(BaseDozer dozer)
        {
            base.Execute(dozer);
            var walls = dozer.GetComponentsInChildren<DozerWall>();

            if (walls == null || walls.Length == 0)
            {
                Fail("No walls found");
                return;
            }

            ExecuteAsync(walls).Forget();
        }

        private async UniTaskVoid ExecuteAsync(DozerWall[] walls)
        {
            var data = GetData<Data>();

            _dozer.SetActiveCamera(BaseDozer.CameraType.FrontTop, true);

            foreach (var wall in walls)
            {
                wall.Open();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(data.wallDuration), cancellationToken: _token);

            foreach (var wall in walls)
            {
                Debug.Log($"wall: {wall.name} is closing");
                wall.Close();
            }

            _dozer.SetActiveCamera(BaseDozer.CameraType.FrontTop, false);

            Done();
        }
    }
}
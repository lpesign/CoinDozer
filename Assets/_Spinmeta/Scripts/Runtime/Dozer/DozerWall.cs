using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LP.Dozer
{
    public class DozerWall : MonoBehaviour
    {
        private readonly int ANIM_OPEN = Animator.StringToHash("IsOpen");
        [SerializeField] private Animator _wallAnimator;

        public void Start()
        {
            var dozer = GetComponentInParent<BaseDozer>();

            dozer.OnGameStop.AddListener(dozer =>
            {
                Close();
            });
        }

        public void Open()
        {
            _wallAnimator.SetBool(ANIM_OPEN, true);
        }

        public void Close()
        {
            _wallAnimator.SetBool(ANIM_OPEN, false);
        }
    }
}
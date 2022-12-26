using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace LP.Dozer
{
    public abstract class DozerSkill
    {
        public abstract class Data
        {
            public Sprite icon;
            [Range(0.1f, 600f)] public float cooltime;
            public abstract string id { get; }
        }

        public ErrorData Error { get; protected set; }
        public bool HasError { get { return Error?.HasError ?? false; } }
        public bool IsDone { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsSuccess { get { return IsDone && !HasError && !IsCancelled; } }

        public string id { get { return _data.id; } }

        private event Action<DozerSkill> _onComplete;
        protected BaseDozer _dozer;
        private DozerSkill.Data _data;
        protected CancellationToken _token;

        public DozerSkill(DozerSkill.Data data)
        {
            _data = data;
        }

        public T GetData<T>() where T : DozerSkill.Data
        {
            return (T)_data;
        }

        public DozerSkill OnComplete(Action<DozerSkill> onComplete)
        {
            _onComplete += onComplete;
            return this;
        }

        public void Done()
        {
            Debug.Log($"[DozerSkill] Done skill: {_data.id} IsSucess: {IsSuccess} {(Error == null ? String.Empty : Error.ToString())}");

            IsDone = true;
            _onComplete?.Invoke(this);
            _onComplete = null;
            _dozer = null;
            _token = default(CancellationToken);
        }

        public virtual void Fail(string errorMessage, int errorCode = -1)
        {
            Fail(new ErrorData().SetError(errorMessage, errorCode));
        }

        public void Fail(ErrorData errorData)
        {
            this.Error = errorData;

            Done();
        }

        public DozerSkill SetToken(CancellationToken token)
        {
            _token = token;
            return this;
        }

        public virtual void Execute(BaseDozer dozer)
        {
            _dozer = dozer;
            Debug.Log($"[DozerSkill] Execute skill: {_data.id}");
        }
    }
}
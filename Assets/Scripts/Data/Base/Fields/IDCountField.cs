using System;
using UnityEngine;

namespace Game.Data.Fields
{
    public abstract class IDCountField : StringField
    {
        [SerializeField] private int _count = 1;

        public virtual int Count
        {
            get => _count;
            protected set => _count = value;
        }

        public event Action<int> OnCountChanged;

        public void SetCount(int count)
        {
            if (!Count.Equals(count))
            {
                Count = count;
                OnCountChanged?.Invoke(count);
                InvokeDataChanged();
            }
        }

        public override void SetInstance(IDataField dataField)
        {
            base.SetInstance(dataField);

            if (dataField is IDCountField idCountField)
            {
                if (!Count.Equals(idCountField.Count))
                {
                    SetCount(idCountField.Count);
                }
            }
        }
    }
}
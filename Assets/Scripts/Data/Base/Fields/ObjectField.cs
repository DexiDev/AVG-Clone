using UnityEngine;

namespace Game.Data.Fields
{
    public abstract class ObjectField<TObject> : DataField<TObject> where TObject : Object
    {
        protected override bool Equals(TObject oldValue, TObject newValue)
        {
            return oldValue == newValue;
        }
    }
}
namespace Game.Data.Fields
{
    // [Serializable]
    public abstract class IntField : DataField<int>
    {
        public void IncreaseValue(int value)
        {
            SetValue(Value + value);
        }
        
        public void DecreaseValue(int value)
        {
            SetValue(Value - value);
        }
    }
}
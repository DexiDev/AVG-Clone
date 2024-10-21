namespace Game.Data.Fields
{
    // [Serializable]
    public abstract class FloatField : DataField<float>
    {
        public void IncreaseValue(float value)
        {
            SetValue(Value + value);
        }
        
        public void DecreaseValue(float value)
        {
            SetValue(Value - value);
        }
    }
}
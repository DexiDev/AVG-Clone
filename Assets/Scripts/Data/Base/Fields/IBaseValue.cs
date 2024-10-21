namespace Game.Data.Fields
{
    public interface IBaseValue<T> : IDataField<T>
    {
        T BaseValue { get; }

        void ResetToBase();
    }
}
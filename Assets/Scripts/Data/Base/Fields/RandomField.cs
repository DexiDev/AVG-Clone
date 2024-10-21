namespace Game.Data.Fields
{
    public struct RandomDataField<T> where T : IDataField
    {
        public T Field;
        public float Chance;
    }
    
    public abstract class RandomField<T> : ListField<RandomDataField<T>> where T : IDataField 
    {
        
    }
}
using System;

namespace Game.Data.Attributes.Fields
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class RequireDataFieldAttribute : Attribute
    {
        public Type RequiredFieldType { get; }

        public RequireDataFieldAttribute(Type requiredFieldType)
        {
            if (!typeof(IDataField).IsAssignableFrom(requiredFieldType))
            {
                throw new ArgumentException("Type must implement IDataField", nameof(requiredFieldType));
            }

            RequiredFieldType = requiredFieldType;
        }
    }
}
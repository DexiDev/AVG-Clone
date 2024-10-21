using UnityEngine;

namespace Game.Data.Fields.Stats
{
    public interface IStatField : IDataField, IValueToString
    {
        Sprite Icon { get; }
        string Name { get; }

        string ValueToStringUI();
    }
}
using Game.Data;
using Game.Data.Attributes;
using Game.Loadings;
using UnityEngine;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Data/Level/Level Data", fileName = "Level Data")]
    public class LevelData : DataScriptable
    {
        [field: SerializeField, DataID(typeof(LoadingConfig))] public string LoadingID { get; private set; }
    }
}
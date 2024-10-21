using Game.Data;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Loadings
{
    [CreateAssetMenu(menuName = "Data/Loading/Loading Data", fileName = "Loading Data")]
    public class LoadingData : DataScriptable
    {
        [field: OdinSerialize] public LoadingSettings[] LoadingSettings { get; private set; }
        [field: SerializeField] public float MinDurationLoading { get; private set; }
    }
}
using Game.Data;
using Game.Data.Attributes;
using Game.Loadings.UI;
using UnityEngine;

namespace Game.Loadings
{
    [CreateAssetMenu(menuName = "Data/Loading/Loading Config", fileName = "Loading Config")]
    public class LoadingConfig : DataConfig<LoadingData>
    {
        [SerializeField] public UILoadingScreen LoadingScreen { get; private set; }
        [SerializeField, DataID(typeof(LoadingConfig))] public string AutorunLoadingID { get; private set; }
        
        [SerializeField, DataID(typeof(LoadingConfig))] public string MainMenuID { get; private set; }
    }
}
using Game.Data;
using Game.Levels.UI;
using UnityEngine;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Data/Level/Level Config", fileName = "Level Config")]
    public class LevelConfig : DataConfig<LevelData>
    {
        [SerializeField] public int SkipForLoop { get; private set; }
        [SerializeField] public UILevelCompletedScreen UIWinScreen { get; private set; }
        [SerializeField] public UILevelCompletedScreen UILoseScreen { get; private set; }
    }
}
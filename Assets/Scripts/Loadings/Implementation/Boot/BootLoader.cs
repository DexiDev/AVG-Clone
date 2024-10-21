using Game.Levels;
using UnityEngine;
using VContainer;

namespace Game.Loadings.Boot
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private int _frameRate = 60;
        
        private LevelManager _levelManager;
        
        [Inject]
        private void Install(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        private void Awake()
        {
            Application.targetFrameRate = _frameRate;
        }

        private void Start()
        {
            _levelManager.LoadCurrentLevel();
        }
    }
}
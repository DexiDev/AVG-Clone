using System;
using System.Linq;
using Game.Data;
using Game.Loadings;
using Game.UI;
using VContainer;

namespace Game.Levels
{
    public class LevelManager : DataManager<LevelData, LevelConfig>
    {
        private const string _saveID = nameof(LevelManager);
        
        private int _currentLevelLoop;
        private int _currentLevelIndex;
        private LevelController _levelController;
        
        private LoadingManager _loadingManager;
        private UIManager _uiManager;

        public int CurrentLevelLoop => _currentLevelLoop;
        public string CurrentLevelID => _config.Datas[_currentLevelIndex]?.ID;
        public LevelController LevelController => _levelController;
        
        public event Action<int> OnLevelChanged;
        public event Action<int> OnLevelLoopChanged;
        public event Action<LevelController> OnControllerChanged;
        
        [Inject]
        private void Install(LoadingManager loadingManager, UIManager uiManager)
        {
            _loadingManager = loadingManager;
            _uiManager = uiManager;
        }
        
        public void RegisterController(LevelController levelController)
        {
            _levelController = levelController;
        }

        public void UnregisterController(LevelController levelController)
        {
            if (_levelController == levelController)
                _levelController = null;
        }

        public void IncreaseLevel()
        {
            _currentLevelLoop++;
            _currentLevelIndex++;

            if (_currentLevelIndex >= _datas.Count) _currentLevelIndex = _config.SkipForLoop;
            
            OnLevelChanged?.Invoke(_currentLevelIndex);
            OnLevelLoopChanged?.Invoke(_currentLevelLoop);
        }

        public string[] GetAvailableLevels()
        {
            return _datas.Keys.ToArray();
        }
        
        public void LoadCurrentLevel()
        {
            LoadLevel(CurrentLevelID);
        }

        public void LoadLevel(string levelID)
        {
            var levelData = GetData(levelID);
            if (levelData != null)
            {
                for (int i = 0; i < _config.Datas.Length; i++)
                {
                    if (_config.Datas[i] == levelData)
                    {
                        _currentLevelIndex = i;
                        break;
                    }
                }
                
                LoadLevel(levelData);
            }
        }

        private void LoadLevel(LevelData levelData)
        {
            if (levelData != null)
            {
                _loadingManager.Load(levelData.LoadingID);   
            }
        }

        public void WinLevel()
        {
            UICompletedLevel(true);
            IncreaseLevel();
            
        }

        public void LoseLevel()
        {
            UICompletedLevel(false);
        }

        private void UICompletedLevel(bool isWin)
        {
            var levelData = GetData(CurrentLevelID);

            if (levelData != null)
            {
                var uiScreen = _uiManager.ShowElement(isWin ? _config.UIWinScreen : _config.UILoseScreen);
                uiScreen.Initialize(levelData);
            }
        }
    }
}
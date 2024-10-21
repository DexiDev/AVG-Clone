using System;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Data;
using Game.Loadings.Scenes;
using Game.Loadings.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using Object = UnityEngine.Object;

namespace Game.Loadings
{
    public class LoadingManager : DataManager<LoadingData, LoadingConfig>
    {
        private AssetsManager _assetsManager;
        
        private bool _isLoading;
        private UILoadingScreen _uiLoadingScreen;
        private LoadingData _currentLoadingData;

        public bool IsLoading => _isLoading;
        public string CurrentLoadingID => _currentLoadingData?.ID;

        [Inject]
        private void Install(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        
        protected override void Initialized()
        {
            base.Initialized();
            
            _uiLoadingScreen = _assetsManager.GetAsset<UILoadingScreen>(_config.LoadingScreen, null);
            
            Object.DontDestroyOnLoad(_uiLoadingScreen);
            
            _uiLoadingScreen.SetActive(false);
        }

        public void LoadMainMenu()
        {
            Load(_config.MainMenuID);
        }

        public void LoadAutorun()
        {
            Load(_config.AutorunLoadingID);
        }
        

        public void Load(string loadingDataID)
        {
            var loadingData = GetData(loadingDataID);

            if (loadingData != null)
            {
                Load(loadingData);
            }
        }

        private async void Load(LoadingData loadingData)
        {
            if (!_isInitialize) await UniTask.WaitWhile(() => !_isInitialize);

            _isLoading = true;
            
            var startTime = DateTime.Now;
            
            _uiLoadingScreen.SetProgress(0f);
            
            _uiLoadingScreen.SetActive(true);
            
            if (loadingData != null && loadingData.LoadingSettings != null)
                _uiLoadingScreen.AddTask(loadingData.LoadingSettings.Length);

            if (loadingData.LoadingSettings is { Length: > 0 } &&
                loadingData.LoadingSettings[0].LoadSceneMode == LoadSceneMode.Additive)
            {
                if (_currentLoadingData != null && _currentLoadingData.LoadingSettings != null)
                {
                    _uiLoadingScreen.AddTask(_currentLoadingData.LoadingSettings.Length);

                    foreach (var loadingSettings in _currentLoadingData.LoadingSettings)
                    {
                        
                        if (loadingSettings.Scene is SceneField sceneField)
                        {
                            var sceneOperation = SceneManager.UnloadSceneAsync(sceneField.SceneName);

                            while (!sceneOperation.isDone)
                            {
                                _uiLoadingScreen.SetProgress(sceneOperation.progress);
                                await UniTask.Yield();
                            }
                        }

                        _uiLoadingScreen.CompletedTask();
                    }
                }
            }

            _currentLoadingData = loadingData;

            if (_currentLoadingData != null && _currentLoadingData.LoadingSettings != null)
            {
                foreach (var loadingSettings in _currentLoadingData.LoadingSettings)
                {
                    Scene scene = default;
                    if (loadingSettings.Scene is SceneField sceneField)
                    {
                        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(sceneField.SceneName, loadingSettings.LoadSceneMode);

                        while (!sceneOperation.isDone)
                        {
                            _uiLoadingScreen.SetProgress(sceneOperation.progress);
                            await UniTask.Yield();
                        }
                        
                        scene = SceneManager.GetSceneByName(sceneField.SceneName);
                    }
                    else {
                        _uiLoadingScreen.CompletedTask();
                        continue;
                    }
                   
                    _uiLoadingScreen.CompletedTask();

                    if (loadingSettings.IsActive)
                    {
                        SceneManager.SetActiveScene(scene);
                    }
                }
            }

            if (loadingData.MinDurationLoading > 0f)
            {
                var time = DateTime.Now - startTime;

                var targetTime = TimeSpan.FromSeconds(loadingData.MinDurationLoading);
                
                if (time < targetTime)
                {
                    var delay = targetTime - time;
                    await UniTask.Delay(delay);
                }
            }
            _uiLoadingScreen.SetActive(false);
            
            _isLoading = false;
        }
    }
}
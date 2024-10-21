using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Loadings
{
    [Serializable]
    public class LoadingSettings
    {
        [SerializeField] private IScene _scene;
        
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        [SerializeField] private bool _isActive;
        
        public IScene Scene => _scene;

        public LoadSceneMode LoadSceneMode => _loadSceneMode;

        public bool IsActive => _isActive;
    }
}
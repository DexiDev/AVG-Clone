using System;
using System.Collections.Generic;
using System.Linq;
using Game.Assets;
using Game.Data.Fields;
using Game.Gameplay;
using Game.Unit;
using Game.Unit.Damageable;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Game.Levels
{
    [DisallowMultipleComponent]
    public class LevelController : SerializedMonoBehaviour
    {
        [SerializeField, TabGroup("Base")] private Camera _camera;
        [SerializeField, TabGroup("Base")] private PlayerController _playerController;
        [SerializeField, TabGroup("Nodes")] private LevelNode _levelNodeContract;
        [SerializeField, TabGroup("Nodes")] private int _startCountNodes;
        [SerializeField, TabGroup("Nodes")] private Vector3 _offsetNodes;
        [SerializeField, TabGroup("Parameters")] private Transform _finalPoint;
        [SerializeField, TabGroup("Parameters")] private GameObject _startCamera;
        
        
        private bool _isRunning;
        private IsBattleStateField _isBattleStateField;
        
        private LevelManager _levelManager;
        private AssetsManager _assetsManager;
        
        private LevelData _currentLevelData;
        private List<LevelNode> _levelNodes = new List<LevelNode>();
        private Vector3 _lastNodePosition;
        
        private List<UnitController> _unitSpawned = new List<UnitController>();
        
        public LevelData CurrentLevelData => _currentLevelData;
        public bool IsRunning => _isRunning;
        
        public event Action OnStart;
        public event Action<bool> OnLevelCompleted;
        
        [Inject]
        private void Install(LevelManager levelManager, AssetsManager assetsManager)
        {
            _levelManager = levelManager;
            _assetsManager = assetsManager;
        }

        private void Awake()
        {
            _lastNodePosition = transform.position - _offsetNodes;
            _currentLevelData = _levelManager.GetData(_levelManager.CurrentLevelID);

            _isBattleStateField = _playerController.GetDataField<IsBattleStateField>(true);
        }

        private void OnEnable()
        {
            InitializeNodes();
            _levelManager.RegisterController(this);
            _playerController.OnKill += OnPlayerKill;
        }

        private void OnDisable()
        {
            ClearNodes();
            _levelManager.UnregisterController(this);
            _playerController.OnKill -= OnPlayerKill;
        }
        
        private void InitializeNodes()
        {
            ClearNodes();
            
            _levelNodes ??= new List<LevelNode>();
            
            for (int i = 0; i < _startCountNodes; i++) AddNode();
        }

        private LevelNode AddNode()
        {
            var position = _lastNodePosition + _offsetNodes;
            var levelNode = _assetsManager.GetAsset<LevelNode>(_levelNodeContract, position, Quaternion.identity, transform);
            
            levelNode.OnVisible += OnLevelNodeVisible;
            levelNode.OnSpawned += OnSpawnUnit;
            _lastNodePosition = position;
            
            _levelNodes ??= new List<LevelNode>();
            
            _levelNodes.Add(levelNode);
            
            return levelNode;
        }

        private void RemoveNode(LevelNode levelNode)
        {
            levelNode.OnSpawned -= OnSpawnUnit;
            levelNode.OnPoolable -= OnLevelNodePoolable;
            levelNode.OnVisible -= OnLevelNodeVisible;
            _levelNodes.Remove(levelNode);
        }

        private void ClearNodes()
        {
            if (_levelNodes == null) return;
            foreach (var levelNode in _levelNodes.ToArray()) RemoveNode(levelNode);
            _levelNodes.Clear();
        }

        private void OnLevelNodePoolable(IAssetInstance assetInstance)
        {
            if (assetInstance is LevelNode levelNode) RemoveNode(levelNode);
            else assetInstance.OnPoolable -= OnLevelNodePoolable;
        }

        private void OnLevelNodeVisible(LevelNode levelNode, bool isVisible)
        {
            if (_playerController == null || _playerController.IsDestroyed()) return;
            
            if (isVisible)
            {
                if(_isRunning) levelNode.SpawnAll();
                
                if (levelNode == _levelNodes.Last())
                {
                    AddNode();
                }
            }
            else if(levelNode.transform.position.z < _playerController.transform.position.z)
            {
                levelNode.gameObject.SetActive(false);
            }
        }
        

        private void OnPlayerKill(IDamageable damageable)
        {
            CompletedLevel(false);
        }

        [Button]
        public void StartLevel()
        {
            foreach (var levelNode in _levelNodes.Skip(1))
            {
                if(levelNode.IsVisible) levelNode.SpawnAll();
                else break;
            }
            
            _isBattleStateField.SetValue(true);
            _isRunning = true;
            _startCamera.gameObject.SetActive(false);
            OnStart?.Invoke();
        }

        [Button]
        public void CompletedLevel(bool isWin)
        {
            _isRunning = false;
            _unitSpawned?.ToArray().ForEach(unit => unit.gameObject.SetActive(false));
            
            _isBattleStateField.SetValue(false);
            
            if (isWin)
            {
                Debug.Log("Win Level");
                _levelManager.WinLevel();
            }
            else
            {
                Debug.Log("Lose Level");
                _levelManager.LoseLevel();
            }
            OnLevelCompleted?.Invoke(isWin);
        }
        
        private void OnSpawnUnit(UnitController unitController)
        {
            unitController.OnPoolable += OnUnitPoolable;
            _unitSpawned.Add(unitController);
        }

        private void OnUnitPoolable(IAssetInstance assetInstance)
        {
            assetInstance.OnPoolable -= OnUnitPoolable;

            if (assetInstance is UnitController unitController)
            {
                _unitSpawned.Remove(unitController);
            }
        }

        public float GetProgress()
        {
            var distance = _finalPoint.position.z - transform.position.z;

            var playerPosition = _playerController.transform.position.z - transform.position.z;

            return playerPosition / distance;
        }

        private void LateUpdate()
        {
            if (_isRunning)
            {
                var progress = GetProgress();

                if (progress >= 1f) CompletedLevel(true);
            }
        }
    }
}
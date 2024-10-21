using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data.Fields.Moving;
using UnityEngine;

namespace Game.Data.Handlers
{
    public abstract class FollowToTargetHandler<TDataController> : IHandler<TDataController> where TDataController : MonoBehaviour, IDataController
    {
        [SerializeField] protected Vector3 _offset;
        
        protected IMoveField _moveField;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        private void OnEnable()
        {
            SetField(_targetData.GetDataField<IMoveField>());
            _targetData.OnFieldsChanged += OnTargetFieldsChangedHandler;
        }

        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            SetField(null);
            _targetData.OnFieldsChanged -= OnTargetFieldsChangedHandler;
        }

        private void OnTargetFieldsChangedHandler(IDataField dataField, bool isAdded)
        {
            if (dataField is IMoveField moveField)
            {
                if (isAdded)
                {
                    SetField(moveField);
                }
                else if (_moveField == moveField)
                {
                    SetField(null);
                }
            }
        }

        private void SetField(IMoveField moveField)
        {
            if (_moveField != null)
            {
                _moveField.OnDataChanged -= OnFieldDataChanged;
                _cancellationTokenSource?.Cancel();
            }
            
            _moveField = moveField;

            if (_moveField != null)
            {
                _moveField.OnDataChanged += OnFieldDataChanged;
                UpdateLoop();
            }
        }

        private void OnFieldDataChanged(IDataField dataField)
        {
            UpdateLoop();
        }

        private async void UpdateLoop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            var token = _cancellationTokenSource.Token;
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    UpdatePosition();
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, token);
                }
            }
            catch (OperationCanceledException) {}
        }

        protected virtual void UpdatePosition()
        {
            _targetData.transform.position = GetPosition() + _offset;
        }

        protected virtual Vector3 GetPosition()
        {
            return _moveField.GetPosition();
        }
    }
}
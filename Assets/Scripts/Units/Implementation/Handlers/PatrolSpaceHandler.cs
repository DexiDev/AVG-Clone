using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using Game.Data.Fields.Moving;
using Game.Extensions.Collider;
using Game.Units.Implementation.Fields;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Unit.Handlers
{
    public class PatrolSpaceHandler : IHandler<UnitController>
    {
        [SerializeField] private Vector2 _minMaxDelay;

        private PatrolSpaceField _patrolSpaceField;
        private CancellationTokenSource _cancellationToken;
        
        private void OnEnable()
        {
            SetPatrolSpaceField(_targetData.GetDataField<PatrolSpaceField>());
            _targetData.OnFieldsChanged += OnFieldsChangedHandler;
        }

        private void OnDisable()
        {
            _cancellationToken?.Cancel();
            SetPatrolSpaceField(null);
            _targetData.OnFieldsChanged -= OnFieldsChangedHandler;
        }
        
        private void OnFieldsChangedHandler(IDataField dataField, bool isAdded)
        {
            if (dataField is PatrolSpaceField)
            {
                SetPatrolSpaceField(_targetData.GetDataField<PatrolSpaceField>());
            }
            else if (!isAdded && dataField is IMoveField)
            { 
                Delay();
            }
        }

        private void SetPatrolSpaceField(PatrolSpaceField patrolSpaceField)
        {
            _patrolSpaceField = patrolSpaceField;

            AddRandomPoint();
        }
        
        private async void Delay()
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new();

            var token = _cancellationToken.Token;
            try
            {
                var delay = Random.Range(_minMaxDelay.x, _minMaxDelay.y);

                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token );

                if (!_cancellationToken.IsCancellationRequested)
                {
                    AddRandomPoint();
                }
            }
            catch (OperationCanceledException)
            {
                
            }
            
        }
        
        private void AddRandomPoint()
        {
            if (_patrolSpaceField == null || _patrolSpaceField.Value == null) return;
            
            var point = _patrolSpaceField.Value.bounds.GetRandomPoint();
            point.y = _patrolSpaceField.Value.transform.position.y;

            var moveField = _targetData.GetDataField<IMoveField>();

            if (moveField != null)
            {
                _targetData.RemoveDataField(moveField);
            }

            moveField = new MoveToPointField(point);
            
            _targetData.AddDataField(moveField);
        }
    }
}
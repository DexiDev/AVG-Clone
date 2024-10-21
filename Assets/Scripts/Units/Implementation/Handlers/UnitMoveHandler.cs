using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using Game.Data.Fields.Moving;
using UnityEngine;

namespace Game.Unit.Handlers
{
    public class UnitMoveHandler : IHandler<UnitController>
    {
        private IMoveField _moveField;

        private CancellationTokenSource _cancellationToken;

        private void OnEnable()
        {
            var moveField = _targetData.GetDataField<IMoveField>();
            
            SetMovePointField(moveField);
            
            _targetData.OnFieldsChanged += OnFieldChangedHandler;
        }
        
        private void OnDisable()
        {
            SetMovePointField(null);
            _targetData.OnFieldsChanged -= OnFieldChangedHandler;
        }

        private void OnFieldChangedHandler(IDataField dataField, bool isAdd)
        {
            if (dataField is IMoveField movePointData)
            {
                SetMovePointField(isAdd ? movePointData : null);
            }
        }

        private void SetMovePointField(IMoveField moveField)
        {
            _moveField = moveField;

            if (_moveField != null) MoveLoop(_moveField);
            else _cancellationToken?.Cancel();
        }

        private async void MoveLoop(IMoveField moveField)
        {
            _cancellationToken?.Cancel();

            if (moveField == null) return;
            
            _cancellationToken = new CancellationTokenSource();
            
            var token = _cancellationToken.Token;
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var targetPosition = moveField.GetPosition();

                    if (moveField.StoppingDistance <= 0)
                    {
                        if (targetPosition == _targetData.transform.position) break;
                    }
                    else if (Vector3.Distance(_targetData.transform.position, targetPosition) <= moveField.StoppingDistance)
                    {
                        break;
                    }

                    // _targetData.Move(_targetData.transform.forward * Time.deltaTime);

                    _targetData.MoveTowards(targetPosition, Time.deltaTime);
                    
                    var targetDirection = targetPosition - _targetData.transform.position;

                    targetDirection.y = _targetData.transform.position.y;

                    targetDirection.Normalize();
            
                    _targetData.RotateTowards(targetDirection, Time.deltaTime);
            
                    _targetData.SetMotion(true, 1f);

                    await UniTask.Yield(cancellationToken: token);
                }
                _targetData.SetMotion(false);
                
                if(moveField.IsAutoRemove) _targetData.RemoveDataField(_moveField);
            }
            
            catch (OperationCanceledException)
            {
                
            }
        }
    }
}
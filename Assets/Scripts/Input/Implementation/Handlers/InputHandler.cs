using Game.Core;
using Game.Data.Fields;
using UnityEngine;
using VContainer;

namespace Game.Input.Handlers
{
    public class InputHandler : IHandler<IInputReceiver>
    {
        [SerializeField] private float _stopAcceleration = 5f;
        [SerializeField] private float _startAcceleration = 5f;
        [SerializeField] private bool _enableXDirection;
        [SerializeField] private bool _enableYDirection;
        [SerializeField] private bool _flipXYDirection;

        private DirectionField _directionField;
        private Vector3 _targetDirection;
        private Vector3 _currentDirection = Vector3.zero;

        private InputManager _inputManager;

        [Inject]
        private void Install(InputManager inputManager)
        {
            _inputManager = inputManager;
        }

        private void Awake()
        {
            _directionField = _targetData.GetDataField<DirectionField>(true);
        }

        public void Update()
        {
            _targetDirection = Vector3.zero;
            
            if (_inputManager.Direction != Vector2.zero)
            {
                _targetDirection = new Vector3(_enableXDirection ? _inputManager.Direction.x : 0f, _enableYDirection ? _inputManager.Direction.y : 0f, 0f);

                if (_flipXYDirection)
                {
                    _targetDirection = new Vector3(_targetDirection.y, _targetDirection.x, 0f);
                }
            }

            _currentDirection = _startAcceleration < 0 ? _targetDirection : Vector3.Lerp(_currentDirection, _targetDirection, _startAcceleration * Time.deltaTime);

            bool isMoving = _currentDirection != Vector3.zero;

            if (isMoving)
            {
                _targetData.ReceiveInput(_currentDirection * Time.deltaTime);

                if (_targetDirection == Vector3.zero)
                {
                    _currentDirection = _stopAcceleration < 0 ? Vector3.zero : Vector3.Lerp(_currentDirection, Vector3.zero, Time.deltaTime * _stopAcceleration);
                }
            }

            _directionField?.SetValue(_currentDirection);
        }
    }
}

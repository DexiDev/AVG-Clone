using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Game.Input
{
    public class Joystick : FloatingJoystick
    {
        private bool _isActive;
        private InputManager _inputManager;
        
        public bool IsActive => _isActive;

        public event Action<bool> OnActiveChanged; 
        
        [Inject]
        private void Install(InputManager inputManager)
        {
            _inputManager = inputManager;
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            _isActive = true;

            _inputManager.SetDirection(Vector2.zero);
            
            OnActiveChanged?.Invoke(true);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            _isActive = false;
            
            _inputManager.SetDirection(Vector2.zero);
            
            OnActiveChanged?.Invoke(false);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            
            _inputManager.SetDirection(Direction);
        }
    }
}
using System;
using UnityEngine;

namespace Game.Input
{
    public class InputManager
    {
        private Vector2 _direction = Vector2.zero;

        public Vector2 Direction => _direction;

        public event Action<Vector2> OnDirectionChanged;
        
        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
            OnDirectionChanged?.Invoke(direction);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputListener : MonoBehaviour, IInput
    {
        public Vector2 mousePosition { get; private set; }
        public event Action onLeftClick;
        public event Action onRightClick;
        
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _playerInput.actions["Left Click"].performed += LeftClick;
            _playerInput.actions["Right Click"].performed += RightClick;
        }

        private void Update()
        {
            mousePosition = _playerInput.actions["Mouse Position"].ReadValue<Vector2>();
        }

        private void LeftClick(InputAction.CallbackContext ctx)
        {
            onLeftClick?.Invoke();
        }
        
        private void RightClick(InputAction.CallbackContext ctx)
        {
            onRightClick?.Invoke();
        }
    }
}

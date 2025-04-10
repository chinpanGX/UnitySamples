#nullable enable
using System;
using AppService.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystemWrapper
{
    public class PointerHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput? playerInput;
        [SerializeField] private bool enableDebugLog = false;
        private Vector3 cursorPosition;
        private Action<Vector3>? onClickCallback;

        public void RegisterOnClickPerformed(Action<Vector3> callBack)
        {
            onClickCallback = callBack;
        }
        
        public void UnregisterOnClickPerformed()
        {
            onClickCallback = null;
        }

        private void OnPointerPerformed(InputAction.CallbackContext context)
        {
            cursorPosition = context.ReadValue<Vector2>();
            OutputDebugLog($"{nameof(PointerHandler)}: Pointer Position {cursorPosition}");
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            onClickCallback?.Invoke(cursorPosition);
            OutputDebugLog($"{nameof(PointerHandler)}: Clicked at {cursorPosition}");
        }

        private void OutputDebugLog(string message)
        {
            if (enableDebugLog)
            {
                Debug.Log(message);
            }
        }

        private void Awake()
        {
            if (playerInput == null)
                throw new ArgumentNullException(nameof(playerInput), "PlayerInput is not assigned.");
            
            playerInput.actions["Point"].performed += OnPointerPerformed;
            playerInput.actions["Click"].performed += OnClickPerformed;
        }

        private void OnDestroy()
        {
            if (playerInput == null)
                throw new ArgumentNullException(nameof(playerInput), "PlayerInput is not assigned.");
            playerInput.actions["Point"].performed -= OnPointerPerformed;
            playerInput.actions["Click"].performed -= OnClickPerformed;
        }
    }
}
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        public bool fire;
        public bool reload;
        public bool escapeKey;
        private bool _menuOnScreen;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private bool _isPlayerAlive = true;


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (_isPlayerAlive)
                MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                if (_isPlayerAlive)

                    LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            if (_isPlayerAlive)
                JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnFire(InputValue value)
        {
            FireInput(value.isPressed);
        }

        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }

        public void OnEscapeKey(InputValue value)
        {
            EscapeKeyInput(value.isPressed);
        }
#endif

        public void EscapeKeyInput(bool newEscapeKey)
        {
            escapeKey = newEscapeKey;
        }

        public void FireInput(bool newShootState)
        {
            fire = newShootState;
        }

        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // Game is not in focus, unlock cursor
                SetCursorState(false);
            }
        }

        public void SetCursorVisible(bool value)
        {
            _menuOnScreen = value;
            // Update cursor state when the menu state changes
            SetCursorState(!_menuOnScreen);
        }

        public bool IsMenuOnScreen()
        {
            return _menuOnScreen; //cannot shoot if menu is on screen
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !newState; // Show cursor if unlocked, hide cursor if locked
        }

        public void IsPlayerAlive(bool isPlayerAlive)
        {
            _isPlayerAlive = isPlayerAlive;
        }
    }
}
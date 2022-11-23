using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
        private InputManager inputManager;
		private PlayerKeyCodes playerKeyCodes;

        [Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
        public bool pause;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		public bool topDownViewCamera = false;

		private void Start()
		{
			inputManager = InputManager.Instance;			
		}

		private void Update()
		{

        }

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		//public void OnMove(InputValue value)
		//{
		//	MoveInput(value.Get<Vector2>());
		//}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		//public void OnJump(InputValue value)
		//{
		//	JumpInput(value.isPressed);
		//}

		//public void OnSprint(InputValue value)
		//{
		//	SprintInput(value.isPressed);
		//}
#endif


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

		public void PauseInput(bool newPauseState)
		{
			pause = newPauseState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (pause)
			{
                SetCursorState(!cursorLocked);
            }
			else
			{
                SetCursorState(cursorLocked);
            }			
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}	
	}
	
}
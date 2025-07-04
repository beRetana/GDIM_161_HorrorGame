using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.InputSystem;

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private void Start()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadingmode)
        {
			if (scene.name == NewNetworkManager.NewSingleton.GameplaySceneName)
			{
				SetCursorLocked(true);
            }
			else
			{
                SetCursorLocked(false);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			ToggleSprint();
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

		public void ToggleSprint()
		{
			sprint = !sprint;
		}

		private void SetCursorLocked(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}
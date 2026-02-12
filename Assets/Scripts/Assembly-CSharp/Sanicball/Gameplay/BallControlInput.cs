using Sanicball.UI;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[RequireComponent(typeof(Ball))]
	public class BallControlInput : MonoBehaviour
	{
		private Ball ball;

		private Vector3 rawDirection;

		private bool hasJumped;

		public Quaternion LookDirection { get; set; }

		private void Start()
		{
			LookDirection = Quaternion.Euler(Vector3.forward);
			ball = GetComponent<Ball>();
		}

		private void Update()
		{
			if (PauseMenu.GamePaused)
			{
				ball.DirectionVector = Vector3.zero;
				return;
			}
			Vector3 target = GameInput.MovementVector(ball.CtrlType);
			rawDirection = Vector3.MoveTowards(rawDirection, target, 0.5f);
			Vector3 vector = rawDirection;
			if (vector != Vector3.zero)
			{
				float magnitude = vector.magnitude;
				vector /= magnitude;
				magnitude = Mathf.Min(1f, magnitude);
				magnitude *= magnitude;
				vector *= magnitude;
			}
			vector = LookDirection * Quaternion.Euler(0f, 90f, 0f) * vector;
			ball.DirectionVector = vector;
			ball.Brake = GameInput.IsBraking(ball.CtrlType);
			if (GameInput.IsJumping(ball.CtrlType))
			{
				if (!hasJumped)
				{
					ball.Jump();
					hasJumped = true;
				}
			}
			else if (hasJumped)
			{
				hasJumped = false;
			}
			if (GameInput.IsRespawning(ball.CtrlType) && ball.CanMove)
			{
				ball.RequestRespawn();
			}
		}
	}
}

using System;

namespace Sanicball.Logic
{
	public class PlayerMovementArgs : EventArgs
	{
		public double Timestamp { get; private set; }

		public PlayerMovement Movement { get; private set; }

		public PlayerMovementArgs(double timestamp, PlayerMovement movement)
		{
			Timestamp = timestamp;
			Movement = movement;
		}
	}
}

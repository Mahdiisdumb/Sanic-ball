using System;

namespace Sanicball.Gameplay
{
	public class CheckpointPassArgs : EventArgs
	{
		public Checkpoint CheckpointPassed { get; private set; }

		public CheckpointPassArgs(Checkpoint c)
		{
			CheckpointPassed = c;
		}
	}
}

using System;

namespace Sanicball.Logic
{
	public class NextCheckpointPassArgs : EventArgs
	{
		public int IndexOfPreviousCheckpoint { get; private set; }

		public TimeSpan CurrentLapTime { get; private set; }

		public NextCheckpointPassArgs(int indexOfPreviousCheckpoint, TimeSpan currentLapTime)
		{
			IndexOfPreviousCheckpoint = indexOfPreviousCheckpoint;
			CurrentLapTime = currentLapTime;
		}
	}
}

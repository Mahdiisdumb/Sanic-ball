using System;

namespace Sanicball.Gameplay
{
	public class CameraCreationArgs : EventArgs
	{
		public IBallCamera CameraCreated { get; private set; }

		public CameraCreationArgs(IBallCamera cameraCreated)
		{
			CameraCreated = cameraCreated;
		}
	}
}

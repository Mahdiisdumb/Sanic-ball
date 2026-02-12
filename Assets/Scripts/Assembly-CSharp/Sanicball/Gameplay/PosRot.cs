using UnityEngine;

namespace Sanicball.Gameplay
{
	internal struct PosRot
	{
		public Vector3 Position { get; set; }

		public Quaternion Rotation { get; set; }

		public PosRot(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}
}

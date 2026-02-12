using System;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[Serializable]
	public class CheckpointData
	{
		public Renderer checkpointMinimap;

		public Material matShown;

		public Material matHidden;

		public Texture2D texMinimapShown;

		public Texture2D texMinimapHidden;

		public LayerMask ballSpawningMask;
	}
}

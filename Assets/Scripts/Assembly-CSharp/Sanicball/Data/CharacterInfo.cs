using System;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Data
{
	[Serializable]
	public class CharacterInfo
	{
		public string name;

		public string artBy;

		public BallStats stats;

		public Material material;

		public Sprite icon;

		public Color color = Color.white;

		public Material minimapIcon;

		public Material trail;

		public float ballSize = 1f;

		public Mesh alternativeMesh;

		public Mesh collisionMesh;

		public CharacterTier tier;

		public bool hidden;
	}
}

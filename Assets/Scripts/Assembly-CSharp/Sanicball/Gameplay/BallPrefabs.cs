using System;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[Serializable]
	public class BallPrefabs
	{
		[SerializeField]
		private DriftySmoke smoke;

		[SerializeField]
		private OmniCamera camera;

		[SerializeField]
		private PivotCamera oldCamera;

		[SerializeField]
		private ParticleSystem removalParticles;

		[SerializeField]
		private SpeedFire speedFire;

		public DriftySmoke Smoke
		{
			get
			{
				return smoke;
			}
		}

		public OmniCamera Camera
		{
			get
			{
				return camera;
			}
		}

		public PivotCamera OldCamera
		{
			get
			{
				return oldCamera;
			}
		}

		public ParticleSystem RemovalParticles
		{
			get
			{
				return removalParticles;
			}
		}

		public SpeedFire SpeedFire
		{
			get
			{
				return speedFire;
			}
		}
	}
}

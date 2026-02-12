using System;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[Serializable]
	public class BallMotionSounds
	{
		[SerializeField]
		private AudioSource jump;

		[SerializeField]
		private AudioSource roll;

		[SerializeField]
		private AudioSource speedNoise;

		[SerializeField]
		private AudioSource brake;

		public AudioSource Jump
		{
			get
			{
				return jump;
			}
		}

		public AudioSource Roll
		{
			get
			{
				return roll;
			}
		}

		public AudioSource SpeedNoise
		{
			get
			{
				return speedNoise;
			}
		}

		public AudioSource Brake
		{
			get
			{
				return brake;
			}
		}
	}
}

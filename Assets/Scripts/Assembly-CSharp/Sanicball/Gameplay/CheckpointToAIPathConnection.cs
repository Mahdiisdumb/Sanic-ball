using System;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[Serializable]
	public class CheckpointToAIPathConnection
	{
		[SerializeField]
		private string name;

		[SerializeField]
		private AINode firstNode;

		[SerializeField]
		private float selectionWeight = 1f;

		[SerializeField]
		private bool usedByBig = true;

		public AINode FirstNode
		{
			get
			{
				return firstNode;
			}
		}

		public float SelectionWeight
		{
			get
			{
				return selectionWeight;
			}
		}

		public bool UsedByBig
		{
			get
			{
				return usedByBig;
			}
		}
	}
}

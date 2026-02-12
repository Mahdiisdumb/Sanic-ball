using System;
using UnityEngine;

namespace Sanicball
{
	[Serializable]
	public class AINodeSplitterTarget
	{
		[SerializeField]
		private AINode node;

		[SerializeField]
		private int weight = 1;

		public AINode Node
		{
			get
			{
				return node;
			}
		}

		public int Weight
		{
			get
			{
				return weight;
			}
		}
	}
}

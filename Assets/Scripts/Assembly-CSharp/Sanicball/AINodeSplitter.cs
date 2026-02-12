using System.Collections.Generic;
using UnityEngine;

namespace Sanicball
{
	public class AINodeSplitter : AINode
	{
		[SerializeField]
		private AINodeSplitterTarget[] targets;

		public override AINode NextNode
		{
			get
			{
				List<int> list = new List<int>();
				for (int i = 0; i < targets.Length; i++)
				{
					for (int j = 0; j < targets[i].Weight; j++)
					{
						list.Add(i);
					}
				}
				int index = Random.Range(0, list.Count);
				return targets[list[index]].Node;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, 3f);
			AINodeSplitterTarget[] array = targets;
			foreach (AINodeSplitterTarget aINodeSplitterTarget in array)
			{
				if (aINodeSplitterTarget.Node != null)
				{
					Gizmos.DrawLine(base.transform.position, aINodeSplitterTarget.Node.transform.position);
				}
			}
		}
	}
}

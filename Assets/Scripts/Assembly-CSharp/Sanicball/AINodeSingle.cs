using UnityEngine;

namespace Sanicball
{
	public class AINodeSingle : AINode
	{
		[SerializeField]
		private AINode nextNode;

		public override AINode NextNode
		{
			get
			{
				return nextNode;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 3f);
			if ((bool)nextNode)
			{
				Gizmos.DrawLine(base.transform.position, nextNode.transform.position);
			}
		}
	}
}

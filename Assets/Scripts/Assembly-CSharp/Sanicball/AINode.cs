using UnityEngine;

namespace Sanicball
{
	public abstract class AINode : MonoBehaviour
	{
		public abstract AINode NextNode { get; }
	}
}

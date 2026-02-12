using UnityEngine;

namespace Sanicball.Gameplay
{
	public class Checkpoint : MonoBehaviour
	{
		[SerializeField]
		private CheckpointData data;

		[SerializeField]
		private AINode firstAINode;

		public AINode FirstAINode
		{
			get
			{
				return firstAINode;
			}
		}

		public void Show()
		{
			GetComponent<Renderer>().material = data.matShown;
			data.checkpointMinimap.material.mainTexture = data.texMinimapShown;
		}

		public void Hide()
		{
			GetComponent<Renderer>().material = data.matHidden;
			data.checkpointMinimap.material.mainTexture = data.texMinimapHidden;
		}

		public Vector3 GetRespawnPoint()
		{
			Vector3 result = base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position + Vector3.up * 100f, Vector3.down, out hitInfo, 200f, data.ballSpawningMask))
			{
				result = hitInfo.point;
			}
			return result;
		}

		private void Start()
		{
			Hide();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = new Color(0.3f, 0.8f, 1f);
			if (firstAINode != null)
			{
				Gizmos.DrawLine(base.transform.position, firstAINode.transform.position);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, GetRespawnPoint());
			Gizmos.DrawSphere(GetRespawnPoint(), 3f);
		}
	}
}

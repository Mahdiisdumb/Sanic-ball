using UnityEngine;

namespace Sanicball
{
	public class WaterAnimation : MonoBehaviour
	{
		public Vector2 speed;

		private Vector2 offset;

		private void Update()
		{
			offset += new Vector2(speed.x * Time.deltaTime, speed.y * Time.deltaTime);
			if (offset.x >= 1f)
			{
				offset += new Vector2(-1f, 0f);
			}
			if (offset.y >= 1f)
			{
				offset += new Vector2(0f, -1f);
			}
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		}
	}
}

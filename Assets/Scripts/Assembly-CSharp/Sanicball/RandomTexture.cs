using UnityEngine;

namespace Sanicball
{
	public class RandomTexture : MonoBehaviour
	{
		public Texture[] textures;

		private int current;

		public int GetCurrentTexture()
		{
			return current;
		}

		public void SetTexture(int i)
		{
			GetComponent<Renderer>().material.mainTexture = textures[i];
			current = i;
		}

		private void Start()
		{
			SwitchTexture();
		}

		private void SwitchTexture()
		{
			int num = Random.Range(0, textures.Length);
			GetComponent<Renderer>().material.mainTexture = textures[num];
			current = num;
		}
	}
}

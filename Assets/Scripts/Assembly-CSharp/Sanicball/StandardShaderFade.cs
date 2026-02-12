using UnityEngine;

namespace Sanicball
{
	public class StandardShaderFade : MonoBehaviour
	{
		private float targetTime;

		private float curTime;

		private bool fadeOut;

		public void FadeIn(float time)
		{
			targetTime = time;
			curTime = 0f;
			fadeOut = false;
		}

		public void FadeOut(float time)
		{
			targetTime = time;
			curTime = 0f;
			fadeOut = true;
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (curTime < targetTime)
			{
				curTime = Mathf.Min(curTime + Time.deltaTime, targetTime);
				GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, (!fadeOut) ? (curTime / targetTime) : (1f - curTime / targetTime)));
			}
		}
	}
}

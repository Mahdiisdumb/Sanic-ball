using UnityEngine;

namespace Sanicball
{
	public class MenuCamera : MonoBehaviour
	{
		public StandardShaderFade fade;

		public MenuCameraPath[] paths;

		public Transform lookTarget;

		public float moveSpeed = 0.2f;

		public float fadeTime = 0.4f;

		private int currentPath;

		private float changePathTimer;

		private void Start()
		{
			CameraFade.StartAlphaFade(Color.black, true, 5f);
			base.transform.position = paths[currentPath].startPoint.position;
		}

		private void Update()
		{
			float num = Vector3.Distance(base.transform.position, paths[currentPath].endPoint.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, paths[currentPath].endPoint.position, moveSpeed * Time.deltaTime);
			float num2 = num / moveSpeed;
			if (num2 < fadeTime && changePathTimer <= 0f)
			{
				changePathTimer = fadeTime;
				fade.FadeIn(fadeTime);
			}
			if (changePathTimer > 0f)
			{
				changePathTimer -= Time.deltaTime;
				if (changePathTimer <= 0f)
				{
					ChangePath();
					fade.FadeOut(fadeTime);
				}
			}
			base.transform.LookAt(lookTarget.position);
		}

		private void ChangePath()
		{
			currentPath++;
			if (currentPath >= paths.Length)
			{
				currentPath = 0;
			}
			base.transform.position = paths[currentPath].startPoint.position;
			lookTarget.GetComponent<CycleMaterial>().Switch();
		}
	}
}

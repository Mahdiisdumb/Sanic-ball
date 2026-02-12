using Sanicball.UI;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Sanicball
{
	[RequireComponent(typeof(Camera))]
	public class BlurCameraOnPause : MonoBehaviour
	{
		private const string pauseTag = "Pause";

		private const float targetBlurSize = 5f;

		[SerializeField]
		private Shader shader;

		private bool paused;

		private BlurOptimized blur;

		private void Start()
		{
		}

		private void Update()
		{
			if (!paused)
			{
				if (PauseMenu.GamePaused)
				{
					paused = true;
					blur = base.gameObject.AddComponent<BlurOptimized>();
					blur.downsample = 1;
					blur.blurSize = 0f;
					blur.blurIterations = 3;
					blur.blurType = BlurOptimized.BlurType.StandardGauss;
					blur.blurShader = shader;
				}
			}
			else
			{
				if (blur.blurSize < 5f)
				{
					blur.blurSize = Mathf.Min(blur.blurSize + Time.unscaledDeltaTime * 40f, 5f);
				}
				if (!PauseMenu.GamePaused)
				{
					paused = false;
					Object.Destroy(blur);
				}
			}
		}
	}
}

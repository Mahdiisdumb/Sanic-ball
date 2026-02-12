using Sanicball.Data;
using Sanicball.Logic;
using UnityEngine;

namespace Sanicball
{
	public class WaitingCamera : MonoBehaviour
	{
		private const float switchTime = 8f;

		private const float moveSpeed = 10f;

		private CameraOrientation[] orientations;

		private int currentOrientation;

		private float timer = 8f;

		private float vol;

		private void Start()
		{
			orientations = StageReferences.Active.waitingCameraOrientations;
			AlignWithCurrentOrientation();
			CameraFade.StartAlphaFade(Color.black, true, 4f);
			AudioListener.volume = vol;
		}

		private void Update()
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				currentOrientation++;
				if (currentOrientation >= orientations.Length)
				{
					currentOrientation = 0;
				}
				AlignWithCurrentOrientation();
				timer = 8f;
			}
			base.transform.Translate(Vector3.forward * 10f * Time.deltaTime);
			if (vol < 1f)
			{
				vol = Mathf.Min(1f, vol + Time.deltaTime / 4f);
				AudioListener.volume = Mathf.Lerp(0f, ActiveData.GameSettings.soundVolume, vol);
			}
		}

		public void OnDestroy()
		{
			AudioListener.volume = ActiveData.GameSettings.soundVolume;
		}

		private void AlignWithCurrentOrientation()
		{
			base.transform.position = orientations[currentOrientation].transform.position;
			base.transform.rotation = orientations[currentOrientation].CameraRotation;
		}
	}
}

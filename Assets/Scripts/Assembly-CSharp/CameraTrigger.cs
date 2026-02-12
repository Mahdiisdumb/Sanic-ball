using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
	private GameObject cameraObject;

	public float showTime = 1f;

	private float timeToDisable;

	private bool shouldDisableCamera;

	private void Start()
	{
		cameraObject = base.transform.Find("Camera").gameObject;
		cameraObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject || !(other.gameObject.GetComponent<ArcadeCar>() == null))
		{
			cameraObject.SetActive(true);
			timeToDisable = 0f;
			shouldDisableCamera = false;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.gameObject || !(other.gameObject.GetComponent<ArcadeCar>() == null))
		{
			timeToDisable = showTime;
			shouldDisableCamera = true;
		}
	}

	private void Update()
	{
		if (timeToDisable > 0f)
		{
			timeToDisable -= Time.deltaTime;
			return;
		}
		timeToDisable = 0f;
		if (shouldDisableCamera)
		{
			cameraObject.SetActive(false);
			shouldDisableCamera = false;
		}
	}
}

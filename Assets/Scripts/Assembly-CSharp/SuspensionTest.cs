using UnityEngine;

public class SuspensionTest : MonoBehaviour
{
	public float amplitude = 0.3f;

	public float speed = 1f;

	private Vector3 originalPos;

	private float time;

	private GameObject dynamic;

	private GameObject cameraObject;

	private void Start()
	{
		dynamic = base.transform.Find("Dynamic").gameObject;
		cameraObject = base.transform.Find("SpecCamera").gameObject;
		cameraObject.SetActive(false);
		originalPos = dynamic.transform.position;
		time = 0f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject || !(other.gameObject.GetComponent<ArcadeCar>() == null))
		{
			cameraObject.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.gameObject || !(other.gameObject.GetComponent<ArcadeCar>() == null))
		{
			cameraObject.SetActive(false);
		}
	}

	private void Update()
	{
		float y = originalPos.y + (Mathf.Cos(time) - 1f) * (amplitude * 0.5f);
		dynamic.transform.position = new Vector3(originalPos.x, y, originalPos.z);
		time += Time.deltaTime * speed;
	}
}

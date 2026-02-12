using UnityEngine;

public class RotateWithMouse : MonoBehaviour
{
	public Vector3 baseRotation;

	public float xIntensity = 1f;

	public float yIntensity = 1f;

	public float speed = 1f;

	private Quaternion targetRotation = Quaternion.identity;

	private void Start()
	{
		targetRotation = Quaternion.Euler(baseRotation);
	}

	private void Update()
	{
		float num = Mathf.Lerp(-1f, 1f, Input.mousePosition.x / (float)Screen.width);
		float num2 = Mathf.Lerp(-1f, 1f, Input.mousePosition.y / (float)Screen.height);
		targetRotation = Quaternion.Euler(baseRotation) * Quaternion.Euler((0f - num2) * yIntensity, 0f, num * xIntensity);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, targetRotation, Time.deltaTime * speed);
	}
}

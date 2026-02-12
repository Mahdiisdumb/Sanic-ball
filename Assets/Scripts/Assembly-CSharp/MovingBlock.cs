using UnityEngine;

public class MovingBlock : MonoBehaviour
{
	public float amplitude = 0.3f;

	public float speed = 0.5f;

	public float initialTime;

	private Vector3 originalPos;

	private float time;

	private void Start()
	{
		originalPos = base.transform.position;
		time = initialTime;
	}

	private void Update()
	{
		float y = originalPos.y + (Mathf.Cos(time) - 1f) * (amplitude * 0.5f);
		base.transform.position = new Vector3(originalPos.x, y, originalPos.z);
		time += Time.deltaTime * speed;
	}
}

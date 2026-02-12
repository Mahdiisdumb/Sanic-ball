using UnityEngine;

public class ScreenshotTool : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Screenshot captured.");
			Application.CaptureScreenshot("screenshot.png", 2);
		}
	}
}

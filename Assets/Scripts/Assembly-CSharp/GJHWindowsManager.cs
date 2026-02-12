using System.Collections.Generic;
using UnityEngine;

public class GJHWindowsManager : MonoBehaviour
{
	private static GJHWindowsManager instance;

	private List<GJHWindow> windows;

	private int currentWindow = -1;

	public static GJHWindowsManager Instance
	{
		get
		{
			if (instance == null)
			{
				GJAPIHelper gJAPIHelper = (GJAPIHelper)Object.FindObjectOfType(typeof(GJAPIHelper));
				if (gJAPIHelper == null)
				{
					Debug.LogError("An instance of GJAPIHelper is needed in the scene, but there is none. Can't initialise GJHWindowManager.");
				}
				else
				{
					instance = gJAPIHelper.gameObject.AddComponent<GJHWindowsManager>();
					if (instance == null)
					{
						Debug.Log("An error occured creating GJHWindowManager.");
					}
				}
			}
			return instance;
		}
	}

	private void OnDestroy()
	{
		windows = null;
		instance = null;
	}

	private void Awake()
	{
		windows = new List<GJHWindow>();
	}

	public static int RegisterWindow(GJHWindow window)
	{
		Instance.windows.Add(window);
		return Instance.windows.Count - 1;
	}

	public static bool ShowWindow(int index)
	{
		if (index < 0)
		{
			Debug.Log("GJAPIHelper: The index of the window can't be negative. Can't show the window " + index);
			return false;
		}
		if (index >= Instance.windows.Count)
		{
			Debug.Log("GJAPIHelper: The index of the window is out of range. Can't show the window " + index);
			return false;
		}
		if (Instance.currentWindow != -1)
		{
			if (Instance.currentWindow == index)
			{
				Debug.Log("GJAPIHelper: The window \"" + Instance.windows[index].Title + "\" is already showing.");
				return false;
			}
			Debug.Log("GJAPIHelper: " + Instance.windows[Instance.currentWindow].Title + " window is already showing. Can't show \"" + Instance.windows[index].Title + "\" window.");
			return false;
		}
		Instance.currentWindow = index;
		return true;
	}

	public static bool DismissWindow(int index)
	{
		if (index < 0)
		{
			Debug.Log("GJAPIHelper: The index of the window can't be negative. Can't dismiss the window " + index);
			return false;
		}
		if (index >= Instance.windows.Count)
		{
			Debug.Log("GJAPIHelper: The index of the window is out of range. Can't dismiss the window " + index);
			return false;
		}
		if (Instance.currentWindow != index)
		{
			Debug.Log("GJAPIHelper: The window \"" + Instance.windows[index].Title + "\" isn't already showing. Can't dismiss it.");
			return false;
		}
		Instance.currentWindow = -1;
		return true;
	}

	public static bool IsWindowShowing(int index)
	{
		return Instance.currentWindow == index;
	}

	private void OnGUI()
	{
		if (currentWindow != -1)
		{
			windows[currentWindow].OnGUI();
		}
	}
}

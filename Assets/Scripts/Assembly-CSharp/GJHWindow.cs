using System.Collections.Generic;
using UnityEngine;

public class GJHWindow
{
	protected enum BaseWindowStates
	{
		Empty = 0,
		Process = 1,
		Success = 2,
		Error = 3
	}

	protected delegate void DrawWindowDelegate();

	public string Title = string.Empty;

	public Rect Position;

	private int windowID;

	private string previousWindowState = string.Empty;

	private string currentWindowState = BaseWindowStates.Empty.ToString();

	protected Dictionary<string, DrawWindowDelegate> drawWindowDelegates;

	private string windowMsg = string.Empty;

	private string windowReturnState = string.Empty;

	private GUIStyle errorStyle;

	private GUIStyle successStyle;

	private GUIStyle ellipsisStyle;

	public GJHWindow()
	{
		Title = "Base Window";
		Position = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300f, 100f);
		windowID = GJHWindowsManager.RegisterWindow(this);
		drawWindowDelegates = new Dictionary<string, DrawWindowDelegate>();
		drawWindowDelegates.Add(BaseWindowStates.Empty.ToString(), DrawWindowEmpty);
		drawWindowDelegates.Add(BaseWindowStates.Process.ToString(), DrawWindowProcessing);
		drawWindowDelegates.Add(BaseWindowStates.Success.ToString(), DrawWindowSuccess);
		drawWindowDelegates.Add(BaseWindowStates.Error.ToString(), DrawWindowError);
		errorStyle = GJAPIHelper.Skin.FindStyle("ErrorMsg") ?? GJAPIHelper.Skin.label;
		successStyle = GJAPIHelper.Skin.FindStyle("SuccessMsg") ?? GJAPIHelper.Skin.label;
		ellipsisStyle = GJAPIHelper.Skin.FindStyle("Ellipsis") ?? GJAPIHelper.Skin.label;
	}

	~GJHWindow()
	{
		drawWindowDelegates = null;
		errorStyle = null;
		successStyle = null;
		ellipsisStyle = null;
	}

	public bool IsShowing()
	{
		return GJHWindowsManager.IsWindowShowing(windowID);
	}

	public virtual bool Show()
	{
		return GJHWindowsManager.ShowWindow(windowID);
	}

	public virtual bool Dismiss()
	{
		return GJHWindowsManager.DismissWindow(windowID);
	}

	public void OnGUI()
	{
		if (GJAPIHelper.Skin != null)
		{
			GUI.skin = GJAPIHelper.Skin;
		}
		GUI.ModalWindow(windowID, Position, DrawWindow, Title);
	}

	private void DrawWindow(int windowID)
	{
		if (drawWindowDelegates.ContainsKey(currentWindowState))
		{
			BeginWindow();
			drawWindowDelegates[currentWindowState]();
			EndWindow();
		}
		else
		{
			Debug.Log("Unknown window state. Can't draw the window.");
		}
	}

	private void DrawWindowEmpty()
	{
		GUILayout.Label("I'm an empty window. Nobody likes me ;-(");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close"))
		{
			Dismiss();
		}
	}

	private void DrawWindowProcessing()
	{
		if (!string.IsNullOrEmpty(windowMsg))
		{
			GUILayout.Label(windowMsg);
		}
		GUILayout.Label(AnimatedEllipsis(5, 1.5f), ellipsisStyle);
		GUILayout.FlexibleSpace();
	}

	private void DrawWindowSuccess()
	{
		if (!string.IsNullOrEmpty(windowMsg))
		{
			GUILayout.Label(windowMsg, successStyle);
		}
		GUILayout.FlexibleSpace();
		if (windowReturnState != string.Empty)
		{
			if (GUILayout.Button("Ok"))
			{
				ChangeState(windowReturnState);
			}
		}
		else if (GUILayout.Button("Close"))
		{
			Dismiss();
		}
	}

	private void DrawWindowError()
	{
		if (!string.IsNullOrEmpty(windowMsg))
		{
			GUILayout.Label(windowMsg, errorStyle);
		}
		GUILayout.FlexibleSpace();
		if (windowReturnState != string.Empty)
		{
			if (GUILayout.Button("Ok"))
			{
				ChangeState(windowReturnState);
			}
		}
		else if (GUILayout.Button("Close"))
		{
			Dismiss();
		}
	}

	protected bool ChangeState(string state)
	{
		if (!drawWindowDelegates.ContainsKey(state))
		{
			Debug.LogWarning("No such state exist. Can't change window state.");
			return false;
		}
		previousWindowState = currentWindowState;
		currentWindowState = state;
		return true;
	}

	protected bool RevertToPreviousState()
	{
		if (string.IsNullOrEmpty(previousWindowState.Trim()))
		{
			Debug.LogWarning("No previous state found. Can't revert to previous window state.");
			return false;
		}
		string text = currentWindowState;
		currentWindowState = previousWindowState;
		previousWindowState = text;
		return true;
	}

	protected void SetWindowMessage(string msg, string returnState = "")
	{
		windowMsg = msg;
		windowReturnState = returnState;
	}

	protected void BeginWindow()
	{
		GUILayout.Space(35f);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
	}

	protected void EndWindow()
	{
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	protected string AnimatedEllipsis(int amount = 3, float speed = 1f)
	{
		return new string('.', (int)(Time.time * speed) % (amount + 1));
	}
}

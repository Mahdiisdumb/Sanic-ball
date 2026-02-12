using System;
using UnityEngine;

public class GJHLoginWindow : GJHWindow
{
	private enum LoginWindowStates
	{
		LoginForm = 0
	}

	private string uName = string.Empty;

	private string uToken = string.Empty;

	public GJHLoginWindow()
	{
		Title = "Login to Game Jolt";
		Position = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300f, 200f);
		drawWindowDelegates.Add(LoginWindowStates.LoginForm.ToString(), DrawForm);
	}

	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			ChangeState(LoginWindowStates.LoginForm.ToString());
		}
		return flag;
	}

	public override bool Dismiss()
	{
		return base.Dismiss();
	}

	private void DrawForm()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Username", GUILayout.Width(100f));
		uName = GUILayout.TextField(uName, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Token", GUILayout.Width(100f));
		uToken = GUILayout.PasswordField(uToken, '*', GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Login"))
		{
			if (uName.Trim() == string.Empty || uToken.Trim() == string.Empty)
			{
				SetWindowMessage("Either Username or Token is empty.", LoginWindowStates.LoginForm.ToString());
				ChangeState(BaseWindowStates.Error.ToString());
			}
			else
			{
				GJUsersMethods users = GJAPI.Users;
				users.VerifyCallback = (GJUsersMethods._VerifyCallback)Delegate.Combine(users.VerifyCallback, new GJUsersMethods._VerifyCallback(OnVerifyUser));
				GJAPI.Users.Verify(uName, uToken);
				SetWindowMessage("Connecting", string.Empty);
				ChangeState(BaseWindowStates.Process.ToString());
			}
		}
		if (GUILayout.Button("Cancel"))
		{
			Dismiss();
		}
	}

	private void OnVerifyUser(bool success)
	{
		GJUsersMethods users = GJAPI.Users;
		users.VerifyCallback = (GJUsersMethods._VerifyCallback)Delegate.Remove(users.VerifyCallback, new GJUsersMethods._VerifyCallback(OnVerifyUser));
		if (!success)
		{
			SetWindowMessage("Error logging in.\nPlease check your username and token.", LoginWindowStates.LoginForm.ToString());
			ChangeState(BaseWindowStates.Error.ToString());
		}
		else
		{
			SetWindowMessage(string.Format("Hello {0}!", uName), string.Empty);
			ChangeState(BaseWindowStates.Success.ToString());
		}
	}
}

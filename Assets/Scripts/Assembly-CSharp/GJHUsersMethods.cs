using System;
using UnityEngine;

public class GJHUsersMethods
{
	private Action<string, string> getFromWebCallback;

	private GJHLoginWindow window;

	public GJHUsersMethods()
	{
		window = new GJHLoginWindow();
	}

	~GJHUsersMethods()
	{
		getFromWebCallback = null;
		window = null;
	}

	public void GetFromWeb(Action<string, string> onComplete)
	{
		getFromWebCallback = null;
		if (onComplete != null)
		{
			onComplete(string.Empty, string.Empty);
		}
		Debug.Log("GJAPIHelper: The method \"GetFromWeb\" can only be called from WebPlayer builds.");
	}

	public void ReadGetFromWebResponse(string response)
	{
		if (getFromWebCallback != null)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (response.ToLower() == "false" || response == string.Empty || response.ToLower() == "Guest")
			{
				empty = "Guest";
				empty2 = string.Empty;
			}
			else
			{
				string[] array = response.Split(':');
				empty = array[0];
				empty2 = array[1];
			}
			getFromWebCallback(empty, empty2);
		}
	}

	public void ShowGreetingNotification()
	{
		if (GJAPI.User == null)
		{
			Debug.LogWarning("GJAPIHelper: There is no verified user to show greetings to ;-(");
			return;
		}
		GJHNotification notification = new GJHNotification(string.Format("Welcome back {0}!", GJAPI.User.Name), string.Empty);
		GJHNotificationsManager.QueueNotification(notification);
	}

	public void ShowLogin()
	{
		window.Show();
	}

	public void DismissLogin()
	{
		window.Dismiss();
	}

	public void DownloadUserAvatar(GJUser user, Action<Texture2D> OnComplete)
	{
		GJAPIHelper.DownloadImage(user.AvatarURL, delegate(Texture2D avatar)
		{
			if (avatar == null)
			{
				avatar = ((Texture2D)Resources.Load("Images/UserAvatar", typeof(Texture2D))) ?? new Texture2D(60, 60);
			}
			if (OnComplete != null)
			{
				OnComplete(avatar);
			}
		});
	}
}

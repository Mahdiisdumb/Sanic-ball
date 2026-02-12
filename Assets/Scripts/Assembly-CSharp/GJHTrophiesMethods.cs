using System;
using UnityEngine;

public class GJHTrophiesMethods
{
	private GJHTrophiesWindow window;

	public GJHTrophiesMethods()
	{
		window = new GJHTrophiesWindow();
	}

	~GJHTrophiesMethods()
	{
		window = null;
	}

	public void SetSecretTrophies(uint[] ids, bool show)
	{
		window.secretTrophies = ids;
		window.showSecretTrophies = show;
	}

	public void ShowTrophies()
	{
		window.Show();
	}

	public void DismissTrophies()
	{
		window.Dismiss();
	}

	public void ShowTrophyUnlockNotification(uint trophyID)
	{
		GJTrophiesMethods trophies = GJAPI.Trophies;
		trophies.GetOneCallback = (GJTrophiesMethods._GetOneCallback)Delegate.Combine(trophies.GetOneCallback, new GJTrophiesMethods._GetOneCallback(OnGetTrophy));
		GJAPI.Trophies.Get(trophyID);
	}

	private void OnGetTrophy(GJTrophy trophy)
	{
		GJTrophiesMethods trophies = GJAPI.Trophies;
		trophies.GetOneCallback = (GJTrophiesMethods._GetOneCallback)Delegate.Remove(trophies.GetOneCallback, new GJTrophiesMethods._GetOneCallback(OnGetTrophy));
		if (trophy != null)
		{
			DownloadTrophyIcon(trophy, delegate(Texture2D tex)
			{
				GJHNotification notification = new GJHNotification(trophy.Title, trophy.Description, tex);
				GJHNotificationsManager.QueueNotification(notification);
			});
		}
	}

	public void DownloadTrophyIcon(GJTrophy trophy, Action<Texture2D> OnComplete)
	{
		GJAPIHelper.DownloadImage(trophy.ImageURL, delegate(Texture2D icon)
		{
			if (icon == null)
			{
				icon = ((Texture2D)Resources.Load("Images/TrophyIcon", typeof(Texture2D))) ?? new Texture2D(75, 75);
			}
			if (OnComplete != null)
			{
				OnComplete(icon);
			}
		});
	}
}

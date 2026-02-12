using System;
using System.Collections.Generic;
using UnityEngine;

public class GJHTrophiesWindow : GJHWindow
{
	private enum TrophiesWindowStates
	{
		TrophiesList = 0
	}

	private Vector2 trophiesScrollViewPosition;

	private GJTrophy[] trophies;

	private Texture2D[] trophiesIcons;

	private GUIStyle trophyTitleStyle;

	private GUIStyle trophyDescriptionStyle;

	public uint[] secretTrophies;

	public bool showSecretTrophies = true;

	public GJHTrophiesWindow()
	{
		Title = "Trophies";
		float num = (float)Screen.width * 0.9f;
		num = ((!(num > 500f)) ? num : 500f);
		float num2 = (float)Screen.height * 0.9f;
		Position = new Rect((float)(Screen.width / 2) - num / 2f, (float)(Screen.height / 2) - num2 / 2f, num, num2);
		drawWindowDelegates.Add(TrophiesWindowStates.TrophiesList.ToString(), DrawTrophiesList);
		trophyTitleStyle = GJAPIHelper.Skin.FindStyle("TrophyTitle") ?? GJAPIHelper.Skin.label;
		trophyDescriptionStyle = GJAPIHelper.Skin.FindStyle("TrophyDescription") ?? GJAPIHelper.Skin.label;
	}

	~GJHTrophiesWindow()
	{
		trophies = null;
		trophiesIcons = null;
		trophyTitleStyle = null;
		trophyDescriptionStyle = null;
		secretTrophies = null;
	}

	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			GetTrophies();
		}
		return flag;
	}

	public override bool Dismiss()
	{
		return base.Dismiss();
	}

	private void GetTrophies()
	{
		SetWindowMessage("Loading trophies", string.Empty);
		ChangeState(BaseWindowStates.Process.ToString());
		GJTrophiesMethods gJTrophiesMethods = GJAPI.Trophies;
		gJTrophiesMethods.GetAllCallback = (GJTrophiesMethods._GetAllCallback)Delegate.Combine(gJTrophiesMethods.GetAllCallback, new GJTrophiesMethods._GetAllCallback(OnGetTrophies));
		GJAPI.Trophies.GetAll();
	}

	private void OnGetTrophies(GJTrophy[] t)
	{
		GJTrophiesMethods gJTrophiesMethods = GJAPI.Trophies;
		gJTrophiesMethods.GetAllCallback = (GJTrophiesMethods._GetAllCallback)Delegate.Remove(gJTrophiesMethods.GetAllCallback, new GJTrophiesMethods._GetAllCallback(OnGetTrophies));
		if (t == null)
		{
			SetWindowMessage("Error loading trophies.", string.Empty);
			ChangeState(BaseWindowStates.Error.ToString());
			return;
		}
		trophies = t;
		int num = trophies.Length;
		trophiesIcons = new Texture2D[num];
		for (int i = 0; i < num; i++)
		{
			trophiesIcons[i] = ((Texture2D)Resources.Load("Images/TrophyIcon", typeof(Texture2D))) ?? new Texture2D(75, 75);
			int index = i;
			GJAPIHelper.Trophies.DownloadTrophyIcon(trophies[i], delegate(Texture2D icon)
			{
				trophiesIcons[index] = icon;
			});
		}
		ChangeState(TrophiesWindowStates.TrophiesList.ToString());
	}

	private void DrawTrophiesList()
	{
		trophiesScrollViewPosition = GUILayout.BeginScrollView(trophiesScrollViewPosition);
		int num = trophies.Length;
		for (int i = 0; i < num; i++)
		{
			if (secretTrophies != null && secretTrophies.Length > 0 && ((ICollection<uint>)secretTrophies).Contains(trophies[i].Id) && !trophies[i].Achieved)
			{
				if (!showSecretTrophies)
				{
					continue;
				}
				DrawTrophy(i, false);
			}
			else
			{
				DrawTrophy(i, true);
			}
			if (i != num - 1)
			{
				GUILayout.Space(10f);
			}
		}
		GUILayout.EndScrollView();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close"))
		{
			Dismiss();
		}
		GUILayout.EndHorizontal();
	}

	private void DrawTrophy(int t, bool show)
	{
		GUILayout.BeginHorizontal();
		GUI.enabled = (trophies[t].Achieved ? true : false);
		GUILayout.Label(trophiesIcons[t]);
		GUI.enabled = true;
		GUILayout.Space(10f);
		GUILayout.BeginVertical("box", GUILayout.Height(75f));
		GUILayout.FlexibleSpace();
		GUILayout.Label((!show) ? "???" : trophies[t].Title, trophyTitleStyle);
		GUILayout.Space(5f);
		GUILayout.Label((!show) ? "???" : trophies[t].Description, trophyDescriptionStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}

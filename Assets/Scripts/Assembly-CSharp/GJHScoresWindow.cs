using System;
using UnityEngine;

public class GJHScoresWindow : GJHWindow
{
	private enum ScoresWindowStates
	{
		ScoresList = 0
	}

	private int tablesToolbarIndex;

	private int lastTablesToolbarIndex;

	private string[] tablesNames;

	private GJTable[] tables;

	private Vector2 scoresScrollViewPosition;

	private GJScore[] scores;

	private GUIStyle userScoreStyle;

	public GJHScoresWindow()
	{
		Title = "Leaderboards";
		float num = (float)Screen.width * 0.9f;
		num = ((!(num > 400f)) ? num : 400f);
		float num2 = (float)Screen.height * 0.9f;
		Position = new Rect((float)(Screen.width / 2) - num / 2f, (float)(Screen.height / 2) - num2 / 2f, num, num2);
		drawWindowDelegates.Add(ScoresWindowStates.ScoresList.ToString(), DrawScoresList);
		userScoreStyle = GJAPIHelper.Skin.FindStyle("UserScore") ?? GJAPIHelper.Skin.label;
	}

	~GJHScoresWindow()
	{
		tables = null;
		scores = null;
		userScoreStyle = null;
	}

	public override bool Show()
	{
		bool flag = base.Show();
		if (flag)
		{
			if (tablesNames != null)
			{
				GetScores();
			}
			else
			{
				GetScoreTables();
			}
		}
		return flag;
	}

	public override bool Dismiss()
	{
		bool flag = base.Dismiss();
		if (flag)
		{
			scores = null;
		}
		return flag;
	}

	private void GetScoreTables()
	{
		SetWindowMessage("Loading score tables", string.Empty);
		ChangeState(BaseWindowStates.Process.ToString());
		tables = null;
		GJScoresMethods gJScoresMethods = GJAPI.Scores;
		gJScoresMethods.GetTablesCallback = (GJScoresMethods._GetTablesCallback)Delegate.Combine(gJScoresMethods.GetTablesCallback, new GJScoresMethods._GetTablesCallback(OnGetScoreTables));
		GJAPI.Scores.GetTables();
	}

	private void OnGetScoreTables(GJTable[] t)
	{
		GJScoresMethods gJScoresMethods = GJAPI.Scores;
		gJScoresMethods.GetTablesCallback = (GJScoresMethods._GetTablesCallback)Delegate.Remove(gJScoresMethods.GetTablesCallback, new GJScoresMethods._GetTablesCallback(OnGetScoreTables));
		if (t == null)
		{
			SetWindowMessage("Error loading score tables.", string.Empty);
			ChangeState(BaseWindowStates.Error.ToString());
			return;
		}
		tables = t;
		int num = t.Length;
		tablesNames = new string[num];
		for (int i = 0; i < num; i++)
		{
			tablesNames[i] = t[i].Name;
		}
		GetScores();
	}

	private void GetScores()
	{
		SetWindowMessage("Loading Scores", string.Empty);
		ChangeState(BaseWindowStates.Process.ToString());
		scores = null;
		GJScoresMethods gJScoresMethods = GJAPI.Scores;
		gJScoresMethods.GetMultipleCallback = (GJScoresMethods._GetMultipleCallback)Delegate.Combine(gJScoresMethods.GetMultipleCallback, new GJScoresMethods._GetMultipleCallback(OnGetScores));
		GJAPI.Scores.Get(false, tables[tablesToolbarIndex].Id, 100u);
	}

	private void OnGetScores(GJScore[] s)
	{
		GJScoresMethods gJScoresMethods = GJAPI.Scores;
		gJScoresMethods.GetMultipleCallback = (GJScoresMethods._GetMultipleCallback)Delegate.Remove(gJScoresMethods.GetMultipleCallback, new GJScoresMethods._GetMultipleCallback(OnGetScores));
		if (s == null)
		{
			SetWindowMessage("Error loading scores.", string.Empty);
			ChangeState(BaseWindowStates.Error.ToString());
		}
		else
		{
			scores = s;
			ChangeState(ScoresWindowStates.ScoresList.ToString());
		}
	}

	private void DrawScoresList()
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		tablesToolbarIndex = GUILayout.Toolbar(tablesToolbarIndex, tablesNames);
		if (tablesToolbarIndex != lastTablesToolbarIndex)
		{
			lastTablesToolbarIndex = tablesToolbarIndex;
			GetScores();
			return;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		scoresScrollViewPosition = GUILayout.BeginScrollView(scoresScrollViewPosition);
		int num = scores.Length;
		for (int i = 0; i < num; i++)
		{
			DrawScore(i);
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

	private void DrawScore(int s)
	{
		if (GJAPI.User != null && (GJAPI.User.Name == scores[s].Name || (GJAPI.User.Type == GJUser.UserType.Developer && GJAPI.User.GetProperty("developer_name") == scores[s].Name)))
		{
			GUILayout.BeginHorizontal(userScoreStyle);
		}
		else
		{
			GUILayout.BeginHorizontal();
		}
		GUILayout.Label(scores[s].Name);
		GUILayout.FlexibleSpace();
		GUILayout.Label(scores[s].Score);
		GUILayout.EndHorizontal();
	}
}

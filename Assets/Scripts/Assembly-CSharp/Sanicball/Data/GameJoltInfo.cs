using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sanicball.Data
{
	[Serializable]
	public class GameJoltInfo
	{
		public int gameID;

		public string privateKey;

		public bool verbose;

		public bool disabled;

		private Dictionary<string, PlayerType> specialUsers = new Dictionary<string, PlayerType>();

		public void Init()
		{
			if (!disabled)
			{
				GJAPI.Init(gameID, privateKey, verbose);
				GJAPI.Data.Get("players");
				GJDataMehods data = GJAPI.Data;
				data.GetCallback = (GJDataMehods._GetCallback)Delegate.Combine(data.GetCallback, new GJDataMehods._GetCallback(LoadSpecialUsersCallback));
				string gameJoltUsername = ActiveData.GameSettings.gameJoltUsername;
				string gameJoltToken = ActiveData.GameSettings.gameJoltToken;
				if (!string.IsNullOrEmpty(gameJoltUsername) && !string.IsNullOrEmpty(gameJoltToken))
				{
					GJAPI.Users.Verify(gameJoltUsername, gameJoltToken);
					GJUsersMethods users = GJAPI.Users;
					users.VerifyCallback = (GJUsersMethods._VerifyCallback)Delegate.Combine(users.VerifyCallback, new GJUsersMethods._VerifyCallback(CheckIfSignedInCallback));
				}
			}
		}

		public PlayerType GetPlayerType(string username)
		{
			if (disabled)
			{
				return PlayerType.Normal;
			}
			PlayerType value;
			if (!string.IsNullOrEmpty(username) && specialUsers.TryGetValue(username, out value))
			{
				return value;
			}
			return PlayerType.Normal;
		}

		public Color GetPlayerColor(PlayerType type)
		{
			switch (type)
			{
			case PlayerType.Anon:
				return new Color(0.88f, 0.88f, 0.88f);
			case PlayerType.Normal:
				return Color.white;
			case PlayerType.Developer:
				return new Color(0.6f, 0.7f, 1f);
			case PlayerType.Special:
				return new Color(0.2f, 0.8f, 0.2f);
			case PlayerType.Donator:
				return new Color(1f, 0.8f, 0.4f);
			default:
				return Color.white;
			}
		}

		private void LoadSpecialUsersCallback(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				Debug.LogError("Failed to load special player types using GJAPI.");
				GJDataMehods data2 = GJAPI.Data;
				data2.GetCallback = (GJDataMehods._GetCallback)Delegate.Remove(data2.GetCallback, new GJDataMehods._GetCallback(LoadSpecialUsersCallback));
				return;
			}
			string[] array = data.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split('=');
				if (array3.Length == 2)
				{
					string key = array3[0];
					string s = array3[1];
					int result;
					if (int.TryParse(s, out result))
					{
						specialUsers.Add(key, (PlayerType)result);
					}
				}
			}
			Debug.Log("Special user list loaded.");
			GJDataMehods data3 = GJAPI.Data;
			data3.GetCallback = (GJDataMehods._GetCallback)Delegate.Remove(data3.GetCallback, new GJDataMehods._GetCallback(LoadSpecialUsersCallback));
		}

		private void CheckIfSignedInCallback(bool isLegit)
		{
			if (!isLegit)
			{
				ActiveData.GameSettings.gameJoltUsername = string.Empty;
				ActiveData.GameSettings.gameJoltToken = string.Empty;
			}
			GJUsersMethods users = GJAPI.Users;
			users.VerifyCallback = (GJUsersMethods._VerifyCallback)Delegate.Remove(users.VerifyCallback, new GJUsersMethods._VerifyCallback(CheckIfSignedInCallback));
		}
	}
}

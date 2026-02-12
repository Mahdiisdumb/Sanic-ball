using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GJUsersMethods
{
	public delegate void _VerifyCallback(bool verified);

	public delegate void _GetVerifiedCallback(GJUser user);

	public delegate void _GetOneCallback(GJUser user);

	public delegate void _GetMultipleCallback(GJUser[] users);

	private const string USERS_AUTH = "users/auth/";

	private const string USERS_FETCH = "users/";

	public _VerifyCallback VerifyCallback;

	public _GetVerifiedCallback GetVerifiedCallback;

	public _GetOneCallback GetOneCallback;

	public _GetMultipleCallback GetMultipleCallback;

	~GJUsersMethods()
	{
		VerifyCallback = null;
		GetVerifiedCallback = null;
		GetOneCallback = null;
		GetMultipleCallback = null;
	}

	public void Verify(string name, string token)
	{
		if (name.Trim() == string.Empty || token.Trim() == string.Empty)
		{
			GJAPI.Instance.GJDebug("Either name or token is empty. Can't verify user.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Verifying user.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("username", name);
		dictionary.Add("user_token", token);
		GJAPI.User = new GJUser();
		GJAPI.User.Name = name;
		GJAPI.User.Token = token;
		GJAPI.Instance.Request("users/auth/", dictionary, false, ReadVerifyResponse);
	}

	private void ReadVerifyResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not verify the user.\n" + response, LogType.Error);
			GJAPI.User = null;
		}
		else
		{
			GJAPI.Instance.GJDebug("User successfully verified.\n" + GJAPI.User.ToString());
		}
		if (VerifyCallback != null)
		{
			VerifyCallback(flag);
		}
	}

	public void GetVerified()
	{
		if (GJAPI.User == null)
		{
			GJAPI.Instance.GJDebug("There is no verified user. Please verify a user before calling GetVerifiedUser.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Getting verified user.");
		GJAPI.Instance.Request("users/", null, true, ReadGetVerifiedResponse);
	}

	private void ReadGetVerifiedResponse(string response)
	{
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the verified user.\n" + response, LogType.Error);
			GJAPI.User = null;
		}
		else
		{
			Dictionary<string, string> dictionary = GJAPI.Instance.ResponseToDictionary(response);
			GJAPI.Instance.CleanDictionary(ref dictionary);
			GJAPI.User.AddProperties(dictionary, true);
			GJAPI.Instance.GJDebug("Verified user successfully fetched.\n" + GJAPI.User.ToString());
		}
		if (GetVerifiedCallback != null)
		{
			GetVerifiedCallback(GJAPI.User);
		}
	}

	public void Get(string name)
	{
		if (name.Trim() == string.Empty)
		{
			GJAPI.Instance.GJDebug("Name is empty. Can't get user.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Getting user.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("username", name);
		GJAPI.Instance.Request("users/", dictionary, false, ReadGetOneResponse);
	}

	public void Get(uint id)
	{
		GJAPI.Instance.GJDebug("Getting user.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("user_id", id.ToString());
		GJAPI.Instance.Request("users/", dictionary, false, ReadGetOneResponse);
	}

	private void ReadGetOneResponse(string response)
	{
		GJUser gJUser;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the user.\n" + response, LogType.Error);
			gJUser = null;
		}
		else
		{
			Dictionary<string, string> dictionary = GJAPI.Instance.ResponseToDictionary(response);
			GJAPI.Instance.CleanDictionary(ref dictionary);
			gJUser = new GJUser(dictionary);
			GJAPI.Instance.GJDebug("User successfully fetched.\n" + gJUser.ToString());
		}
		if (GetOneCallback != null)
		{
			GetOneCallback(gJUser);
		}
	}

	public void Get(uint[] ids)
	{
		if (ids == null)
		{
			GJAPI.Instance.GJDebug("IDs are null. Can't get users.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Getting users.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string value = string.Join(",", new List<uint>(ids).ConvertAll((uint i) => i.ToString()).ToArray());
		dictionary.Add("user_id", value);
		GJAPI.Instance.Request("users/", dictionary, false, ReadGetMultipleResponse);
	}

	private void ReadGetMultipleResponse(string response)
	{
		GJUser[] array;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the users.\n" + response, LogType.Error);
			array = null;
		}
		else
		{
			Dictionary<string, string>[] dictionaries = GJAPI.Instance.ResponseToDictionaries(response);
			GJAPI.Instance.CleanDictionaries(ref dictionaries);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Users successfully fetched.\n");
			int num = dictionaries.Length;
			array = new GJUser[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new GJUser(dictionaries[i]);
				stringBuilder.Append(array[i].ToString());
			}
			GJAPI.Instance.GJDebug(stringBuilder.ToString());
		}
		if (GetMultipleCallback != null)
		{
			GetMultipleCallback(array);
		}
	}
}

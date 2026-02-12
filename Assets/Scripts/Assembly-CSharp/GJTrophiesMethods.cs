using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GJTrophiesMethods
{
	public delegate void _AddTrophyCallback(bool success);

	public delegate void _GetOneCallback(GJTrophy trophy);

	public delegate void _GetMultipleCallback(GJTrophy[] trophies);

	public delegate void _GetAllCallback(GJTrophy[] trophies);

	private const string TROPHIES_ADD = "trophies/add-achieved/";

	private const string TROPHIES_FETCH = "trophies/";

	public _AddTrophyCallback AddCallback;

	public _GetOneCallback GetOneCallback;

	public _GetMultipleCallback GetMultipleCallback;

	public _GetAllCallback GetAllCallback;

	~GJTrophiesMethods()
	{
		AddCallback = null;
		GetOneCallback = null;
		GetMultipleCallback = null;
		GetAllCallback = null;
	}

	public void Add(uint id)
	{
		if (id == 0)
		{
			GJAPI.Instance.GJDebug("ID is null. Can't add Trophy.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Adding Trophy: " + id + ".");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("trophy_id", id.ToString());
		GJAPI.Instance.Request("trophies/add-achieved/", dictionary, true, ReadAddResponse);
	}

	private void ReadAddResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not add Trophy.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Trophy successfully added.");
		}
		if (AddCallback != null)
		{
			AddCallback(flag);
		}
	}

	public void Get(uint id)
	{
		if (id == 0)
		{
			GJAPI.Instance.GJDebug("ID is null. Can't get trophy.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Getting Trophy: " + id + ".");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("trophy_id", id.ToString());
		GJAPI.Instance.Request("trophies/", dictionary, true, ReadGetOneResponse);
	}

	private void ReadGetOneResponse(string response)
	{
		GJTrophy gJTrophy;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the trophy.\n" + response, LogType.Error);
			gJTrophy = null;
		}
		else
		{
			Dictionary<string, string> dictionary = GJAPI.Instance.ResponseToDictionary(response);
			GJAPI.Instance.CleanDictionary(ref dictionary);
			gJTrophy = new GJTrophy(dictionary);
			GJAPI.Instance.GJDebug("Trophy successfully fetched.\n" + gJTrophy.ToString());
		}
		if (GetOneCallback != null)
		{
			GetOneCallback(gJTrophy);
		}
	}

	public void Get(uint[] ids)
	{
		if (ids == null)
		{
			GJAPI.Instance.GJDebug("IDs are null. Can't get trophies.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Getting trophies.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string value = string.Join(",", new List<uint>(ids).ConvertAll((uint i) => i.ToString()).ToArray());
		dictionary.Add("trophy_id", value);
		GJAPI.Instance.Request("trophies/", dictionary, true, ReadGetMultipleResponse);
	}

	private void ReadGetMultipleResponse(string response)
	{
		GJTrophy[] array;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the trophies.\n" + response, LogType.Error);
			array = null;
		}
		else
		{
			Dictionary<string, string>[] dictionaries = GJAPI.Instance.ResponseToDictionaries(response);
			GJAPI.Instance.CleanDictionaries(ref dictionaries);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Trophies successfully fetched.\n");
			int num = dictionaries.Length;
			array = new GJTrophy[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new GJTrophy(dictionaries[i]);
				stringBuilder.Append(array[i].ToString());
			}
			GJAPI.Instance.GJDebug(stringBuilder.ToString());
		}
		if (GetMultipleCallback != null)
		{
			GetMultipleCallback(array);
		}
	}

	public void GetAll()
	{
		GJAPI.Instance.GJDebug("Getting all trophies.");
		GJAPI.Instance.Request("trophies/", null, true, ReadGetAllResponse);
	}

	public void GetAll(bool achieved)
	{
		if (achieved)
		{
			GJAPI.Instance.GJDebug("Getting all trophies the verified user has achieved.");
		}
		else
		{
			GJAPI.Instance.GJDebug("Getting all trophies the verified user hasn't achieved.");
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("achieved", achieved.ToString().ToLower());
		GJAPI.Instance.Request("trophies/", dictionary, true, ReadGetAllResponse);
	}

	private void ReadGetAllResponse(string response)
	{
		GJTrophy[] array;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the trophies.\n" + response, LogType.Error);
			array = null;
		}
		else
		{
			Dictionary<string, string>[] dictionaries = GJAPI.Instance.ResponseToDictionaries(response);
			GJAPI.Instance.CleanDictionaries(ref dictionaries);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Trophies successfully fetched.\n");
			int num = dictionaries.Length;
			array = new GJTrophy[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new GJTrophy(dictionaries[i]);
				stringBuilder.Append(array[i].ToString());
			}
			GJAPI.Instance.GJDebug(stringBuilder.ToString());
		}
		if (GetAllCallback != null)
		{
			GetAllCallback(array);
		}
	}
}

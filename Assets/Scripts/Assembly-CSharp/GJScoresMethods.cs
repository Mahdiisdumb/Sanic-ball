using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GJScoresMethods
{
	public delegate void _AddCallback(bool success);

	public delegate void _GetMultipleCallback(GJScore[] scores);

	public delegate void _GetTablesCallback(GJTable[] tables);

	private const string SCORES_ADD = "scores/add/";

	private const string SCORES_FETCH = "scores/";

	private const string SCORES_TABLES = "scores/tables/";

	public _AddCallback AddCallback;

	public _GetMultipleCallback GetMultipleCallback;

	public _GetTablesCallback GetTablesCallback;

	~GJScoresMethods()
	{
		AddCallback = null;
		GetMultipleCallback = null;
		GetTablesCallback = null;
	}

	public void Add(string score, uint sort, uint table = 0, string extraData = "")
	{
		if (score.Trim() == string.Empty || sort == 0)
		{
			GJAPI.Instance.GJDebug("Either score is empty or sort equal to zero (or both). Can't add score.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Adding score for verified user: " + sort);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("score", score);
		dictionary.Add("sort", sort.ToString());
		if (extraData.Trim() != string.Empty)
		{
			dictionary.Add("extra_data", extraData);
		}
		if (table != 0)
		{
			dictionary.Add("table_id", table.ToString());
		}
		GJAPI.Instance.Request("scores/add/", dictionary, true, ReadAddResponse);
	}

	public void AddForGuest(string score, uint sort, string name = "Guest", uint table = 0, string extraData = "")
	{
		if (score.Trim() == string.Empty || sort == 0 || name == string.Empty)
		{
			GJAPI.Instance.GJDebug("Either score is empty or sort equal to zero or name is empty (or all of them). Can't add score.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Adding score for guest: " + sort);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("score", score);
		dictionary.Add("sort", sort.ToString());
		dictionary.Add("guest", name);
		if (extraData.Trim() != string.Empty)
		{
			dictionary.Add("extra_data", extraData);
		}
		if (table != 0)
		{
			dictionary.Add("table_id", table.ToString());
		}
		GJAPI.Instance.Request("scores/add/", dictionary, false, ReadAddResponse);
	}

	private void ReadAddResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not add score.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Score successfully added.");
		}
		if (AddCallback != null)
		{
			AddCallback(flag);
		}
	}

	public void Get(bool ofVerifiedUserOnly = false, uint table = 0, uint limit = 10)
	{
		switch (limit)
		{
		case 0u:
			GJAPI.Instance.GJDebug("Limit can't be equal to zero. Limit will be set to 1.", LogType.Warning);
			limit = 1u;
			break;
		default:
			GJAPI.Instance.GJDebug("Limit can't be greater than 100. Limit will be set to 100.", LogType.Warning);
			limit = 100u;
			break;
		case 1u:
		case 2u:
		case 3u:
		case 4u:
		case 5u:
		case 6u:
		case 7u:
		case 8u:
		case 9u:
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
		case 15u:
		case 16u:
		case 17u:
		case 18u:
		case 19u:
		case 20u:
		case 21u:
		case 22u:
		case 23u:
		case 24u:
		case 25u:
		case 26u:
		case 27u:
		case 28u:
		case 29u:
		case 30u:
		case 31u:
		case 32u:
		case 33u:
		case 34u:
		case 35u:
		case 36u:
		case 37u:
		case 38u:
		case 39u:
		case 40u:
		case 41u:
		case 42u:
		case 43u:
		case 44u:
		case 45u:
		case 46u:
		case 47u:
		case 48u:
		case 49u:
		case 50u:
		case 51u:
		case 52u:
		case 53u:
		case 54u:
		case 55u:
		case 56u:
		case 57u:
		case 58u:
		case 59u:
		case 60u:
		case 61u:
		case 62u:
		case 63u:
		case 64u:
		case 65u:
		case 66u:
		case 67u:
		case 68u:
		case 69u:
		case 70u:
		case 71u:
		case 72u:
		case 73u:
		case 74u:
		case 75u:
		case 76u:
		case 77u:
		case 78u:
		case 79u:
		case 80u:
		case 81u:
		case 82u:
		case 83u:
		case 84u:
		case 85u:
		case 86u:
		case 87u:
		case 88u:
		case 89u:
		case 90u:
		case 91u:
		case 92u:
		case 93u:
		case 94u:
		case 95u:
		case 96u:
		case 97u:
		case 98u:
		case 99u:
		case 100u:
			break;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("limit", limit.ToString());
		if (table != 0)
		{
			dictionary.Add("table_id", table.ToString());
		}
		GJAPI.Instance.Request("scores/", dictionary, ofVerifiedUserOnly, ReadGetResponse);
	}

	private void ReadGetResponse(string response)
	{
		GJScore[] array;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not fetch scores.\n" + response, LogType.Error);
			array = null;
		}
		else
		{
			Dictionary<string, string>[] dictionaries = GJAPI.Instance.ResponseToDictionaries(response);
			GJAPI.Instance.CleanDictionaries(ref dictionaries);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Scores successfully fetched.\n");
			int num = dictionaries.Length;
			array = new GJScore[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new GJScore(dictionaries[i]);
				stringBuilder.Append(array[i].ToString());
			}
			GJAPI.Instance.GJDebug(stringBuilder.ToString());
		}
		if (GetMultipleCallback != null)
		{
			GetMultipleCallback(array);
		}
	}

	public void GetTables()
	{
		GJAPI.Instance.GJDebug("Getting score tables.");
		GJAPI.Instance.Request("scores/tables/", null, false, ReadGetTablesResponse);
	}

	private void ReadGetTablesResponse(string response)
	{
		GJTable[] array;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not fetch score tables.\n" + response, LogType.Error);
			array = null;
		}
		else
		{
			Dictionary<string, string>[] dictionaries = GJAPI.Instance.ResponseToDictionaries(response);
			GJAPI.Instance.CleanDictionaries(ref dictionaries);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Score Tables successfully fetched.\n");
			int num = dictionaries.Length;
			array = new GJTable[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new GJTable(dictionaries[i]);
				stringBuilder.Append(array[i].ToString());
			}
			GJAPI.Instance.GJDebug(stringBuilder.ToString());
		}
		if (GetTablesCallback != null)
		{
			GetTablesCallback(array);
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class GJDataMehods
{
	public enum UpdateOperation
	{
		Add = 0,
		Subtract = 1,
		Multiply = 2,
		Divide = 3,
		Append = 4,
		Prepend = 5
	}

	public delegate void _SetCallback(bool success);

	public delegate void _UpdateSuccessCallback(bool success);

	public delegate void _UpdateCallback(string data);

	public delegate void _GetCallback(string data);

	public delegate void _RemoveKey(bool success);

	public delegate void _GetKeysCallback(string[] keys);

	private const string DATA_FETCH = "data-store/";

	private const string DATA_SET = "data-store/set/";

	private const string DATA_UPDATE = "data-store/update/";

	private const string DATA_REMOVE = "data-store/remove/";

	private const string DATA_KEYS = "data-store/get-keys/";

	public _SetCallback SetCallback;

	public _UpdateSuccessCallback UpdateSuccessCallback;

	public _UpdateCallback UpdateCallback;

	public _GetCallback GetCallback;

	public _RemoveKey RemoveKeyCallback;

	public _GetKeysCallback GetKeysCallback;

	~GJDataMehods()
	{
		SetCallback = null;
		UpdateSuccessCallback = null;
		UpdateCallback = null;
		GetCallback = null;
		RemoveKeyCallback = null;
		GetKeysCallback = null;
	}

	public void Set(string key, string val, bool userData = false)
	{
		if (key.Trim() == string.Empty)
		{
			GJAPI.Instance.GJDebug("Key is empty. Can't add data.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Adding data.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("data", val);
		GJAPI.Instance.Request("data-store/set/", dictionary, dictionary2, userData, ReadSetResponse);
	}

	private void ReadSetResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not add data.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Data successfully added.");
		}
		if (SetCallback != null)
		{
			SetCallback(flag);
		}
	}

	public void Update(string key, string val, UpdateOperation operation, bool userData = false)
	{
		if (key.Trim() == string.Empty)
		{
			GJAPI.Instance.GJDebug("Key is empty. Can't get data.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Updating data.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		dictionary.Add("operation", operation.ToString().ToLower());
		dictionary.Add("format", "dump");
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("value", val);
		GJAPI.Instance.Request("data-store/update/", dictionary, dictionary2, userData, ReadUpdateResponse);
	}

	private void ReadUpdateResponse(string response)
	{
		string data = string.Empty;
		bool flag = GJAPI.Instance.IsDumpResponseSuccessful(ref response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not update data.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.DumpResponseToString(ref response, out data);
			if (data == string.Empty)
			{
				GJAPI.Instance.GJDebug("Data successfully updated. However data is empty.", LogType.Warning);
			}
			else
			{
				GJAPI.Instance.GJDebug("Data successfully updated.\n" + data);
			}
		}
		if (UpdateSuccessCallback != null)
		{
			UpdateSuccessCallback(flag);
		}
		if (UpdateCallback != null)
		{
			UpdateCallback(data);
		}
	}

	public void Get(string key, bool userData = false)
	{
		if (key.Trim() == string.Empty)
		{
			GJAPI.Instance.GJDebug("Key is empty. Can't get data.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Getting data.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		dictionary.Add("format", "dump");
		GJAPI.Instance.Request("data-store/", dictionary, userData, ReadGetResponse);
	}

	private void ReadGetResponse(string response)
	{
		string data = string.Empty;
		if (!GJAPI.Instance.IsDumpResponseSuccessful(ref response))
		{
			GJAPI.Instance.GJDebug("Could not fetch data.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.DumpResponseToString(ref response, out data);
			if (data == string.Empty)
			{
				GJAPI.Instance.GJDebug("Data successfully fetched. However data is empty.", LogType.Warning);
			}
			else
			{
				GJAPI.Instance.GJDebug("Data successfully fetched.\n" + data);
			}
		}
		if (GetCallback != null)
		{
			GetCallback(data);
		}
	}

	public void RemoveKey(string key, bool userData = false)
	{
		if (key.Trim() == string.Empty)
		{
			GJAPI.Instance.GJDebug("Key is empty. Can't remove key.", LogType.Error);
			return;
		}
		GJAPI.Instance.GJDebug("Removing key.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", key);
		GJAPI.Instance.Request("data-store/remove/", dictionary, userData, ReadRemoveKeyResponse);
	}

	private void ReadRemoveKeyResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not remove key.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Key successfully removed.");
		}
		if (RemoveKeyCallback != null)
		{
			RemoveKeyCallback(flag);
		}
	}

	public void GetKeys(bool userKeys = false)
	{
		GJAPI.Instance.GJDebug("Getting data keys.");
		GJAPI.Instance.Request("data-store/get-keys/", null, userKeys, ReadGetKeysResponse);
	}

	private void ReadGetKeysResponse(string response)
	{
		GJAPI.Instance.GJDebug(response);
		string[] array;
		if (!GJAPI.Instance.IsResponseSuccessful(response))
		{
			GJAPI.Instance.GJDebug("Could not get the data keys.\n" + response, LogType.Error);
			array = null;
		}
		else
		{
			Dictionary<string, string> dictionary = GJAPI.Instance.ResponseToDictionary(response, true);
			GJAPI.Instance.CleanDictionary(ref dictionary, new string[1] { "success0" });
			array = new string[dictionary.Count];
			dictionary.Values.CopyTo(array, 0);
			GJAPI.Instance.GJDebug("Keys successfully fetched.\n" + string.Join("\n", array));
		}
		if (GetKeysCallback != null)
		{
			GetKeysCallback(array);
		}
	}
}

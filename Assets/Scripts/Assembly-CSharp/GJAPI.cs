using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class GJAPI : MonoBehaviour
{
	private const string PROTOCOL = "http://";

	private const string API_ROOT = "gamejolt.com/api/game/";

	private static GJAPI instance;

	private int gameId;

	private string privateKey = string.Empty;

	private bool verbose = true;

	private int version;

	private float timeout = 5f;

	private GJUser user;

	private GJUsersMethods users;

	private GJSessionsMethods sessions;

	private GJTrophiesMethods trophies;

	private GJScoresMethods scores;

	private GJDataMehods data;

	public static GJAPI Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("_GameJoltAPI").AddComponent<GJAPI>();
				UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}
	}

	public static int GameID
	{
		get
		{
			return Instance.gameId;
		}
	}

	public static string PrivateKey
	{
		get
		{
			return Instance.privateKey;
		}
	}

	public static bool Verbose
	{
		get
		{
			return Instance.verbose;
		}
		set
		{
			Instance.verbose = value;
		}
	}

	public static int Version
	{
		get
		{
			return Instance.version;
		}
		set
		{
			Instance.version = value;
		}
	}

	public static float Timeout
	{
		get
		{
			return Instance.timeout;
		}
		set
		{
			Instance.timeout = value;
		}
	}

	public static GJUser User
	{
		get
		{
			return Instance.user;
		}
		set
		{
			Instance.user = value;
		}
	}

	public static GJUsersMethods Users
	{
		get
		{
			if (Instance.users == null)
			{
				Instance.users = new GJUsersMethods();
			}
			return Instance.users;
		}
	}

	public static GJSessionsMethods Sessions
	{
		get
		{
			if (Instance.sessions == null)
			{
				Instance.sessions = new GJSessionsMethods();
			}
			return Instance.sessions;
		}
	}

	public static GJTrophiesMethods Trophies
	{
		get
		{
			if (Instance.trophies == null)
			{
				Instance.trophies = new GJTrophiesMethods();
			}
			return Instance.trophies;
		}
	}

	public static GJScoresMethods Scores
	{
		get
		{
			if (Instance.scores == null)
			{
				Instance.scores = new GJScoresMethods();
			}
			return Instance.scores;
		}
	}

	public static GJDataMehods Data
	{
		get
		{
			if (Instance.data == null)
			{
				Instance.data = new GJDataMehods();
			}
			return Instance.data;
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		user = null;
		users = null;
		sessions = null;
		trophies = null;
		scores = null;
		data = null;
		instance = null;
		Debug.Log("GJAPI: Quit");
	}

	public static void Init(int gameId, string privateKey, bool verbose = true, int version = 1)
	{
		Instance.gameId = gameId;
		Instance.privateKey = privateKey.Trim();
		Instance.verbose = verbose;
		Instance.version = version;
		Instance.user = null;
		Instance.GJDebug("Initialisation complete.\n" + Instance.ToString());
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(" [GJAPI]");
		stringBuilder.AppendFormat("Game ID: {0}\n", Instance.gameId.ToString());
		stringBuilder.Append("Private Key: [FILTERED]\n");
		stringBuilder.AppendFormat("Verbose: {0}\n", Instance.verbose.ToString());
		stringBuilder.AppendFormat("Version: {0}\n", Instance.version.ToString());
		return stringBuilder.ToString();
	}

	public void Request(string method, Dictionary<string, string> parameters, bool requireVerified = false, Action<string> OnResponseComplete = null)
	{
		if (gameId == 0 || privateKey == string.Empty || version == 0)
		{
			GJDebug("Please initialise GameJolt API first.", LogType.Error);
			if (OnResponseComplete != null)
			{
				OnResponseComplete("Error:\nAPI needs to be initialised first.");
			}
			return;
		}
		if (parameters == null)
		{
			parameters = new Dictionary<string, string>();
		}
		if (requireVerified)
		{
			if (user == null)
			{
				GJDebug("Authentification required for " + method, LogType.Error);
				if (OnResponseComplete != null)
				{
					OnResponseComplete("Error:\nThe method " + method + " requires authentification.");
				}
				return;
			}
			parameters.Add("username", user.Name);
			parameters.Add("user_token", user.Token);
		}
		string requestURL = GetRequestURL(method, parameters);
		StartCoroutine(OpenURLAndGetResponse(requestURL, OnResponseComplete));
	}

	public void Request(string method, Dictionary<string, string> parameters, Dictionary<string, string> postParameters, bool requireVerified = false, Action<string> OnResponseComplete = null)
	{
		if (gameId == 0 || privateKey == string.Empty || version == 0)
		{
			GJDebug("Please initialise GameJolt API first.", LogType.Error);
			if (OnResponseComplete != null)
			{
				OnResponseComplete("Error:\nAPI needs to be initialised first.");
			}
			return;
		}
		if (parameters == null)
		{
			parameters = new Dictionary<string, string>();
		}
		if (requireVerified)
		{
			if (user == null)
			{
				GJDebug("Authentification required for " + method, LogType.Error);
				if (OnResponseComplete != null)
				{
					OnResponseComplete("Error:\nThe method " + method + " requires authentification.");
				}
				return;
			}
			parameters.Add("username", user.Name);
			parameters.Add("user_token", user.Token);
		}
		string requestURL = GetRequestURL(method, parameters);
		StartCoroutine(OpenURLAndGetResponse(requestURL, postParameters, OnResponseComplete));
	}

	private string GetRequestURL(string method, Dictionary<string, string> parameters)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("http://");
		stringBuilder.Append("gamejolt.com/api/game/");
		stringBuilder.Append("v");
		stringBuilder.Append(version);
		stringBuilder.Append("/");
		stringBuilder.Append(method);
		stringBuilder.Append("?game_id=");
		stringBuilder.Append(gameId);
		foreach (KeyValuePair<string, string> parameter in parameters)
		{
			stringBuilder.Append("&");
			stringBuilder.Append(parameter.Key);
			stringBuilder.Append("=");
			stringBuilder.Append(parameter.Value.Replace(" ", "%20"));
		}
		string signature = GetSignature(stringBuilder.ToString());
		stringBuilder.Append("&signature=");
		stringBuilder.Append(signature);
		return stringBuilder.ToString();
	}

	private string GetSignature(string input)
	{
		string text = MD5(input + privateKey);
		if (text.Length != 32)
		{
			text += new string('0', 32 - text.Length);
		}
		return text;
	}

	private string MD5(string input)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < bytes.Length; i++)
		{
			text += bytes[i].ToString("x2").ToLower();
		}
		return text;
	}

	private IEnumerator OpenURLAndGetResponse(string url, Action<string> OnResponseComplete = null)
	{
		GJDebug("Opening URL: " + url);
		WWW www = new WWW(url);
		float callTimeout = Time.time + timeout;
		string msg = null;
		while (!www.isDone)
		{
			if (Time.time > callTimeout)
			{
				GJDebug("Timeout opening URL:\n" + url, LogType.Error);
				msg = "Timeout";
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		if (www.error != null)
		{
			GJDebug("Error opening URL:\n" + www.error, LogType.Error);
			msg = www.error;
		}
		if (OnResponseComplete != null)
		{
			OnResponseComplete(msg ?? www.text);
		}
	}

	private IEnumerator OpenURLAndGetResponse(string url, Dictionary<string, string> postParameters, Action<string> OnResponseComplete = null)
	{
		StringBuilder debugMsg = new StringBuilder();
		debugMsg.AppendFormat("Opening URL with post parameters: {0}\n", url);
		if (postParameters == null || postParameters.Count == 0)
		{
			GJDebug("Post parameters is null. Can't make the request.", LogType.Error);
			yield break;
		}
		WWWForm form = new WWWForm();
		foreach (KeyValuePair<string, string> postParameter in postParameters)
		{
			debugMsg.AppendFormat("Post parameter: {0}: {1}\n", postParameter.Key, postParameter.Value);
			form.AddField(postParameter.Key, postParameter.Value);
		}
		GJDebug(debugMsg.ToString());
		WWW www = new WWW(url, form);
		float callTimeout = Time.time + 5f;
		string msg = null;
		while (!www.isDone)
		{
			if (Time.time > callTimeout)
			{
				GJDebug("Timeout opening URL:\n" + url, LogType.Error);
				msg = "Timeout";
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		if (www.error != null)
		{
			GJDebug("Error opening URL:\n" + www.error, LogType.Error);
			msg = www.error;
		}
		if (OnResponseComplete != null)
		{
			OnResponseComplete(msg ?? www.text);
		}
	}

	public bool IsResponseSuccessful(string response)
	{
		string[] array = response.Split('\n');
		return array[0].Trim().Equals("success:\"true\"");
	}

	public bool IsDumpResponseSuccessful(ref string response)
	{
		int num = response.IndexOf('\n');
		if (num == -1)
		{
			GJDebug("Wrong response format. Can't read response.", LogType.Error);
			return false;
		}
		string text = response.Substring(0, num).Trim();
		return text == "SUCCESS";
	}

	public Dictionary<string, string> ResponseToDictionary(string response, bool addIndexToKey = false)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int num = 0;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string[] array = response.Split('\n');
		int num2 = array.Length;
		for (int i = 0; i < num2; i++)
		{
			if (!(array[i] != string.Empty))
			{
				continue;
			}
			num = array[i].IndexOf(':');
			if (num == -1)
			{
				GJDebug("Wrong line format. The following line of the response will be skipped: " + array[i], LogType.Warning);
				continue;
			}
			empty = array[i].Substring(0, num);
			empty2 = array[i].Substring(num + 1);
			empty2 = empty2.Trim().Trim('"');
			if (addIndexToKey)
			{
				dictionary.Add(empty + i, empty2);
			}
			else
			{
				dictionary.Add(empty, empty2);
			}
		}
		return dictionary;
	}

	public Dictionary<string, string>[] ResponseToDictionaries(string response, bool addIndexToKey = false)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string text = string.Empty;
		string[] array = response.Split('\n');
		int num4 = array.Length;
		for (int i = 0; i < num4; i++)
		{
			if (!(array[i] != string.Empty))
			{
				continue;
			}
			num = array[i].IndexOf(':');
			if (num == -1)
			{
				continue;
			}
			empty = array[i].Substring(0, num);
			if (empty != "success" && empty != "message")
			{
				if (text == string.Empty)
				{
					text = empty;
					num2++;
				}
				else if (text == empty)
				{
					num2++;
				}
			}
		}
		text = string.Empty;
		Dictionary<string, string>[] array2 = new Dictionary<string, string>[num2];
		for (int j = 0; j < num2; j++)
		{
			array2[j] = new Dictionary<string, string>();
		}
		for (int k = 0; k < num4; k++)
		{
			if (!(array[k] != string.Empty))
			{
				continue;
			}
			num = array[k].IndexOf(':');
			if (num == 1)
			{
				GJDebug("Wrong line format. The following line of the response will be skipped: " + array[k], LogType.Warning);
				continue;
			}
			empty = array[k].Substring(0, num);
			empty2 = array[k].Substring(num + 1);
			empty2 = empty2.Trim().Trim('"');
			if (empty != "success" && empty != "message")
			{
				if (text == string.Empty)
				{
					text = empty;
				}
				else if (text == empty)
				{
					num3++;
				}
			}
			if (addIndexToKey)
			{
				array2[num3].Add(empty + k, empty2);
			}
			else
			{
				array2[num3].Add(empty, empty2);
			}
		}
		return array2;
	}

	public void CleanDictionary(ref Dictionary<string, string> dictionary, string[] keysToClean = null)
	{
		if (keysToClean == null)
		{
			keysToClean = new string[2] { "success", "message" };
		}
		int num = keysToClean.Length;
		for (int i = 0; i < num; i++)
		{
			if (dictionary.ContainsKey(keysToClean[i]))
			{
				dictionary.Remove(keysToClean[i]);
			}
		}
	}

	public void CleanDictionaries(ref Dictionary<string, string>[] dictionaries, string[] keysToClean = null)
	{
		int num = dictionaries.Length;
		for (int i = 0; i < num; i++)
		{
			CleanDictionary(ref dictionaries[i], keysToClean);
		}
	}

	public void DumpResponseToString(ref string response, out string data)
	{
		int num = response.IndexOf('\n');
		if (num == -1)
		{
			GJDebug("Wrong response format. Can't read response.", LogType.Error);
			data = string.Empty;
		}
		else
		{
			data = response.Substring(num + 1);
		}
	}

	public void GJDebug(string message, LogType type = LogType.Log)
	{
		if (verbose)
		{
			switch (type)
			{
			default:
				Debug.Log("GJAPI: " + message);
				break;
			case LogType.Warning:
				Debug.LogWarning("GJAPI: " + message);
				break;
			case LogType.Error:
				Debug.LogError("GJAPI: " + message);
				break;
			}
		}
	}
}

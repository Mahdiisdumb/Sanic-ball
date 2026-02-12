using System;
using System.Collections;
using UnityEngine;

public class GJAPIHelper : MonoBehaviour
{
	private static GJAPIHelper instance;

	protected GUISkin skin;

	private GJHUsersMethods users;

	private GJHScoresMethods scores;

	private GJHTrophiesMethods trophies;

	public static GJAPIHelper Instance
	{
		get
		{
			if (instance == null)
			{
				GJAPI gJAPI = (GJAPI)UnityEngine.Object.FindObjectOfType(typeof(GJAPI));
				if (gJAPI == null)
				{
					Debug.LogError("An instance of GJAPI is needed in the scene, but there is none. Can't initialise GJAPIHelper.");
				}
				else
				{
					instance = gJAPI.gameObject.AddComponent<GJAPIHelper>();
					if (instance == null)
					{
						Debug.Log("An error occured creating GJAPIHelper.");
					}
				}
			}
			return instance;
		}
	}

	public static GUISkin Skin
	{
		get
		{
			if (Instance.skin == null)
			{
				Instance.skin = ((GUISkin)Resources.Load("GJSkin", typeof(GUISkin))) ?? GUI.skin;
			}
			return Instance.skin;
		}
		set
		{
			Instance.skin = value;
		}
	}

	public static GJHUsersMethods Users
	{
		get
		{
			if (Instance.users == null)
			{
				Instance.users = new GJHUsersMethods();
			}
			return Instance.users;
		}
	}

	public static GJHScoresMethods Scores
	{
		get
		{
			if (Instance.scores == null)
			{
				Instance.scores = new GJHScoresMethods();
			}
			return Instance.scores;
		}
	}

	public static GJHTrophiesMethods Trophies
	{
		get
		{
			if (Instance.trophies == null)
			{
				Instance.trophies = new GJHTrophiesMethods();
			}
			return Instance.trophies;
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		skin = null;
		users = null;
		scores = null;
		trophies = null;
		instance = null;
	}

	public static void DownloadImage(string url, Action<Texture2D> OnComplete)
	{
		Instance.StartCoroutine(Instance.DownloadImageCoroutine(url, OnComplete));
	}

	private IEnumerator DownloadImageCoroutine(string url, Action<Texture2D> OnComplete)
	{
		if (!string.IsNullOrEmpty(url))
		{
			WWW www = new WWW(url);
			yield return www;
			Texture2D tex;
			if (www.error == null)
			{
				tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
				tex.LoadImage(www.bytes);
				tex.wrapMode = TextureWrapMode.Clamp;
			}
			else
			{
				Debug.Log("GJAPIHelper: Error downloading image:\n" + www.error);
				tex = null;
			}
			if (OnComplete != null)
			{
				OnComplete(tex);
			}
		}
	}

	public void OnGetUserFromWeb(string response)
	{
		users.ReadGetFromWebResponse(response);
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class GJScore : GJObject
{
	public string Score
	{
		get
		{
			return (!properties.ContainsKey("score")) ? string.Empty : properties["score"];
		}
		set
		{
			properties["score"] = value;
		}
	}

	public uint Sort
	{
		get
		{
			if (properties.ContainsKey("sort"))
			{
				if (properties["sort"] == string.Empty)
				{
					Debug.Log("Sort is empty. Returning 0.");
					return 0u;
				}
				try
				{
					return Convert.ToUInt32(properties["sort"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting Score Sort to uint. Returning 0. " + ex.Message);
					return 0u;
				}
			}
			return 0u;
		}
		set
		{
			properties["sort"] = value.ToString();
		}
	}

	public string ExtraData
	{
		get
		{
			return (!properties.ContainsKey("extra_data")) ? string.Empty : properties["extra_data"];
		}
		set
		{
			properties["extra_data"] = value;
		}
	}

	public string Username
	{
		get
		{
			return (!properties.ContainsKey("user")) ? string.Empty : properties["user"];
		}
		set
		{
			properties["user"] = value;
		}
	}

	public uint UserID
	{
		get
		{
			if (properties.ContainsKey("user_id"))
			{
				if (properties["user_id"] == string.Empty)
				{
					Debug.Log("User ID is empty. Returning 0.");
					return 0u;
				}
				try
				{
					return Convert.ToUInt32(properties["user_id"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting User ID to uint. Returning 0. " + ex.Message);
					return 0u;
				}
			}
			return 0u;
		}
		set
		{
			properties["user_id"] = value.ToString();
		}
	}

	public string Guestname
	{
		get
		{
			return (!properties.ContainsKey("guest")) ? string.Empty : properties["guest"];
		}
		set
		{
			properties["guest"] = value;
		}
	}

	public string Name
	{
		get
		{
			return (!isUserScore) ? Guestname : Username;
		}
	}

	public string Stored
	{
		get
		{
			return (!properties.ContainsKey("stored")) ? string.Empty : properties["stored"];
		}
		set
		{
			properties["stored"] = value;
		}
	}

	public bool isUserScore
	{
		get
		{
			return properties.ContainsKey("user") && properties["user"] != string.Empty;
		}
	}

	public GJScore()
	{
	}

	public GJScore(string score, uint sort)
	{
		AddProperty("score", score);
		AddProperty("sort", sort.ToString());
	}

	public GJScore(Dictionary<string, string> properties)
	{
		AddProperties(properties);
	}
}

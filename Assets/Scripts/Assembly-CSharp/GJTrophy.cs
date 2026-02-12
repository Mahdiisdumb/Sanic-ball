using System;
using System.Collections.Generic;
using UnityEngine;

public class GJTrophy : GJObject
{
	public enum TrophyDifficulty
	{
		Undefined = 0,
		Bronze = 1,
		Silver = 2,
		Gold = 3,
		Platinium = 4
	}

	public uint Id
	{
		get
		{
			if (properties.ContainsKey("id"))
			{
				if (properties["id"] == string.Empty)
				{
					Debug.Log("Trophy ID is empty. Returning 0.");
					return 0u;
				}
				try
				{
					return Convert.ToUInt32(properties["id"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting Trophy ID to uint. Returning 0. " + ex.Message);
					return 0u;
				}
			}
			return 0u;
		}
		set
		{
			properties["id"] = value.ToString();
		}
	}

	public string Title
	{
		get
		{
			return (!properties.ContainsKey("title")) ? string.Empty : properties["title"];
		}
		set
		{
			properties["title"] = value;
		}
	}

	public string Description
	{
		get
		{
			return (!properties.ContainsKey("description")) ? string.Empty : properties["description"];
		}
		set
		{
			properties["description"] = value;
		}
	}

	public TrophyDifficulty Difficulty
	{
		get
		{
			if (properties.ContainsKey("difficulty"))
			{
				try
				{
					return (TrophyDifficulty)(int)Enum.Parse(typeof(TrophyDifficulty), properties["difficulty"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting Trophy Difficulty to TrophyDifficulty. Returning Undefined. " + ex.Message);
					return TrophyDifficulty.Undefined;
				}
			}
			return TrophyDifficulty.Undefined;
		}
		set
		{
			properties["difficulty"] = value.ToString();
		}
	}

	public bool Achieved
	{
		get
		{
			if (properties.ContainsKey("achieved") && !(properties["achieved"] == "false"))
			{
				return true;
			}
			return false;
		}
		set
		{
			properties["achieved"] = value.ToString();
		}
	}

	public string AchievedTime
	{
		get
		{
			if (properties.ContainsKey("achieved") && !(properties["achieved"] == "false"))
			{
				return properties["achieved"];
			}
			return "NA";
		}
		set
		{
			properties["achieved"] = value;
		}
	}

	public string ImageURL
	{
		get
		{
			return (!properties.ContainsKey("image_url")) ? string.Empty : properties["image_url"];
		}
		set
		{
			properties["image_url"] = value.ToString();
		}
	}

	public GJTrophy()
	{
	}

	public GJTrophy(uint id, string title, TrophyDifficulty difficulty, bool achieved, string description = "", string imageURL = "")
	{
		AddProperty("id", id.ToString());
		AddProperty("title", title);
		AddProperty("difficulty", difficulty.ToString());
		AddProperty("achieved", achieved.ToString());
		AddProperty("description", description);
		AddProperty("image_url", imageURL);
	}

	public GJTrophy(Dictionary<string, string> properties)
	{
		AddProperties(properties);
	}
}

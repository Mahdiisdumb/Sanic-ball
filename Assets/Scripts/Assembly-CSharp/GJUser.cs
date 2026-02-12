using System;
using System.Collections.Generic;
using UnityEngine;

public class GJUser : GJObject
{
	public enum UserType
	{
		Undefined = 0,
		User = 1,
		Developer = 2,
		Moderator = 3,
		Admin = 4
	}

	public enum UserStatus
	{
		Undefined = 0,
		Active = 1,
		Banned = 2
	}

	public string Name
	{
		get
		{
			return (!properties.ContainsKey("username")) ? string.Empty : properties["username"];
		}
		set
		{
			properties["username"] = value;
		}
	}

	public string Token
	{
		get
		{
			return (!properties.ContainsKey("user_token")) ? string.Empty : properties["user_token"];
		}
		set
		{
			properties["user_token"] = value;
		}
	}

	public uint Id
	{
		get
		{
			if (properties.ContainsKey("id"))
			{
				if (properties["id"] == string.Empty)
				{
					Debug.Log("User ID is empty. Returning 0.");
					return 0u;
				}
				try
				{
					return Convert.ToUInt32(properties["id"]);
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
			properties["id"] = value.ToString();
		}
	}

	public UserType Type
	{
		get
		{
			if (properties.ContainsKey("type"))
			{
				try
				{
					return (UserType)(int)Enum.Parse(typeof(UserType), properties["type"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting User Type to UserType. Value will be Undefined. " + ex.Message);
					return UserType.Undefined;
				}
			}
			return UserType.Undefined;
		}
		set
		{
			properties["type"] = value.ToString();
		}
	}

	public UserStatus Status
	{
		get
		{
			if (properties.ContainsKey("status"))
			{
				try
				{
					return (UserStatus)(int)Enum.Parse(typeof(UserStatus), properties["status"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting User Status to UserStatus. Value will be Undefined. " + ex.Message);
					return UserStatus.Undefined;
				}
			}
			return UserStatus.Undefined;
		}
		set
		{
			properties["status"] = value.ToString();
		}
	}

	public string AvatarURL
	{
		get
		{
			return (!properties.ContainsKey("avatar_url")) ? string.Empty : properties["avatar_url"];
		}
		set
		{
			properties["avatar_url"] = value;
		}
	}

	public GJUser()
	{
	}

	public GJUser(string username, string user_token)
	{
		AddProperty("username", username);
		AddProperty("user_token", user_token);
	}

	public GJUser(Dictionary<string, string> properties)
	{
		AddProperties(properties);
	}
}

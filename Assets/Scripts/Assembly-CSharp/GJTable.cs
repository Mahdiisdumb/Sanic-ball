using System;
using System.Collections.Generic;
using UnityEngine;

public class GJTable : GJObject
{
	public uint Id
	{
		get
		{
			if (properties.ContainsKey("id"))
			{
				if (properties["id"] == string.Empty)
				{
					Debug.Log("Table ID is empty. Returning 0.");
					return 0u;
				}
				try
				{
					return Convert.ToUInt32(properties["id"]);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error converting Table ID to uint. Returning 0. " + ex.Message);
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

	public string Name
	{
		get
		{
			return (!properties.ContainsKey("name")) ? string.Empty : properties["name"];
		}
		set
		{
			properties["name"] = value;
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

	public bool Primary
	{
		get
		{
			if (properties.ContainsKey("primary"))
			{
				return properties["primary"] == "1";
			}
			return false;
		}
		set
		{
			properties["primary"] = ((!value) ? "0" : "1");
		}
	}

	public GJTable()
	{
	}

	public GJTable(uint id, string name, bool primary, string description = "")
	{
		AddProperty("id", id.ToString());
		AddProperty("name", name);
		AddProperty("primary", primary.ToString());
		AddProperty("description", description);
	}

	public GJTable(Dictionary<string, string> properties)
	{
		AddProperties(properties);
	}
}

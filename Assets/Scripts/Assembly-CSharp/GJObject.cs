using System.Collections.Generic;
using System.Text;

public abstract class GJObject
{
	protected Dictionary<string, string> properties;

	public GJObject()
	{
		properties = new Dictionary<string, string>();
	}

	~GJObject()
	{
		properties = null;
	}

	public void AddProperty(string key, string value, bool overwrite = false)
	{
		if (!properties.ContainsKey(key) || overwrite)
		{
			properties[key] = value;
		}
	}

	public void AddProperties(Dictionary<string, string> properties, bool overwrite = false)
	{
		foreach (KeyValuePair<string, string> property in properties)
		{
			AddProperty(property.Key, property.Value, overwrite);
		}
	}

	public string GetProperty(string key)
	{
		return (!properties.ContainsKey(key)) ? string.Empty : properties[key];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(" [{0}]\n", GetType().ToString());
		foreach (KeyValuePair<string, string> property in properties)
		{
			stringBuilder.AppendFormat("{0}: {1}\n", property.Key, property.Value);
		}
		return stringBuilder.ToString();
	}
}

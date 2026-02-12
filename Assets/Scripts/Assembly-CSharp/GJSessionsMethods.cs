using System.Collections.Generic;
using UnityEngine;

public class GJSessionsMethods
{
	public delegate void _OpenCallback(bool success);

	public delegate void _PingCallback(bool success);

	public delegate void _CloseCallback(bool success);

	private const string SESSIONS_OPEN = "sessions/open/";

	private const string SESSIONS_PING = "sessions/ping/";

	private const string SESSIONS_CLOSE = "sessions/close/";

	public _OpenCallback OpenCallback;

	public _PingCallback PingCallback;

	public _CloseCallback CloseCallback;

	~GJSessionsMethods()
	{
		OpenCallback = null;
		PingCallback = null;
		CloseCallback = null;
	}

	public void Open()
	{
		GJAPI.Instance.GJDebug("Openning Session.");
		GJAPI.Instance.Request("sessions/open/", null, true, ReadOpenResponse);
	}

	private void ReadOpenResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not open the session.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Session successfully opened.");
		}
		if (OpenCallback != null)
		{
			OpenCallback(flag);
		}
	}

	public void Ping(bool active = true)
	{
		GJAPI.Instance.GJDebug("Pinging Session.");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string value = ((!active) ? "idle" : "active");
		dictionary.Add("status", value);
		GJAPI.Instance.Request("sessions/ping/", dictionary, true, ReadPingResponse);
	}

	private void ReadPingResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not ping the session.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Session successfully pinged.");
		}
		if (PingCallback != null)
		{
			PingCallback(flag);
		}
	}

	public void Close()
	{
		GJAPI.Instance.GJDebug("Closing Session.");
		GJAPI.Instance.Request("sessions/close/", null, true, ReadCloseResponse);
	}

	private void ReadCloseResponse(string response)
	{
		bool flag = GJAPI.Instance.IsResponseSuccessful(response);
		if (!flag)
		{
			GJAPI.Instance.GJDebug("Could not close the session.\n" + response, LogType.Error);
		}
		else
		{
			GJAPI.Instance.GJDebug("Session successfully closed.");
		}
		if (CloseCallback != null)
		{
			CloseCallback(flag);
		}
	}
}

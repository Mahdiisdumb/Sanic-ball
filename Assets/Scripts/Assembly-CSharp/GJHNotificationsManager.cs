using System.Collections.Generic;
using UnityEngine;

public class GJHNotificationsManager : MonoBehaviour
{
	private static GJHNotificationsManager instance;

	private Queue<GJHNotification> queue = new Queue<GJHNotification>();

	private GJHNotification currentNotification;

	private float currentNotificationAppearTime;

	public static GJHNotificationsManager Instance
	{
		get
		{
			if (instance == null)
			{
				GJAPIHelper gJAPIHelper = (GJAPIHelper)Object.FindObjectOfType(typeof(GJAPIHelper));
				if (gJAPIHelper == null)
				{
					Debug.LogError("An instance of GJAPIHelper is needed in the scene, but there is none. Can't initialise GJHNotificationsManager.");
				}
				else
				{
					instance = gJAPIHelper.gameObject.AddComponent<GJHNotificationsManager>();
					if (instance == null)
					{
						Debug.Log("An error occured creating GJHNotificationsManager.");
					}
				}
			}
			return instance;
		}
	}

	private void OnDestroy()
	{
		queue = null;
		currentNotification = null;
		instance = null;
	}

	public static void QueueNotification(GJHNotification notification)
	{
		Instance.queue.Enqueue(notification);
	}

	private void OnGUI()
	{
		if (currentNotification != null)
		{
			if (Time.time > currentNotificationAppearTime + currentNotification.DisplayTime)
			{
				currentNotification = null;
				return;
			}
			if (GJAPIHelper.Skin != null)
			{
				GUI.skin = GJAPIHelper.Skin;
			}
			currentNotification.OnGUI();
		}
		else if (queue.Count > 0)
		{
			currentNotification = queue.Dequeue();
			currentNotificationAppearTime = Time.time;
		}
	}
}

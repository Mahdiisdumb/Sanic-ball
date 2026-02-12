using UnityEngine;

public class GJHNotification
{
	private GJHNotificationAnchor anchor = GJHNotificationAnchor.TopCenter;

	private GJHNotificationType type;

	private float displayTime = 5f;

	private Rect position;

	public string Title = string.Empty;

	public string Description = string.Empty;

	public Texture2D Icon;

	private GUIStyle notificationBgStyle;

	private GUIStyle notificationTitleStyle;

	private GUIStyle notificationDescriptionStyle;

	private GUIStyle smallNotificationTitleStyle;

	public GJHNotificationAnchor Anchor
	{
		get
		{
			return anchor;
		}
		set
		{
			anchor = value;
			SetPosition();
		}
	}

	public GJHNotificationType Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
			SetPosition();
		}
	}

	public float DisplayTime
	{
		get
		{
			return displayTime;
		}
		set
		{
			displayTime = ((!(value >= 1f)) ? 1f : value);
		}
	}

	public GJHNotification(string title, string description = "", Texture2D icon = null)
	{
		Title = title;
		if (!string.IsNullOrEmpty(description))
		{
			Description = description;
			Icon = icon;
			type = GJHNotificationType.WithIcon;
		}
		else
		{
			type = GJHNotificationType.Simple;
		}
		SetPosition();
		notificationBgStyle = GJAPIHelper.Skin.FindStyle("NotificationBg") ?? GJAPIHelper.Skin.label;
		notificationTitleStyle = GJAPIHelper.Skin.FindStyle("NotificationTitle") ?? GJAPIHelper.Skin.label;
		notificationDescriptionStyle = GJAPIHelper.Skin.FindStyle("NotificationDescription") ?? GJAPIHelper.Skin.label;
		smallNotificationTitleStyle = GJAPIHelper.Skin.FindStyle("SmallNotificationTitle") ?? GJAPIHelper.Skin.label;
	}

	~GJHNotification()
	{
		notificationBgStyle = null;
		notificationTitleStyle = null;
		notificationDescriptionStyle = null;
		smallNotificationTitleStyle = null;
		Icon = null;
	}

	public void OnGUI()
	{
		GJHNotificationType gJHNotificationType = type;
		if (gJHNotificationType == GJHNotificationType.Simple || gJHNotificationType != GJHNotificationType.WithIcon)
		{
			DrawSmallNotification();
		}
		else
		{
			DrawMediumNotification();
		}
	}

	private void DrawSmallNotification()
	{
		GUI.BeginGroup(position, notificationBgStyle);
		GUI.Label(new Rect(0f, 0f, position.width, position.height), Title, smallNotificationTitleStyle);
		GUI.EndGroup();
	}

	private void DrawMediumNotification()
	{
		GUI.BeginGroup(position, notificationBgStyle);
		GUI.DrawTexture(new Rect(10f, 10f, 75f, 75f), Icon);
		GUI.Label(new Rect(100f, 10f, 290f, 20f), Title, notificationTitleStyle);
		GUI.Label(new Rect(100f, 40f, 290f, 45f), Description, notificationDescriptionStyle);
		GUI.EndGroup();
	}

	private void SetPosition()
	{
		GJHNotificationType gJHNotificationType = Type;
		if (gJHNotificationType == GJHNotificationType.Simple || gJHNotificationType != GJHNotificationType.WithIcon)
		{
			position = new Rect(0f, 0f, 250f, 25f);
		}
		else
		{
			position = new Rect(0f, 0f, 400f, 95f);
		}
		switch (Anchor)
		{
		default:
			position.x = 10f;
			position.y = 10f;
			break;
		case GJHNotificationAnchor.TopCenter:
			position.x = (float)(Screen.width / 2) - position.width / 2f;
			position.y = 10f;
			break;
		case GJHNotificationAnchor.TopRight:
			position.x = (float)Screen.width - 10f - position.width;
			position.y = 10f;
			break;
		case GJHNotificationAnchor.BottomLeft:
			position.x = 10f;
			position.y = (float)Screen.height - 10f - position.height;
			break;
		case GJHNotificationAnchor.BottomCenter:
			position.x = (float)(Screen.width / 2) - position.width / 2f;
			position.y = (float)Screen.height - 10f - position.height;
			break;
		case GJHNotificationAnchor.BottomRight:
			position.x = (float)Screen.width - 10f - position.width;
			position.y = (float)Screen.height - 10f - position.height;
			break;
		}
	}
}

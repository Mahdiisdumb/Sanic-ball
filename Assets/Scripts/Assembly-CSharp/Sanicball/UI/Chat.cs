using System;
using SanicballCore.MatchMessages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sanicball.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class Chat : MonoBehaviour
	{
		private const float MAX_VISIBLE_TIME = 4f;

		private const float FADE_TIME = 0.2f;

		[SerializeField]
		private Text chatMessagePrefab;

		[SerializeField]
		private RectTransform chatMessageContainer;

		[SerializeField]
		private InputField messageInputField;

		[SerializeField]
		private RectTransform hoverArea;

		private GameObject prevSelectedObject;

		private bool shouldEnableInput;

		private CanvasGroup canvasGroup;

		private float visibleTime;

		public event EventHandler<ChatMessageArgs> MessageSent;

		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
		}

		public void Update()
		{
			EventSystem current = EventSystem.current;
			if (GameInput.IsOpeningChat() && current.currentSelectedGameObject != messageInputField.gameObject)
			{
				prevSelectedObject = current.currentSelectedGameObject;
				current.SetSelectedGameObject(messageInputField.gameObject);
			}
			if (current.currentSelectedGameObject == messageInputField.gameObject)
			{
				visibleTime = 4f;
				if (Input.GetKeyDown(KeyCode.Return))
				{
					SendMessage();
				}
			}
			if (Input.mousePosition.x < hoverArea.sizeDelta.x && Input.mousePosition.y < hoverArea.sizeDelta.y)
			{
				visibleTime = 4f;
			}
			if (visibleTime > 0f)
			{
				visibleTime -= Time.deltaTime;
				canvasGroup.alpha = 1f;
			}
			else if (canvasGroup.alpha > 0f)
			{
				canvasGroup.alpha = Mathf.Max(canvasGroup.alpha - Time.deltaTime / 0.2f, 0f);
			}
		}

		public void LateUpdate()
		{
			if (shouldEnableInput)
			{
				GameInput.KeyboardDisabled = false;
				shouldEnableInput = false;
			}
		}

		public void ShowMessage(ChatMessageType type, string from, string text)
		{
			Text text2 = UnityEngine.Object.Instantiate(chatMessagePrefab);
			text2.transform.SetParent(chatMessageContainer, false);
			switch (type)
			{
			case ChatMessageType.User:
				text2.text = string.Format("<color=#6688ff><b>{0}</b></color>: {1}", from, text);
				break;
			case ChatMessageType.System:
				text2.text = string.Format("<color=#ffff77><b>{0}</b></color>", text);
				break;
			}
			visibleTime = 4f;
		}

		public void DisableInput()
		{
			GameInput.KeyboardDisabled = true;
		}

		public void EnableInput()
		{
			shouldEnableInput = true;
		}

		public void SendMessage()
		{
			string text = messageInputField.text;
			if (text.Trim() != string.Empty && this.MessageSent != null)
			{
				this.MessageSent(this, new ChatMessageArgs(text));
			}
			EventSystem.current.SetSelectedGameObject(prevSelectedObject);
			messageInputField.text = string.Empty;
		}
	}
}

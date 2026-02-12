using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class CharacterSelectPanel : MonoBehaviour
	{
		private const int COLUMN_COUNT = 4;

		[SerializeField]
		private RectTransform entryContainer;

		[SerializeField]
		private CharacterSelectEntry entryPrefab;

		[SerializeField]
		private Text characterNameLabel;

		[SerializeField]
		private float scrollSpeed = 1f;

		[SerializeField]
		private float normalIconSize = 64f;

		[SerializeField]
		private float selectedIconSize = 96f;

		private int selected;

		private Sanicball.Data.CharacterInfo selectedChar;

		private float targetX;

		private float targetY;

		private List<CharacterSelectEntry> activeEntries = new List<CharacterSelectEntry>();

		[SerializeField]
		private Sprite cancelIconSprite;

		public event EventHandler<CharacterSelectionArgs> CharacterSelected;

		public event EventHandler CancelSelected;

		private IEnumerator Start()
		{
			Sanicball.Data.CharacterInfo[] charList = ActiveData.Characters.OrderBy((Sanicball.Data.CharacterInfo a) => a.tier).ToArray();
			if (ActiveData.GameSettings.eSportsReady)
			{
				charList = charList.Where((Sanicball.Data.CharacterInfo a) => a.tier == CharacterTier.Hyperspeed).ToArray();
			}
			CharacterSelectEntry cancelEnt = UnityEngine.Object.Instantiate(entryPrefab);
			cancelEnt.IconImage.sprite = cancelIconSprite;
			cancelEnt.transform.SetParent(entryContainer.transform, false);
			activeEntries.Add(cancelEnt);
			for (int i = 0; i < charList.Length; i++)
			{
				if (!charList[i].hidden)
				{
					CharacterSelectEntry characterEnt = UnityEngine.Object.Instantiate(entryPrefab);
					characterEnt.Init(charList[i]);
					characterEnt.transform.SetParent(entryContainer.transform, false);
					activeEntries.Add(characterEnt);
				}
			}
			yield return null;
			Select(1);
		}

		public void Right()
		{
			if (selected < activeEntries.Count - 1)
			{
				Select(selected + 1);
			}
			else
			{
				Select(0);
			}
		}

		public void Left()
		{
			if (selected > 0)
			{
				Select(selected - 1);
			}
			else
			{
				Select(activeEntries.Count - 1);
			}
		}

		public void Up()
		{
			if (activeEntries.Count > 4)
			{
				int num = selected - 4;
				if (num < 0)
				{
					num += activeEntries.Count;
				}
				Select(num);
			}
		}

		public void Down()
		{
			if (activeEntries.Count > 4)
			{
				int num = selected + 4;
				if (num > activeEntries.Count - 1)
				{
					num -= activeEntries.Count;
				}
				Select(num);
			}
		}

		private void Select(int newSelection)
		{
			selected = newSelection;
			selectedChar = activeEntries[selected].Character;
			if (selected == 0)
			{
				characterNameLabel.text = "Leave match";
			}
			else
			{
				characterNameLabel.text = selectedChar.name;
			}
		}

		private void Update()
		{
			targetX = entryContainer.sizeDelta.x / 2f - activeEntries[selected].RectTransform.anchoredPosition.x;
			targetY = (0f - entryContainer.sizeDelta.y) / 2f - activeEntries[selected].RectTransform.anchoredPosition.y;
			if (!Mathf.Approximately(entryContainer.anchoredPosition.x, targetX))
			{
				float x = Mathf.Lerp(entryContainer.anchoredPosition.x, targetX, scrollSpeed * Time.deltaTime);
				entryContainer.anchoredPosition = new Vector2(x, entryContainer.anchoredPosition.y);
			}
			if (!Mathf.Approximately(entryContainer.anchoredPosition.y, targetY))
			{
				float y = Mathf.Lerp(entryContainer.anchoredPosition.y, targetY, scrollSpeed * Time.deltaTime);
				entryContainer.anchoredPosition = new Vector2(entryContainer.anchoredPosition.x, y);
			}
			for (int i = 0; i < activeEntries.Count; i++)
			{
				CharacterSelectEntry characterSelectEntry = activeEntries[i];
				float b = ((i != selected) ? normalIconSize : selectedIconSize);
				if (!Mathf.Approximately(characterSelectEntry.Size, b))
				{
					characterSelectEntry.Size = Mathf.Lerp(characterSelectEntry.Size, b, scrollSpeed * Time.deltaTime);
				}
			}
		}

		public void Accept()
		{
			if (selected == 0)
			{
				if (this.CancelSelected != null)
				{
					this.CancelSelected(this, EventArgs.Empty);
				}
			}
			else if (this.CharacterSelected != null)
			{
				this.CharacterSelected(this, new CharacterSelectionArgs(Array.IndexOf(ActiveData.Characters, selectedChar)));
			}
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}
	}
}

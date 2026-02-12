using System;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class LocalPlayerPanel : MonoBehaviour
	{
		[NonSerialized]
		public LocalPlayerManager playerManager;

		public ImageColorToggle readyIndicator;

		[SerializeField]
		private Image ctrlTypeImageField;

		[SerializeField]
		private Text helpTextField;

		[SerializeField]
		private Sprite[] controlTypeIcons;

		[SerializeField]
		private CharacterSelectPanel characterSelectSubpanel;

		private bool uiPressed;

		public ControlType AssignedCtrlType { get; set; }

		public MatchPlayer AssignedPlayer { get; set; }

		private void Start()
		{
			characterSelectSubpanel.CharacterSelected += CharacterSelectSubpanel_CharacterSelected;
			characterSelectSubpanel.CancelSelected += CharacterSelectSubpanel_Cancelled;
			playerManager.LocalPlayerJoined += PlayerManager_LocalPlayerJoined;
			ctrlTypeImageField.sprite = controlTypeIcons[(int)AssignedCtrlType];
			ShowCharacterSelectHelp();
		}

		private void Update()
		{
			if (PauseMenu.GamePaused)
			{
				return;
			}
			bool flag = GameInput.IsOpeningMenu(AssignedCtrlType);
			bool flag2 = GameInput.UILeft(AssignedCtrlType);
			bool flag3 = GameInput.UIRight(AssignedCtrlType);
			bool flag4 = GameInput.UIUp(AssignedCtrlType);
			bool flag5 = GameInput.UIDown(AssignedCtrlType);
			bool activeSelf = characterSelectSubpanel.gameObject.activeSelf;
			if (GameInput.IsRespawning(AssignedCtrlType) && !activeSelf)
			{
				ToggleReady();
			}
			if (flag || flag2 || flag3 || flag4 || flag5)
			{
				if (uiPressed)
				{
					return;
				}
				if (flag)
				{
					if (activeSelf)
					{
						characterSelectSubpanel.Accept();
						ShowMainHelp();
					}
					else
					{
						characterSelectSubpanel.gameObject.SetActive(true);
						ShowCharacterSelectHelp();
					}
				}
				if (flag2 && activeSelf)
				{
					characterSelectSubpanel.Left();
				}
				if (flag3 && activeSelf)
				{
					characterSelectSubpanel.Right();
				}
				if (flag4 && activeSelf)
				{
					characterSelectSubpanel.Up();
				}
				if (flag5 && activeSelf)
				{
					characterSelectSubpanel.Down();
				}
				uiPressed = true;
			}
			else
			{
				uiPressed = false;
			}
		}

		public void LeaveMatch()
		{
			if (AssignedPlayer != null)
			{
				playerManager.LeaveMatch(AssignedPlayer);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void ToggleReady()
		{
			if (AssignedPlayer != null)
			{
				readyIndicator.On = !AssignedPlayer.ReadyToRace;
				playerManager.SetReady(AssignedPlayer, !AssignedPlayer.ReadyToRace);
			}
		}

		public void SetCharacter(int c)
		{
			if (AssignedPlayer == null)
			{
				playerManager.CreatePlayerForControlType(AssignedCtrlType, c);
			}
			else
			{
				playerManager.SetCharacter(AssignedPlayer, c);
			}
			if (characterSelectSubpanel.gameObject.activeSelf)
			{
				characterSelectSubpanel.gameObject.SetActive(false);
			}
		}

		private void PlayerManager_LocalPlayerJoined(object sender, MatchPlayerEventArgs e)
		{
			if (e.Player.CtrlType == AssignedCtrlType)
			{
				AssignedPlayer = e.Player;
			}
		}

		private void CharacterSelectSubpanel_CharacterSelected(object sender, CharacterSelectionArgs e)
		{
			SetCharacter(e.SelectedCharacter);
			characterSelectSubpanel.gameObject.SetActive(false);
		}

		private void CharacterSelectSubpanel_Cancelled(object sender, EventArgs e)
		{
			if (AssignedPlayer == null)
			{
				playerManager.RemoveControlType(AssignedCtrlType);
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				LeaveMatch();
			}
		}

		private void ShowMainHelp()
		{
			string keyCodeName = GameInput.GetKeyCodeName(ActiveData.Keybinds[Keybind.Menu]);
			string keyCodeName2 = GameInput.GetKeyCodeName(ActiveData.Keybinds[Keybind.Respawn]);
			string arg = ((AssignedCtrlType != ControlType.Keyboard) ? "X" : keyCodeName);
			string arg2 = ((AssignedCtrlType != ControlType.Keyboard) ? "Y" : keyCodeName2);
			helpTextField.text = string.Format("<b>{0}</b>: Change character\n<b>{1}</b>: Toggle ready to play", arg, arg2);
		}

		private void ShowCharacterSelectHelp()
		{
			string keyCodeName = GameInput.GetKeyCodeName(ActiveData.Keybinds[Keybind.Menu]);
			string arg = ((AssignedCtrlType != ControlType.Keyboard) ? "Left" : "Left");
			string arg2 = ((AssignedCtrlType != ControlType.Keyboard) ? "Right" : "Right");
			string arg3 = ((AssignedCtrlType != ControlType.Keyboard) ? "X" : keyCodeName);
			helpTextField.text = string.Format("<b>{0}</b>/<b>{1}</b>: Select character\n<b>{2}</b>: Confirm", arg, arg2, arg3);
		}
	}
}

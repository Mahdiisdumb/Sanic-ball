using System;
using System.Collections.Generic;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class LocalPlayerManager : MonoBehaviour
	{
		private const int maxPlayers = 4;

		public LocalPlayerPanel localPlayerPanelPrefab;

		[SerializeField]
		private Text matchJoiningHelpField;

		private MatchManager manager;

		private List<ControlType> usedControls = new List<ControlType>();

		public event EventHandler<MatchPlayerEventArgs> LocalPlayerJoined;

		private void Start()
		{
			manager = UnityEngine.Object.FindObjectOfType<MatchManager>();
			if ((bool)manager)
			{
				foreach (MatchPlayer player in manager.Players)
				{
					if (player.ClientGuid == manager.LocalClientGuid && player.CtrlType != ControlType.None)
					{
						LocalPlayerPanel localPlayerPanel = CreatePanelForControlType(player.CtrlType, true);
						localPlayerPanel.AssignedPlayer = player;
						localPlayerPanel.SetCharacter(player.CharacterId);
					}
				}
				manager.MatchPlayerAdded += Manager_MatchPlayerAdded;
			}
			else
			{
				Debug.LogWarning("Game manager not found - players cannot be added");
			}
			UpdateHelpText();
		}

		private void Update()
		{
			if (PauseMenu.GamePaused)
			{
				return;
			}
			foreach (int value in Enum.GetValues(typeof(ControlType)))
			{
				if (GameInput.IsOpeningMenu((ControlType)value))
				{
					if (!manager || usedControls.Count >= 4 || usedControls.Contains((ControlType)value))
					{
						break;
					}
					CreatePanelForControlType((ControlType)value, false);
				}
			}
		}

		private void OnDestroy()
		{
			manager.MatchPlayerAdded -= Manager_MatchPlayerAdded;
		}

		private LocalPlayerPanel CreatePanelForControlType(ControlType ctrlType, bool alreadyJoined)
		{
			usedControls.Add(ctrlType);
			UpdateHelpText();
			LocalPlayerPanel localPlayerPanel = UnityEngine.Object.Instantiate(localPlayerPanelPrefab);
			localPlayerPanel.transform.SetParent(base.transform, false);
			localPlayerPanel.playerManager = this;
			localPlayerPanel.AssignedCtrlType = ctrlType;
			localPlayerPanel.gameObject.SetActive(true);
			return localPlayerPanel;
		}

		public void CreatePlayerForControlType(ControlType ctrlType, int character)
		{
			manager.RequestPlayerJoin(ctrlType, character);
		}

		private void Manager_MatchPlayerAdded(object sender, MatchPlayerEventArgs e)
		{
			if (e.IsLocal && this.LocalPlayerJoined != null)
			{
				this.LocalPlayerJoined(this, e);
			}
		}

		public void SetCharacter(MatchPlayer player, int c)
		{
			manager.RequestCharacterChange(player.CtrlType, c);
		}

		public void SetReady(MatchPlayer player, bool ready)
		{
			manager.RequestReadyChange(player.CtrlType, ready);
		}

		public void RemoveControlType(ControlType ctrlType)
		{
			usedControls.Remove(ctrlType);
			UpdateHelpText();
		}

		public void LeaveMatch(MatchPlayer player)
		{
			manager.RequestPlayerLeave(player.CtrlType);
			usedControls.Remove(player.CtrlType);
			UpdateHelpText();
		}

		private void UpdateHelpText()
		{
			bool flag = usedControls.Count < 4;
			bool flag2 = usedControls.Contains(ControlType.Keyboard);
			matchJoiningHelpField.text = string.Empty;
			if (flag)
			{
				if (!flag2)
				{
					Text text = matchJoiningHelpField;
					text.text = text.text + "Press <b>" + GameInput.GetKeyCodeName(ActiveData.Keybinds[Keybind.Menu]) + "</b> to join with a keyboard. ";
				}
				matchJoiningHelpField.text += "Press <b>X</b> to join with a joystick.";
			}
		}
	}
}

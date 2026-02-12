using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sanicball.Data
{
	[Serializable]
	public class KeybindCollection
	{
		public Dictionary<Keybind, KeyCode> keybinds = new Dictionary<Keybind, KeyCode>();

		public KeyCode this[Keybind b]
		{
			get
			{
				return keybinds[b];
			}
			set
			{
				keybinds[b] = value;
			}
		}

		public KeybindCollection()
		{
			keybinds.Add(Keybind.Forward, KeyCode.W);
			keybinds.Add(Keybind.Left, KeyCode.A);
			keybinds.Add(Keybind.Back, KeyCode.S);
			keybinds.Add(Keybind.Right, KeyCode.D);
			keybinds.Add(Keybind.CameraUp, KeyCode.UpArrow);
			keybinds.Add(Keybind.CameraLeft, KeyCode.LeftArrow);
			keybinds.Add(Keybind.CameraDown, KeyCode.DownArrow);
			keybinds.Add(Keybind.CameraRight, KeyCode.RightArrow);
			keybinds.Add(Keybind.Brake, KeyCode.LeftShift);
			keybinds.Add(Keybind.Jump, KeyCode.Space);
			keybinds.Add(Keybind.Respawn, KeyCode.R);
			keybinds.Add(Keybind.Menu, KeyCode.Return);
			keybinds.Add(Keybind.NextSong, KeyCode.N);
			keybinds.Add(Keybind.Chat, KeyCode.T);
		}

		public void CopyValues(KeybindCollection original)
		{
			keybinds = new Dictionary<Keybind, KeyCode>(original.keybinds);
		}
	}
}

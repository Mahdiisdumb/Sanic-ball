using Sanicball.Data;
using SanicballCore;
using UnityEngine;

namespace Sanicball
{
	public static class GameInput
	{
		private const string joystick1LeftX = "joy1-lx";

		private const string joystick1LeftY = "joy1-ly";

		private const string joystick1RightX = "joy1-rx";

		private const string joystick1RightY = "joy1-ry";

		private const string joystick1DpadX = "joy1-dx";

		private const string joystick1DpadY = "joy1-dy";

		private const string joystick2LeftX = "joy2-lx";

		private const string joystick2LeftY = "joy2-ly";

		private const string joystick2RightX = "joy2-rx";

		private const string joystick2RightY = "joy2-ry";

		private const string joystick2DpadX = "joy2-dx";

		private const string joystick2DpadY = "joy2-dy";

		private const string joystick3LeftX = "joy3-lx";

		private const string joystick3LeftY = "joy3-ly";

		private const string joystick3RightX = "joy3-rx";

		private const string joystick3RightY = "joy3-ry";

		private const string joystick3DpadX = "joy3-dx";

		private const string joystick3DpadY = "joy3-dy";

		private const string joystick4LeftX = "joy4-lx";

		private const string joystick4LeftY = "joy4-ly";

		private const string joystick4RightX = "joy4-rx";

		private const string joystick4RightY = "joy4-ry";

		private const string joystick4DpadX = "joy4-dx";

		private const string joystick4DpadY = "joy4-dy";

		public static bool KeyboardDisabled { get; set; }

		public static string GetKeyCodeName(KeyCode kc)
		{
			string text = kc.ToString();
			for (int i = 0; i < text.Length; i++)
			{
				if (i != 0 && !char.IsLower(text[i]) && char.IsLower(text[i - 1]))
				{
					text = text.Substring(0, i) + " " + text.Substring(i);
					i++;
				}
			}
			return text;
		}

		public static string GetControlTypeName(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return "Keyboard";
			case ControlType.Joystick1:
				return "Joystick #1";
			case ControlType.Joystick2:
				return "Joystick #2";
			case ControlType.Joystick3:
				return "Joystick #3";
			case ControlType.Joystick4:
				return "Joystick #4";
			default:
				return null;
			}
		}

		private static Vector2 KeysToVector2(bool right, bool left, bool up, bool down)
		{
			Vector2 result = Vector2.zero;
			if (right && !left)
			{
				result = new Vector3(1f, result.y);
			}
			if (left && !right)
			{
				result = new Vector3(-1f, result.y);
			}
			if (up && !down)
			{
				result = new Vector3(result.x, 1f);
			}
			if (down && !up)
			{
				result = new Vector3(result.x, -1f);
			}
			return result;
		}

		private static Vector3 KeysToVector3(bool right, bool left, bool up, bool down, bool forward, bool back)
		{
			Vector3 result = Vector3.zero;
			if (right && !left)
			{
				result = new Vector3(1f, result.y, result.z);
			}
			if (left && !right)
			{
				result = new Vector3(-1f, result.y, result.z);
			}
			if (up && !down)
			{
				result = new Vector3(result.x, 1f, result.z);
			}
			if (down && !up)
			{
				result = new Vector3(result.x, -1f, result.z);
			}
			if (forward && !back)
			{
				result = new Vector3(result.x, result.y, 1f);
			}
			if (back && !forward)
			{
				result = new Vector3(result.x, result.y, -1f);
			}
			return result;
		}

		public static Vector3 MovementVector(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
			{
				bool forward = Input.GetKey(ActiveData.Keybinds[Keybind.Forward]) && !KeyboardDisabled;
				bool left = Input.GetKey(ActiveData.Keybinds[Keybind.Left]) && !KeyboardDisabled;
				bool back = Input.GetKey(ActiveData.Keybinds[Keybind.Back]) && !KeyboardDisabled;
				bool right = Input.GetKey(ActiveData.Keybinds[Keybind.Right]) && !KeyboardDisabled;
				return KeysToVector3(right, left, false, false, forward, back);
			}
			case ControlType.Joystick1:
				return new Vector3(Input.GetAxis("joy1-lx"), 0f, Input.GetAxis("joy1-ly"));
			case ControlType.Joystick2:
				return new Vector3(Input.GetAxis("joy2-lx"), 0f, Input.GetAxis("joy2-ly"));
			case ControlType.Joystick3:
				return new Vector3(Input.GetAxis("joy3-lx"), 0f, Input.GetAxis("joy3-ly"));
			case ControlType.Joystick4:
				return new Vector3(Input.GetAxis("joy4-lx"), 0f, Input.GetAxis("joy4-ly"));
			default:
				return Vector2.zero;
			}
		}

		public static Vector2 CameraVector(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
			{
				bool up = Input.GetKey(ActiveData.Keybinds[Keybind.CameraUp]) && !KeyboardDisabled;
				bool left = Input.GetKey(ActiveData.Keybinds[Keybind.CameraLeft]) && !KeyboardDisabled;
				bool down = Input.GetKey(ActiveData.Keybinds[Keybind.CameraDown]) && !KeyboardDisabled;
				bool right = Input.GetKey(ActiveData.Keybinds[Keybind.CameraRight]) && !KeyboardDisabled;
				return KeysToVector2(right, left, up, down);
			}
			case ControlType.Joystick1:
				return new Vector2(Input.GetAxis("joy1-rx"), Input.GetAxis("joy1-ry"));
			case ControlType.Joystick2:
				return new Vector2(Input.GetAxis("joy2-rx"), Input.GetAxis("joy2-ry"));
			case ControlType.Joystick3:
				return new Vector2(Input.GetAxis("joy3-rx"), Input.GetAxis("joy3-ry"));
			case ControlType.Joystick4:
				return new Vector2(Input.GetAxis("joy4-rx"), Input.GetAxis("joy4-ry"));
			default:
				return Vector2.zero;
			}
		}

		public static bool UIUp(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKey(KeyCode.UpArrow);
			case ControlType.Joystick1:
				return Input.GetAxis("joy1-dy") == 1f;
			case ControlType.Joystick2:
				return Input.GetAxis("joy2-dy") == 1f;
			case ControlType.Joystick3:
				return Input.GetAxis("joy3-dy") == 1f;
			case ControlType.Joystick4:
				return Input.GetAxis("joy4-dy") == 1f;
			default:
				return false;
			}
		}

		public static bool UIDown(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKey(KeyCode.DownArrow);
			case ControlType.Joystick1:
				return Input.GetAxis("joy1-dy") == -1f;
			case ControlType.Joystick2:
				return Input.GetAxis("joy2-dy") == -1f;
			case ControlType.Joystick3:
				return Input.GetAxis("joy3-dy") == -1f;
			case ControlType.Joystick4:
				return Input.GetAxis("joy4-dy") == -1f;
			default:
				return false;
			}
		}

		public static bool UILeft(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKey(KeyCode.LeftArrow);
			case ControlType.Joystick1:
				return Input.GetAxis("joy1-dx") == -1f;
			case ControlType.Joystick2:
				return Input.GetAxis("joy2-dx") == -1f;
			case ControlType.Joystick3:
				return Input.GetAxis("joy3-dx") == -1f;
			case ControlType.Joystick4:
				return Input.GetAxis("joy4-dx") == -1f;
			default:
				return false;
			}
		}

		public static bool UIRight(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKey(KeyCode.RightArrow);
			case ControlType.Joystick1:
				return Input.GetAxis("joy1-dx") == 1f;
			case ControlType.Joystick2:
				return Input.GetAxis("joy2-dx") == 1f;
			case ControlType.Joystick3:
				return Input.GetAxis("joy3-dx") == 1f;
			case ControlType.Joystick4:
				return Input.GetAxis("joy4-dx") == 1f;
			default:
				return false;
			}
		}

		public static bool IsBraking(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKey(ActiveData.Keybinds[Keybind.Brake]) && !KeyboardDisabled;
			case ControlType.Joystick1:
				return Input.GetKey(KeyCode.Joystick1Button1);
			case ControlType.Joystick2:
				return Input.GetKey(KeyCode.Joystick2Button1);
			case ControlType.Joystick3:
				return Input.GetKey(KeyCode.Joystick3Button1);
			case ControlType.Joystick4:
				return Input.GetKey(KeyCode.Joystick4Button1);
			default:
				return false;
			}
		}

		public static bool IsJumping(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKeyDown(ActiveData.Keybinds[Keybind.Jump]) && !KeyboardDisabled;
			case ControlType.Joystick1:
				return Input.GetKeyDown(KeyCode.Joystick1Button0);
			case ControlType.Joystick2:
				return Input.GetKeyDown(KeyCode.Joystick2Button0);
			case ControlType.Joystick3:
				return Input.GetKeyDown(KeyCode.Joystick3Button0);
			case ControlType.Joystick4:
				return Input.GetKeyDown(KeyCode.Joystick4Button0);
			default:
				return false;
			}
		}

		public static bool IsRespawning(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKeyDown(ActiveData.Keybinds[Keybind.Respawn]) && !KeyboardDisabled;
			case ControlType.Joystick1:
				return Input.GetKeyDown(KeyCode.Joystick1Button3);
			case ControlType.Joystick2:
				return Input.GetKeyDown(KeyCode.Joystick2Button3);
			case ControlType.Joystick3:
				return Input.GetKeyDown(KeyCode.Joystick3Button3);
			case ControlType.Joystick4:
				return Input.GetKeyDown(KeyCode.Joystick4Button3);
			default:
				return false;
			}
		}

		public static bool IsOpeningMenu(ControlType ctrlType)
		{
			switch (ctrlType)
			{
			case ControlType.Keyboard:
				return Input.GetKeyDown(ActiveData.Keybinds[Keybind.Menu]) && !KeyboardDisabled;
			case ControlType.Joystick1:
				return Input.GetKeyDown(KeyCode.Joystick1Button2);
			case ControlType.Joystick2:
				return Input.GetKeyDown(KeyCode.Joystick2Button2);
			case ControlType.Joystick3:
				return Input.GetKeyDown(KeyCode.Joystick3Button2);
			case ControlType.Joystick4:
				return Input.GetKeyDown(KeyCode.Joystick4Button2);
			default:
				return false;
			}
		}

		public static bool IsChangingSong()
		{
			return (Input.GetKeyDown(ActiveData.Keybinds[Keybind.NextSong]) && !KeyboardDisabled) || Input.GetKeyDown(KeyCode.JoystickButton6);
		}

		public static bool IsOpeningChat()
		{
			return Input.GetKeyDown(ActiveData.Keybinds[Keybind.Chat]) && !KeyboardDisabled;
		}
	}
}

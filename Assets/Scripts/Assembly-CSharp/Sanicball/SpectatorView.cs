using Sanicball.Gameplay;
using Sanicball.Logic;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball
{
	public class SpectatorView : MonoBehaviour
	{
		[SerializeField]
		private OmniCamera omniCameraPrefab;

		[SerializeField]
		private PlayerUI playerUIPrefab;

		[SerializeField]
		private Text spectatingField;

		private RacePlayer target;

		private OmniCamera activeOmniCamera;

		private PlayerUI activePlayerUI;

		private bool leftPressed;

		private bool rightPressed;

		public RacePlayer Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
				spectatingField.text = "Spectating <b>" + target.Name + "</b>";
				if (activeOmniCamera == null)
				{
					activeOmniCamera = Object.Instantiate(omniCameraPrefab);
				}
				activeOmniCamera.Target = target.Transform.GetComponent<Rigidbody>();
				if (activePlayerUI == null)
				{
					activePlayerUI = Object.Instantiate(playerUIPrefab);
					activePlayerUI.TargetManager = TargetManager;
				}
				activePlayerUI.TargetCamera = activeOmniCamera.AttachedCamera;
				activePlayerUI.TargetPlayer = target;
			}
		}

		public RaceManager TargetManager { get; set; }

		private void Start()
		{
		}

		private void Update()
		{
			if (GameInput.MovementVector(ControlType.Keyboard).x < 0f)
			{
				if (!leftPressed)
				{
					int num = FindIndex() - 1;
					if (num < 0)
					{
						num = TargetManager.PlayerCount - 1;
					}
					Target = TargetManager[num];
					leftPressed = true;
				}
			}
			else if (leftPressed)
			{
				leftPressed = false;
			}
			if (GameInput.MovementVector(ControlType.Keyboard).x > 0f)
			{
				if (!rightPressed)
				{
					int num2 = FindIndex() + 1;
					if (num2 >= TargetManager.PlayerCount)
					{
						num2 = 0;
					}
					Target = TargetManager[num2];
					rightPressed = true;
				}
			}
			else if (rightPressed)
			{
				rightPressed = false;
			}
		}

		private int FindIndex()
		{
			for (int i = 0; i < TargetManager.PlayerCount; i++)
			{
				if (TargetManager[i] == Target)
				{
					return i;
				}
			}
			return -1;
		}
	}
}

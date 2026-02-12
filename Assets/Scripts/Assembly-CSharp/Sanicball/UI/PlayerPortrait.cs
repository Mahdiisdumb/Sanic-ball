using System;
using Sanicball.Data;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class PlayerPortrait : MonoBehaviour
	{
		private const int spacing = 64;

		private int targetPosition;

		[SerializeField]
		private Text positionField;

		[SerializeField]
		private Image characterImage;

		[SerializeField]
		private Text nameField;

		private RectTransform trans;

		public RacePlayer TargetPlayer { get; set; }

		public void Move(int newPosition)
		{
			targetPosition = newPosition;
		}

		private void Start()
		{
			trans = GetComponent<RectTransform>();
			TargetPlayer.Destroyed += DestroyedCallback;
		}

		private void DestroyedCallback(object sender, EventArgs e)
		{
			if ((bool)this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			TargetPlayer.Destroyed -= DestroyedCallback;
		}

		private void Update()
		{
			targetPosition = ((!TargetPlayer.RaceFinished) ? TargetPlayer.Position : TargetPlayer.FinishReport.Position);
			positionField.text = Utils.GetPosString(targetPosition);
			if (TargetPlayer.RaceFinished)
			{
				positionField.color = new Color(0f, 0.5f, 1f);
			}
			characterImage.color = ActiveData.Characters[TargetPlayer.Character].color;
			nameField.text = TargetPlayer.Name;
			float y = trans.anchoredPosition.y;
			y = Mathf.Lerp(y, -(targetPosition - 1) * 64, Time.deltaTime * 10f);
			trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, y);
		}
	}
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class ButtonHorizontalSplit : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IMoveHandler
	{
		public RectTransform splitPosition;

		public UnityEvent onClickLeft;

		public UnityEvent onClickRight;

		public void OnPointerClick(PointerEventData data)
		{
			float x = data.position.x;
			Vector3[] array = new Vector3[4];
			splitPosition.GetWorldCorners(array);
			float x2 = array[0].x;
			float x3 = array[3].x;
			float num = Mathf.Lerp(x2, x3, 0.5f);
			if (x > num)
			{
				onClickRight.Invoke();
			}
			else
			{
				onClickLeft.Invoke();
			}
		}

		public void OnMove(AxisEventData data)
		{
			MoveDirection moveDir = data.moveDir;
			if (moveDir == MoveDirection.Left)
			{
				onClickLeft.Invoke();
				data.Use();
			}
			if (moveDir == MoveDirection.Right)
			{
				onClickRight.Invoke();
				data.Use();
			}
			Button component = GetComponent<Button>();
			if ((moveDir == MoveDirection.Left || moveDir == MoveDirection.Right) && (bool)component)
			{
				component.onClick.Invoke();
			}
		}
	}
}

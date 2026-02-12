using UnityEngine;

namespace Sanicball
{
	public class SpriteSheetGUI : MonoBehaviour
	{
		public int colCount = 4;

		public int rowCount = 4;

		public int rowNumber;

		public int colNumber;

		public int totalCells = 4;

		public int fps = 10;

		private Vector2 offset;

		private Vector2 size;

		public Vector2 Offset
		{
			get
			{
				return offset;
			}
		}

		public Vector2 Size
		{
			get
			{
				return size;
			}
		}

		private void Update()
		{
			SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells, fps);
		}

		private void SetSpriteAnimation(int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, int fps)
		{
			int num = (int)(Time.time * (float)fps);
			num %= totalCells;
			float x = 1f / (float)colCount;
			float y = 1f / (float)rowCount;
			size = new Vector2(x, y);
			int num2 = num % colCount;
			int num3 = num / colCount;
			float x2 = 1f - size.x - (float)(num2 + colNumber) * size.x;
			float y2 = 1f - size.y - (float)(num3 + rowNumber) * size.y;
			offset = new Vector2(x2, y2);
		}
	}
}

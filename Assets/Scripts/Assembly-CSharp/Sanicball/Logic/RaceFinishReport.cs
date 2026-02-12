using System;

namespace Sanicball.Logic
{
	public class RaceFinishReport
	{
		public const int DISQUALIFIED_POS = -1;

		private TimeSpan time;

		private int position;

		public int Position
		{
			get
			{
				return position;
			}
		}

		public TimeSpan Time
		{
			get
			{
				return time;
			}
		}

		public bool Disqualified
		{
			get
			{
				return position == -1;
			}
		}

		public RaceFinishReport(int position, TimeSpan time)
		{
			this.position = position;
			this.time = time;
		}
	}
}

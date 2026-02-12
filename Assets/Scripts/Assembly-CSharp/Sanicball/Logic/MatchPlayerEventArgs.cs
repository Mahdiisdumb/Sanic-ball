using System;

namespace Sanicball.Logic
{
	public class MatchPlayerEventArgs : EventArgs
	{
		public MatchPlayer Player { get; private set; }

		public bool IsLocal { get; private set; }

		public MatchPlayerEventArgs(MatchPlayer player, bool isLocal)
		{
			Player = player;
			IsLocal = isLocal;
		}
	}
}

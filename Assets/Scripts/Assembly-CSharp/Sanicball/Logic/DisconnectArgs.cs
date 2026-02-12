using System;

namespace Sanicball.Logic
{
	public class DisconnectArgs : EventArgs
	{
		public string Reason { get; private set; }

		public DisconnectArgs(string reason)
		{
			Reason = reason;
		}
	}
}

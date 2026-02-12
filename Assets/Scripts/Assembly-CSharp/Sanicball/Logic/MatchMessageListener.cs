using System;

namespace Sanicball.Logic
{
	public class MatchMessageListener
	{
		public Type MessageType { get; private set; }

		public object Handler { get; private set; }

		public MatchMessageListener(Type messageType, object handler)
		{
			MessageType = messageType;
			Handler = handler;
		}
	}
}

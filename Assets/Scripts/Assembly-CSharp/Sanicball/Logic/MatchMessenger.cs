using System.Collections.Generic;
using SanicballCore;

namespace Sanicball.Logic
{
	public abstract class MatchMessenger
	{
		protected List<MatchMessageListener> listeners = new List<MatchMessageListener>();

		public abstract void SendMessage<T>(T message) where T : MatchMessage;

		public abstract void UpdateListeners();

		public abstract void Close();

		public void CreateListener<T>(MatchMessageHandler<T> handler) where T : MatchMessage
		{
			listeners.Add(new MatchMessageListener(typeof(T), handler));
		}

		public bool RemoveListener<T>(MatchMessageHandler<T> handler) where T : MatchMessage
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				MatchMessageListener matchMessageListener = listeners[i];
				if (matchMessageListener.MessageType == typeof(T) && (MatchMessageHandler<T>)matchMessageListener.Handler == handler)
				{
					listeners.Remove(matchMessageListener);
					return true;
				}
			}
			return false;
		}
	}
}

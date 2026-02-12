using SanicballCore;

namespace Sanicball.Logic
{
	public class LocalMatchMessenger : MatchMessenger
	{
		public override void SendMessage<T>(T message)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				MatchMessageListener matchMessageListener = listeners[i];
				if (matchMessageListener.MessageType == message.GetType())
				{
					((MatchMessageHandler<T>)matchMessageListener.Handler)(message, 0f);
				}
			}
		}

		public override void UpdateListeners()
		{
		}

		public override void Close()
		{
		}
	}
}

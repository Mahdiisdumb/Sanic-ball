using System;

namespace Sanicball.UI
{
	public class ChatMessageArgs : EventArgs
	{
		public string Text { get; private set; }

		public ChatMessageArgs(string text)
		{
			Text = text;
		}
	}
}

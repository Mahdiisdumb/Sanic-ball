using System;

namespace Sanicball.UI
{
	public class CharacterSelectionArgs : EventArgs
	{
		public int SelectedCharacter { get; set; }

		public CharacterSelectionArgs(int selectedCharacter)
		{
			SelectedCharacter = selectedCharacter;
		}
	}
}

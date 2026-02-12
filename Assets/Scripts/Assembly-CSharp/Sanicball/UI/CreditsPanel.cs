using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
	public class CreditsPanel : MonoBehaviour
	{
		public Text characterList;

		public Text trackList;

		public MusicPlayer musicPlayerPrefab;

		private void Start()
		{
			List<string> list = new List<string>();
			Sanicball.Data.CharacterInfo[] characters = ActiveData.Characters;
			foreach (Sanicball.Data.CharacterInfo item in from a in characters
				where !a.hidden
				orderby a.tier
				select a)
			{
				list.Add(item.name + ": <b>" + item.artBy + "</b>");
			}
			characterList.text = string.Join("\n", list.ToArray());
			List<string> list2 = new List<string>();
			Song[] playlist = musicPlayerPrefab.playlist;
			Song[] array = playlist;
			foreach (Song song in array)
			{
				list2.Add("<b>" + song.name + "</b>");
			}
			trackList.text = string.Join("\n", list2.ToArray());
		}

		private void Update()
		{
		}
	}
}

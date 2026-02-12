using System;
using System.Collections.Generic;
using System.Linq;
using Sanicball.Data;
using Sanicball.Logic;
using Sanicball.UI;
using UnityEngine;

namespace Sanicball
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicPlayer : MonoBehaviour
	{
		public MusicPlayerCanvas playerCanvasPrefab;

		public bool playerCanvasLobbyOffset;

		private MusicPlayerCanvas playerCanvas;

		public bool startPlaying;

		public bool fadeIn;

		public Song[] playlist;

		public AudioSource fastSource;

		[NonSerialized]
		public bool fastMode;

		private int currentSongID;

		private bool isPlaying;

		private string currentSongCredits;

		private float timer;

		private float slidePosition;

		private float slidePositionMax = 20f;

		private AudioSource aSource;

		public void Play()
		{
			Play(playlist[currentSongID].name);
		}

		public void Play(string credits)
		{
			if (ActiveData.GameSettings.music)
			{
				playerCanvas.Show(credits);
				isPlaying = true;
				aSource.Play();
			}
		}

		public void Pause()
		{
			aSource.Pause();
			isPlaying = false;
		}

		private void Start()
		{
			playerCanvas = UnityEngine.Object.Instantiate(playerCanvasPrefab);
			if (playerCanvasLobbyOffset)
			{
				playerCanvas.lobbyOffset = true;
			}
			aSource = GetComponent<AudioSource>();
			slidePosition = slidePositionMax;
			ShuffleSongs();
			if (ActiveData.ESportsFullyReady)
			{
				MatchManager matchManager = UnityEngine.Object.FindObjectOfType<MatchManager>();
				if (!matchManager.InLobby)
				{
					List<Song> list = playlist.ToList();
					Song song = new Song();
					song.name = "Skrollex - Bungee Ride";
					song.clip = ActiveData.ESportsMusic;
					list.Insert(0, song);
					playlist = list.ToArray();
				}
			}
			aSource.clip = playlist[0].clip;
			currentSongID = 0;
			isPlaying = aSource.isPlaying;
			if (startPlaying && ActiveData.GameSettings.music)
			{
				Play();
			}
			if (fadeIn)
			{
				aSource.volume = 0f;
			}
			if (!ActiveData.GameSettings.music)
			{
				fastSource.Stop();
			}
		}

		private void Update()
		{
			if (fadeIn && aSource.volume < 0.5f)
			{
				aSource.volume = Mathf.Min(aSource.volume + Time.deltaTime * 0.1f, 0.5f);
			}
			if ((!aSource.isPlaying || GameInput.IsChangingSong()) && isPlaying)
			{
				if (currentSongID < playlist.Length - 1)
				{
					currentSongID++;
				}
				else
				{
					currentSongID = 0;
				}
				aSource.clip = playlist[currentSongID].clip;
				slidePosition = slidePositionMax;
				Play();
			}
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			if (fastMode && fastSource.volume < 1f)
			{
				fastSource.volume = Mathf.Min(1f, fastSource.volume + Time.deltaTime * 0.25f);
				aSource.volume = 0.5f - fastSource.volume / 2f;
			}
			if (!fastMode && fastSource.volume > 0f)
			{
				fastSource.volume = Mathf.Max(0f, fastSource.volume - Time.deltaTime * 0.5f);
				aSource.volume = 0.5f - fastSource.volume / 2f;
			}
			if (timer > 0f)
			{
				slidePosition = Mathf.Lerp(slidePosition, 0f, Time.deltaTime * 4f);
			}
			else
			{
				slidePosition = Mathf.Lerp(slidePosition, slidePositionMax, Time.deltaTime * 2f);
			}
		}

		private void ShuffleSongs()
		{
			for (int num = playlist.Length; num > 1; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				Song song = playlist[num2];
				playlist[num2] = playlist[num - 1];
				playlist[num - 1] = song;
			}
		}
	}
}

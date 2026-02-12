using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using Sanicball.Logic;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Data
{
	public class ActiveData : MonoBehaviour
	{
		public List<RaceRecord> raceRecords = new List<RaceRecord>();

		private static ActiveData instance;

		private GameSettings gameSettings = new GameSettings();

		private KeybindCollection keybinds = new KeybindCollection();

		private MatchSettings matchSettings = MatchSettings.CreateDefault();

		[SerializeField]
		[Header("Static data")]
		private StageInfo[] stages;

		[SerializeField]
		private CharacterInfo[] characters;

		[SerializeField]
		private GameJoltInfo gameJoltInfo;

		[SerializeField]
		private GameObject christmasHat;

		[SerializeField]
		private Material eSportsTrail;

		[SerializeField]
		private GameObject eSportsHat;

		[SerializeField]
		private AudioClip eSportsMusic;

		[SerializeField]
		private ESportMode eSportsPrefab;

		public static GameSettings GameSettings
		{
			get
			{
				return instance.gameSettings;
			}
		}

		public static KeybindCollection Keybinds
		{
			get
			{
				return instance.keybinds;
			}
		}

		public static MatchSettings MatchSettings
		{
			get
			{
				return instance.matchSettings;
			}
			set
			{
				instance.matchSettings = value;
			}
		}

		public static List<RaceRecord> RaceRecords
		{
			get
			{
				return instance.raceRecords;
			}
		}

		public static StageInfo[] Stages
		{
			get
			{
				return instance.stages;
			}
		}

		public static CharacterInfo[] Characters
		{
			get
			{
				return instance.characters;
			}
		}

		public static GameJoltInfo GameJoltInfo
		{
			get
			{
				return instance.gameJoltInfo;
			}
		}

		public static GameObject ChristmasHat
		{
			get
			{
				return instance.christmasHat;
			}
		}

		public static Material ESportsTrail
		{
			get
			{
				return instance.eSportsTrail;
			}
		}

		public static GameObject ESportsHat
		{
			get
			{
				return instance.eSportsHat;
			}
		}

		public static AudioClip ESportsMusic
		{
			get
			{
				return instance.eSportsMusic;
			}
		}

		public static ESportMode ESportsPrefab
		{
			get
			{
				return instance.eSportsPrefab;
			}
		}

		public static bool ESportsFullyReady
		{
			get
			{
				bool result = false;
				if (GameSettings.eSportsReady)
				{
					MatchManager matchManager = Object.FindObjectOfType<MatchManager>();
					if ((bool)matchManager)
					{
						ReadOnlyCollection<MatchPlayer> players = matchManager.Players;
						foreach (MatchPlayer item in players)
						{
							if (item.CtrlType != ControlType.None)
							{
								if (item.CharacterId != 13)
								{
									return false;
								}
								result = true;
							}
						}
					}
				}
				return result;
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnEnable()
		{
			LoadAll();
			gameJoltInfo.Init();
		}

		private void OnApplicationQuit()
		{
			SaveAll();
		}

		public void LoadAll()
		{
			Load("GameSettings.json", ref gameSettings);
			Load("GameKeybinds.json", ref keybinds);
			Load("MatchSettings.json", ref matchSettings);
			Load("Records.json", ref raceRecords);
		}

		public void SaveAll()
		{
			Save("GameSettings.json", gameSettings);
			Save("GameKeybinds.json", keybinds);
			Save("MatchSettings.json", matchSettings);
			Save("Records.json", raceRecords);
		}

		private void Load<T>(string filename, ref T output)
		{
			string path = Application.persistentDataPath + "/" + filename;
			if (File.Exists(path))
			{
				string value;
				using (StreamReader streamReader = new StreamReader(path))
				{
					value = streamReader.ReadToEnd();
				}
				try
				{
					T val = JsonConvert.DeserializeObject<T>(value);
					if (val != null)
					{
						output = val;
						Debug.Log(filename + " loaded successfully.");
					}
					else
					{
						Debug.LogError("Failed to load " + filename + ": file is empty.");
					}
					return;
				}
				catch (JsonException ex)
				{
					Debug.LogError("Failed to parse " + filename + "! JSON converter info: " + ex.Message);
					return;
				}
			}
			Debug.Log(filename + " has not been loaded - file not found.");
		}

		private void Save(string filename, object objToSave)
		{
			string value = JsonConvert.SerializeObject(objToSave);
			using (StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + "/" + filename))
			{
				streamWriter.Write(value);
			}
			Debug.Log(filename + " saved successfully.");
		}
	}
}

using System;
using Sanicball.Data;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[RequireComponent(typeof(Rigidbody))]
	public class Ball : MonoBehaviour
	{
		[SerializeField]
		[Header("Initial stats")]
		private BallType type;

		[SerializeField]
		private ControlType ctrlType;

		[SerializeField]
		private int characterId;

		[SerializeField]
		private string nickname;

		[SerializeField]
		private GameObject hatPrefab;

		[SerializeField]
		[Header("Subcategories")]
		private BallPrefabs prefabs;

		[SerializeField]
		private BallMotionSounds sounds;

		private BallStats characterStats;

		private bool canMove = true;

		private BallControlInput input;

		private bool grounded;

		private float groundedTimer;

		private float upResetTimer;

		private DriftySmoke smoke;

		private SpeedFire speedFire;

		private Rigidbody rb;

		public BallType Type
		{
			get
			{
				return type;
			}
		}

		public ControlType CtrlType
		{
			get
			{
				return ctrlType;
			}
		}

		public int CharacterId
		{
			get
			{
				return characterId;
			}
		}

		public bool CanMove
		{
			get
			{
				return canMove;
			}
			set
			{
				canMove = value;
			}
		}

		public bool AutoBrake { get; set; }

		public Vector3 DirectionVector { get; set; }

		public Vector3 Up { get; set; }

		public bool Brake { get; set; }

		public string Nickname
		{
			get
			{
				return nickname;
			}
		}

		public BallControlInput Input
		{
			get
			{
				return input;
			}
		}

		public event EventHandler<CheckpointPassArgs> CheckpointPassed;

		public event EventHandler RespawnRequested;

		public event EventHandler<CameraCreationArgs> CameraCreated;

		public void Jump()
		{
			if (grounded && CanMove)
			{
				rb.AddForce(Up * characterStats.jumpHeight, ForceMode.Impulse);
				if (sounds.Jump != null)
				{
					sounds.Jump.Play();
				}
				grounded = false;
			}
		}

		public void RequestRespawn()
		{
			if (this.RespawnRequested != null)
			{
				this.RespawnRequested(this, EventArgs.Empty);
			}
		}

		public void Init(BallType type, ControlType ctrlType, int characterId, string nickname)
		{
			this.type = type;
			this.ctrlType = ctrlType;
			this.characterId = characterId;
			this.nickname = nickname;
		}

		private void Start()
		{
			Up = Vector3.up;
			smoke = UnityEngine.Object.Instantiate(prefabs.Smoke);
			smoke.target = this;
			smoke.DriftAudio = sounds.Brake;
			rb = GetComponent<Rigidbody>();
			rb.maxAngularVelocity = 1000f;
			base.gameObject.name = type.ToString() + " - " + nickname;
			if (CharacterId >= 0 && CharacterId < ActiveData.Characters.Length)
			{
				SetCharacter(ActiveData.Characters[CharacterId]);
			}
			speedFire = UnityEngine.Object.Instantiate(prefabs.SpeedFire);
			speedFire.Init(this);
			DateTime now = DateTime.Now;
			if (now.Month == 12 && now.Day > 20 && now.Day <= 31)
			{
				hatPrefab = ActiveData.ChristmasHat;
			}
			if (ActiveData.GameSettings.eSportsReady)
			{
				hatPrefab = ActiveData.ESportsHat;
			}
			if ((bool)hatPrefab)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(hatPrefab);
				gameObject.transform.SetParent(base.transform, false);
			}
			if (type == BallType.Player && ctrlType != ControlType.None)
			{
				IBallCamera ballCamera;
				if (ActiveData.GameSettings.useOldControls)
				{
					ballCamera = UnityEngine.Object.Instantiate(prefabs.OldCamera);
					((PivotCamera)ballCamera).UseMouse = ctrlType == ControlType.Keyboard;
				}
				else
				{
					ballCamera = UnityEngine.Object.Instantiate(prefabs.Camera);
				}
				ballCamera.Target = rb;
				ballCamera.CtrlType = ctrlType;
				if (this.CameraCreated != null)
				{
					this.CameraCreated(this, new CameraCreationArgs(ballCamera));
				}
			}
			if (type == BallType.LobbyPlayer)
			{
				LobbyCamera lobbyCamera = UnityEngine.Object.FindObjectOfType<LobbyCamera>();
				if ((bool)lobbyCamera)
				{
					lobbyCamera.AddBall(this);
				}
			}
			if ((type == BallType.Player || type == BallType.LobbyPlayer) && ctrlType != ControlType.None)
			{
				input = base.gameObject.AddComponent<BallControlInput>();
			}
			if (type == BallType.AI)
			{
				base.gameObject.AddComponent<BallControlAI>();
			}
		}

		private void SetCharacter(Sanicball.Data.CharacterInfo c)
		{
			GetComponent<Renderer>().material = c.material;
			GetComponent<TrailRenderer>().material = c.trail;
			if (c.name == "Super Sanic" && ActiveData.GameSettings.eSportsReady)
			{
				GetComponent<TrailRenderer>().material = ActiveData.ESportsTrail;
			}
			base.transform.localScale = new Vector3(c.ballSize, c.ballSize, c.ballSize);
			if (c.alternativeMesh != null)
			{
				GetComponent<MeshFilter>().mesh = c.alternativeMesh;
			}
			if (c.collisionMesh != null)
			{
				if (c.collisionMesh.vertexCount <= 255)
				{
					UnityEngine.Object.Destroy(GetComponent<Collider>());
					MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = c.collisionMesh;
					meshCollider.convex = true;
				}
				else
				{
					Debug.LogWarning("Vertex count for " + c.name + "'s collision mesh is bigger than 255!");
				}
			}
			characterStats = c.stats;
		}

		private void FixedUpdate()
		{
			if (CanMove)
			{
				if (DirectionVector != Vector3.zero)
				{
					rb.AddTorque(DirectionVector * characterStats.rollSpeed);
				}
				if (!grounded)
				{
					rb.AddForce(Quaternion.Euler(0f, -90f, 0f) * DirectionVector * characterStats.airSpeed);
				}
			}
			if (AutoBrake)
			{
				Brake = true;
			}
			if (Brake)
			{
				rb.angularVelocity = Vector3.zero;
			}
			if (!grounded)
			{
			}
		}

		private void Update()
		{
			if (grounded)
			{
				float a = Mathf.Clamp(rb.angularVelocity.magnitude / 230f, 0f, 16f);
				float value = (-128f + rb.velocity.magnitude) / 256f;
				value = Mathf.Clamp(value, 0f, 1f);
				if (sounds.Roll != null)
				{
					sounds.Roll.pitch = Mathf.Max(a, 0.8f);
					sounds.Roll.volume = Mathf.Min(a, 1f);
				}
				if (sounds.SpeedNoise != null)
				{
					sounds.SpeedNoise.pitch = 0.8f + value;
					sounds.SpeedNoise.volume = value;
				}
			}
			else
			{
				if (sounds.Roll != null && sounds.Roll.volume > 0f)
				{
					sounds.Roll.volume = Mathf.Max(0f, sounds.Roll.volume - 0.2f);
				}
				if (sounds.SpeedNoise != null && sounds.SpeedNoise.volume > 0f)
				{
					sounds.SpeedNoise.volume = Mathf.Max(0f, sounds.SpeedNoise.volume - 0.01f);
				}
			}
			if (groundedTimer > 0f)
			{
				groundedTimer -= Time.deltaTime;
				if (groundedTimer <= 0f)
				{
					grounded = false;
					upResetTimer = 1f;
				}
			}
			if (!grounded)
			{
				if (upResetTimer > 0f)
				{
					upResetTimer -= Time.deltaTime;
				}
				else
				{
					Up = Vector3.MoveTowards(Up, Vector3.up, Time.deltaTime * 10f);
				}
			}
			if (smoke != null)
			{
				smoke.grounded = grounded;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			Checkpoint component = other.GetComponent<Checkpoint>();
			if ((bool)component && this.CheckpointPassed != null)
			{
				this.CheckpointPassed(this, new CheckpointPassArgs(component));
			}
			if ((bool)other.GetComponent<TriggerRespawn>())
			{
				RequestRespawn();
			}
		}

		private void OnCollisionStay(Collision c)
		{
			grounded = true;
			groundedTimer = 0f;
			Up = c.contacts[0].normal;
		}

		private void OnCollisionExit(Collision c)
		{
			groundedTimer = 0.08f;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(base.transform.position, Up);
		}

		public void CreateRemovalParticles()
		{
			UnityEngine.Object.Instantiate(prefabs.RemovalParticles, base.transform.position, base.transform.rotation);
		}
	}
}

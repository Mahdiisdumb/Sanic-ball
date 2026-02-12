using Sanicball.Logic;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Gameplay
{
	[RequireComponent(typeof(Ball))]
	public class BallControlAI : MonoBehaviour
	{
		private const float AUTO_RESPAWN_TIME = 6.66f;

		private const float TARGET_OFFSET_CHANGE_TIME = 3.33f;

		private Ball ball;

		private AINode target;

		private AISkillLevel skillLevel = AISkillLevel.Average;

		private float targetPointMaxOffset = 10f;

		private Vector3 targetPointOffsetCurrent = Vector3.zero;

		private Vector3 targetPointOffsetGoal = Vector3.zero;

		private float targetPointOffsetChangeTimer = 3.33f;

		private float autoRespawnTimer = 6.66f;

		public AINode Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
				autoRespawnTimer = 6.66f;
			}
		}

		private void TriggerJump()
		{
			ball.Jump();
		}

		private void Start()
		{
			ball = GetComponent<Ball>();
			RaceManager raceManager = Object.FindObjectOfType<RaceManager>();
			if ((bool)raceManager)
			{
				skillLevel = raceManager.Settings.AISkill;
				switch (skillLevel)
				{
				case AISkillLevel.Retarded:
					targetPointMaxOffset = 200f;
					break;
				case AISkillLevel.Average:
					targetPointMaxOffset = 20f;
					break;
				case AISkillLevel.Dank:
					targetPointMaxOffset = 0f;
					break;
				}
			}
			target = StageReferences.Active.checkpoints[0].FirstAINode;
		}

		private void Update()
		{
			ball.Brake = false;
			if ((bool)target)
			{
				Vector3 velocity = GetComponent<Rigidbody>().velocity;
				Quaternion quaternion = ((!(velocity != Vector3.zero)) ? Quaternion.LookRotation(target.transform.position) : Quaternion.LookRotation(velocity));
				Ray ray = new Ray(base.transform.position, quaternion * Vector3.forward);
				float num = Mathf.Max(0f, Mathf.Min(velocity.magnitude * 1f, Vector3.Distance(base.transform.position, target.transform.position) - 35f));
				Vector3 vector = base.transform.position + ray.direction * num;
				Vector3 vector2 = target.transform.position + targetPointOffsetCurrent;
				Quaternion quaternion2 = Quaternion.LookRotation(vector - vector2);
				ball.DirectionVector = quaternion2 * Vector3.left;
				Debug.DrawLine(vector, vector2, Color.white);
			}
			if (ball.CanMove)
			{
				autoRespawnTimer -= Time.deltaTime;
				if (autoRespawnTimer <= 0f)
				{
					ball.RequestRespawn();
					autoRespawnTimer = 6.66f;
				}
			}
			targetPointOffsetCurrent = Vector3.Lerp(targetPointOffsetCurrent, targetPointOffsetGoal, Time.deltaTime);
			targetPointOffsetChangeTimer -= Time.deltaTime;
			if (targetPointOffsetChangeTimer <= 0f)
			{
				targetPointOffsetChangeTimer += 3.33f;
				targetPointOffsetGoal = Random.insideUnitSphere * Random.Range(0f, targetPointMaxOffset);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			AINode component = other.GetComponent<AINode>();
			if ((bool)component && component == target && (bool)target.NextNode)
			{
				Target = target.NextNode;
			}
		}
	}
}

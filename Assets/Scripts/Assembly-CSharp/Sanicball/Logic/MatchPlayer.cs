using System;
using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
	[Serializable]
	public class MatchPlayer
	{
		private Guid clientGuid;

		private ControlType ctrlType;

		private double latestMovementTimestamp = -2147483648.0;

		public Guid ClientGuid
		{
			get
			{
				return clientGuid;
			}
		}

		public ControlType CtrlType
		{
			get
			{
				return ctrlType;
			}
		}

		public int CharacterId { get; set; }

		public Ball BallObject { get; set; }

		public bool ReadyToRace { get; set; }

		public MatchPlayer(Guid clientGuid, ControlType ctrlType, int initialCharacterId)
		{
			this.clientGuid = clientGuid;
			this.ctrlType = ctrlType;
			CharacterId = initialCharacterId;
		}

		public void ProcessMovement(double timestamp, PlayerMovement movement)
		{
			if (timestamp > latestMovementTimestamp)
			{
				Rigidbody component = BallObject.GetComponent<Rigidbody>();
				BallObject.transform.position = movement.Position;
				BallObject.transform.rotation = movement.Rotation;
				component.velocity = movement.Velocity;
				component.angularVelocity = movement.AngularVelocity;
				BallObject.DirectionVector = movement.DirectionVector;
				latestMovementTimestamp = timestamp;
			}
		}
	}
}

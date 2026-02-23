using System;
using Lidgren.Network;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public class PlayerMovement
    {
        public Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }
        public Vector3 DirectionVector { get; private set; }

        public PlayerMovement(Guid clientGuid, ControlType ctrlType, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity, Vector3 directionVector)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            DirectionVector = directionVector;
        }

        public static PlayerMovement CreateFromPlayer(MatchPlayer player)
        {
            Rigidbody rb = player.BallObject.GetComponent<Rigidbody>();
            return new PlayerMovement(
                player.ClientGuid,
                player.CtrlType,
                player.BallObject.transform.position,
                player.BallObject.transform.rotation,
                rb.linearVelocity,
                rb.angularVelocity,
                player.BallObject.DirectionVector
            );
        }

        // --- NetBuffer helpers ---
        public void WriteToMessage(NetBuffer msg)
        {
            msg.Write(ClientGuid.ToByteArray());
            msg.Write((byte)CtrlType);
            WriteVector3(msg, Position);
            WriteQuaternion(msg, Rotation);
            WriteVector3(msg, Velocity);
            WriteVector3(msg, AngularVelocity);
            WriteVector3(msg, DirectionVector);
        }

        public static PlayerMovement ReadFromMessage(NetBuffer msg)
        {
            Guid guid = new Guid(msg.ReadBytes(16));
            ControlType ctrl = (ControlType)msg.ReadByte();
            Vector3 pos = ReadVector3(msg);
            Quaternion rot = ReadQuaternion(msg);
            Vector3 vel = ReadVector3(msg);
            Vector3 angVel = ReadVector3(msg);
            Vector3 dir = ReadVector3(msg);

            return new PlayerMovement(guid, ctrl, pos, rot, vel, angVel, dir);
        }

        // --- Vector3 / Quaternion serialization ---
        private static void WriteVector3(NetBuffer msg, Vector3 v)
        {
            msg.Write(v.x);
            msg.Write(v.y);
            msg.Write(v.z);
        }

        private static Vector3 ReadVector3(NetBuffer msg)
        {
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            float z = msg.ReadFloat();
            return new Vector3(x, y, z);
        }

        private static void WriteQuaternion(NetBuffer msg, Quaternion q)
        {
            msg.Write(q.x);
            msg.Write(q.y);
            msg.Write(q.z);
            msg.Write(q.w);
        }

        private static Quaternion ReadQuaternion(NetBuffer msg)
        {
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            float z = msg.ReadFloat();
            float w = msg.ReadFloat();
            return new Quaternion(x, y, z, w);
        }
    }
}
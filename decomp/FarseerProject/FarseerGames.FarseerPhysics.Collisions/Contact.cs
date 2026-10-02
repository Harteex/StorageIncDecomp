using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct Contact(Vector2 position, Vector2 normal, float separation, ContactId contactId) : IEquatable<Contact>
{
	internal float BounceVelocity = 0f;

	internal ContactId ContactId = contactId;

	internal float MassNormal = 0f;

	internal float MassTangent = 0f;

	public Vector2 Normal = normal;

	internal float NormalImpulse = 0f;

	internal float NormalImpulseBias = 0f;

	internal float NormalVelocityBias = 0f;

	public Vector2 Position = position;

	internal Vector2 R1 = Vector2.Zero;

	internal Vector2 R2 = Vector2.Zero;

	public float Separation = separation;

	internal float TangentImpulse = 0f;

	public bool Equals(Contact other)
	{
		return ContactId == other.ContactId;
	}

	public void SetMassNormal(float massNormal)
	{
		MassNormal = massNormal;
	}

	public void SetMassTangent(float massTangent)
	{
		MassTangent = massTangent;
	}

	public void SetBias(float bias)
	{
		NormalVelocityBias = bias;
	}

	public void SetNormalImpulse(float normalImpulse)
	{
		NormalImpulse = normalImpulse;
	}

	public void SetTangentImpulse(float tangentImpulse)
	{
		TangentImpulse = tangentImpulse;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Contact))
		{
			return false;
		}
		return Equals((Contact)obj);
	}

	public static bool operator ==(Contact contact1, Contact contact2)
	{
		return contact1.Equals(contact2);
	}

	public static bool operator !=(Contact contact1, Contact contact2)
	{
		return !contact1.Equals(contact2);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

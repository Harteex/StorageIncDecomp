using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics;

public class Arbiter : IEquatable<Arbiter>
{
	private static Comparison<Contact> _contactComparer = CompareSeperation;

	private ContactList _contactList;

	private float _float1;

	private float _float2;

	private float _frictionCoefficientCombined;

	private ContactList _mergedContactList;

	private ContactList _newContactList;

	private PhysicsSimulator _physicsSimulator;

	private Vector2 _vec1 = Vector2.Zero;

	private Vector2 _vec2 = Vector2.Zero;

	public Geom GeometryA;

	public Geom GeometryB;

	private float _kNormal;

	private float _kTangent;

	private float _min;

	private Vector2 _r1;

	private Vector2 _r2;

	private float _restitution;

	private float _rn1;

	private float _rn2;

	private float _rt1;

	private float _rt2;

	private Feature _feature;

	private Vector2 _localVertex;

	private Vector2 _vertRef;

	private Contact _contact;

	private Vector2 _dv = Vector2.Zero;

	private Vector2 _impulse = Vector2.Zero;

	private Vector2 _impulseBias = Vector2.Zero;

	private float _maxTangentImpulse;

	private float _normalImpulse;

	private float _normalImpulseBias;

	private float _normalImpulseBiasOriginal;

	private float _normalVelocityBias;

	private float _oldNormalImpulse;

	private float _oldTangentImpulse;

	private Vector2 _tangent = Vector2.Zero;

	private float _tangentImpulse;

	private float _vn;

	private float _vt;

	public Geom GeomA
	{
		get
		{
			return GeometryA;
		}
		set
		{
			GeometryA = value;
		}
	}

	public Geom GeomB
	{
		get
		{
			return GeometryB;
		}
		set
		{
			GeometryB = value;
		}
	}

	public ContactList ContactList => _contactList;

	internal int ContactCount => _contactList.Count;

	public Arbiter()
	{
	}

	internal Arbiter(Geom geometry1, Geom geometry2, PhysicsSimulator physicsSimulator)
	{
		ConstructArbiter(geometry1, geometry2, physicsSimulator);
	}

	public bool Equals(Arbiter other)
	{
		return GeometryA == other.GeometryA && GeometryB == other.GeometryB;
	}

	internal void ConstructArbiter(Geom geometry1, Geom geometry2, PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
		if (geometry1 < geometry2)
		{
			GeometryA = geometry1;
			GeometryB = geometry2;
		}
		else
		{
			GeometryA = geometry2;
			GeometryB = geometry1;
		}
		switch (physicsSimulator.frictionType)
		{
		case FrictionType.Average:
			_frictionCoefficientCombined = (GeometryA.frictionCoefficient + GeometryB.frictionCoefficient) / 2f;
			break;
		case FrictionType.Minimum:
			_frictionCoefficientCombined = Math.Min(GeometryA.frictionCoefficient, GeometryB.frictionCoefficient);
			break;
		default:
			_frictionCoefficientCombined = (GeometryA.frictionCoefficient + GeometryB.frictionCoefficient) / 2f;
			break;
		}
		InitializeContactLists(physicsSimulator.maxContactsToDetect);
	}

	internal void PreStepImpulse(float inverseDt)
	{
		if (GeometryA.collisionResponseEnabled && GeometryB.collisionResponseEnabled)
		{
			for (int i = 0; i < _contactList.Count; i++)
			{
				Contact value = _contactList[i];
				Vector2.Subtract(ref value.Position, ref GeometryA.body.position, out _r1);
				Vector2.Subtract(ref value.Position, ref GeometryB.body.position, out _r2);
				Vector2.Dot(ref _r1, ref value.Normal, out _rn1);
				Vector2.Dot(ref _r2, ref value.Normal, out _rn2);
				float num = GeometryA.body.inverseMass + GeometryB.body.inverseMass;
				Vector2.Dot(ref _r1, ref _r1, out _float1);
				Vector2.Dot(ref _r2, ref _r2, out _float2);
				_kNormal = num + GeometryA.body.inverseMomentOfInertia * (_float1 - _rn1 * _rn1) + GeometryB.body.inverseMomentOfInertia * (_float2 - _rn2 * _rn2);
				value.MassNormal = 1f / _kNormal;
				_float1 = 1f;
				Calculator.Cross(ref value.Normal, ref _float1, out _tangent);
				Vector2.Dot(ref _r1, ref _tangent, out _rt1);
				Vector2.Dot(ref _r2, ref _tangent, out _rt2);
				_kTangent = GeometryA.Body.InverseMass + GeometryB.Body.InverseMass;
				Vector2.Dot(ref _r1, ref _r1, out _float1);
				Vector2.Dot(ref _r2, ref _r2, out _float2);
				_kTangent += GeometryA.body.inverseMomentOfInertia * (_float1 - _rt1 * _rt1) + GeometryB.Body.InverseMomentOfInertia * (_float2 - _rt2 * _rt2);
				value.MassTangent = 1f / _kTangent;
				_min = Math.Min(0f, _physicsSimulator.allowedPenetration + value.Separation);
				value.NormalVelocityBias = (0f - _physicsSimulator.biasFactor) * inverseDt * _min;
				_restitution = (GeometryA.restitutionCoefficient + GeometryB.restitutionCoefficient) * 0.5f;
				GeometryA.body.GetVelocityAtWorldOffset(ref _r1, out _vec1);
				GeometryB.body.GetVelocityAtWorldOffset(ref _r2, out _vec2);
				Vector2.Subtract(ref _vec2, ref _vec1, out _dv);
				Vector2.Dot(ref _dv, ref value.Normal, out _vn);
				value.BounceVelocity = _vn * _restitution;
				Vector2.Multiply(ref value.Normal, value.NormalImpulse, out _vec1);
				Vector2.Multiply(ref _tangent, value.TangentImpulse, out _vec2);
				Vector2.Add(ref _vec1, ref _vec2, out _impulse);
				GeometryB.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _r2);
				Vector2.Multiply(ref _impulse, -1f, out _impulse);
				GeometryA.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _r1);
				value.NormalImpulseBias = 0f;
				_contactList[i] = value;
			}
		}
	}

	internal void ApplyImpulse()
	{
		if (GeometryA.collisionResponseEnabled && GeometryB.collisionResponseEnabled)
		{
			for (int i = 0; i < _contactList.Count; i++)
			{
				_contact = _contactList[i];
				_contact.R1.X = _contact.Position.X - GeometryA.body.position.X;
				_contact.R1.Y = _contact.Position.Y - GeometryA.body.position.Y;
				_contact.R2.X = _contact.Position.X - GeometryB.body.position.X;
				_contact.R2.Y = _contact.Position.Y - GeometryB.body.position.Y;
				GeometryA.body.GetVelocityAtWorldOffset(ref _contact.R1, out _vec1);
				GeometryB.body.GetVelocityAtWorldOffset(ref _contact.R2, out _vec2);
				_dv.X = _vec2.X - _vec1.X;
				_dv.Y = _vec2.Y - _vec1.Y;
				_vn = _dv.X * _contact.Normal.X + _dv.Y * _contact.Normal.Y;
				_normalImpulse = _contact.MassNormal * (0f - (_vn + _contact.BounceVelocity));
				_oldNormalImpulse = _contact.NormalImpulse;
				_contact.NormalImpulse = Math.Max(_oldNormalImpulse + _normalImpulse, 0f);
				_normalImpulse = _contact.NormalImpulse - _oldNormalImpulse;
				_impulse.X = _contact.Normal.X * _normalImpulse;
				_impulse.Y = _contact.Normal.Y * _normalImpulse;
				GeometryB.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.R2);
				_impulse.X *= -1f;
				_impulse.Y *= -1f;
				GeometryA.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.R1);
				GeometryA.body.GetVelocityBiasAtWorldOffset(ref _contact.R1, out _vec1);
				GeometryB.body.GetVelocityBiasAtWorldOffset(ref _contact.R2, out _vec2);
				_dv.X = _vec2.X - _vec1.X;
				_dv.Y = _vec2.Y - _vec1.Y;
				_normalVelocityBias = _dv.X * _contact.Normal.X + _dv.Y * _contact.Normal.Y;
				_normalImpulseBias = _contact.MassNormal * (0f - _normalVelocityBias + _contact.NormalVelocityBias);
				_normalImpulseBiasOriginal = _contact.NormalImpulseBias;
				_contact.NormalImpulseBias = Math.Max(_normalImpulseBiasOriginal + _normalImpulseBias, 0f);
				_normalImpulseBias = _contact.NormalImpulseBias - _normalImpulseBiasOriginal;
				_impulseBias.X = _contact.Normal.X * _normalImpulseBias;
				_impulseBias.Y = _contact.Normal.Y * _normalImpulseBias;
				GeometryB.body.ApplyBiasImpulseAtWorldOffset(ref _impulseBias, ref _contact.R2);
				_impulseBias.X *= -1f;
				_impulseBias.Y *= -1f;
				GeometryA.body.ApplyBiasImpulseAtWorldOffset(ref _impulseBias, ref _contact.R1);
				GeometryA.body.GetVelocityAtWorldOffset(ref _contact.R1, out _vec1);
				GeometryB.body.GetVelocityAtWorldOffset(ref _contact.R2, out _vec2);
				_dv.X = _vec2.X - _vec1.X;
				_dv.Y = _vec2.Y - _vec1.Y;
				_maxTangentImpulse = _frictionCoefficientCombined * _contact.NormalImpulse;
				_float1 = 1f;
				_tangent.X = _float1 * _contact.Normal.Y;
				_tangent.Y = (0f - _float1) * _contact.Normal.X;
				_vt = _dv.X * _tangent.X + _dv.Y * _tangent.Y;
				_tangentImpulse = _contact.MassTangent * (0f - _vt);
				_oldTangentImpulse = _contact.TangentImpulse;
				_contact.TangentImpulse = Calculator.Clamp(_oldTangentImpulse + _tangentImpulse, 0f - _maxTangentImpulse, _maxTangentImpulse);
				_tangentImpulse = _contact.TangentImpulse - _oldTangentImpulse;
				_impulse.X = _tangent.X * _tangentImpulse;
				_impulse.Y = _tangent.Y * _tangentImpulse;
				GeometryB.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.R2);
				_impulse.X *= -1f;
				_impulse.Y *= -1f;
				GeometryA.body.ApplyImpulseAtWorldOffset(ref _impulse, ref _contact.R1);
				_contactList[i] = _contact;
			}
		}
	}

	internal bool ContainsDisposedGeom()
	{
		return GeometryA.IsDisposed || GeometryB.IsDisposed || (GeometryA.body.isStatic && GeometryB.body.isStatic);
	}

	internal void Reset()
	{
		GeometryA = null;
		GeometryB = null;
		_contactList.Clear();
		_newContactList.Clear();
		_mergedContactList.Clear();
	}

	internal void Collide()
	{
		_newContactList.Clear();
		if (!GeomA.body.Enabled && !GeomB.body.Enabled)
		{
			_mergedContactList.Clear();
			_contactList.Clear();
			return;
		}
		Collide(GeomA, GeomB, _newContactList);
		_mergedContactList.Clear();
		for (int i = 0; i < _newContactList.Count; i++)
		{
			int num = _contactList.IndexOfSafe(_newContactList[i]);
			if (num > -1)
			{
				Contact item = _newContactList[i];
				item.NormalImpulse = _contactList[num].NormalImpulse;
				item.TangentImpulse = _contactList[num].TangentImpulse;
				_mergedContactList.Add(item);
			}
			else
			{
				_mergedContactList.Add(_newContactList[i]);
			}
		}
		_contactList.Clear();
		for (int i = 0; i < _mergedContactList.Count; i++)
		{
			_contactList.Add(_mergedContactList[i]);
		}
	}

	private void Collide(Geom geometry1, Geom geometry2, ContactList contactList)
	{
		int num = -1;
		for (int i = 0; i < geometry2.worldVertices.Count; i++)
		{
			if (contactList.Count == _physicsSimulator.maxContactsToDetect)
			{
				break;
			}
			if (geometry1.grid == null)
			{
				break;
			}
			num++;
			_vertRef = geometry2.WorldVertices[i];
			geometry1.TransformToLocalCoordinates(ref _vertRef, out _localVertex);
			if (geometry1.Intersect(ref _localVertex, out _feature) && _feature.Distance < 0f)
			{
				geometry1.TransformNormalToWorld(ref _feature.Normal, out _feature.Normal);
				Contact item = new Contact(geometry2.WorldVertices[i], _feature.Normal, _feature.Distance, new ContactId(2, num, 1));
				contactList.Add(item);
			}
		}
		for (int i = 0; i < geometry1.WorldVertices.Count; i++)
		{
			if (contactList.Count == _physicsSimulator.maxContactsToDetect)
			{
				break;
			}
			if (geometry2.grid == null)
			{
				break;
			}
			num++;
			_vertRef = geometry1.WorldVertices[i];
			geometry2.TransformToLocalCoordinates(ref _vertRef, out _localVertex);
			if (geometry2.Intersect(ref _localVertex, out _feature) && _feature.Distance < 0f)
			{
				geometry2.TransformNormalToWorld(ref _feature.Normal, out _feature.Normal);
				_feature.Normal = -_feature.Normal;
				Contact item = new Contact(geometry1.WorldVertices[i], _feature.Normal, _feature.Distance, new ContactId(2, num, 1));
				contactList.Add(item);
			}
		}
		contactList.Sort(_contactComparer);
		int count = contactList.Count;
		if (count > _physicsSimulator.maxContactsToResolve)
		{
			contactList.RemoveRange(_physicsSimulator.maxContactsToResolve, count - _physicsSimulator.maxContactsToResolve);
		}
		if (geometry1.OnCollision != null && contactList.Count > 0 && !geometry1.OnCollision(geometry1, geometry2, contactList))
		{
			contactList.Clear();
		}
		if (geometry2.OnCollision != null && contactList.Count > 0 && !geometry2.OnCollision(geometry2, geometry1, contactList))
		{
			contactList.Clear();
		}
	}

	private static int CompareSeperation(Contact c1, Contact c2)
	{
		if (c1.Separation < c2.Separation)
		{
			return -1;
		}
		if (c1.Separation == c2.Separation)
		{
			return 0;
		}
		return 1;
	}

	private void InitializeContactLists(int maxContactsToDetect)
	{
		if (_contactList == null)
		{
			_contactList = new ContactList(maxContactsToDetect);
		}
		if (_newContactList == null)
		{
			_newContactList = new ContactList(maxContactsToDetect);
		}
		if (_mergedContactList == null)
		{
			_mergedContactList = new ContactList(maxContactsToDetect);
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Arbiter))
		{
			return false;
		}
		return Equals((Arbiter)obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Arbiter arbiter1, Arbiter arbiter2)
	{
		return arbiter1.Equals(arbiter2);
	}

	public static bool operator !=(Arbiter arbiter1, Arbiter arbiter2)
	{
		return !arbiter1.Equals(arbiter2);
	}
}

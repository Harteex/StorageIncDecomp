#define DEBUG
using System;
using System.Diagnostics;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class RevoluteJoint : Joint
{
	private Vector2 _accumulatedImpulse;

	private Vector2 _anchor;

	private Matrix _b;

	private Body _body1;

	private Body _body2;

	private Vector2 _currentAnchor;

	private Vector2 _dv;

	private Vector2 _dvBias;

	private Vector2 _impulse;

	private Vector2 _localAnchor1;

	private Vector2 _localAnchor2;

	private Matrix _matrix;

	private Vector2 _r1;

	private Vector2 _r2;

	private Vector2 _velocityBias;

	private float _body1InverseMass;

	private float _body1InverseMomentOfInertia;

	private Matrix _body1MatrixTemp;

	private float _body2InverseMass;

	private float _body2InverseMomentOfInertia;

	private Matrix _body2MatrixTemp;

	private float _floatTemp1;

	private Matrix _k;

	private Matrix _k1;

	private Matrix _k2;

	private Matrix _k3;

	private Vector2 _vectorTemp1 = Vector2.Zero;

	private Vector2 _vectorTemp2 = Vector2.Zero;

	private Vector2 _vectorTemp3 = Vector2.Zero;

	private Vector2 _vectorTemp4 = Vector2.Zero;

	public Body Body1
	{
		get
		{
			return _body1;
		}
		set
		{
			_body1 = value;
		}
	}

	public Body Body2
	{
		get
		{
			return _body2;
		}
		set
		{
			_body2 = value;
		}
	}

	public Vector2 Anchor
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _anchor;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_anchor = value;
			_body1.GetLocalPosition(ref _anchor, out _localAnchor1);
			_body2.GetLocalPosition(ref _anchor, out _localAnchor2);
		}
	}

	public Vector2 CurrentAnchor
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			Vector2.Add(ref _body1.position, ref _r1, ref _currentAnchor);
			return _currentAnchor;
		}
	}

	public RevoluteJoint()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	public RevoluteJoint(Body body1, Body body2, Vector2 anchor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		_body1 = body1;
		_body2 = body2;
		_anchor = anchor;
		body1.GetLocalPosition(ref anchor, out _localAnchor1);
		body2.GetLocalPosition(ref anchor, out _localAnchor2);
		_accumulatedImpulse = Vector2.Zero;
	}

	public void SetInitialAnchor(Vector2 initialAnchor)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		_anchor = initialAnchor;
		if (_body1 == null)
		{
			throw new ArgumentNullException("initialAnchor", "Body must be set prior to setting the _anchor of the Revolute Joint");
		}
		_body1.GetLocalPosition(ref initialAnchor, out _localAnchor1);
		_body2.GetLocalPosition(ref initialAnchor, out _localAnchor2);
	}

	public override void Validate()
	{
		if (_body1.IsDisposed || _body2.IsDisposed)
		{
			Dispose();
		}
	}

	public override void PreStep(float inverseDt)
	{
		if (!IsDisposed)
		{
			_body1InverseMass = _body1.inverseMass;
			_body1InverseMomentOfInertia = _body1.inverseMomentOfInertia;
			_body2InverseMass = _body2.inverseMass;
			_body2InverseMomentOfInertia = _body2.inverseMomentOfInertia;
			_body1.GetBodyMatrix(out _body1MatrixTemp);
			_body2.GetBodyMatrix(out _body2MatrixTemp);
			Vector2.TransformNormal(ref _localAnchor1, ref _body1MatrixTemp, ref _r1);
			Vector2.TransformNormal(ref _localAnchor2, ref _body2MatrixTemp, ref _r2);
			_k1.M11 = _body1InverseMass + _body2InverseMass;
			_k1.M12 = 0f;
			_k1.M21 = 0f;
			_k1.M22 = _body1InverseMass + _body2InverseMass;
			_k2.M11 = _body1InverseMomentOfInertia * _r1.Y * _r1.Y;
			_k2.M12 = (0f - _body1InverseMomentOfInertia) * _r1.X * _r1.Y;
			_k2.M21 = (0f - _body1InverseMomentOfInertia) * _r1.X * _r1.Y;
			_k2.M22 = _body1InverseMomentOfInertia * _r1.X * _r1.X;
			_k3.M11 = _body2InverseMomentOfInertia * _r2.Y * _r2.Y;
			_k3.M12 = (0f - _body2InverseMomentOfInertia) * _r2.X * _r2.Y;
			_k3.M21 = (0f - _body2InverseMomentOfInertia) * _r2.X * _r2.Y;
			_k3.M22 = _body2InverseMomentOfInertia * _r2.X * _r2.X;
			Matrix.Add(ref _k1, ref _k2, ref _k);
			Matrix.Add(ref _k, ref _k3, ref _k);
			ref Matrix k = ref _k;
			k.M11 += Softness;
			ref Matrix k2 = ref _k;
			k2.M12 += Softness;
			MatrixInvert2D(ref _k, out _matrix);
			Vector2.Add(ref _body1.position, ref _r1, ref _vectorTemp1);
			Vector2.Add(ref _body2.position, ref _r2, ref _vectorTemp2);
			Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp1, ref _vectorTemp3);
			Vector2.Multiply(ref _vectorTemp3, (0f - BiasFactor) * inverseDt, ref _velocityBias);
			JointError = _vectorTemp3.Length();
			_body2.ApplyImmediateImpulse(ref _accumulatedImpulse);
			Calculator.Cross(ref _r2, ref _accumulatedImpulse, out _floatTemp1);
			_body2.ApplyAngularImpulse(_floatTemp1);
			Vector2.Multiply(ref _accumulatedImpulse, -1f, ref _vectorTemp1);
			_body1.ApplyImmediateImpulse(ref _vectorTemp1);
			Calculator.Cross(ref _r1, ref _accumulatedImpulse, out _floatTemp1);
			_body1.ApplyAngularImpulse(0f - _floatTemp1);
		}
	}

	private void MatrixInvert2D(ref Matrix matrix, out Matrix invertedMatrix)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		float m = matrix.M11;
		float m2 = matrix.M12;
		float m3 = matrix.M21;
		float m4 = matrix.M22;
		float num = m * m4 - m2 * m3;
		Debug.Assert(num != 0f);
		num = 1f / num;
		_b.M11 = num * m4;
		_b.M12 = (0f - num) * m2;
		_b.M21 = (0f - num) * m3;
		_b.M22 = num * m;
		invertedMatrix = _b;
	}

	public override void Update()
	{
		base.Update();
		if (!IsDisposed)
		{
			_vectorTemp1.X = (0f - _body2.AngularVelocity) * _r2.Y;
			_vectorTemp1.Y = _body2.AngularVelocity * _r2.X;
			_vectorTemp2.X = (0f - _body1.AngularVelocity) * _r1.Y;
			_vectorTemp2.Y = _body1.AngularVelocity * _r1.X;
			_vectorTemp3.X = _body2.LinearVelocity.X + _vectorTemp1.X;
			_vectorTemp3.Y = _body2.LinearVelocity.Y + _vectorTemp1.Y;
			_vectorTemp4.X = _body1.LinearVelocity.X + _vectorTemp2.X;
			_vectorTemp4.Y = _body1.LinearVelocity.Y + _vectorTemp2.Y;
			_dv.X = _vectorTemp3.X - _vectorTemp4.X;
			_dv.Y = _vectorTemp3.Y - _vectorTemp4.Y;
			_vectorTemp1.X = _velocityBias.X - _dv.X;
			_vectorTemp1.Y = _velocityBias.Y - _dv.Y;
			_vectorTemp2.X = _accumulatedImpulse.X * Softness;
			_vectorTemp2.Y = _accumulatedImpulse.Y * Softness;
			_dvBias.X = _vectorTemp1.X - _vectorTemp2.X;
			_dvBias.Y = _vectorTemp1.Y - _vectorTemp2.Y;
			float x = _dvBias.X * _matrix.M11 + _dvBias.Y * _matrix.M21 + _matrix.M41;
			float y = _dvBias.X * _matrix.M12 + _dvBias.Y * _matrix.M22 + _matrix.M42;
			_impulse.X = x;
			_impulse.Y = y;
			_dv.X = _impulse.X * _body2.inverseMass;
			_dv.Y = _impulse.Y * _body2.inverseMass;
			_body2.LinearVelocity.X = _dv.X + _body2.LinearVelocity.X;
			_body2.LinearVelocity.Y = _dv.Y + _body2.LinearVelocity.Y;
			_floatTemp1 = _r2.X * _impulse.Y - _r2.Y * _impulse.X;
			_body2.AngularVelocity += _floatTemp1 * _body2.inverseMomentOfInertia;
			_vectorTemp1.X = _impulse.X * -1f;
			_vectorTemp1.Y = _impulse.Y * -1f;
			_dv.X = _vectorTemp1.X * _body1.inverseMass;
			_dv.Y = _vectorTemp1.Y * _body1.inverseMass;
			_body1.LinearVelocity.X = _dv.X + _body1.LinearVelocity.X;
			_body1.LinearVelocity.Y = _dv.Y + _body1.LinearVelocity.Y;
			_floatTemp1 = _r1.X * _impulse.Y - _r1.Y * _impulse.X;
			_floatTemp1 = 0f - _floatTemp1;
			_body1.AngularVelocity += _floatTemp1 * _body1.inverseMomentOfInertia;
			_accumulatedImpulse.X += _impulse.X;
			_accumulatedImpulse.Y += _impulse.Y;
		}
	}
}

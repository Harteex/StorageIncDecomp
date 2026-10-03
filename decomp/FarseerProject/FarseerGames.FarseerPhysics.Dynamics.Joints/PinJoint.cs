using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class PinJoint : Joint
{
	private float _accumulatedImpulse;

	private Vector2 _anchor1;

	private Vector2 _anchor2;

	private Body _body1;

	private Body _body2;

	private float _effectiveMass;

	private Vector2 _r1;

	private Vector2 _r2;

	private float _targetDistance;

	private float _velocityBias;

	private Vector2 _worldAnchor1;

	private Vector2 _worldAnchor2;

	private Vector2 _worldAnchorDifferenceNormalized;

	private Vector2 _angularVelocityComponent1;

	private Vector2 _angularVelocityComponent2;

	private Vector2 _dv;

	private float _dvNormal;

	private Vector2 _impulse;

	private float _impulseMagnitude;

	private Vector2 _velocity1;

	private Vector2 _velocity2;

	private Vector2 _accumulatedImpulseVector;

	private float _angularImpulse;

	private Matrix _body1MatrixTemp;

	private Matrix _body2MatrixTemp;

	private float _kNormal;

	private float _r1cn;

	private float _r2cn;

	private Vector2 _worldAnchorDifference;

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

	public float TargetDistance
	{
		get
		{
			return _targetDistance;
		}
		set
		{
			_targetDistance = value;
		}
	}

	public Vector2 Anchor1
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _anchor1;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_anchor1 = value;
			_body1.GetBodyMatrix(out _body1MatrixTemp);
			Vector2.TransformNormal(ref _anchor1, ref _body1MatrixTemp, ref _r1);
			Vector2.Add(ref _body1.position, ref _r1, ref _worldAnchor1);
		}
	}

	public Vector2 Anchor2
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _anchor2;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_anchor2 = value;
			_body2.GetBodyMatrix(out _body2MatrixTemp);
			Vector2.TransformNormal(ref _anchor2, ref _body2MatrixTemp, ref _r2);
			Vector2.Add(ref _body2.position, ref _r2, ref _worldAnchor2);
		}
	}

	public Vector2 WorldAnchor1
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _worldAnchor1;
		}
	}

	public Vector2 WorldAnchor2
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _worldAnchor2;
		}
	}

	public PinJoint()
	{
	}

	public PinJoint(Body body1, Vector2 anchor1, Body body2, Vector2 anchor2)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_body1 = body1;
		_body2 = body2;
		Vector2 val = body2.GetWorldPosition(anchor2) - body1.GetWorldPosition(anchor1);
		_targetDistance = val.Length();
		Anchor1 = anchor1;
		Anchor2 = anchor2;
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
			_body1.GetBodyMatrix(out _body1MatrixTemp);
			_body2.GetBodyMatrix(out _body2MatrixTemp);
			Vector2.TransformNormal(ref _anchor1, ref _body1MatrixTemp, ref _r1);
			Vector2.TransformNormal(ref _anchor2, ref _body2MatrixTemp, ref _r2);
			Vector2.Add(ref _body1.position, ref _r1, ref _worldAnchor1);
			Vector2.Add(ref _body2.position, ref _r2, ref _worldAnchor2);
			Vector2.Subtract(ref _worldAnchor2, ref _worldAnchor1, ref _worldAnchorDifference);
			float num = _worldAnchorDifference.Length();
			JointError = num - _targetDistance;
			Vector2.Multiply(ref _worldAnchorDifference, 1f / ((num != 0f) ? num : float.PositiveInfinity), ref _worldAnchorDifferenceNormalized);
			_velocityBias = (0f - BiasFactor) * inverseDt * (num - _targetDistance);
			Calculator.Cross(ref _r1, ref _worldAnchorDifferenceNormalized, out _r1cn);
			Calculator.Cross(ref _r2, ref _worldAnchorDifferenceNormalized, out _r2cn);
			_kNormal = _body1.inverseMass + _body2.inverseMass + _body1.inverseMomentOfInertia * _r1cn * _r1cn + _body2.inverseMomentOfInertia * _r2cn * _r2cn;
			_effectiveMass = 1f / (_kNormal + Softness);
			Vector2.Multiply(ref _worldAnchorDifferenceNormalized, _accumulatedImpulse, ref _accumulatedImpulseVector);
			_body2.ApplyImmediateImpulse(ref _accumulatedImpulseVector);
			Calculator.Cross(ref _r2, ref _accumulatedImpulseVector, out _angularImpulse);
			_body2.ApplyAngularImpulse(_angularImpulse);
			Vector2.Multiply(ref _accumulatedImpulseVector, -1f, ref _accumulatedImpulseVector);
			_body1.ApplyImmediateImpulse(ref _accumulatedImpulseVector);
			Calculator.Cross(ref _r1, ref _accumulatedImpulseVector, out _angularImpulse);
			_body1.ApplyAngularImpulse(_angularImpulse);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!IsDisposed)
		{
			Calculator.Cross(ref _body1.AngularVelocity, ref _r1, out _angularVelocityComponent1);
			Vector2.Add(ref _body1.LinearVelocity, ref _angularVelocityComponent1, ref _velocity1);
			Calculator.Cross(ref _body2.AngularVelocity, ref _r2, out _angularVelocityComponent2);
			Vector2.Add(ref _body2.LinearVelocity, ref _angularVelocityComponent2, ref _velocity2);
			Vector2.Subtract(ref _velocity2, ref _velocity1, ref _dv);
			Vector2.Dot(ref _dv, ref _worldAnchorDifferenceNormalized, ref _dvNormal);
			_impulseMagnitude = (_velocityBias - _dvNormal - Softness * _accumulatedImpulse) * _effectiveMass;
			Vector2.Multiply(ref _worldAnchorDifferenceNormalized, _impulseMagnitude, ref _impulse);
			_body2.ApplyImmediateImpulse(ref _impulse);
			Calculator.Cross(ref _r2, ref _impulse, out _angularImpulse);
			_body2.ApplyAngularImpulse(_angularImpulse);
			Vector2.Multiply(ref _impulse, -1f, ref _impulse);
			_body1.ApplyImmediateImpulse(ref _impulse);
			Calculator.Cross(ref _r1, ref _impulse, out _angularImpulse);
			_body1.ApplyAngularImpulse(_angularImpulse);
			_accumulatedImpulse += _impulseMagnitude;
		}
	}
}

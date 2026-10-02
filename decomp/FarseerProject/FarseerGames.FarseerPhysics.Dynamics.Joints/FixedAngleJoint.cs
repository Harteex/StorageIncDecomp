using System;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class FixedAngleJoint : Joint
{
	private Body _body;

	private float _massFactor;

	private float _maxImpulse = float.MaxValue;

	private float _targetAngle;

	private float _velocityBias;

	public Body Body
	{
		get
		{
			return _body;
		}
		set
		{
			_body = value;
		}
	}

	public float TargetAngle
	{
		get
		{
			return _targetAngle;
		}
		set
		{
			_targetAngle = value;
		}
	}

	public float MaxImpulse
	{
		get
		{
			return _maxImpulse;
		}
		set
		{
			_maxImpulse = value;
		}
	}

	public FixedAngleJoint()
	{
	}

	public FixedAngleJoint(Body body)
	{
		_body = body;
	}

	public FixedAngleJoint(Body body, float targetAngle)
	{
		_body = body;
		_targetAngle = targetAngle;
	}

	public override void Validate()
	{
		if (_body.IsDisposed)
		{
			Dispose();
		}
	}

	public override void PreStep(float inverseDt)
	{
		if (!IsDisposed)
		{
			JointError = _body.totalRotation - _targetAngle;
			_velocityBias = (0f - BiasFactor) * inverseDt * JointError;
			_massFactor = (1f - Softness) / _body.inverseMomentOfInertia;
		}
	}

	public override void Update()
	{
		base.Update();
		if (!IsDisposed)
		{
			float value = (_velocityBias - _body.AngularVelocity) * _massFactor;
			_body.AngularVelocity += _body.inverseMomentOfInertia * (float)Math.Sign(value) * Math.Min(Math.Abs(value), _maxImpulse);
		}
	}
}

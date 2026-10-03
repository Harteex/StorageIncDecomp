using System;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class SliderJoint : Joint
{
	private float _accumulatedImpulse;

	private Vector2 _anchor;

	private Vector2 _anchor1;

	private Vector2 _anchor2;

	private Body _body1;

	private Body _body2;

	private float _effectiveMass;

	private bool _lowerLimitViolated;

	private float _max;

	private float _min;

	private Vector2 _r1;

	private Vector2 _r2;

	private float _slop = 0.01f;

	private bool _upperLimitViolated;

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

	private float _distance;

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

	public float Slop
	{
		get
		{
			return _slop;
		}
		set
		{
			_slop = value;
		}
	}

	public float Min
	{
		get
		{
			return _min;
		}
		set
		{
			_min = value;
		}
	}

	public float Max
	{
		get
		{
			return _max;
		}
		set
		{
			_max = value;
		}
	}

	public Vector2 Anchor1
	{
		get
		{
			return _anchor1;
		}
		set
		{
			_anchor1 = value;
			_body1.GetBodyMatrix(out _body1MatrixTemp);
			Vector2.TransformNormal(ref _anchor1, ref _body1MatrixTemp, out _r1);
			Vector2.Add(ref _body1.position, ref _r1, out _worldAnchor1);
		}
	}

	public Vector2 Anchor2
	{
		get
		{
			return _anchor2;
		}
		set
		{
			_anchor2 = value;
			_body2.GetBodyMatrix(out _body2MatrixTemp);
			Vector2.TransformNormal(ref _anchor2, ref _body2MatrixTemp, out _r2);
			Vector2.Add(ref _body2.position, ref _r2, out _worldAnchor2);
		}
	}

	public Vector2 WorldAnchor1
	{
		get
		{
			return _worldAnchor1;
		}
	}

	public Vector2 WorldAnchor2
	{
		get
		{
			return _worldAnchor2;
		}
	}

	public Vector2 CurrentAnchorPosition
	{
		get
		{
			Vector2.Add(ref _body1.position, ref _r1, out _anchor);
			return _anchor;
		}
	}

	public SliderJoint()
	{
	}

	public SliderJoint(Body body1, Vector2 anchor1, Body body2, Vector2 anchor2, float min, float max)
	{
		_body1 = body1;
		_body2 = body2;
		_anchor1 = anchor1;
		_anchor2 = anchor2;
		_min = min;
		_max = max;
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
		if (IsDisposed)
		{
			return;
		}
		_body1.GetBodyMatrix(out _body1MatrixTemp);
		_body2.GetBodyMatrix(out _body2MatrixTemp);
		Vector2.TransformNormal(ref _anchor1, ref _body1MatrixTemp, out _r1);
		Vector2.TransformNormal(ref _anchor2, ref _body2MatrixTemp, out _r2);
		Vector2.Add(ref _body1.position, ref _r1, out _worldAnchor1);
		Vector2.Add(ref _body2.position, ref _r2, out _worldAnchor2);
		Vector2.Subtract(ref _worldAnchor2, ref _worldAnchor1, out _worldAnchorDifference);
		_distance = _worldAnchorDifference.Length();
		JointError = 0f;
		if (_distance > _max)
		{
			if (_lowerLimitViolated)
			{
				_accumulatedImpulse = 0f;
				_lowerLimitViolated = false;
			}
			_upperLimitViolated = true;
			if (_distance < _max + _slop)
			{
				JointError = 0f;
			}
			else
			{
				JointError = _distance - _max;
			}
		}
		else if (_distance < _min)
		{
			if (_upperLimitViolated)
			{
				_accumulatedImpulse = 0f;
				_upperLimitViolated = false;
			}
			_lowerLimitViolated = true;
			if (_distance > _min - _slop)
			{
				JointError = 0f;
			}
			else
			{
				JointError = _distance - _min;
			}
		}
		else
		{
			_upperLimitViolated = false;
			_lowerLimitViolated = false;
			JointError = 0f;
			_accumulatedImpulse = 0f;
		}
		Vector2.Multiply(ref _worldAnchorDifference, 1f / ((_distance != 0f) ? _distance : float.PositiveInfinity), out _worldAnchorDifferenceNormalized);
		_velocityBias = BiasFactor * inverseDt * JointError;
		Calculator.Cross(ref _r1, ref _worldAnchorDifferenceNormalized, out _r1cn);
		Calculator.Cross(ref _r2, ref _worldAnchorDifferenceNormalized, out _r2cn);
		_kNormal = _body1.inverseMass + _body2.inverseMass + _body1.inverseMomentOfInertia * _r1cn * _r1cn + _body2.inverseMomentOfInertia * _r2cn * _r2cn;
		_effectiveMass = 1f / (_kNormal + Softness);
		Vector2.Multiply(ref _worldAnchorDifferenceNormalized, _accumulatedImpulse, out _accumulatedImpulseVector);
		_body2.ApplyImmediateImpulse(ref _accumulatedImpulseVector);
		Calculator.Cross(ref _r2, ref _accumulatedImpulseVector, out _angularImpulse);
		_body2.ApplyAngularImpulse(_angularImpulse);
		Vector2.Multiply(ref _accumulatedImpulseVector, -1f, out _accumulatedImpulseVector);
		_body1.ApplyImmediateImpulse(ref _accumulatedImpulseVector);
		Calculator.Cross(ref _r1, ref _accumulatedImpulseVector, out _angularImpulse);
		_body1.ApplyAngularImpulse(_angularImpulse);
	}

	public override void Update()
	{
		base.Update();
		if (!IsDisposed && (_upperLimitViolated || _lowerLimitViolated))
		{
			Calculator.Cross(ref _body1.AngularVelocity, ref _r1, out _angularVelocityComponent1);
			Vector2.Add(ref _body1.LinearVelocity, ref _angularVelocityComponent1, out _velocity1);
			Calculator.Cross(ref _body2.AngularVelocity, ref _r2, out _angularVelocityComponent2);
			Vector2.Add(ref _body2.LinearVelocity, ref _angularVelocityComponent2, out _velocity2);
			Vector2.Subtract(ref _velocity2, ref _velocity1, out _dv);
			Vector2.Dot(ref _dv, ref _worldAnchorDifferenceNormalized, out _dvNormal);
			_impulseMagnitude = (0f - _velocityBias - _dvNormal - Softness * _accumulatedImpulse) * _effectiveMass;
			float accumulatedImpulse = _accumulatedImpulse;
			if (_upperLimitViolated)
			{
				_accumulatedImpulse = Math.Min(accumulatedImpulse + _impulseMagnitude, 0f);
			}
			else if (_lowerLimitViolated)
			{
				_accumulatedImpulse = Math.Max(accumulatedImpulse + _impulseMagnitude, 0f);
			}
			_impulseMagnitude = _accumulatedImpulse - accumulatedImpulse;
			Vector2.Multiply(ref _worldAnchorDifferenceNormalized, _impulseMagnitude, out _impulse);
			_body2.ApplyImmediateImpulse(ref _impulse);
			Calculator.Cross(ref _r2, ref _impulse, out _angularImpulse);
			_body2.ApplyAngularImpulse(_angularImpulse);
			Vector2.Multiply(ref _impulse, -1f, out _impulse);
			_body1.ApplyImmediateImpulse(ref _impulse);
			Calculator.Cross(ref _r1, ref _impulse, out _angularImpulse);
			_body1.ApplyAngularImpulse(_angularImpulse);
		}
	}
}

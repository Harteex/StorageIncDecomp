using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class AngleLimitJoint : Joint
{
	private float _accumlatedAngularImpulseOld;

	private float _accumulatedAngularImpulse;

	private float _angularImpulse;

	private Body _body1;

	private Body _body2;

	private float _difference;

	private float _lowerLimit;

	private bool _lowerLimitViolated;

	private float _massFactor;

	private float _slop = 0.01f;

	private float _upperLimit;

	private bool _upperLimitViolated;

	private float _velocityBias;

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

	public float UpperLimit
	{
		get
		{
			return _upperLimit;
		}
		set
		{
			_upperLimit = value;
		}
	}

	public float LowerLimit
	{
		get
		{
			return _lowerLimit;
		}
		set
		{
			_lowerLimit = value;
		}
	}

	public AngleLimitJoint()
	{
	}

	public AngleLimitJoint(Body body1, Body body2, float lowerLimit, float upperLimit)
	{
		_body1 = body1;
		_body2 = body2;
		_lowerLimit = lowerLimit;
		_upperLimit = upperLimit;
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
		_difference = _body2.totalRotation - _body1.totalRotation;
		JointError = 0f;
		if (_difference > _upperLimit)
		{
			if (_lowerLimitViolated)
			{
				_accumulatedAngularImpulse = 0f;
				_lowerLimitViolated = false;
			}
			_upperLimitViolated = true;
			if (_difference < _upperLimit + _slop)
			{
				JointError = 0f;
			}
			else
			{
				JointError = _difference - _upperLimit;
			}
		}
		else if (_difference < _lowerLimit)
		{
			if (_upperLimitViolated)
			{
				_accumulatedAngularImpulse = 0f;
				_upperLimitViolated = false;
			}
			_lowerLimitViolated = true;
			if (_difference > _lowerLimit - _slop)
			{
				JointError = 0f;
			}
			else
			{
				JointError = _difference - _lowerLimit;
			}
		}
		else
		{
			_upperLimitViolated = false;
			_lowerLimitViolated = false;
			JointError = 0f;
			_accumulatedAngularImpulse = 0f;
		}
		_velocityBias = BiasFactor * inverseDt * JointError;
		_massFactor = 1f / (Softness + _body1.inverseMomentOfInertia + _body2.inverseMomentOfInertia);
		_body1.AngularVelocity -= _body1.inverseMomentOfInertia * _accumulatedAngularImpulse;
		_body2.AngularVelocity += _body2.inverseMomentOfInertia * _accumulatedAngularImpulse;
	}

	public override void Update()
	{
		base.Update();
		if (!IsDisposed && (_upperLimitViolated || _lowerLimitViolated))
		{
			_angularImpulse = 0f;
			_angularImpulse = (0f - (_velocityBias + (_body2.AngularVelocity - _body1.AngularVelocity) + Softness * _accumulatedAngularImpulse)) * _massFactor;
			_accumlatedAngularImpulseOld = _accumulatedAngularImpulse;
			if (_upperLimitViolated)
			{
				_accumulatedAngularImpulse = MathHelper.Min(_accumlatedAngularImpulseOld + _angularImpulse, 0f);
			}
			else if (_lowerLimitViolated)
			{
				_accumulatedAngularImpulse = MathHelper.Max(_accumlatedAngularImpulseOld + _angularImpulse, 0f);
			}
			_angularImpulse = _accumulatedAngularImpulse - _accumlatedAngularImpulseOld;
			_body1.AngularVelocity -= _body1.inverseMomentOfInertia * _angularImpulse;
			_body2.AngularVelocity += _body2.inverseMomentOfInertia * _angularImpulse;
		}
	}
}

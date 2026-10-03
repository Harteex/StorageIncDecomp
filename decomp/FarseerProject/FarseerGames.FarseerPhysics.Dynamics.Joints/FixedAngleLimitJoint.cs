using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class FixedAngleLimitJoint : Joint
{
	private float _accumlatedAngularImpulseOld;

	private float _accumulatedAngularImpulse;

	private float _angularImpulse;

	private Body _body;

	private float _difference;

	private float _lowerLimit;

	private bool _lowerLimitViolated;

	private float _massFactor;

	private float _slop = 0.01f;

	private float _upperLimit;

	private bool _upperLimitViolated;

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

	public FixedAngleLimitJoint()
	{
	}

	public FixedAngleLimitJoint(Body body, float lowerLimit, float upperLimit)
	{
		_body = body;
		_lowerLimit = lowerLimit;
		_upperLimit = upperLimit;
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
		if (IsDisposed)
		{
			return;
		}
		_difference = _body.totalRotation;
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
			_velocityBias = BiasFactor * inverseDt * JointError;
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
		_massFactor = (1f - Softness) / _body.inverseMomentOfInertia;
		_body.AngularVelocity += _body.inverseMomentOfInertia * _accumulatedAngularImpulse;
	}

	public override void Update()
	{
		base.Update();
		if (!IsDisposed && (_upperLimitViolated || _lowerLimitViolated))
		{
			_angularImpulse = 0f;
			_angularImpulse = (0f - (_velocityBias + _body.AngularVelocity + Softness * _accumulatedAngularImpulse)) * _massFactor;
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
			_body.AngularVelocity += _body.inverseMomentOfInertia * _angularImpulse;
		}
	}
}

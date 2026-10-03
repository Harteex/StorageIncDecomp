using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public class FixedLinearSpring : Spring
{
	private const float _epsilon = 1E-05f;

	private Body _body;

	private Vector2 _bodyAttachPoint;

	private Vector2 _difference = Vector2.Zero;

	private float _restLength;

	private Vector2 _worldAttachPoint;

	private Vector2 _bodyVelocity = Vector2.Zero;

	private Vector2 _bodyWorldPoint = Vector2.Zero;

	private float _dampningForce;

	private Vector2 _differenceNormalized;

	private Vector2 _force;

	private float _springForce;

	private float _temp;

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

	public Vector2 Position
	{
		get
		{
			return _worldAttachPoint;
		}
		set
		{
			_worldAttachPoint = value;
		}
	}

	public Vector2 BodyAttachPoint
	{
		get
		{
			return _bodyAttachPoint;
		}
		set
		{
			_bodyAttachPoint = value;
		}
	}

	public Vector2 WorldAttachPoint
	{
		get
		{
			return _worldAttachPoint;
		}
		set
		{
			_worldAttachPoint = value;
		}
	}

	public float RestLength
	{
		get
		{
			return _restLength;
		}
		set
		{
			_restLength = value;
		}
	}

	public FixedLinearSpring()
	{
	}

	public FixedLinearSpring(Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampingConstant)
	{
		_body = body;
		_bodyAttachPoint = bodyAttachPoint;
		_worldAttachPoint = worldAttachPoint;
		SpringConstant = springConstant;
		DampingConstant = dampingConstant;
		_difference = worldAttachPoint - _body.GetWorldPosition(bodyAttachPoint);
		_restLength = _difference.Length();
	}

	public override void Validate()
	{
		if (_body.IsDisposed)
		{
			Dispose();
		}
	}

	public override void Update(float dt)
	{
		base.Update(dt);
		if (!IsDisposed && !_body.isStatic)
		{
			_body.GetWorldPosition(ref _bodyAttachPoint, out _bodyWorldPoint);
			Vector2.Subtract(ref _bodyWorldPoint, ref _worldAttachPoint, out _difference);
			float num = _difference.Length();
			if (!(num < 1E-05f))
			{
				SpringError = num - _restLength;
				Vector2.Normalize(ref _difference, out _differenceNormalized);
				_springForce = SpringConstant * SpringError;
				_body.GetVelocityAtLocalPoint(ref _bodyAttachPoint, out _bodyVelocity);
				Vector2.Dot(ref _bodyVelocity, ref _difference, out _temp);
				_dampningForce = DampingConstant * _temp / num;
				Vector2.Multiply(ref _differenceNormalized, 0f - (_springForce + _dampningForce), out _force);
				_body.ApplyForceAtLocalPoint(ref _force, ref _bodyAttachPoint);
			}
		}
	}
}

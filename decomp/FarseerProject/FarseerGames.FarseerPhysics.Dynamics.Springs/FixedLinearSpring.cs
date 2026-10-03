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
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _worldAttachPoint;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_worldAttachPoint = value;
		}
	}

	public Vector2 BodyAttachPoint
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _bodyAttachPoint;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_bodyAttachPoint = value;
		}
	}

	public Vector2 WorldAttachPoint
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _worldAttachPoint;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	public FixedLinearSpring(Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampingConstant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
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
			Vector2.Subtract(ref _bodyWorldPoint, ref _worldAttachPoint, ref _difference);
			float num = _difference.Length();
			if (!(num < 1E-05f))
			{
				SpringError = num - _restLength;
				Vector2.Normalize(ref _difference, ref _differenceNormalized);
				_springForce = SpringConstant * SpringError;
				_body.GetVelocityAtLocalPoint(ref _bodyAttachPoint, out _bodyVelocity);
				Vector2.Dot(ref _bodyVelocity, ref _difference, ref _temp);
				_dampningForce = DampingConstant * _temp / num;
				Vector2.Multiply(ref _differenceNormalized, 0f - (_springForce + _dampningForce), ref _force);
				_body.ApplyForceAtLocalPoint(ref _force, ref _bodyAttachPoint);
			}
		}
	}
}

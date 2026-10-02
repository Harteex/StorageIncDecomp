using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Controllers;

public class InactivityController : Controller
{
	private float _activationDistance = 100f;

	private int _bodiesEnabled;

	private Vector2 _difference = Vector2.Zero;

	private float _maxIdleTime = 1000f;

	private PhysicsSimulator _physicsSimulator;

	public int BodiesEnabled => _bodiesEnabled;

	public float ActivationDistance
	{
		get
		{
			return _activationDistance;
		}
		set
		{
			_activationDistance = value;
		}
	}

	public float MaxIdleTime
	{
		get
		{
			return _maxIdleTime;
		}
		set
		{
			_maxIdleTime = value;
		}
	}

	public InactivityController(PhysicsSimulator physicsSimulator)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_physicsSimulator = physicsSimulator;
		_physicsSimulator.Add(this);
		Enabled = false;
	}

	public override void Validate()
	{
	}

	public override void Update(float dt)
	{
		if (IsDisposed)
		{
			return;
		}
		float num = dt * 1000f;
		_bodiesEnabled = 0;
		foreach (Body body in _physicsSimulator.BodyList)
		{
			if (body.IsStatic || !body.Enabled)
			{
				continue;
			}
			_bodiesEnabled++;
			if (!body.Moves && body.IsAutoIdle)
			{
				body.IdleTime += num;
				if (body.IdleTime >= _maxIdleTime)
				{
					body.Enabled = false;
				}
				continue;
			}
			body.IdleTime = 0f;
			foreach (Body body2 in _physicsSimulator.bodyList)
			{
				if (!body2.Enabled && body2.IsAutoIdle && IsInActivationDistance(body, body2))
				{
					body2.Enabled = true;
					body2.IdleTime = 0f;
				}
			}
		}
	}

	public bool IsInActivationDistance(Body b1, Body b2)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		_difference = b1.position - b2.position;
		float num = _difference.Length();
		if (num < 0f)
		{
			_difference *= -1f;
		}
		return num <= _activationDistance;
	}
}

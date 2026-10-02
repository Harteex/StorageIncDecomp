using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Factories;

public class SpringFactory
{
	private static SpringFactory _instance;

	public static SpringFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SpringFactory();
			}
			return _instance;
		}
	}

	private SpringFactory()
	{
	}

	public LinearSpring CreateLinearSpring(PhysicsSimulator physicsSimulator, Body body1, Vector2 attachPoint1, Body body2, Vector2 attachPoint2, float springConstant, float dampingConstant)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		LinearSpring linearSpring = CreateLinearSpring(body1, attachPoint1, body2, attachPoint2, springConstant, dampingConstant);
		physicsSimulator.Add(linearSpring);
		return linearSpring;
	}

	public LinearSpring CreateLinearSpring(Body body1, Vector2 attachPoint1, Body body2, Vector2 attachPoint2, float springConstant, float dampingConstant)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return new LinearSpring(body1, attachPoint1, body2, attachPoint2, springConstant, dampingConstant);
	}

	public FixedLinearSpring CreateFixedLinearSpring(PhysicsSimulator physicsSimulator, Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampingConstant)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		FixedLinearSpring fixedLinearSpring = CreateFixedLinearSpring(body, bodyAttachPoint, worldAttachPoint, springConstant, dampingConstant);
		physicsSimulator.Add(fixedLinearSpring);
		return fixedLinearSpring;
	}

	public FixedLinearSpring CreateFixedLinearSpring(Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampingConstant)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return new FixedLinearSpring(body, bodyAttachPoint, worldAttachPoint, springConstant, dampingConstant);
	}

	public AngleSpring CreateAngleSpring(PhysicsSimulator physicsSimulator, Body body1, Body body2, float springConstant, float dampingConstant)
	{
		AngleSpring angleSpring = CreateAngleSpring(body1, body2, springConstant, dampingConstant);
		physicsSimulator.Add(angleSpring);
		return angleSpring;
	}

	public AngleSpring CreateAngleSpring(Body body1, Body body2, float springConstant, float dampingConstant)
	{
		return new AngleSpring(body1, body2, springConstant, dampingConstant);
	}

	public FixedAngleSpring CreateFixedAngleSpring(PhysicsSimulator physicsSimulator, Body body, float springConstant, float dampingConstant)
	{
		FixedAngleSpring fixedAngleSpring = CreateFixedAngleSpring(body, springConstant, dampingConstant);
		physicsSimulator.Add(fixedAngleSpring);
		return fixedAngleSpring;
	}

	public FixedAngleSpring CreateFixedAngleSpring(Body body, float springConstant, float dampingConstant)
	{
		return new FixedAngleSpring(body, springConstant, dampingConstant);
	}
}

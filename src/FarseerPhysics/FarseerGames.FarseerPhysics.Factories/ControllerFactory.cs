using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Factories;

public class ControllerFactory
{
	private static ControllerFactory _instance;

	public static ControllerFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ControllerFactory();
			}
			return _instance;
		}
	}

	private ControllerFactory()
	{
	}

	public LinearSpring CreateLinearSpring(PhysicsSimulator physicsSimulator, Body body1, Vector2 attachPoint1, Body body2, Vector2 attachPoint2, float springConstant, float dampningConstant)
	{
		LinearSpring linearSpring = CreateLinearSpring(body1, attachPoint1, body2, attachPoint2, springConstant, dampningConstant);
		physicsSimulator.Add(linearSpring);
		return linearSpring;
	}

	public LinearSpring CreateLinearSpring(Body body1, Vector2 attachPoint1, Body body2, Vector2 attachPoint2, float springConstant, float dampningConstant)
	{
		return new LinearSpring(body1, attachPoint1, body2, attachPoint2, springConstant, dampningConstant);
	}

	public FixedLinearSpring CreateFixedLinearSpring(PhysicsSimulator physicsSimulator, Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampningConstant)
	{
		FixedLinearSpring fixedLinearSpring = CreateFixedLinearSpring(body, bodyAttachPoint, worldAttachPoint, springConstant, dampningConstant);
		physicsSimulator.Add(fixedLinearSpring);
		return fixedLinearSpring;
	}

	public FixedLinearSpring CreateFixedLinearSpring(Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampningConstant)
	{
		return new FixedLinearSpring(body, bodyAttachPoint, worldAttachPoint, springConstant, dampningConstant);
	}

	public AngleSpring CreateAngleSpring(PhysicsSimulator physicsSimulator, Body body1, Body body2, float springConstant, float dampningConstant)
	{
		AngleSpring angleSpring = CreateAngleSpring(body1, body2, springConstant, dampningConstant);
		physicsSimulator.Add(angleSpring);
		return angleSpring;
	}

	public AngleSpring CreateAngleSpring(Body body1, Body body2, float springConstant, float dampningConstant)
	{
		return new AngleSpring(body1, body2, springConstant, dampningConstant);
	}

	public FixedAngleSpring CreateFixedAngleSpring(PhysicsSimulator physicsSimulator, Body body, float springConstant, float dampningConstant)
	{
		FixedAngleSpring fixedAngleSpring = CreateFixedAngleSpring(body, springConstant, dampningConstant);
		physicsSimulator.Add(fixedAngleSpring);
		return fixedAngleSpring;
	}

	public FixedAngleSpring CreateFixedAngleSpring(Body body, float springConstant, float dampningConstant)
	{
		return new FixedAngleSpring(body, springConstant, dampningConstant);
	}
}

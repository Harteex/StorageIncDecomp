using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Factories;

public class JointFactory
{
	private static JointFactory _instance;

	public static JointFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new JointFactory();
			}
			return _instance;
		}
	}

	private JointFactory()
	{
	}

	public RevoluteJoint CreateRevoluteJoint(PhysicsSimulator physicsSimulator, Body body1, Body body2, Vector2 initialAnchorPosition)
	{
		RevoluteJoint revoluteJoint = CreateRevoluteJoint(body1, body2, initialAnchorPosition);
		physicsSimulator.Add(revoluteJoint);
		return revoluteJoint;
	}

	public RevoluteJoint CreateRevoluteJoint(Body body1, Body body2, Vector2 initialAnchorPosition)
	{
		return new RevoluteJoint(body1, body2, initialAnchorPosition);
	}

	public FixedRevoluteJoint CreateFixedRevoluteJoint(PhysicsSimulator physicsSimulator, Body body, Vector2 anchor)
	{
		FixedRevoluteJoint fixedRevoluteJoint = CreateFixedRevoluteJoint(body, anchor);
		physicsSimulator.Add(fixedRevoluteJoint);
		return fixedRevoluteJoint;
	}

	public FixedRevoluteJoint CreateFixedRevoluteJoint(Body body, Vector2 anchor)
	{
		FixedRevoluteJoint fixedRevoluteJoint = new FixedRevoluteJoint(body, anchor);
		if (body.isStatic)
		{
			fixedRevoluteJoint.Enabled = false;
		}
		return fixedRevoluteJoint;
	}

	public PinJoint CreatePinJoint(PhysicsSimulator physicsSimulator, Body body1, Vector2 anchor1, Body body2, Vector2 anchor2)
	{
		PinJoint pinJoint = CreatePinJoint(body1, anchor1, body2, anchor2);
		physicsSimulator.Add(pinJoint);
		return pinJoint;
	}

	public PinJoint CreatePinJoint(Body body1, Vector2 anchor1, Body body2, Vector2 anchor2)
	{
		return new PinJoint(body1, anchor1, body2, anchor2);
	}

	public SliderJoint CreateSliderJoint(PhysicsSimulator physicsSimulator, Body body1, Vector2 anchor1, Body body2, Vector2 anchor2, float min, float max)
	{
		SliderJoint sliderJoint = CreateSliderJoint(body1, anchor1, body2, anchor2, min, max);
		physicsSimulator.Add(sliderJoint);
		return sliderJoint;
	}

	public SliderJoint CreateSliderJoint(Body body1, Vector2 anchor1, Body body2, Vector2 anchor2, float min, float max)
	{
		return new SliderJoint(body1, anchor1, body2, anchor2, min, max);
	}

	public AngleJoint CreateAngleJoint(PhysicsSimulator physicsSimulator, Body body1, Body body2)
	{
		AngleJoint angleJoint = CreateAngleJoint(body1, body2);
		physicsSimulator.Add(angleJoint);
		return angleJoint;
	}

	public AngleJoint CreateAngleJoint(Body body1, Body body2)
	{
		return new AngleJoint(body1, body2);
	}

	public AngleJoint CreateAngleJoint(PhysicsSimulator physicsSimulator, Body body1, Body body2, float softness, float biasFactor)
	{
		AngleJoint angleJoint = CreateAngleJoint(body1, body2, softness, biasFactor);
		physicsSimulator.Add(angleJoint);
		return angleJoint;
	}

	public AngleJoint CreateAngleJoint(Body body1, Body body2, float softness, float biasFactor)
	{
		AngleJoint angleJoint = new AngleJoint(body1, body2);
		angleJoint.Softness = softness;
		angleJoint.BiasFactor = biasFactor;
		return angleJoint;
	}

	public FixedAngleJoint CreateFixedAngleJoint(PhysicsSimulator physicsSimulator, Body body)
	{
		FixedAngleJoint fixedAngleJoint = CreateFixedAngleJoint(body);
		physicsSimulator.Add(fixedAngleJoint);
		return fixedAngleJoint;
	}

	public FixedAngleJoint CreateFixedAngleJoint(Body body)
	{
		return new FixedAngleJoint(body);
	}

	public AngleLimitJoint CreateAngleLimitJoint(PhysicsSimulator physicsSimulator, Body body1, Body body2, float min, float max)
	{
		AngleLimitJoint angleLimitJoint = CreateAngleLimitJoint(body1, body2, min, max);
		physicsSimulator.Add(angleLimitJoint);
		return angleLimitJoint;
	}

	public AngleLimitJoint CreateAngleLimitJoint(Body body1, Body body2, float min, float max)
	{
		return new AngleLimitJoint(body1, body2, min, max);
	}

	public FixedAngleLimitJoint CreateFixedAngleLimitJoint(PhysicsSimulator physicsSimulator, Body body, float min, float max)
	{
		FixedAngleLimitJoint fixedAngleLimitJoint = CreateFixedAngleLimitJoint(body, min, max);
		physicsSimulator.Add(fixedAngleLimitJoint);
		return fixedAngleLimitJoint;
	}

	public FixedAngleLimitJoint CreateFixedAngleLimitJoint(Body body, float min, float max)
	{
		return new FixedAngleLimitJoint(body, min, max);
	}
}

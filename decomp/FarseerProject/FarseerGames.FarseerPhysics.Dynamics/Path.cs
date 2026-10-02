using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics;

public class Path
{
	private const float _controlPointSize = 6f;

	private const float _precision = 0.0005f;

	private BodyList _bodies;

	private Vertices _controlPoints;

	private GeomList _geoms;

	private SpringList _springs;

	private float _height;

	private JointList _joints;

	private bool _loop;

	private float _mass;

	private bool _recalculate = true;

	private float _width;

	public BodyList Bodies => _bodies;

	public JointList Joints => _joints;

	public GeomList Geoms => _geoms;

	public SpringList Springs => _springs;

	public Vertices ControlPoints => _controlPoints;

	public Path(float width, float height, float mass, bool endless)
	{
		_width = width;
		_height = height;
		_loop = endless;
		_mass = mass;
		_geoms = new GeomList();
		_bodies = new BodyList();
		_joints = new JointList();
		_springs = new SpringList();
		_controlPoints = new Vertices();
	}

	public void LinkBodies(LinkType type, float min, float max, float springConstant, float dampingConstant)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _bodies.Count; i++)
		{
			if (i >= _bodies.Count - 1)
			{
				continue;
			}
			float num = ((!(_bodies[i].Position.X < _bodies[i + 1].Position.X)) ? ((_bodies[i + 1].Position.X - _bodies[i].Position.X) * 0.5f) : (Math.Abs(_bodies[i].Position.X - _bodies[i + 1].Position.X) * 0.5f));
			float num2 = ((!(_bodies[i].Position.Y < _bodies[i + 1].Position.Y)) ? ((_bodies[i + 1].Position.Y - _bodies[i].Position.Y) * 0.5f) : (Math.Abs(_bodies[i].Position.Y - _bodies[i + 1].Position.Y) * 0.5f));
			Vector2 initialAnchorPosition = new Vector2(_bodies[i].Position.X + num, _bodies[i].Position.Y + num2);
			switch (type)
			{
			case LinkType.RevoluteJoint:
			{
				RevoluteJoint revoluteJoint = JointFactory.Instance.CreateRevoluteJoint(_bodies[i], _bodies[i + 1], initialAnchorPosition);
				revoluteJoint.BiasFactor = 0.2f;
				revoluteJoint.Softness = 0.01f;
				_joints.Add(revoluteJoint);
				break;
			}
			case LinkType.LinearSpring:
			{
				LinearSpring linearSpring = SpringFactory.Instance.CreateLinearSpring(_bodies[i], new Vector2((0f - _width) / 2f, 0f), _bodies[i + 1], new Vector2(_width / 2f, 0f), springConstant, dampingConstant);
				if (i >= 1)
				{
					LinearSpring linearSpring2 = (LinearSpring)_springs[i - 1];
					linearSpring.RestLength = Vector2.Distance(linearSpring2.AttachPoint2, linearSpring.AttachPoint1);
				}
				_springs.Add(linearSpring);
				break;
			}
			case LinkType.PinJoint:
			{
				PinJoint pinJoint = JointFactory.Instance.CreatePinJoint(_bodies[i], new Vector2((0f - _width) / 2f, 0f), _bodies[i + 1], new Vector2(_width / 2f, 0f));
				pinJoint.BiasFactor = 0.2f;
				pinJoint.Softness = 0.01f;
				if (i >= 1)
				{
					PinJoint pinJoint2 = (PinJoint)_joints[i - 1];
					pinJoint.TargetDistance = Vector2.Distance(pinJoint2.Anchor2, pinJoint.Anchor1);
				}
				_joints.Add(pinJoint);
				break;
			}
			case LinkType.SliderJoint:
			{
				SliderJoint sliderJoint = JointFactory.Instance.CreateSliderJoint(_bodies[i], new Vector2((0f - _width) / 2f, 0f), _bodies[i + 1], new Vector2(_width / 2f, 0f), min, max);
				sliderJoint.BiasFactor = 0.2f;
				sliderJoint.Softness = 0.01f;
				_joints.Add(sliderJoint);
				break;
			}
			}
		}
		if (_loop)
		{
			float num = ((!(_bodies[0].Position.X < _bodies[_bodies.Count - 1].Position.X)) ? ((_bodies[_bodies.Count - 1].Position.X - _bodies[0].Position.X) * 0.5f) : (Math.Abs(_bodies[0].Position.X - _bodies[_bodies.Count - 1].Position.X) * 0.5f));
			float num2 = ((!(_bodies[0].Position.Y < _bodies[_bodies.Count - 1].Position.Y)) ? ((_bodies[_bodies.Count - 1].Position.Y - _bodies[0].Position.Y) * 0.5f) : (Math.Abs(_bodies[0].Position.Y - _bodies[_bodies.Count - 1].Position.Y) * 0.5f));
			Vector2 initialAnchorPosition = new Vector2(_bodies[0].Position.X + num, _bodies[0].Position.Y + num2);
			switch (type)
			{
			case LinkType.RevoluteJoint:
			{
				RevoluteJoint revoluteJoint = JointFactory.Instance.CreateRevoluteJoint(_bodies[0], _bodies[_bodies.Count - 1], initialAnchorPosition);
				revoluteJoint.BiasFactor = 0.2f;
				revoluteJoint.Softness = 0.01f;
				_joints.Add(revoluteJoint);
				break;
			}
			case LinkType.LinearSpring:
			{
				LinearSpring linearSpring = SpringFactory.Instance.CreateLinearSpring(_bodies[0], new Vector2((0f - _width) / 2f, 0f), _bodies[_bodies.Count - 1], new Vector2(_width / 2f, 0f), springConstant, dampingConstant);
				linearSpring.RestLength = _width;
				_springs.Add(linearSpring);
				break;
			}
			case LinkType.PinJoint:
			{
				PinJoint pinJoint = JointFactory.Instance.CreatePinJoint(_bodies[0], new Vector2((0f - _width) / 2f, 0f), _bodies[_bodies.Count - 1], new Vector2(_width / 2f, 0f));
				pinJoint.BiasFactor = 0.2f;
				pinJoint.Softness = 0.01f;
				pinJoint.TargetDistance = _width;
				_joints.Add(pinJoint);
				break;
			}
			case LinkType.SliderJoint:
			{
				SliderJoint sliderJoint = JointFactory.Instance.CreateSliderJoint(_bodies[0], new Vector2((0f - _width) / 2f, 0f), _bodies[_bodies.Count - 1], new Vector2(_width / 2f, 0f), min, max);
				sliderJoint.BiasFactor = 0.2f;
				sliderJoint.Softness = 0.01f;
				_joints.Add(sliderJoint);
				break;
			}
			}
		}
	}

	public void AddToPhysicsSimulator(PhysicsSimulator physicsSimulator)
	{
		foreach (Body body in _bodies)
		{
			physicsSimulator.Add(body);
		}
		foreach (Geom geom in _geoms)
		{
			physicsSimulator.Add(geom);
		}
		foreach (Joint joint in _joints)
		{
			physicsSimulator.Add(joint);
		}
		foreach (Spring spring in _springs)
		{
			physicsSimulator.Add(spring);
		}
	}

	public void CreateGeoms()
	{
		foreach (Body body in _bodies)
		{
			Geom geom = GeomFactory.Instance.CreateRectangleGeom(body, _width, _height);
			geom.CollisionCategories = CollisionCategory.Cat2;
			geom.CollidesWith = CollisionCategory.Cat1;
			_geoms.Add(geom);
		}
	}

	public int PointInControlPoint(Vector2 point)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		foreach (Vector2 controlPoint in _controlPoints)
		{
			AABB aABB = new AABB(new Vector2(controlPoint.X - 3f, controlPoint.Y - 3f), new Vector2(controlPoint.X + 3f, controlPoint.Y + 3f));
			if (aABB.Contains(ref point))
			{
				return _controlPoints.IndexOf(controlPoint);
			}
		}
		return -1;
	}

	public void MoveControlPoint(Vector2 position, int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_controlPoints[index] = position;
		_recalculate = true;
	}

	public void Add(Vector2 controlPoint)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_controlPoints.Add(controlPoint);
		_recalculate = true;
	}

	public void Add(Body body)
	{
		_bodies.Add(body);
	}

	public void Add(Geom geom)
	{
		_geoms.Add(geom);
	}

	public void Add(Joint joint)
	{
		_joints.Add(joint);
	}

	public void Add(Spring spring)
	{
		_springs.Add(spring);
	}

	public void Remove(Vector2 controlPoint)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_controlPoints.Remove(controlPoint);
		_recalculate = true;
	}

	public void Remove(int index)
	{
		_controlPoints.RemoveAt(index);
		_recalculate = true;
	}

	public void Update()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected Obj, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected Obj, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Vector2 val = default;
		Vector2 val2 = default;
		if (!_recalculate)
		{
			return;
		}
		Curve val3 = new Curve();
		Curve val4 = new Curve();
		float num2 = 1f / (float)_controlPoints.Count;
		float num3;
		for (int i = 0; i < _controlPoints.Count; i++)
		{
			num3 = num2 * (float)(i + 1);
			val3.Keys.Add(new CurveKey(num3, _controlPoints[i].X));
			val4.Keys.Add(new CurveKey(num3, _controlPoints[i].Y));
		}
		num3 = 0f;
		val3.ComputeTangents((CurveTangent)2);
		val4.ComputeTangents((CurveTangent)2);
		while (num < _width / 2f)
		{
			num3 += 0.0005f;
			val = new Vector2(val3.Evaluate(num3), val4.Evaluate(num3));
			num = Vector2.Distance(_controlPoints[0], val);
		}
		while (num < _width)
		{
			num3 += 0.0005f;
			val2 = new Vector2(val3.Evaluate(num3), val4.Evaluate(num3));
			num = Vector2.Distance(_controlPoints[0], val2);
		}
		Body body = BodyFactory.Instance.CreateRectangleBody(_width, _height, _mass);
		body.Position = val;
		body.Rotation = FindNormalAngle(FindVertexNormal(_controlPoints[0], val, val2));
		_bodies.Add(body);
		Vector2 val5 = val;
		while (num3 < 1f)
		{
			num = 0f;
			while (num < _width && num3 < 1f)
			{
				num3 += 0.0005f;
				val = new Vector2(val3.Evaluate(num3), val4.Evaluate(num3));
				num = Vector2.Distance(val, val5);
			}
			num = 0f;
			while (num < _width && num3 < 1f)
			{
				num3 += 0.0005f;
				val2 = new Vector2(val3.Evaluate(num3), val4.Evaluate(num3));
				num = Vector2.Distance(val, val2);
			}
			body = BodyFactory.Instance.CreateRectangleBody(_width, _height, _mass);
			body.Position = val;
			body.Rotation = FindNormalAngle(FindVertexNormal(val5, val, val2));
			_bodies.Add(body);
			val5 = val;
		}
		MoveControlPoint(val2, _controlPoints.Count - 1);
		_recalculate = false;
	}

	public static Vector2 FindMidpoint(Vector2 firstVector, Vector2 secondVector)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		float num = ((!(firstVector.X < secondVector.X)) ? ((secondVector.X - firstVector.X) * 0.5f) : Math.Abs((firstVector.X - secondVector.X) * 0.5f));
		float num2 = ((!(firstVector.Y < secondVector.Y)) ? ((secondVector.Y - firstVector.Y) * 0.5f) : Math.Abs((firstVector.Y - secondVector.Y) * 0.5f));
		return new Vector2(firstVector.X + num, firstVector.Y + num2);
	}

	private Vector2 FindEdgeNormal(Vector2 firstVector, Vector2 secondVector)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.Zero;
		Vector2 val = new Vector2(firstVector.X - secondVector.X, firstVector.Y - secondVector.Y);
		zero.X = 0f - val.Y;
		zero.Y = val.X;
		return zero;
	}

	private Vector2 FindVertexNormal(Vector2 firstVector, Vector2 secondVector, Vector2 c)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector2 result = FindEdgeNormal(firstVector, secondVector) + FindEdgeNormal(secondVector, c);
		result.Normalize();
		return result;
	}

	private float FindNormalAngle(Vector2 n)
	{
		if (n.Y > 0f && n.X > 0f)
		{
			return (float)Math.Atan(n.X / (0f - n.Y));
		}
		if (n.Y < 0f && n.X > 0f)
		{
			return (float)Math.Atan(n.X / (0f - n.Y));
		}
		if (n.Y > 0f && n.X < 0f)
		{
			return (float)Math.Atan((0f - n.X) / n.Y);
		}
		if (n.Y < 0f && n.X < 0f)
		{
			return (float)Math.Atan((0f - n.X) / n.Y);
		}
		return 0f;
	}
}

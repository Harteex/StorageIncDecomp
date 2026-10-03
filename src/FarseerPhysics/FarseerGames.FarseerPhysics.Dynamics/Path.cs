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
		_controlPoints[index] = position;
		_recalculate = true;
	}

	public void Add(Vector2 controlPoint)
	{
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
		float num = ((!(firstVector.X < secondVector.X)) ? ((secondVector.X - firstVector.X) * 0.5f) : Math.Abs((firstVector.X - secondVector.X) * 0.5f));
		float num2 = ((!(firstVector.Y < secondVector.Y)) ? ((secondVector.Y - firstVector.Y) * 0.5f) : Math.Abs((firstVector.Y - secondVector.Y) * 0.5f));
		return new Vector2(firstVector.X + num, firstVector.Y + num2);
	}

	private Vector2 FindEdgeNormal(Vector2 firstVector, Vector2 secondVector)
	{
		Vector2 zero = Vector2.Zero;
		Vector2 val = new Vector2(firstVector.X - secondVector.X, firstVector.Y - secondVector.Y);
		zero.X = 0f - val.Y;
		zero.Y = val.X;
		return zero;
	}

	private Vector2 FindVertexNormal(Vector2 firstVector, Vector2 secondVector, Vector2 c)
	{
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

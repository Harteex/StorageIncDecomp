using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Factories;

public class ComplexFactory
{
	private static ComplexFactory _instance;

	public float Min { get; set; }

	public float Max { get; set; }

	public float SpringConstant { get; set; }

	public float DampingConstant { get; set; }

	public static ComplexFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ComplexFactory();
			}
			return _instance;
		}
	}

	private ComplexFactory()
	{
	}

	public Path CreateChain(PhysicsSimulator physicsSimulator, Vector2 start, Vector2 end, int links, float height, float mass, int collisionGroup, LinkType type)
	{
		Path path = CreateChain(start, end, Vector2.Distance(start, end) / (float)links, height, mass, collisionGroup, type);
		path.AddToPhysicsSimulator(physicsSimulator);
		return path;
	}

	public Path CreateChain(Vector2 start, Vector2 end, int links, float height, float mass, int collisionGroup, LinkType type)
	{
		return CreateChain(start, end, Vector2.Distance(start, end) / (float)links, height, mass, collisionGroup, type);
	}

	public Path CreateChain(PhysicsSimulator physicsSimulator, Vector2 start, Vector2 end, int links, float mass, int collisionGroup, LinkType type)
	{
		Path path = CreateChain(start, end, Vector2.Distance(start, end) / (float)links, Vector2.Distance(start, end) / (float)links * (1f / 3f), mass, collisionGroup, type);
		path.AddToPhysicsSimulator(physicsSimulator);
		return path;
	}

	public Path CreateChain(Vector2 start, Vector2 end, int links, float mass, int collisionGroup, LinkType type)
	{
		return CreateChain(start, end, Vector2.Distance(start, end) / (float)links, Vector2.Distance(start, end) / (float)links * (1f / 3f), mass, collisionGroup, type);
	}

	public Path CreateChain(PhysicsSimulator physicsSimulator, Vector2 start, Vector2 end, float width, float height, float mass, int collisionGroup, LinkType type)
	{
		Path path = CreateChain(start, end, width, height, mass, pinStart: false, pinEnd: false, collisionGroup, type);
		path.AddToPhysicsSimulator(physicsSimulator);
		return path;
	}

	public Path CreateChain(PhysicsSimulator physicsSimulator, Vector2 start, Vector2 end, float width, float height, float mass, bool pinStart, bool pinEnd, int collisionGroup, LinkType type)
	{
		Path path = CreateChain(start, end, width, height, mass, pinStart, pinEnd, collisionGroup, type);
		path.AddToPhysicsSimulator(physicsSimulator);
		return path;
	}

	public Path CreateChain(Vector2 start, Vector2 end, float width, float height, float mass, int collisionGroup, LinkType type)
	{
		return CreateChain(start, end, width, height, mass, pinStart: false, pinEnd: false, collisionGroup, type);
	}

	public Path CreateChain(Vector2 start, Vector2 end, float width, float height, float mass, bool pinStart, bool pinEnd, int collisionGroup, LinkType type)
	{
		Path path = new Path(width, height, mass, endless: false);
		path.Add(start);
		path.Add(Path.FindMidpoint(start, end));
		path.Add(end);
		path.Update();
		for (int i = 0; i < path.Bodies.Count; i++)
		{
			Geom geom = GeomFactory.Instance.CreateRectangleGeom(path.Bodies[i], width, height);
			geom.collisionGroup = collisionGroup;
			path.Add(geom);
		}
		path.LinkBodies(type, Min, Max, SpringConstant, DampingConstant);
		if (pinStart)
		{
			path.Add((Joint)JointFactory.Instance.CreateFixedRevoluteJoint(path.Bodies[0], start));
		}
		if (pinEnd)
		{
			path.Add((Joint)JointFactory.Instance.CreateFixedRevoluteJoint(path.Bodies[path.Bodies.Count - 1], path.ControlPoints[2]));
		}
		foreach (Joint joint in path.Joints)
		{
			joint.BiasFactor = 0.01f;
			joint.Softness = 0.05f;
		}
		return path;
	}

	public Path CreateRope(Vector2 start, Vector2 end, float width, float height, float mass, bool pinStart, bool pinEnd, int collisionGroup, LinkType type)
	{
		Path path = new Path(width, height, mass, endless: false);
		path.Add(start);
		path.Add(Path.FindMidpoint(start, end));
		path.Add(end);
		path.Update();
		for (int i = 0; i < path.Bodies.Count; i++)
		{
			Geom geom = GeomFactory.Instance.CreateRectangleGeom(path.Bodies[i], width, height);
			geom.collisionGroup = collisionGroup;
			path.Add(geom);
		}
		path.LinkBodies(type, Min, Max, SpringConstant, DampingConstant);
		if (pinStart)
		{
			path.Add((Joint)JointFactory.Instance.CreateFixedRevoluteJoint(path.Bodies[0], start));
		}
		if (pinEnd)
		{
			path.Add((Joint)JointFactory.Instance.CreateFixedRevoluteJoint(path.Bodies[path.Bodies.Count - 1], path.ControlPoints[2]));
		}
		foreach (Joint joint in path.Joints)
		{
			joint.BiasFactor = 0.01f;
			joint.Softness = 0.05f;
		}
		return path;
	}

	public Path CreateTrack(Vertices points, float width, float height, float mass, bool endless, int collisionGroup, LinkType type)
	{
		Path path = new Path(width, height, mass, endless);
		foreach (Vector2 point in points)
		{
			path.Add(point);
		}
		path.Update();
		for (int i = 0; i < path.Bodies.Count; i++)
		{
			Geom geom = GeomFactory.Instance.CreateRectangleGeom(path.Bodies[i], width, height);
			geom.collisionGroup = collisionGroup;
			path.Add(geom);
		}
		path.LinkBodies(type, Min, Max, SpringConstant, DampingConstant);
		return path;
	}

	public Path CreateTrack(PhysicsSimulator physicsSimulator, Vertices points, float width, float height, float mass, bool endless, int collisionGroup, LinkType type)
	{
		Path path = CreateTrack(points, width, height, mass, endless, collisionGroup, type);
		path.AddToPhysicsSimulator(physicsSimulator);
		return path;
	}
}

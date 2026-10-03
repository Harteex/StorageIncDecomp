using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;

namespace FarseerGames.FarseerPhysics.Factories;

public class BodyFactory
{
	private static BodyFactory _instance;

	public static BodyFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BodyFactory();
			}
			return _instance;
		}
	}

	private BodyFactory()
	{
	}

	public Body CreateRectangleBody(PhysicsSimulator physicsSimulator, float width, float height, float mass)
	{
		Body body = CreateRectangleBody(width, height, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateRectangleBody(float width, float height, float mass)
	{
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = mass * (width * width + height * height) / 12f;
		return body;
	}

	public Body CreateCircleBody(PhysicsSimulator physicsSimulator, float radius, float mass)
	{
		Body body = CreateCircleBody(radius, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateCircleBody(float radius, float mass)
	{
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = 0.5f * mass * (float)Math.Pow(radius, 2.0);
		return body;
	}

	public Body CreatePolygonBody(PhysicsSimulator physicsSimulator, Vertices vertices, float mass)
	{
		Body body = CreatePolygonBody(vertices, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreatePolygonBody(Vertices vertices, float mass)
	{
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = mass * vertices.GetMomentOfInertia();
		body.position = vertices.GetCentroid();
		return body;
	}

	public Body CreateBody(PhysicsSimulator physicsSimulator, float mass, float momentOfInertia)
	{
		Body body = CreateBody(mass, momentOfInertia);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateBody(float mass, float momentOfInertia)
	{
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = momentOfInertia;
		return body;
	}

	public Body CreateBody(PhysicsSimulator physicsSimulator, Body body)
	{
		Body body2 = CreateBody(body);
		physicsSimulator.Add(body2);
		return body2;
	}

	public Body CreateBody(Body body)
	{
		return new Body(body);
	}

	public Body CreateEllipseBody(PhysicsSimulator physicsSimulator, float xRadius, float yRadius, float mass)
	{
		Body body = CreateEllipseBody(xRadius, yRadius, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateEllipseBody(float xRadius, float yRadius, float mass)
	{
		Body body = new Body();
		body.Mass = mass;
		if (xRadius == yRadius)
		{
			body.MomentOfInertia = 0.5f * mass * (float)Math.Pow(xRadius, 2.0);
		}
		else
		{
			body.MomentOfInertia = mass * (xRadius * xRadius + yRadius * yRadius) / 12f;
		}
		return body;
	}
}

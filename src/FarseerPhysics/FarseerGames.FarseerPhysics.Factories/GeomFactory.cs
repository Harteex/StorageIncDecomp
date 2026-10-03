using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Factories;

public class GeomFactory
{
	private static GeomFactory _instance;

	private float _gridCellSizeAABBFactor = 0.1f;

	public static GeomFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GeomFactory();
			}
			return _instance;
		}
	}

	public float GridCellSizeAABBFactor
	{
		get
		{
			return _gridCellSizeAABBFactor;
		}
		set
		{
			_gridCellSizeAABBFactor = value;
		}
	}

	private GeomFactory()
	{
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height)
	{
		return CreateRectangleGeom(physicsSimulator, body, width, height, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height)
	{
		return CreateRectangleGeom(body, width, height, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, Vector2 offset, float rotationOffset)
	{
		return CreateRectangleGeom(physicsSimulator, body, width, height, offset, rotationOffset, 0f);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, Vector2 offset, float rotationOffset)
	{
		return CreateRectangleGeom(body, width, height, offset, rotationOffset, 0f);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, float collisionGridCellSize)
	{
		return CreateRectangleGeom(physicsSimulator, body, width, height, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, float collisionGridCellSize)
	{
		return CreateRectangleGeom(body, width, height, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Geom geom = CreateRectangleGeom(body, width, height, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Vertices vertices = Vertices.CreateRectangle(width, height);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges)
	{
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges)
	{
		return CreateCircleGeom(body, radius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		return CreateCircleGeom(body, radius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, float collisionGridCellSize)
	{
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, float collisionGridCellSize)
	{
		return CreateCircleGeom(body, radius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Geom geom = CreateCircleGeom(body, radius, numberOfEdges, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Vertices vertices = Vertices.CreateCircle(radius, numberOfEdges);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}

	public Geom CreatePolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, float collisionGridCellSize)
	{
		return CreatePolygonGeom(physicsSimulator, body, vertices, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreatePolygonGeom(Body body, Vertices vertices, float collisionGridCellSize)
	{
		return CreatePolygonGeom(body, vertices, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreatePolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Geom geom = CreatePolygonGeom(body, vertices, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreatePolygonGeom(Body body, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Vector2 centroid = vertices.GetCentroid();
		vertices.Translate(-centroid);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}

	public Geom CreateGeom(PhysicsSimulator physicsSimulator, Body body, Geom geometry)
	{
		Geom geom = CreateGeom(body, geometry);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateGeom(Body body, Geom geometry)
	{
		return new Geom(body, geometry);
	}

	public Geom CreateGeom(PhysicsSimulator physicsSimulator, Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		Geom geom = CreateGeom(body, geometry, offset, rotationOffset);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateGeom(Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		return new Geom(body, geometry, offset, rotationOffset);
	}

	public float CalculateGridCellSizeFromAABB(Vertices vertices)
	{
		AABB aABB = new AABB(vertices);
		return aABB.GetShortestSide() * _gridCellSizeAABBFactor;
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges)
	{
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges)
	{
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, float collisionGridCellSize)
	{
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, float collisionGridCellSize)
	{
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Geom geom = CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		Vertices vertices = Vertices.CreateEllipse(xRadius, yRadius, numberOfEdges);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}
}

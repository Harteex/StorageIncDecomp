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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(physicsSimulator, body, width, height, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(body, width, height, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, Vector2 offset, float rotationOffset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(physicsSimulator, body, width, height, offset, rotationOffset, 0f);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, Vector2 offset, float rotationOffset)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(body, width, height, offset, rotationOffset, 0f);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, float collisionGridCellSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(physicsSimulator, body, width, height, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, float collisionGridCellSize)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(body, width, height, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateRectangleGeom(body, width, height, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = Vertices.CreateRectangle(width, height);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(body, radius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(body, radius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, float collisionGridCellSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, float collisionGridCellSize)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(body, radius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateCircleGeom(body, radius, numberOfEdges, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = Vertices.CreateCircle(radius, numberOfEdges);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}

	public Geom CreatePolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, float collisionGridCellSize)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygonGeom(physicsSimulator, body, vertices, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreatePolygonGeom(Body body, Vertices vertices, float collisionGridCellSize)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygonGeom(body, vertices, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreatePolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreatePolygonGeom(body, vertices, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreatePolygonGeom(Body body, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateGeom(body, geometry, offset, rotationOffset);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateGeom(Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return new Geom(body, geometry, offset, rotationOffset);
	}

	public float CalculateGridCellSizeFromAABB(Vertices vertices)
	{
		AABB aABB = new AABB(vertices);
		return aABB.GetShortestSide() * _gridCellSizeAABBFactor;
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, float collisionGridCellSize)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, float collisionGridCellSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, collisionGridCellSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = Vertices.CreateEllipse(xRadius, yRadius, numberOfEdges);
		if (collisionGridCellSize <= 0f)
		{
			collisionGridCellSize = CalculateGridCellSizeFromAABB(vertices);
		}
		return new Geom(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}
}

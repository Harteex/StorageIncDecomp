using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class AABB
{
	private const float _epsilon = 1E-05f;

	private Vector2 _vector;

	internal Vector2 max = Vector2.Zero;

	internal Vector2 min = Vector2.Zero;

	public Vector2 Min
	{
		get
		{
			return min;
		}
	}

	public Vector2 Max
	{
		get
		{
			return max;
		}
	}

	public float Width => max.X - min.X;

	public float Height => max.Y - min.Y;

	public AABB()
	{
	}

	public AABB(AABB aabb)
	{
		min = aabb.Min;
		max = aabb.Max;
	}

	public AABB(Vector2 min, Vector2 max)
	{
		this.min = min;
		this.max = max;
	}

	public AABB(Vertices vertices)
	{
		Update(ref vertices);
	}

	public Vertices GetVertices()
	{
		Vertices vertices = new Vertices();
		vertices.Add(Min);
		vertices.Add(new Vector2(Min.X, Max.Y));
		vertices.Add(Max);
		vertices.Add(new Vector2(Max.X, Min.Y));
		return vertices;
	}

	public Vector2 GetCenter()
	{
		return new Vector2(Min.X + (Max.X - Min.X) / 2f, Min.Y + (Max.Y - Min.Y) / 2f);
	}

	public float GetShortestSide()
	{
		float val = max.X - min.X;
		float val2 = max.Y - min.Y;
		return Math.Min(val, val2);
	}

	public void Update(ref Vertices vertices)
	{
		min = vertices[0];
		max = min;
		for (int i = 0; i < vertices.Count; i++)
		{
			_vector = vertices[i];
			if (_vector.X < min.X)
			{
				min.X = _vector.X;
			}
			if (_vector.X > max.X)
			{
				max.X = _vector.X;
			}
			if (_vector.Y < min.Y)
			{
				min.Y = _vector.Y;
			}
			if (_vector.Y > max.Y)
			{
				max.Y = _vector.Y;
			}
		}
	}

	public bool Contains(Vector2 point)
	{
		if (point.X > min.X + 1E-05f && point.X < max.X - 1E-05f && point.Y > min.Y + 1E-05f && point.Y < max.Y - 1E-05f)
		{
			return true;
		}
		return false;
	}

	public bool Contains(ref Vector2 point)
	{
		if (point.X > min.X + 1E-05f && point.X < max.X - 1E-05f && point.Y > min.Y + 1E-05f && point.Y < max.Y - 1E-05f)
		{
			return true;
		}
		return false;
	}

	public static bool Intersect(AABB aabb1, AABB aabb2)
	{
		if (aabb1.min.X > aabb2.max.X || aabb2.min.X > aabb1.max.X)
		{
			return false;
		}
		if (aabb1.min.Y > aabb2.Max.Y || aabb2.min.Y > aabb1.Max.Y)
		{
			return false;
		}
		return true;
	}
}

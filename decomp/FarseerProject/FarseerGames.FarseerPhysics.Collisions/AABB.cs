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
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return min;
		}
	}

	public Vector2 Max
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return max;
		}
	}

	public float Width => max.X - min.X;

	public float Height => max.Y - min.Y;

	public AABB()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public AABB(AABB aabb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		min = aabb.Min;
		max = aabb.Max;
	}

	public AABB(Vector2 min, Vector2 max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		this.min = min;
		this.max = max;
	}

	public AABB(Vertices vertices)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Update(ref vertices);
	}

	public Vertices GetVertices()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		vertices.Add(Min);
		vertices.Add(new Vector2(Min.X, Max.Y));
		vertices.Add(Max);
		vertices.Add(new Vector2(Max.X, Min.Y));
		return vertices;
	}

	public Vector2 GetCenter()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
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

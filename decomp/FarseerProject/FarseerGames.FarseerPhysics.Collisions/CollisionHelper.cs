using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public static class CollisionHelper
{
	private const float _defaultFloatTolerance = 1E-05f;

	public static bool FloatEquals(float a, float b, float delta)
	{
		return FloatInRange(a, b - delta, b + delta);
	}

	public static bool FloatInRange(float a, float min, float max)
	{
		return a >= min && a <= max;
	}

	public static bool LineIntersect(ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, ref Vector2 p4, bool firstIsSegment, bool secondIsSegment, float floatTolerance, out Vector2 point)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		point = default;
		float num = p4.Y - p3.Y;
		float num2 = p2.X - p1.X;
		float num3 = p4.X - p3.X;
		float num4 = p2.Y - p1.Y;
		float num5 = num * num2 - num3 * num4;
		if (!FloatEquals(num5, 0f, floatTolerance))
		{
			float num6 = p1.Y - p3.Y;
			float num7 = p1.X - p3.X;
			float num8 = 1f / num5;
			float num9 = num3 * num6 - num * num7;
			num9 *= num8;
			if (!firstIsSegment || FloatInRange(num9, 0f, 1f))
			{
				float num10 = num2 * num6 - num4 * num7;
				num10 *= num8;
				if ((!secondIsSegment || FloatInRange(num10, 0f, 1f)) && (!FloatEquals(num9, 0f, floatTolerance) || !FloatEquals(num10, 0f, floatTolerance)))
				{
					result = true;
					point.X = p1.X + num9 * num2;
					point.Y = p1.Y + num9 * num4;
				}
			}
		}
		return result;
	}

	public static bool LineIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, bool firstIsSegment, bool secondIsSegment, float floatTolerance, out Vector2 point)
	{
		return LineIntersect(ref p1, ref p2, ref p3, ref p4, firstIsSegment, secondIsSegment, floatTolerance, out point);
	}

	public static bool LineIntersect(ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, ref Vector2 p4, out Vector2 point)
	{
		return LineIntersect(ref p1, ref p2, ref p3, ref p4, firstIsSegment: true, secondIsSegment: true, 1E-05f, out point);
	}

	public static bool LineIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 point)
	{
		return LineIntersect(ref p1, ref p2, ref p3, ref p4, firstIsSegment: true, secondIsSegment: true, 1E-05f, out point);
	}

	public static void LineSegmentVerticesIntersect(ref Vector2 p1, ref Vector2 p2, Vertices vertices, ref List<Vector2> points)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < vertices.Count; i++)
		{
			if (LineIntersect(vertices[i], vertices[vertices.NextIndex(i)], p1, p2, firstIsSegment: true, secondIsSegment: true, 1E-05f, out var point))
			{
				points.Add(point);
			}
		}
	}

	public static void LineSegmentAABBIntersect(ref Vector2 p1, ref Vector2 p2, AABB aabb, ref List<Vector2> points)
	{
		LineSegmentVerticesIntersect(ref p1, ref p2, aabb.GetVertices(), ref points);
	}

	public static void LineSegmentGeomIntersect(Vector2 p1, Vector2 p2, Geom geom, bool detectUsingAABB, ref List<LineIntersectInfo> lineIntersectInfoList)
	{
		LineSegmentGeomIntersect(ref p1, ref p2, geom, detectUsingAABB, ref lineIntersectInfoList);
	}

	public static void LineSegmentGeomIntersect(ref Vector2 p1, ref Vector2 p2, Geom geom, bool detectUsingAABB, ref List<LineIntersectInfo> lineIntersectInfoList)
	{
		List<Vector2> points = new List<Vector2>();
		if (detectUsingAABB)
		{
			LineSegmentAABBIntersect(ref p1, ref p2, geom.AABB, ref points);
			if (points.Count > 0)
			{
				lineIntersectInfoList.Add(new LineIntersectInfo(geom, points));
			}
		}
		else
		{
			LineSegmentVerticesIntersect(ref p1, ref p2, geom.WorldVertices, ref points);
			if (points.Count > 0)
			{
				lineIntersectInfoList.Add(new LineIntersectInfo(geom, points));
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class Vertices : List<Vector2>
{
	private Vector2 _res;

	private Vector2 _vectorTemp1 = Vector2.Zero;

	private Vector2 _vectorTemp2 = Vector2.Zero;

	private Vector2 _vectorTemp3 = Vector2.Zero;

	private Vector2 _vectorTemp4 = Vector2.Zero;

	private Vector2 _vectorTemp5 = Vector2.Zero;

	private static readonly int[,] _closePixels = new int[8, 2]
	{
		{ -1, -1 },
		{ 0, -1 },
		{ 1, -1 },
		{ 1, 0 },
		{ 1, 1 },
		{ 0, 1 },
		{ -1, 1 },
		{ -1, 0 }
	};

	public Vertices()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
	}

	public Vertices(Vector2[] vector2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < vector2.Length; i++)
		{
			Add(vector2[i]);
		}
	}

	public Vertices(Vertices vertices)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < vertices.Count; i++)
		{
			Add(vertices[i]);
		}
	}

	public Vector2[] GetVerticesArray()
	{
		return ToArray();
	}

	public int NextIndex(int index)
	{
		if (index == Count - 1)
		{
			return 0;
		}
		return index + 1;
	}

	public int PreviousIndex(int index)
	{
		if (index == 0)
		{
			return Count - 1;
		}
		return index - 1;
	}

	public Vector2 GetEdge(int index)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		int index2 = NextIndex(index);
		_vectorTemp2 = base[index2];
		_vectorTemp3 = base[index];
		Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp3, ref _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetEdge(int index, out Vector2 edge)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		int index2 = NextIndex(index);
		_vectorTemp2 = base[index2];
		_vectorTemp3 = base[index];
		Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp3, ref edge);
	}

	public Vector2 GetEdgeMidPoint(int index)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		GetEdge(index, out _vectorTemp1);
		Vector2.Multiply(ref _vectorTemp1, 0.5f, ref _vectorTemp2);
		_vectorTemp3 = base[index];
		Vector2.Add(ref _vectorTemp3, ref _vectorTemp2, ref _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetEdgeMidPoint(int index, out Vector2 midPoint)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		GetEdge(index, out _vectorTemp1);
		Vector2.Multiply(ref _vectorTemp1, 0.5f, ref _vectorTemp2);
		_vectorTemp3 = base[index];
		Vector2.Add(ref _vectorTemp3, ref _vectorTemp2, ref midPoint);
	}

	public Vector2 GetEdgeNormal(int index)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		GetEdge(index, out _vectorTemp1);
		_vectorTemp2.X = 0f - _vectorTemp1.Y;
		_vectorTemp2.Y = _vectorTemp1.X;
		Vector2.Normalize(ref _vectorTemp2, ref _vectorTemp3);
		return _vectorTemp3;
	}

	public void GetEdgeNormal(int index, out Vector2 edgeNormal)
	{
		GetEdge(index, out _vectorTemp4);
		_vectorTemp5.X = 0f - _vectorTemp4.Y;
		_vectorTemp5.Y = _vectorTemp4.X;
		Vector2.Normalize(ref _vectorTemp5, ref edgeNormal);
	}

	public Vector2 GetVertexNormal(int index)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		GetEdgeNormal(index, out _vectorTemp1);
		int index2 = PreviousIndex(index);
		GetEdgeNormal(index2, out _vectorTemp2);
		Vector2.Add(ref _vectorTemp1, ref _vectorTemp2, ref _vectorTemp3);
		Vector2.Normalize(ref _vectorTemp3, ref _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetVertexNormal(int index, out Vector2 vertexNormal)
	{
		GetEdgeNormal(index, out _vectorTemp1);
		int index2 = PreviousIndex(index);
		GetEdgeNormal(index2, out _vectorTemp2);
		Vector2.Add(ref _vectorTemp1, ref _vectorTemp2, ref _vectorTemp3);
		Vector2.Normalize(ref _vectorTemp3, ref vertexNormal);
	}

	public float GetShortestEdge()
	{
		float num = float.MaxValue;
		for (int i = 0; i < Count; i++)
		{
			GetEdge(i, out _vectorTemp1);
			float num2 = _vectorTemp1.Length();
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public void SubDivideEdges(float maxEdgeLength)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		Vector2 val3 = default;
		for (int i = 0; i < Count; i++)
		{
			Vector2 val = base[i];
			Vector2 val2 = base[NextIndex(i)];
			Vector2.Subtract(ref val, ref val2, ref val3);
			float num = val3.Length();
			vertices.Add(val);
			if (num > maxEdgeLength)
			{
				double num2 = Math.Ceiling((double)num / (double)maxEdgeLength);
				for (int j = 0; (double)j < num2 - 1.0; j++)
				{
					Vector2 item = Vector2.Lerp(val, val2, (float)(j + 1) / (float)num2);
					vertices.Add(item);
				}
			}
		}
		Clear();
		for (int k = 0; k < vertices.Count; k++)
		{
			Add(vertices[k]);
		}
	}

	public void ForceCounterClockWiseOrder()
	{
		float signedArea = GetSignedArea();
		if (signedArea > 0f)
		{
			Reverse();
		}
	}

	private float GetSignedArea()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < Count; i++)
		{
			int index = (i + 1) % Count;
			num += base[i].X * base[index].Y;
			num -= base[i].Y * base[index].X;
		}
		return num / 2f;
	}

	public float GetArea()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < Count; i++)
		{
			int index = (i + 1) % Count;
			num += base[i].X * base[index].Y;
			num -= base[i].Y * base[index].X;
		}
		num /= 2f;
		return (num < 0f) ? (0f - num) : num;
	}

	public Vector2 GetCentroid()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		float area = GetArea();
		return GetCentroid(area);
	}

	public Vector2 GetCentroid(float area)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices(this);
		vertices.ForceCounterClockWiseOrder();
		float num = 0f;
		float num2 = 0f;
		float num3;
		for (int i = 0; i < Count; i++)
		{
			int index = (i + 1) % Count;
			num3 = 0f - (vertices[i].X * vertices[index].Y - vertices[index].X * vertices[i].Y);
			num += (vertices[i].X + vertices[index].X) * num3;
			num2 += (vertices[i].Y + vertices[index].Y) * num3;
		}
		area *= 6f;
		num3 = 1f / area;
		num *= num3;
		num2 *= num3;
		_res.X = num;
		_res.Y = num2;
		return _res;
	}

	public float GetMomentOfInertia()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices(this);
		vertices.ForceCounterClockWiseOrder();
		Vector2 centroid = vertices.GetCentroid();
		vertices.Translate(-centroid);
		if (vertices.Count == 0)
		{
			throw new ArgumentException("Can't calculate MOI on zero vertices");
		}
		if (vertices.Count == 1)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = 0f;
		Vector2 value = vertices[vertices.Count - 1];
		int num3 = 0;
		float num4 = default;
		float num5 = default;
		float num6 = default;
		while (num3 < vertices.Count)
		{
			Vector2 value2 = vertices[num3];
			Vector2.Dot(ref value2, ref value2, ref num4);
			Vector2.Dot(ref value2, ref value, ref num5);
			Vector2.Dot(ref value, ref value, ref num6);
			Calculator.Cross(ref value, ref value2, out var ret);
			ret = Math.Abs(ret);
			num2 += ret;
			num += (num4 + num5 + num6) * ret;
			num3++;
			value = value2;
		}
		return num / (num2 * 6f);
	}

	public void ProjectToAxis(ref Vector2 axis, out float min, out float max)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		max = (min = Vector2.Dot(axis, base[0]));
		for (int i = 0; i < Count; i++)
		{
			float num = Vector2.Dot(base[i], axis);
			if (num < min)
			{
				min = num;
			}
			else if (num > max)
			{
				max = num;
			}
		}
	}

	public void Translate(Vector2 vector)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Count; i++)
		{
			base[i] = Vector2.Add(base[i], vector);
		}
	}

	public void Scale(Vector2 value)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		for (int num = Count - 1; num >= 0; num--)
		{
			List<Vector2> list;
			int index;
			(list = this)[index = num] = list[index] * value;
		}
	}

	public static Vertices CreateRectangle(float width, float height)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		vertices.Add(new Vector2((0f - width) * 0.5f, (0f - height) * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.5f, (0f - height) * 0.25f));
		vertices.Add(new Vector2((0f - width) * 0.5f, 0f));
		vertices.Add(new Vector2((0f - width) * 0.5f, height * 0.25f));
		vertices.Add(new Vector2((0f - width) * 0.5f, height * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.25f, height * 0.5f));
		vertices.Add(new Vector2(0f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.25f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.5f, height * 0.5f));
		vertices.Add(new Vector2(width * 0.5f, height * 0.25f));
		vertices.Add(new Vector2(width * 0.5f, 0f));
		vertices.Add(new Vector2(width * 0.5f, (0f - height) * 0.25f));
		vertices.Add(new Vector2(width * 0.5f, (0f - height) * 0.5f));
		vertices.Add(new Vector2(width * 0.25f, (0f - height) * 0.5f));
		vertices.Add(new Vector2(0f, (0f - height) * 0.5f));
		vertices.Add(new Vector2((0f - width) * 0.25f, (0f - height) * 0.5f));
		return vertices;
	}

	public static Vertices CreateCircle(float radius, int numberOfEdges)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		float num = (float)Math.PI * 2f / (float)numberOfEdges;
		vertices.Add(new Vector2(radius, 0f));
		for (int i = 1; i < numberOfEdges; i++)
		{
			vertices.Add(new Vector2(radius * Calculator.Cos(num * (float)i), (0f - radius) * Calculator.Sin(num * (float)i)));
		}
		return vertices;
	}

	public static Vertices CreateEllipse(float xRadius, float yRadius, int numberOfEdges)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		float num = (float)Math.PI * 2f / (float)numberOfEdges;
		vertices.Add(new Vector2(xRadius, 0f));
		for (int i = 1; i < numberOfEdges; i++)
		{
			vertices.Add(new Vector2(xRadius * Calculator.Cos(num * (float)i), (0f - yRadius) * Calculator.Sin(num * (float)i)));
		}
		return vertices;
	}

	public override string ToString()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Count; i++)
		{
			stringBuilder.Append(((object)base[i]/*cast due to constrained. prefix*/).ToString());
			if (i < Count - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygon(textureBits, textureWidth, textureHeight, Vector2.Zero, 127, 2f);
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight, Vector2 textureOrigin)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygon(textureBits, textureWidth, textureHeight, textureOrigin, 127, 2f);
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight, Vector2 textureOrigin, byte alphaTolerance)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygon(textureBits, textureWidth, textureHeight, textureOrigin, alphaTolerance, 2f);
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight, Vector2 textureOrigin, byte alphaTolerance, float hullTolerance)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		Vertices vertices = new Vertices();
		Vertices hullArea = new Vertices();
		uint alphaTolerance2 = (uint)(alphaTolerance << 24);
		if (textureBits.Length == textureWidth * textureHeight)
		{
			if (GetHullEntrance(ref textureBits, ref textureWidth, ref alphaTolerance2, out var entrance))
			{
				Vector2 current = new Vector2(entrance.X - 1f, entrance.Y);
				Vector2 next = entrance;
				vertices.Add(entrance);
				do
				{
					hullArea.Add(next);
					if (SearchForOutstandingVertex(ref hullArea, ref hullTolerance, out var outstanding))
					{
						vertices.Add(outstanding);
						hullArea.RemoveRange(0, hullArea.IndexOf(outstanding));
					}
					Vector2 last = current;
					current = next;
					if (!GetNextHullPoint(ref textureBits, ref textureWidth, ref textureHeight, ref alphaTolerance2, ref last, ref current, out next))
					{
						next = entrance;
					}
				}
				while (next != entrance);
				if (textureOrigin != Vector2.Zero)
				{
					for (int i = 0; i < vertices.Count; i++)
					{
						List<Vector2> list;
						int index;
						(list = vertices)[index = i] = list[index] - textureOrigin;
					}
				}
			}
			return vertices;
		}
		throw new Exception("Sizes don't match: Color array must contain texture width * texture height elements.");
	}

	private static bool GetHullEntrance(ref uint[] textureBits, ref int textureWidth, ref uint alphaTolerance, out Vector2 entrance)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < textureBits.Length; i++)
		{
			if ((textureBits[i] & 0xFF000000u) >= alphaTolerance)
			{
				int num = i % textureWidth;
				int num2 = (i - num) / textureWidth;
				entrance = new Vector2((float)num, (float)num2);
				return true;
			}
		}
		entrance = Vector2.Zero;
		return false;
	}

	private static bool GetNextHullPoint(ref uint[] textureBits, ref int textureWidth, ref int textureHeight, ref uint alphaTolerance, ref Vector2 last, ref Vector2 current, out Vector2 next)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		int indexOfFirstPixelToCheck = GetIndexOfFirstPixelToCheck(last, current);
		for (int i = 0; i < 8; i++)
		{
			int num = (indexOfFirstPixelToCheck + i) % 8;
			int num2 = (int)current.X + _closePixels[num, 0];
			int num3 = (int)current.Y + _closePixels[num, 1];
			if (num2 >= 0 && num2 < textureWidth && num3 >= 0 && num3 < textureHeight && (textureBits[num2 + num3 * textureWidth] & 0xFF000000u) >= alphaTolerance)
			{
				next = new Vector2((float)num2, (float)num3);
				return true;
			}
		}
		next = Vector2.Zero;
		return false;
	}

	private static bool SearchForOutstandingVertex(ref Vertices hullArea, ref float hullTolerance, out Vector2 outstanding)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		int num = hullArea.Count - 1;
		float num2 = 0f;
		bool flag = false;
		Vector2 val = Vector2.Zero;
		for (int i = 1; i < num; i++)
		{
			float num3 = Calculator.DistanceBetweenPointAndLineSegment(hullArea[i], hullArea[0], hullArea[num]);
			if (!flag)
			{
				if (num3 > hullTolerance)
				{
					flag = true;
					num2 = num3;
					val = hullArea[i];
				}
			}
			else if (num3 > num2)
			{
				num2 = num3;
				val = hullArea[i];
			}
		}
		outstanding = val;
		return flag;
	}

	private static int GetIndexOfFirstPixelToCheck(Vector2 last, Vector2 current)
	{
		switch ((int)(current.X - last.X))
		{
		case 1:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 1;
			case 0:
				return 0;
			case -1:
				return 7;
			}
			break;
		case 0:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 2;
			case -1:
				return 6;
			}
			break;
		case -1:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 3;
			case 0:
				return 4;
			case -1:
				return 5;
			}
			break;
		}
		return 0;
	}
}

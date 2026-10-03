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
	}

	public Vertices(Vector2[] vector2)
	{
		for (int i = 0; i < vector2.Length; i++)
		{
			Add(vector2[i]);
		}
	}

	public Vertices(Vertices vertices)
	{
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
		int index2 = NextIndex(index);
		_vectorTemp2 = base[index2];
		_vectorTemp3 = base[index];
		Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp3, out _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetEdge(int index, out Vector2 edge)
	{
		int index2 = NextIndex(index);
		_vectorTemp2 = base[index2];
		_vectorTemp3 = base[index];
		Vector2.Subtract(ref _vectorTemp2, ref _vectorTemp3, out edge);
	}

	public Vector2 GetEdgeMidPoint(int index)
	{
		GetEdge(index, out _vectorTemp1);
		Vector2.Multiply(ref _vectorTemp1, 0.5f, out _vectorTemp2);
		_vectorTemp3 = base[index];
		Vector2.Add(ref _vectorTemp3, ref _vectorTemp2, out _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetEdgeMidPoint(int index, out Vector2 midPoint)
	{
		GetEdge(index, out _vectorTemp1);
		Vector2.Multiply(ref _vectorTemp1, 0.5f, out _vectorTemp2);
		_vectorTemp3 = base[index];
		Vector2.Add(ref _vectorTemp3, ref _vectorTemp2, out midPoint);
	}

	public Vector2 GetEdgeNormal(int index)
	{
		GetEdge(index, out _vectorTemp1);
		_vectorTemp2.X = 0f - _vectorTemp1.Y;
		_vectorTemp2.Y = _vectorTemp1.X;
		Vector2.Normalize(ref _vectorTemp2, out _vectorTemp3);
		return _vectorTemp3;
	}

	public void GetEdgeNormal(int index, out Vector2 edgeNormal)
	{
		GetEdge(index, out _vectorTemp4);
		_vectorTemp5.X = 0f - _vectorTemp4.Y;
		_vectorTemp5.Y = _vectorTemp4.X;
		Vector2.Normalize(ref _vectorTemp5, out edgeNormal);
	}

	public Vector2 GetVertexNormal(int index)
	{
		GetEdgeNormal(index, out _vectorTemp1);
		int index2 = PreviousIndex(index);
		GetEdgeNormal(index2, out _vectorTemp2);
		Vector2.Add(ref _vectorTemp1, ref _vectorTemp2, out _vectorTemp3);
		Vector2.Normalize(ref _vectorTemp3, out _vectorTemp1);
		return _vectorTemp1;
	}

	public void GetVertexNormal(int index, out Vector2 vertexNormal)
	{
		GetEdgeNormal(index, out _vectorTemp1);
		int index2 = PreviousIndex(index);
		GetEdgeNormal(index2, out _vectorTemp2);
		Vector2.Add(ref _vectorTemp1, ref _vectorTemp2, out _vectorTemp3);
		Vector2.Normalize(ref _vectorTemp3, out vertexNormal);
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
		Vertices vertices = new Vertices();
		Vector2 val3 = default;
		for (int i = 0; i < Count; i++)
		{
			Vector2 val = base[i];
			Vector2 val2 = base[NextIndex(i)];
			Vector2.Subtract(ref val, ref val2, out val3);
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
		float area = GetArea();
		return GetCentroid(area);
	}

	public Vector2 GetCentroid(float area)
	{
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
			Vector2.Dot(ref value2, ref value2, out num4);
			Vector2.Dot(ref value2, ref value, out num5);
			Vector2.Dot(ref value, ref value, out num6);
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
		for (int i = 0; i < Count; i++)
		{
			base[i] = Vector2.Add(base[i], vector);
		}
	}

	public void Scale(Vector2 value)
	{
		for (int num = Count - 1; num >= 0; num--)
		{
			List<Vector2> list;
			int index;
			(list = this)[index = num] = list[index] * value;
		}
	}

	public static Vertices CreateRectangle(float width, float height)
	{
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
		return CreatePolygon(textureBits, textureWidth, textureHeight, Vector2.Zero, 127, 2f);
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight, Vector2 textureOrigin)
	{
		return CreatePolygon(textureBits, textureWidth, textureHeight, textureOrigin, 127, 2f);
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight, Vector2 textureOrigin, byte alphaTolerance)
	{
		return CreatePolygon(textureBits, textureWidth, textureHeight, textureOrigin, alphaTolerance, 2f);
	}

	public static Vertices CreatePolygon(uint[] textureBits, int textureWidth, int textureHeight, Vector2 textureOrigin, byte alphaTolerance, float hullTolerance)
	{
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

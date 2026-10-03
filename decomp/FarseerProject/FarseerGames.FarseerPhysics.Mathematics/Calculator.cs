using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Mathematics;

public static class Calculator
{
	public const float DegreesToRadiansRatio = 57.29578f;

	public const float RadiansToDegreesRatio = (float)Math.PI / 180f;

	private static Vector2 _curveEnd;

	private static Random _random = new Random();

	private static Vector2 _startCurve;

	private static Vector2 _temp;

	private static float _tPow2;

	private static float _wayToGo;

	private static float _wayToGoPow2;

	public static float Sin(float angle)
	{
		return (float)Math.Sin(angle);
	}

	public static float Cos(float angle)
	{
		return (float)Math.Cos(angle);
	}

	public static float ACos(float value)
	{
		return (float)Math.Acos(value);
	}

	public static float ATan2(float y, float x)
	{
		return (float)Math.Atan2(y, x);
	}

	public static float BiLerp(Vector2 point, Vector2 min, Vector2 max, float value1, float value2, float value3, float value4, float minValue, float maxValue)
	{
		float x = point.X;
		float y = point.Y;
		x = MathHelper.Clamp(x, min.X, max.X);
		y = MathHelper.Clamp(y, min.Y, max.Y);
		float num = (x - min.X) / (max.X - min.X);
		float num2 = (y - min.Y) / (max.Y - min.Y);
		float num3 = MathHelper.Lerp(value1, value4, num);
		float num4 = MathHelper.Lerp(value2, value3, num);
		float num5 = MathHelper.Lerp(num3, num4, num2);
		return MathHelper.Clamp(num5, minValue, maxValue);
	}

	public static float Clamp(float value, float low, float high)
	{
		return Math.Max(low, Math.Min(value, high));
	}

	public static float DistanceBetweenPointAndPoint(Vector2 point1, Vector2 point2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.Subtract(point1, point2);
		return val.Length();
	}

	public static float DistanceBetweenPointAndPoint(ref Vector2 point1, ref Vector2 point2)
	{
		Vector2 val = default;
		Vector2.Subtract(ref point1, ref point2, ref val);
		return val.Length();
	}

	public static float DistanceBetweenPointAndLineSegment(Vector2 point, Vector2 lineEndPoint1, Vector2 lineEndPoint2)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.Subtract(lineEndPoint2, lineEndPoint1);
		Vector2 val2 = Vector2.Subtract(point, lineEndPoint1);
		float num = Vector2.Dot(val2, val);
		if (num <= 0f)
		{
			return DistanceBetweenPointAndPoint(point, lineEndPoint1);
		}
		float num2 = Vector2.Dot(val, val);
		if (num2 <= num)
		{
			return DistanceBetweenPointAndPoint(point, lineEndPoint2);
		}
		float num3 = num / num2;
		Vector2 point2 = Vector2.Add(lineEndPoint1, Vector2.Multiply(val, num3));
		return DistanceBetweenPointAndPoint(point, point2);
	}

	public static float Cross(Vector2 value1, Vector2 value2)
	{
		return value1.X * value2.Y - value1.Y * value2.X;
	}

	public static Vector2 Cross(Vector2 value1, float value2)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(value2 * value1.Y, (0f - value2) * value1.X);
	}

	public static Vector2 Cross(float value2, Vector2 value1)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((0f - value2) * value1.Y, value2 * value1.X);
	}

	public static void Cross(ref Vector2 value1, ref Vector2 value2, out float ret)
	{
		ret = value1.X * value2.Y - value1.Y * value2.X;
	}

	public static void Cross(ref Vector2 value1, ref float value2, out Vector2 ret)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ret = value1;
		ret.X = value2 * value1.Y;
		ret.Y = (0f - value2) * value1.X;
	}

	public static void Cross(ref float value2, ref Vector2 value1, out Vector2 ret)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ret = value1;
		ret.X = (0f - value2) * value1.Y;
		ret.Y = value2 * value1.X;
	}

	public static Vector2 Project(Vector2 projectVector, Vector2 onToVector)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = onToVector.X * projectVector.X + onToVector.Y * projectVector.Y;
		float num3 = onToVector.X * onToVector.X + onToVector.Y * onToVector.Y;
		if (num3 != 0f)
		{
			num = num2 / num3;
		}
		return Vector2.Multiply(onToVector, num);
	}

	public static void Truncate(ref Vector2 vector, float maxLength, out Vector2 truncatedVector)
	{
		float val = vector.Length();
		val = Math.Min(val, maxLength);
		if (val > 0f)
		{
			vector.Normalize();
		}
		Vector2.Multiply(ref vector, val, ref truncatedVector);
	}

	public static float DegreesToRadians(float degrees)
	{
		return degrees * ((float)Math.PI / 180f);
	}

	public static float RandomNumber(float min, float max)
	{
		return (float)((double)(max - min) * _random.NextDouble() + (double)min);
	}

	public static bool IsBetweenNonInclusive(float number, float min, float max)
	{
		if (number > min && number < max)
		{
			return true;
		}
		return false;
	}

	public static float VectorToRadians(Vector2 vector)
	{
		return (float)Math.Atan2(vector.X, 0.0 - (double)vector.Y);
	}

	public static Vector2 RadiansToVector(float radians)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)Math.Sin(radians), 0f - (float)Math.Cos(radians));
	}

	public static void RadiansToVector(float radians, ref Vector2 vector)
	{
		vector.X = (float)Math.Sin(radians);
		vector.Y = 0f - (float)Math.Cos(radians);
	}

	public static void RotateVector(ref Vector2 vector, float radians)
	{
		float num = vector.Length();
		float num2 = (float)Math.Atan2(vector.X, 0.0 - (double)vector.Y) + radians;
		vector.X = (float)Math.Sin(num2) * num;
		vector.Y = (0f - (float)Math.Cos(num2)) * num;
	}

	public static Vector2 LinearBezierCurve(Vector2 start, Vector2 end, float t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return start + (end - start) * t;
	}

	public static Vector2 QuadraticBezierCurve(Vector2 start, Vector2 curve, Vector2 end, float t)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_wayToGo = 1f - t;
		return _wayToGo * _wayToGo * start + 2f * t * _wayToGo * curve + t * t * end;
	}

	public static Vector2 QuadraticBezierCurve(Vector2 start, Vector2 curve, Vector2 end, float t, ref float radians)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		_startCurve = start + (curve - start) * t;
		_curveEnd = curve + (end - curve) * t;
		_temp = _curveEnd - _startCurve;
		radians = (float)Math.Atan2(_temp.X, 0.0 - (double)_temp.Y);
		return _startCurve + _temp * t;
	}

	public static Vector2 CubicBezierCurve2(Vector2 start, Vector2 startPointsTo, Vector2 end, Vector2 endPointsTo, float t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return CubicBezierCurve(start, start + startPointsTo, end + endPointsTo, end, t);
	}

	public static Vector2 CubicBezierCurve2(Vector2 start, Vector2 startPointsTo, Vector2 end, Vector2 endPointsTo, float t, ref float radians)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		return CubicBezierCurve(start, start + startPointsTo, end + endPointsTo, end, t, ref radians);
	}

	public static Vector2 CubicBezierCurve2(Vector2 start, float startPointDirection, float startPointLength, Vector2 end, float endPointDirection, float endPointLength, float t, ref float radians)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		return CubicBezierCurve(start, RadiansToVector(startPointDirection) * startPointLength, RadiansToVector(endPointDirection) * endPointLength, end, t, ref radians);
	}

	public static Vector2 CubicBezierCurve(Vector2 start, Vector2 curve1, Vector2 curve2, Vector2 end, float t)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		_tPow2 = t * t;
		_wayToGo = 1f - t;
		_wayToGoPow2 = _wayToGo * _wayToGo;
		return _wayToGo * _wayToGoPow2 * start + 3f * t * _wayToGoPow2 * curve1 + 3f * _tPow2 * _wayToGo * curve2 + t * _tPow2 * end;
	}

	public static Vector2 CubicBezierCurve(Vector2 start, Vector2 curve1, Vector2 curve2, Vector2 end, float t, ref float radians)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		return QuadraticBezierCurve(start + (curve1 - start) * t, curve1 + (curve2 - curve1) * t, curve2 + (end - curve2) * t, t, ref radians);
	}

	public static Vector2 InterpolateNormal(Vector2 vector1, Vector2 vector2, float t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		vector1 += (vector2 - vector1) * t;
		vector1.Normalize();
		return vector1;
	}

	public static void InterpolateNormal(Vector2 vector1, Vector2 vector2, float t, out Vector2 vector)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		vector = vector1 + (vector2 - vector1) * t;
		vector.Normalize();
	}

	public static void InterpolateNormal(ref Vector2 vector1, Vector2 vector2, float t)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		vector1 += (vector2 - vector1) * t;
		vector1.Normalize();
	}

	public static float InterpolateRotation(float radians1, float radians2, float t)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2((float)Math.Sin(radians1), 0f - (float)Math.Cos(radians1));
		Vector2 val2 = new Vector2((float)Math.Sin(radians2), 0f - (float)Math.Cos(radians2));
		val += (val2 - val) * t;
		val.Normalize();
		return (float)Math.Atan2(val.X, 0.0 - (double)val.Y);
	}

	public static void ProjectToAxis(ref Vector2[] points, ref Vector2 axis, out float min, out float max)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		axis.Normalize();
		max = (min = Vector2.Dot(axis, points[0]));
		for (int i = 0; i < points.Length; i++)
		{
			float num = Vector2.Dot(points[i], axis);
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
}

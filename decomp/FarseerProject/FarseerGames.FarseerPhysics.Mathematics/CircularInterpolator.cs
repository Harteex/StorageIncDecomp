using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Mathematics;

public class CircularInterpolator
{
	private const float _twoOverPi = 2f / (float)Math.PI;

	private float _circleValue1;

	private float _circleValue2;

	private float _maxValue = float.MaxValue;

	private float _minValue = float.MinValue;

	private float _value1;

	private float _value2;

	private float _value3;

	private float _value4;

	public CircularInterpolator(float value1, float value2, float value3, float value4, float minValue, float maxValue)
	{
		_value1 = value1;
		_value2 = value2;
		_value3 = value3;
		_value4 = value4;
		_minValue = minValue;
		_maxValue = maxValue;
	}

	public float GetValue(Vector2 position)
	{
		return GetValue(position.X, position.Y);
	}

	public float GetValue(float x, float y)
	{
		float num = 0f;
		float num2 = MathHelper.Distance(x, y);
		if (x > 0f && y > 0f)
		{
			float num3 = Calculator.ATan2(y, x);
			float num4 = num3 * (2f / (float)Math.PI);
			_circleValue1 = MathHelper.Lerp(_value1, _value2, num4);
			_circleValue2 = MathHelper.Lerp(_value3, _value4, num4);
			num = MathHelper.Lerp(_circleValue1, _circleValue2, (1f - num2) * 0.5f);
		}
		if (x < 0f && y > 0f)
		{
			float num3 = Calculator.ATan2(y, x);
			float num4 = (num3 - (float)Math.PI / 2f) * (2f / (float)Math.PI);
			_circleValue1 = MathHelper.Lerp(_value2, _value3, num4);
			_circleValue2 = MathHelper.Lerp(_value4, _value1, num4);
			num = MathHelper.Lerp(_circleValue1, _circleValue2, (1f - num2) * 0.5f);
		}
		if (x < 0f && y < 0f)
		{
			float num3 = Calculator.ATan2(y, x) + (float)Math.PI * 2f;
			float num4 = (num3 - (float)Math.PI) * (2f / (float)Math.PI);
			_circleValue1 = MathHelper.Lerp(_value3, _value4, num4);
			_circleValue2 = MathHelper.Lerp(_value1, _value2, num4);
			num = MathHelper.Lerp(_circleValue1, _circleValue2, (1f - num2) * 0.5f);
		}
		if (x > 0f && y < 0f)
		{
			float num3 = Calculator.ATan2(y, x) + (float)Math.PI * 2f;
			float num4 = (num3 - 4.712389f) * (2f / (float)Math.PI);
			_circleValue1 = MathHelper.Lerp(_value4, _value1, num4);
			_circleValue2 = MathHelper.Lerp(_value2, _value3, num4);
			num = MathHelper.Lerp(_circleValue1, _circleValue2, (1f - num2) * 0.5f);
		}
		if (x == 0f && y > 0f)
		{
			num = MathHelper.Lerp(_value2, _value4, (1f - y) / 2f);
		}
		if (x == 0f && y < 0f)
		{
			num = MathHelper.Lerp(_value4, _value2, (1f + y) / 2f);
		}
		if (x > 0f && y == 0f)
		{
			num = MathHelper.Lerp(_value1, _value3, (1f - x) / 2f);
		}
		if (x < 0f && y == 0f)
		{
			num = MathHelper.Lerp(_value3, _value1, (1f + x) / 2f);
		}
		return MathHelper.Clamp(num, _minValue, _maxValue);
	}
}

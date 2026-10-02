using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Mathematics;

public class BilinearInterpolator
{
	private Vector2 _max;

	private float _maxValue = float.MaxValue;

	private Vector2 _min;

	private float _minValue = float.MinValue;

	private float _value1;

	private float _value2;

	private float _value3;

	private float _value4;

	public Vector2 Min
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _min;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_min = value;
		}
	}

	public Vector2 Max
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _max;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_max = value;
		}
	}

	public BilinearInterpolator()
	{
	}

	public BilinearInterpolator(Vector2 min, Vector2 max)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_max = max;
		_min = min;
	}

	public BilinearInterpolator(Vector2 min, Vector2 max, float value1, float value2, float value3, float value4, float minValue, float maxValue)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_min = min;
		_max = max;
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
		x = MathHelper.Clamp(x, _min.X, _max.X);
		y = MathHelper.Clamp(y, _min.Y, _max.Y);
		float num = (x - _min.X) / (_max.X - _min.X);
		float num2 = (y - _min.Y) / (_max.Y - _min.Y);
		float num3 = MathHelper.Lerp(_value1, _value4, num);
		float num4 = MathHelper.Lerp(_value2, _value3, num);
		float num5 = MathHelper.Lerp(num3, num4, num2);
		return MathHelper.Clamp(num5, _minValue, _maxValue);
	}
}

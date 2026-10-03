using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Interfaces;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Controllers;

public class WaveController : Controller, IFluidContainer
{
	private AABB _aabb;

	private float _aabbMin = float.MaxValue;

	private float[] _currentWave;

	private float _dampningCoefficient = 0.95f;

	private float _frequency = 0.18f;

	private bool _goingUp = true;

	private float _height;

	private int _nodeCount;

	private Vector2 _pointVector;

	private Vector2 _position;

	private float[] _previousWave;

	private float[] _resultWave;

	private float _singleWaveWidth;

	private float _timePassed;

	private Vector2 _waveEdgeVector;

	private float _waveGeneratorCount;

	private float _waveGeneratorMax;

	private float _waveGeneratorMin;

	private float _waveGeneratorStep;

	private float _width;

	private float[] _xPosition;

	public Vector2 VectorFarWaveEdge;

	public Vector2 VectorNearWaveEdge;

	public Vector2 VectorPoint;

	public float Width
	{
		get
		{
			return _width;
		}
		set
		{
			_width = value;
		}
	}

	public float Height
	{
		get
		{
			return _height;
		}
		set
		{
			_height = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
		}
	}

	public int NodeCount
	{
		get
		{
			return _nodeCount;
		}
		set
		{
			_nodeCount = value;
		}
	}

	public float DampingCoefficient
	{
		get
		{
			return _dampningCoefficient;
		}
		set
		{
			_dampningCoefficient = value;
		}
	}

	public float[] CurrentWave => _currentWave;

	public float[] PreviousWave => _previousWave;

	public float[] XPosition
	{
		get
		{
			return _xPosition;
		}
		set
		{
			_xPosition = value;
		}
	}

	public float WaveGeneratorMax
	{
		get
		{
			return _waveGeneratorMax;
		}
		set
		{
			_waveGeneratorMax = value;
		}
	}

	public float WaveGeneratorMin
	{
		get
		{
			return _waveGeneratorMin;
		}
		set
		{
			_waveGeneratorMin = value;
		}
	}

	public float WaveGeneratorStep
	{
		get
		{
			return _waveGeneratorStep;
		}
		set
		{
			_waveGeneratorStep = value;
		}
	}

	public float Frequency
	{
		get
		{
			return _frequency;
		}
		set
		{
			_frequency = value;
		}
	}

	public bool Intersect(AABB aabb)
	{
		return AABB.Intersect(aabb, _aabb);
	}

	public bool Contains(ref Vector2 vector)
	{
		int num = (int)Math.Floor((vector.X - _xPosition[0]) / _singleWaveWidth);
		if (num > _nodeCount - 2)
		{
			num = _nodeCount - 2;
		}
		if (num < 0)
		{
			num = 0;
		}
		VectorNearWaveEdge.X = _xPosition[num];
		VectorNearWaveEdge.Y = _position.Y + _currentWave[num];
		VectorFarWaveEdge.X = _xPosition[num + 1];
		VectorFarWaveEdge.Y = _position.Y + _currentWave[num + 1];
		VectorPoint = vector;
		_waveEdgeVector.X = _xPosition[num + 1] - _xPosition[num];
		_waveEdgeVector.Y = _currentWave[num + 1] - _currentWave[num];
		_pointVector.X = vector.X - _xPosition[num];
		_pointVector.Y = vector.Y - (_position.Y + _currentWave[num]);
		Calculator.Cross(ref _waveEdgeVector, ref _pointVector, out var ret);
		if (ret < 0f)
		{
			return false;
		}
		return true;
	}

	public void Disturb(int node, float offset)
	{
		_currentWave[node] += offset;
	}

	public void Initialize()
	{
		_xPosition = new float[_nodeCount];
		_currentWave = new float[_nodeCount];
		_previousWave = new float[_nodeCount];
		_resultWave = new float[_nodeCount];
		for (int i = 0; i < _nodeCount; i++)
		{
			_xPosition[i] = MathHelper.Lerp(_position.X, _position.X + _width, (float)i / (float)(_nodeCount - 1));
			_currentWave[i] = 0f;
			_previousWave[i] = 0f;
			_resultWave[i] = 0f;
		}
		_aabb = new AABB(_position, new Vector2(_position.X + _width, _position.Y + _height));
		_singleWaveWidth = _width / (float)(_nodeCount - 1);
	}

	public override void Update(float dt)
	{
		if (_timePassed < _frequency)
		{
			_timePassed += dt;
			return;
		}
		_timePassed = 0f;
		_aabbMin = float.MaxValue;
		_aabb.min.Y = _aabbMin;
		for (int i = 1; i < _nodeCount - 1; i++)
		{
			_resultWave[i] = _currentWave[i - 1] + _currentWave[i + 1] - _previousWave[i];
			_resultWave[i] *= _dampningCoefficient;
			if (_resultWave[i] + _position.Y < _aabbMin)
			{
				_aabbMin = _resultWave[i] + _position.Y;
			}
		}
		_aabb.min.Y = _aabbMin;
		_currentWave.CopyTo(_previousWave, 0);
		_resultWave.CopyTo(_currentWave, 0);
		if (_goingUp)
		{
			if (_waveGeneratorCount > _waveGeneratorMax)
			{
				_goingUp = false;
			}
			else
			{
				_waveGeneratorCount += _waveGeneratorStep;
			}
		}
		else if (_waveGeneratorCount < _waveGeneratorMin)
		{
			_goingUp = true;
		}
		else
		{
			_waveGeneratorCount -= _waveGeneratorStep;
		}
		_currentWave[_currentWave.Length - 1] = ConvertUnits.ToSimUnits(_waveGeneratorCount);
	}

	public override void Validate()
	{
	}
}

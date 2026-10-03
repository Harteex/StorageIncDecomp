using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics;

public class Body : IDisposable
{
	public delegate void UpdatedEventHandler(ref Vector2 position, ref float rotation);

	private float _momentOfInertia = 1f;

	private float _previousAngularVelocity;

	private Vector2 _previousLinearVelocity = Vector2.Zero;

	private Vector2 _previousPosition = Vector2.Zero;

	private float _previousRotation;

	private int _revolutions;

	private Vector2 _tempVelocity = Vector2.Zero;

	private float _torque;

	public bool IsGoods = false;

	public bool IsTruck = false;

	public bool IsInGateZone = false;

	public float AngularVelocity;

	internal float angularVelocityBias;

	public bool Enabled = true;

	internal Vector2 force = Vector2.Zero;

	public bool IgnoreGravity;

	internal Vector2 impulse = Vector2.Zero;

	internal float inverseMass = 1f;

	internal float inverseMomentOfInertia = 1f;

	public bool IsDisposed;

	public bool IsQuadraticDragEnabled;

	internal bool isStatic;

	public float LinearDragCoefficient = 0.001f;

	public Vector2 LinearVelocity = Vector2.Zero;

	internal Vector2 linearVelocityBias = Vector2.Zero;

	internal float mass = 1f;

	internal Vector2 position = Vector2.Zero;

	public float QuadraticDragCoefficient = 0.001f;

	internal float rotation;

	public float RotationalDragCoefficient = 0.001f;

	public object Tag;

	internal float totalRotation;

	public UpdatedEventHandler Updated;

	public float IdleTime;

	public bool IsAutoIdle;

	public float MinimumVelocity = 55f;

	private Vector2 _velocityTemp = Vector2.Zero;

	private Vector2 _worldPositionTemp = Vector2.Zero;

	private float _bodyAngularVelocity;

	private Vector2 _bodylinearVelocity;

	private Vector2 _dp;

	private float _rotationChange;

	private Vector2 _acceleration = Vector2.Zero;

	private Vector2 _dv = Vector2.Zero;

	private float _dw;

	private Vector2 _linearDrag = Vector2.Zero;

	private Vector2 _quadraticDrag = Vector2.Zero;

	private float _rotationalDrag;

	private float _speed;

	private Vector2 _totalDrag = Vector2.Zero;

	private Vector2 _diff;

	private Vector2 _r1 = Vector2.Zero;

	private Vector2 _localPositionTemp = Vector2.Zero;

	private Matrix _bodyMatrixTemp = Matrix.Identity;

	private Matrix _rotationMatrixTemp = Matrix.Identity;

	private Matrix _translationMatrixTemp = Matrix.Identity;

	public bool Moves => LinearVelocity.Length() >= MinimumVelocity;

	public float Mass
	{
		get
		{
			return mass;
		}
		set
		{
			if (value == 0f)
			{
				throw new ArgumentException("Mass cannot be 0", "value");
			}
			mass = value;
			if (isStatic)
			{
				inverseMass = 0f;
			}
			else
			{
				inverseMass = 1f / value;
			}
		}
	}

	public float InverseMass => inverseMass;

	public float MomentOfInertia
	{
		get
		{
			return _momentOfInertia;
		}
		set
		{
			if (value <= 0f)
			{
				throw new ArgumentException("Moment of inertia cannot be 0 or less", "value");
			}
			_momentOfInertia = value;
			if (isStatic)
			{
				inverseMomentOfInertia = 0f;
			}
			else
			{
				inverseMomentOfInertia = 1f / value;
			}
		}
	}

	public float InverseMomentOfInertia => inverseMomentOfInertia;

	public bool IsStatic
	{
		get
		{
			return isStatic;
		}
		set
		{
			isStatic = value;
			if (isStatic)
			{
				inverseMass = 0f;
				inverseMomentOfInertia = 0f;
			}
			else
			{
				inverseMass = 1f / mass;
				inverseMomentOfInertia = 1f / _momentOfInertia;
			}
		}
	}

	public Vector2 Position
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			position = value;
			if (Updated != null)
			{
				Updated(ref position, ref rotation);
			}
		}
	}

	public int Revolutions => _revolutions;

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
			int num = (int)(rotation / ((float)Math.PI * 2f));
			if (rotation < 0f)
			{
				num--;
			}
			rotation -= (float)num * ((float)Math.PI * 2f);
			_revolutions += num;
			totalRotation = rotation + (float)_revolutions * ((float)Math.PI * 2f);
			if (Updated != null)
			{
				Updated(ref position, ref rotation);
			}
		}
	}

	public float TotalRotation => totalRotation;

	public Vector2 Force
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return force;
		}
	}

	public float Torque => _torque;

	public Vector2 XVectorInWorldCoordinates
	{
		get
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			_bodyMatrixTemp = GetBodyMatrix();
			return new Vector2(_bodyMatrixTemp.Right.X, _bodyMatrixTemp.Right.Y);
		}
	}

	public Vector2 YVectorInWorldCoordinates
	{
		get
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			_bodyMatrixTemp = GetBodyMatrix();
			return new Vector2(_bodyMatrixTemp.Up.X, _bodyMatrixTemp.Up.Y);
		}
	}

	public event EventHandler<EventArgs> Disposed;

	public Body()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
	}

	public Body(Body body)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		Mass = body.Mass;
		MomentOfInertia = body.MomentOfInertia;
		LinearDragCoefficient = body.LinearDragCoefficient;
		RotationalDragCoefficient = body.RotationalDragCoefficient;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void ResetDynamics()
	{
		LinearVelocity.X = 0f;
		LinearVelocity.Y = 0f;
		_previousLinearVelocity.X = 0f;
		_previousLinearVelocity.Y = 0f;
		AngularVelocity = 0f;
		_previousAngularVelocity = 0f;
		position.X = 0f;
		position.Y = 0f;
		_previousPosition.X = 0f;
		_previousPosition.Y = 0f;
		rotation = 0f;
		_revolutions = 0;
		totalRotation = 0f;
		force.X = 0f;
		force.Y = 0f;
		_torque = 0f;
		impulse.X = 0f;
		impulse.Y = 0f;
		_linearDrag.X = 0f;
		_linearDrag.Y = 0f;
		_rotationalDrag = 0f;
	}

	public Matrix GetBodyMatrix()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreateTranslation(position.X, position.Y, 0f, ref _translationMatrixTemp);
		Matrix.CreateRotationZ(rotation, ref _rotationMatrixTemp);
		Matrix.Multiply(ref _rotationMatrixTemp, ref _translationMatrixTemp, ref _bodyMatrixTemp);
		return _bodyMatrixTemp;
	}

	public void GetBodyMatrix(out Matrix bodyMatrix)
	{
		Matrix.CreateTranslation(position.X, position.Y, 0f, ref _translationMatrixTemp);
		Matrix.CreateRotationZ(rotation, ref _rotationMatrixTemp);
		Matrix.Multiply(ref _rotationMatrixTemp, ref _translationMatrixTemp, ref bodyMatrix);
	}

	public Matrix GetBodyRotationMatrix()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreateRotationZ(rotation, ref _rotationMatrixTemp);
		return _rotationMatrixTemp;
	}

	public void GetBodyRotationMatrix(out Matrix rotationMatrix)
	{
		Matrix.CreateRotationZ(rotation, ref rotationMatrix);
	}

	public Vector2 GetWorldPosition(Vector2 localPosition)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		GetBodyMatrix(out _bodyMatrixTemp);
		Vector2.Transform(ref localPosition, ref _bodyMatrixTemp, ref _worldPositionTemp);
		return _worldPositionTemp;
	}

	public void GetWorldPosition(ref Vector2 localPosition, out Vector2 worldPosition)
	{
		GetBodyMatrix(out _bodyMatrixTemp);
		Vector2.Transform(ref localPosition, ref _bodyMatrixTemp, ref worldPosition);
	}

	public Vector2 GetLocalPosition(Vector2 worldPosition)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		GetBodyRotationMatrix(out _rotationMatrixTemp);
		Matrix.Transpose(ref _rotationMatrixTemp, ref _rotationMatrixTemp);
		Vector2.Subtract(ref worldPosition, ref position, ref _localPositionTemp);
		Vector2.Transform(ref _localPositionTemp, ref _rotationMatrixTemp, ref _localPositionTemp);
		return _localPositionTemp;
	}

	public void GetLocalPosition(ref Vector2 worldPosition, out Vector2 localPosition)
	{
		GetBodyRotationMatrix(out _rotationMatrixTemp);
		Matrix.Transpose(ref _rotationMatrixTemp, ref _rotationMatrixTemp);
		Vector2.Subtract(ref worldPosition, ref position, ref localPosition);
		Vector2.Transform(ref localPosition, ref _rotationMatrixTemp, ref localPosition);
	}

	public Vector2 GetVelocityAtLocalPoint(Vector2 localPoint)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		GetVelocityAtLocalPoint(ref localPoint, out _tempVelocity);
		return _tempVelocity;
	}

	public void GetVelocityAtLocalPoint(ref Vector2 localPoint, out Vector2 velocity)
	{
		GetWorldPosition(ref localPoint, out _r1);
		Vector2.Subtract(ref _r1, ref position, ref _r1);
		GetVelocityAtWorldOffset(ref _r1, out velocity);
	}

	public void GetVelocityAtWorldPoint(ref Vector2 worldPoint, out Vector2 velocity)
	{
		Vector2.Subtract(ref worldPoint, ref position, ref _r1);
		GetVelocityAtWorldOffset(ref _r1, out velocity);
	}

	public void GetVelocityAtWorldOffset(ref Vector2 offset, out Vector2 velocity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		velocity = _velocityTemp;
		velocity.X = (0f - AngularVelocity) * offset.Y;
		velocity.Y = AngularVelocity * offset.X;
		velocity.X += LinearVelocity.X;
		velocity.Y += LinearVelocity.Y;
	}

	public void GetVelocityBiasAtWorldOffset(ref Vector2 offset, out Vector2 velocityBias)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		velocityBias = _velocityTemp;
		velocityBias.X = (0f - angularVelocityBias) * offset.Y;
		velocityBias.Y = angularVelocityBias * offset.X;
		velocityBias.X += linearVelocityBias.X;
		velocityBias.Y += linearVelocityBias.Y;
	}

	public void ApplyForce(Vector2 amount)
	{
		force.X += amount.X;
		force.Y += amount.Y;
	}

	public void ApplyForce(ref Vector2 amount)
	{
		force.X += amount.X;
		force.Y += amount.Y;
	}

	public void ApplyForceAtLocalPoint(Vector2 amount, Vector2 point)
	{
		GetWorldPosition(ref point, out _diff);
		Vector2.Subtract(ref _diff, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtLocalPoint(ref Vector2 amount, ref Vector2 point)
	{
		GetWorldPosition(ref point, out _diff);
		Vector2.Subtract(ref _diff, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtWorldPoint(ref Vector2 amount, ref Vector2 point)
	{
		Vector2.Subtract(ref point, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtWorldPoint(Vector2 amount, Vector2 point)
	{
		Vector2.Subtract(ref point, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ClearForce()
	{
		force.X = 0f;
		force.Y = 0f;
	}

	public void ApplyTorque(float torque)
	{
		_torque += torque;
	}

	public void ClearTorque()
	{
		_torque = 0f;
	}

	public void ApplyImpulse(Vector2 amount)
	{
		_dv.X = amount.X * inverseMass;
		_dv.Y = amount.Y * inverseMass;
		ref Vector2 reference = ref impulse;
		reference.X += _dv.X;
		ref Vector2 reference2 = ref impulse;
		reference2.Y += _dv.Y;
	}

	public void ApplyImpulse(ref Vector2 amount)
	{
		_dv.X = amount.X * inverseMass;
		_dv.Y = amount.Y * inverseMass;
		ref Vector2 reference = ref impulse;
		reference.X += _dv.X;
		ref Vector2 reference2 = ref impulse;
		reference2.Y += _dv.Y;
	}

	internal void ApplyImpulses()
	{
		ApplyImmediateImpulse(ref impulse);
		impulse.X = 0f;
		impulse.Y = 0f;
	}

	internal void ApplyImmediateImpulse(ref Vector2 amount)
	{
		_dv.X = amount.X * inverseMass;
		_dv.Y = amount.Y * inverseMass;
		LinearVelocity.X = _dv.X + LinearVelocity.X;
		LinearVelocity.Y = _dv.Y + LinearVelocity.Y;
	}

	public void ClearImpulse()
	{
		impulse.X = 0f;
		impulse.Y = 0f;
	}

	public void ApplyAngularImpulse(float amount)
	{
		AngularVelocity += amount * inverseMomentOfInertia;
	}

	public void ApplyAngularImpulse(ref float amount)
	{
		AngularVelocity += amount * inverseMomentOfInertia;
	}

	private void ApplyDrag()
	{
		if (IsQuadraticDragEnabled)
		{
			float num = LinearVelocity.X * LinearVelocity.X + LinearVelocity.Y * LinearVelocity.Y;
			_speed = (float)Math.Sqrt(num);
		}
		_linearDrag.X = (0f - LinearVelocity.X) * LinearDragCoefficient;
		_linearDrag.Y = (0f - LinearVelocity.Y) * LinearDragCoefficient;
		if (IsQuadraticDragEnabled)
		{
			_quadraticDrag.X = (0f - QuadraticDragCoefficient) * _speed * LinearVelocity.X;
			_quadraticDrag.Y = (0f - QuadraticDragCoefficient) * _speed * LinearVelocity.Y;
			_totalDrag.X = _linearDrag.X + _quadraticDrag.X;
			_totalDrag.Y = _linearDrag.Y + _quadraticDrag.Y;
			ApplyForce(ref _totalDrag);
		}
		else
		{
			ApplyForce(ref _linearDrag);
		}
		_rotationalDrag = AngularVelocity * AngularVelocity * (float)Math.Sign(AngularVelocity);
		_rotationalDrag *= 0f - RotationalDragCoefficient;
		ApplyTorque(_rotationalDrag);
	}

	internal void IntegrateVelocity(float dt)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!isStatic)
		{
			ApplyDrag();
			_acceleration.X = force.X * inverseMass;
			_acceleration.Y = force.Y * inverseMass;
			_dv.X = _acceleration.X * dt;
			_dv.Y = _acceleration.Y * dt;
			_previousLinearVelocity = LinearVelocity;
			LinearVelocity.X = _previousLinearVelocity.X + _dv.X;
			LinearVelocity.Y = _previousLinearVelocity.Y + _dv.Y;
			_dw = _torque * inverseMomentOfInertia * dt;
			_previousAngularVelocity = AngularVelocity;
			AngularVelocity = _previousAngularVelocity + _dw;
		}
	}

	public bool HasMoved()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (Position == _previousPosition && Rotation == _previousRotation)
		{
			return false;
		}
		return true;
	}

	internal void IntegratePosition(float dt)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!isStatic)
		{
			_bodylinearVelocity.X = LinearVelocity.X + linearVelocityBias.X;
			_bodylinearVelocity.Y = LinearVelocity.Y + linearVelocityBias.Y;
			_dp.X = _bodylinearVelocity.X * dt;
			_dp.Y = _bodylinearVelocity.Y * dt;
			_previousPosition = position;
			position.X = _previousPosition.X + _dp.X;
			position.Y = _previousPosition.Y + _dp.Y;
			linearVelocityBias.X = 0f;
			linearVelocityBias.Y = 0f;
			_bodyAngularVelocity = AngularVelocity + angularVelocityBias;
			_rotationChange = _bodyAngularVelocity * dt;
			_previousRotation = rotation;
			rotation = _previousRotation + _rotationChange;
			int num = (int)(rotation / ((float)Math.PI * 2f));
			if (rotation < 0f)
			{
				num--;
			}
			rotation -= (float)num * ((float)Math.PI * 2f);
			_revolutions += num;
			totalRotation = rotation + (float)_revolutions * ((float)Math.PI * 2f);
			angularVelocityBias = 0f;
			if (Updated != null)
			{
				Updated(ref position, ref rotation);
			}
		}
	}

	internal void ApplyImpulseAtWorldOffset(ref Vector2 amount, ref Vector2 offset)
	{
		_dv.X = amount.X * inverseMass;
		_dv.Y = amount.Y * inverseMass;
		LinearVelocity.X = _dv.X + LinearVelocity.X;
		LinearVelocity.Y = _dv.Y + LinearVelocity.Y;
		float num = offset.X * amount.Y - offset.Y * amount.X;
		num *= inverseMomentOfInertia;
		AngularVelocity += num;
	}

	internal void ApplyBiasImpulseAtWorldOffset(ref Vector2 impulseBias, ref Vector2 offset)
	{
		_dv.X = impulseBias.X * inverseMass;
		_dv.Y = impulseBias.Y * inverseMass;
		linearVelocityBias.X = _dv.X + linearVelocityBias.X;
		linearVelocityBias.Y = _dv.Y + linearVelocityBias.Y;
		float num = offset.X * impulseBias.Y - offset.Y * impulseBias.X;
		num *= inverseMomentOfInertia;
		angularVelocityBias += num;
	}

	protected virtual void Dispose(bool disposing)
	{
		IsDisposed = true;
		if (Disposed != null)
		{
			Disposed(this, EventArgs.Empty);
		}
	}
}

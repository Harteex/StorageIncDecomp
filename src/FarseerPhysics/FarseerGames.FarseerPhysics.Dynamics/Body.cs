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
			return position;
		}
		set
		{
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
			return force;
		}
	}

	public float Torque => _torque;

	public Vector2 XVectorInWorldCoordinates
	{
		get
		{
			_bodyMatrixTemp = GetBodyMatrix();
			return new Vector2(_bodyMatrixTemp.Right.X, _bodyMatrixTemp.Right.Y);
		}
	}

	public Vector2 YVectorInWorldCoordinates
	{
		get
		{
			_bodyMatrixTemp = GetBodyMatrix();
			return new Vector2(_bodyMatrixTemp.Up.X, _bodyMatrixTemp.Up.Y);
		}
	}

	public event EventHandler<EventArgs> Disposed;

	public Body()
	{
	}

	public Body(Body body)
	{
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
		Matrix.CreateTranslation(position.X, position.Y, 0f, out _translationMatrixTemp);
		Matrix.CreateRotationZ(rotation, out _rotationMatrixTemp);
		Matrix.Multiply(ref _rotationMatrixTemp, ref _translationMatrixTemp, out _bodyMatrixTemp);
		return _bodyMatrixTemp;
	}

	public void GetBodyMatrix(out Matrix bodyMatrix)
	{
		Matrix.CreateTranslation(position.X, position.Y, 0f, out _translationMatrixTemp);
		Matrix.CreateRotationZ(rotation, out _rotationMatrixTemp);
		Matrix.Multiply(ref _rotationMatrixTemp, ref _translationMatrixTemp, out bodyMatrix);
	}

	public Matrix GetBodyRotationMatrix()
	{
		Matrix.CreateRotationZ(rotation, out _rotationMatrixTemp);
		return _rotationMatrixTemp;
	}

	public void GetBodyRotationMatrix(out Matrix rotationMatrix)
	{
		Matrix.CreateRotationZ(rotation, out rotationMatrix);
	}

	public Vector2 GetWorldPosition(Vector2 localPosition)
	{
		GetBodyMatrix(out _bodyMatrixTemp);
		Vector2.Transform(ref localPosition, ref _bodyMatrixTemp, out _worldPositionTemp);
		return _worldPositionTemp;
	}

	public void GetWorldPosition(ref Vector2 localPosition, out Vector2 worldPosition)
	{
		GetBodyMatrix(out _bodyMatrixTemp);
		Vector2.Transform(ref localPosition, ref _bodyMatrixTemp, out worldPosition);
	}

	public Vector2 GetLocalPosition(Vector2 worldPosition)
	{
		GetBodyRotationMatrix(out _rotationMatrixTemp);
		Matrix.Transpose(ref _rotationMatrixTemp, out _rotationMatrixTemp);
		Vector2.Subtract(ref worldPosition, ref position, out _localPositionTemp);
		Vector2.Transform(ref _localPositionTemp, ref _rotationMatrixTemp, out _localPositionTemp);
		return _localPositionTemp;
	}

	public void GetLocalPosition(ref Vector2 worldPosition, out Vector2 localPosition)
	{
		GetBodyRotationMatrix(out _rotationMatrixTemp);
		Matrix.Transpose(ref _rotationMatrixTemp, out _rotationMatrixTemp);
		Vector2.Subtract(ref worldPosition, ref position, out localPosition);
		Vector2.Transform(ref localPosition, ref _rotationMatrixTemp, out localPosition);
	}

	public Vector2 GetVelocityAtLocalPoint(Vector2 localPoint)
	{
		GetVelocityAtLocalPoint(ref localPoint, out _tempVelocity);
		return _tempVelocity;
	}

	public void GetVelocityAtLocalPoint(ref Vector2 localPoint, out Vector2 velocity)
	{
		GetWorldPosition(ref localPoint, out _r1);
		Vector2.Subtract(ref _r1, ref position, out _r1);
		GetVelocityAtWorldOffset(ref _r1, out velocity);
	}

	public void GetVelocityAtWorldPoint(ref Vector2 worldPoint, out Vector2 velocity)
	{
		Vector2.Subtract(ref worldPoint, ref position, out _r1);
		GetVelocityAtWorldOffset(ref _r1, out velocity);
	}

	public void GetVelocityAtWorldOffset(ref Vector2 offset, out Vector2 velocity)
	{
		velocity = _velocityTemp;
		velocity.X = (0f - AngularVelocity) * offset.Y;
		velocity.Y = AngularVelocity * offset.X;
		velocity.X += LinearVelocity.X;
		velocity.Y += LinearVelocity.Y;
	}

	public void GetVelocityBiasAtWorldOffset(ref Vector2 offset, out Vector2 velocityBias)
	{
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
		Vector2.Subtract(ref _diff, ref position, out _diff);
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
		Vector2.Subtract(ref _diff, ref position, out _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtWorldPoint(ref Vector2 amount, ref Vector2 point)
	{
		Vector2.Subtract(ref point, ref position, out _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtWorldPoint(Vector2 amount, Vector2 point)
	{
		Vector2.Subtract(ref point, ref position, out _diff);
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
		if (Position == _previousPosition && Rotation == _previousRotation)
		{
			return false;
		}
		return true;
	}

	internal void IntegratePosition(float dt)
	{
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

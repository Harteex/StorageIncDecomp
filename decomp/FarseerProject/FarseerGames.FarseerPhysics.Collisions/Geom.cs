using System;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class Geom : IEquatable<Geom>, IDisposable
{
	public delegate bool CollisionEventHandler(Geom geometry1, Geom geometry2, ContactList contactList);

	public delegate void SeparationEventHandler(Geom geometry1, Geom geometry2);

	private float _collisionGridCellSize;

	private bool _isSensor;

	private Matrix _matrix = Matrix.Identity;

	private Matrix _matrixInverse = Matrix.Identity;

	private Matrix _matrixInverseTemp;

	private Vector2 _offset = Vector2.Zero;

	private Vector2 _position = new Vector2(0f, 0f);

	private float _rotation;

	private float _rotationOffset;

	private Vector2 _vector;

	internal AABB aabb = new AABB();

	internal Body body;

	internal CollisionCategory collidesWith = CollisionCategory.All;

	internal CollisionCategory collisionCategories = CollisionCategory.All;

	internal bool collisionEnabled = true;

	internal int collisionGroup;

	internal bool collisionResponseEnabled = true;

	internal float frictionCoefficient;

	internal Grid grid;

	public bool IsDisposed;

	internal bool isRemoved = true;

	internal Vertices localVertices;

	public CollisionEventHandler OnCollision;

	public SeparationEventHandler OnSeparation;

	internal float restitutionCoefficient;

	internal Vertices worldVertices;

	private static int _newId = -1;

	private Vector2 _localVertice;

	private Vector2 _vertice = Vector2.Zero;

	private Vector2 _newPos = Vector2.Zero;

	public Vector2 Position
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _position;
		}
	}

	public float Rotation => _rotation;

	public float CollisionGridCellSize
	{
		get
		{
			return _collisionGridCellSize;
		}
		set
		{
			_collisionGridCellSize = value;
		}
	}

	public Vertices LocalVertices => localVertices;

	public Vertices WorldVertices => worldVertices;

	public Matrix Matrix
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return _matrix;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_matrix = value;
			Update();
		}
	}

	public Matrix MatrixInverse
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			Matrix.Invert(ref _matrix, ref _matrixInverse);
			return _matrixInverse;
		}
	}

	public AABB AABB
	{
		get
		{
			return aabb;
		}
		set
		{
			aabb = value;
		}
	}

	public int CollisionGroup
	{
		get
		{
			return collisionGroup;
		}
		set
		{
			collisionGroup = value;
		}
	}

	public bool CollisionEnabled
	{
		get
		{
			return collisionEnabled;
		}
		set
		{
			collisionEnabled = value;
		}
	}

	public bool IsSensor
	{
		get
		{
			return _isSensor;
		}
		set
		{
			_isSensor = value;
			if (_isSensor)
			{
				body.isStatic = true;
				collisionResponseEnabled = false;
			}
			else
			{
				body.isStatic = false;
				collisionResponseEnabled = true;
			}
		}
	}

	public bool CollisionResponseEnabled
	{
		get
		{
			return collisionResponseEnabled;
		}
		set
		{
			collisionResponseEnabled = value;
		}
	}

	public CollisionCategory CollisionCategories
	{
		get
		{
			return collisionCategories;
		}
		set
		{
			collisionCategories = value;
		}
	}

	public CollisionCategory CollidesWith
	{
		get
		{
			return collidesWith;
		}
		set
		{
			collidesWith = value;
		}
	}

	public Grid Grid => grid;

	public Body Body => body;

	public float RestitutionCoefficient
	{
		get
		{
			return restitutionCoefficient;
		}
		set
		{
			restitutionCoefficient = value;
		}
	}

	public float FrictionCoefficient
	{
		get
		{
			return frictionCoefficient;
		}
		set
		{
			frictionCoefficient = value;
		}
	}

	public object Tag { get; set; }

	internal int Id { get; private set; }

	public Geom()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Id = GetNextId();
		grid = new Grid();
	}

	public Geom(Body body, Vertices vertices, float collisionGridCellSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Construct(body, vertices, Vector2.Zero, 0f, collisionGridCellSize);
	}

	public Geom(Body body, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Construct(body, vertices, offset, rotationOffset, collisionGridCellSize);
	}

	public Geom(Body body, Geom geometry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		ConstructClone(body, geometry, geometry._offset, geometry._rotationOffset);
	}

	public Geom(Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ConstructClone(body, geometry, offset, rotationOffset);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public bool Equals(Geom other)
	{
		if ((object)other == null)
		{
			return false;
		}
		return Id == other.Id;
	}

	private void Construct(Body bodyToSet, Vertices vertices, Vector2 offset, float rotationOffset, float collisionGridCellSize)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Id = GetNextId();
		_collisionGridCellSize = collisionGridCellSize;
		_offset = offset;
		_rotationOffset = rotationOffset;
		grid = new Grid();
		SetVertices(vertices);
		ComputeCollisionGrid();
		SetBody(bodyToSet);
	}

	private void ConstructClone(Body bodyToSet, Geom geometry, Vector2 offset, float rotationOffset)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Id = GetNextId();
		_collisionGridCellSize = geometry._collisionGridCellSize;
		grid = geometry.grid.Clone();
		restitutionCoefficient = geometry.restitutionCoefficient;
		frictionCoefficient = geometry.frictionCoefficient;
		collisionGroup = geometry.collisionGroup;
		collisionEnabled = geometry.collisionEnabled;
		collisionResponseEnabled = geometry.collisionResponseEnabled;
		_collisionGridCellSize = geometry._collisionGridCellSize;
		_offset = offset;
		_rotationOffset = rotationOffset;
		collisionCategories = geometry.collisionCategories;
		collidesWith = geometry.collidesWith;
		SetVertices(geometry.localVertices);
		SetBody(bodyToSet);
	}

	public void SetVertices(Vertices vertices)
	{
		vertices.ForceCounterClockWiseOrder();
		localVertices = new Vertices(vertices);
		worldVertices = new Vertices(vertices);
		aabb.Update(ref vertices);
	}

	public void SetBody(Body bodyToSet)
	{
		body = bodyToSet;
		bodyToSet.Updated = (Body.UpdatedEventHandler)Delegate.Combine(bodyToSet.Updated, new Body.UpdatedEventHandler(BodyOnChange));
		bodyToSet.Disposed += BodyOnDisposed;
		Update(ref bodyToSet.position, ref bodyToSet.rotation);
	}

	public void ComputeCollisionGrid()
	{
		if (localVertices.Count > 2)
		{
			grid.ComputeGrid(this, _collisionGridCellSize);
		}
		else
		{
			grid = null;
		}
	}

	public Vector2 GetWorldPosition(Vector2 localPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Transform(localPosition, _matrix);
	}

	public float GetNearestDistance(Vector2 point)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		int index = 0;
		for (int i = 0; i < localVertices.Count; i++)
		{
			float distanceToEdge = GetDistanceToEdge(ref point, i);
			if (distanceToEdge < num)
			{
				num = distanceToEdge;
				index = i;
			}
		}
		Feature nearestFeature = GetNearestFeature(point, index);
		Vector2 val = Vector2.Subtract(point, nearestFeature.Position);
		float num2 = Vector2.Dot(val, nearestFeature.Normal);
		if (num2 < 0f)
		{
			return 0f - nearestFeature.Distance;
		}
		return nearestFeature.Distance;
	}

	private float GetDistanceToEdge(ref Vector2 point, int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 edge = localVertices.GetEdge(index);
		Vector2 val = Vector2.Subtract(point, localVertices[index]);
		float num = Vector2.Dot(val, edge);
		if (num < 0f)
		{
			return Math.Abs(val.Length());
		}
		float num2 = Vector2.Dot(edge, edge);
		if (num2 <= num)
		{
			return Vector2.Distance(point, localVertices[localVertices.NextIndex(index)]);
		}
		float num3 = num / num2;
		edge = Vector2.Multiply(edge, num3);
		Vector2 val2 = Vector2.Add(localVertices[index], edge);
		return Vector2.Distance(point, val2);
	}

	public Feature GetNearestFeature(Vector2 point, int index)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Feature result = default;
		Vector2 edge = localVertices.GetEdge(index);
		Vector2 val = Vector2.Subtract(point, localVertices[index]);
		float num = Vector2.Dot(val, edge);
		if (num < 0f)
		{
			result.Position = localVertices[index];
			result.Normal = localVertices.GetVertexNormal(index);
			result.Distance = Math.Abs(val.Length());
			return result;
		}
		float num2 = Vector2.Dot(edge, edge);
		if (num2 <= num)
		{
			Vector2 val2 = Vector2.Subtract(point, localVertices[localVertices.NextIndex(index)]);
			result.Position = localVertices[localVertices.NextIndex(index)];
			result.Normal = localVertices.GetVertexNormal(localVertices.NextIndex(index));
			result.Distance = Math.Abs(val2.Length());
			return result;
		}
		float num3 = num / num2;
		edge = Vector2.Multiply(edge, num3);
		Vector2 val3 = Vector2.Add(localVertices[index], edge);
		Vector2 val4 = Vector2.Subtract(point, val3);
		result.Position = val3;
		result.Normal = localVertices.GetEdgeNormal(index);
		result.Distance = val4.Length();
		return result;
	}

	public bool Collide(Vector2 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		point = Vector2.Transform(point, MatrixInverse);
		grid.Intersect(ref point, out var feature);
		if (feature.Distance < 0f)
		{
			return true;
		}
		return false;
	}

	public bool Collide(Geom geometry)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < worldVertices.Count; i++)
		{
			_vector = worldVertices[i];
			if (geometry.Collide(_vector))
			{
				return true;
			}
		}
		for (int i = 0; i < geometry.worldVertices.Count; i++)
		{
			_vector = geometry.worldVertices[i];
			if (Collide(_vector))
			{
				return true;
			}
		}
		return false;
	}

	public bool Intersect(ref Vector2 localVertex, out Feature feature)
	{
		return grid.Intersect(ref localVertex, out feature);
	}

	public void TransformToLocalCoordinates(ref Vector2 worldVertex, out Vector2 localVertex)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_matrixInverseTemp = MatrixInverse;
		Vector2.Transform(ref worldVertex, ref _matrixInverseTemp, ref localVertex);
	}

	public void TransformNormalToWorld(ref Vector2 localNormal, out Vector2 worldNormal)
	{
		Vector2.TransformNormal(ref localNormal, ref _matrix, ref worldNormal);
	}

	private void Update(ref Vector2 position, ref float orientation)
	{
		Matrix.CreateRotationZ(orientation + _rotationOffset, ref _matrix);
		float x = _offset.X * _matrix.M11 + _offset.Y * _matrix.M21 + _matrix.M41;
		float y = _offset.X * _matrix.M12 + _offset.Y * _matrix.M22 + _matrix.M42;
		_newPos.X = x;
		_newPos.Y = y;
		_matrix.M41 = position.X + _newPos.X;
		_matrix.M42 = position.Y + _newPos.Y;
		_matrix.M44 = 1f;
		_position.X = _matrix.M41;
		_position.Y = _matrix.M42;
		_rotation = body.rotation + _rotationOffset;
		Update();
	}

	private void Update()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < localVertices.Count; i++)
		{
			_localVertice = localVertices[i];
			float x = _localVertice.X * _matrix.M11 + _localVertice.Y * _matrix.M21 + _matrix.M41;
			float y = _localVertice.X * _matrix.M12 + _localVertice.Y * _matrix.M22 + _matrix.M42;
			_vertice.X = x;
			_vertice.Y = y;
			worldVertices[i] = _vertice;
		}
		aabb.Update(ref worldVertices);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Geom other))
		{
			return false;
		}
		return Equals(other);
	}

	public static bool operator ==(Geom geometry1, Geom geometry2)
	{
		if (object.ReferenceEquals(geometry1, geometry2))
		{
			return true;
		}
		if ((object)geometry1 == null || (object)geometry2 == null)
		{
			return false;
		}
		return geometry1.Equals(geometry2);
	}

	public static bool operator !=(Geom geometry1, Geom geometry2)
	{
		return !(geometry1 == geometry2);
	}

	public static bool operator <(Geom geometry1, Geom geometry2)
	{
		return geometry1.Id < geometry2.Id;
	}

	public static bool operator >(Geom geometry1, Geom geometry2)
	{
		return geometry1.Id > geometry2.Id;
	}

	public static int GetNextId()
	{
		_newId++;
		return _newId;
	}

	private void BodyOnChange(ref Vector2 position, ref float rotation)
	{
		Update(ref position, ref rotation);
	}

	private void BodyOnDisposed(object sender, EventArgs e)
	{
		Dispose();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed && disposing && this.body != null)
		{
			Body body = this.body;
			body.Updated = (Body.UpdatedEventHandler)Delegate.Remove(body.Updated, new Body.UpdatedEventHandler(BodyOnChange));
			this.body.Disposed -= BodyOnDisposed;
		}
		IsDisposed = true;
	}
}

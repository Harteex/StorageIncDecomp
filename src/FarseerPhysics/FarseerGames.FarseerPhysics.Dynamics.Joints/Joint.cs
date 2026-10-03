using System;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public abstract class Joint : IDisposable
{
	public float BiasFactor = 0.2f;

	public float Breakpoint = float.MaxValue;

	public bool Enabled = true;

	public bool IsDisposed;

	public float Softness;

	public object Tag { get; set; }

	public float JointError { get; protected set; }

	public event EventHandler<EventArgs> Broke;

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public abstract void Validate();

	public abstract void PreStep(float inverseDt);

	public virtual void Update()
	{
		if (Enabled && !(Math.Abs(JointError) <= Breakpoint))
		{
			Enabled = false;
			if (Broke != null)
			{
				Broke(this, EventArgs.Empty);
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (IsDisposed || disposing)
		{
		}
		IsDisposed = true;
	}
}

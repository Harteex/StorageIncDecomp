using System;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public abstract class Spring : IDisposable
{
	public float Breakpoint = float.MaxValue;

	public float DampingConstant;

	public bool Enabled = true;

	public bool IsDisposed;

	public float SpringConstant;

	public object Tag { get; set; }

	public float SpringError { get; protected set; }

	public event EventHandler<EventArgs> Broke;

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public abstract void Validate();

	public virtual void Update(float dt)
	{
		if (Enabled && !(Math.Abs(SpringError) <= Breakpoint))
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

using System;

namespace FarseerGames.FarseerPhysics.Controllers;

public abstract class Controller : IDisposable
{
	public bool Enabled = true;

	public bool IsDisposed;

	public object Tag { get; set; }

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public abstract void Validate();

	public abstract void Update(float dt);

	protected virtual void Dispose(bool disposing)
	{
		if (IsDisposed || disposing)
		{
		}
		IsDisposed = true;
	}
}

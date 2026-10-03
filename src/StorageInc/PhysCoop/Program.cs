using System;
using Microsoft.Xna.Framework;

namespace PhysCoop;

internal static class Program
{
	private static void Main()
	{
		PhysCoop physCoop = new PhysCoop();
		try
		{
			((Game)physCoop).Run();
		}
		finally
		{
			((IDisposable)physCoop)?.Dispose();
		}
	}
}

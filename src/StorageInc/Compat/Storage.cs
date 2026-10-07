// XNA 3.0 storage API on top of plain directories. The game opens a container and then
// uses System.IO on container.Path; XNA 4 / FNA containers have no Path, so this keeps the
// decompiled save/load code unchanged.
using System;
using System.IO;

namespace Microsoft.Xna.Framework.Storage
{
	public sealed class LegacyStorageContainer : IDisposable
	{
		internal LegacyStorageContainer(string titleName)
		{
			Path = System.IO.Path.Combine(SaveRoot, titleName, "AllPlayers");
			Directory.CreateDirectory(Path);
		}

		/// <summary>Directory holding this container's files.</summary>
		public string Path { get; }

		/// <summary>Directory the game was started from (XNA 3.0 StorageContainer.TitleLocation).</summary>
		public static string TitleLocation => AppContext.BaseDirectory;

		/// <summary>
		/// Per-OS save folder, matching FNA's own StorageDevice roots: Documents/SavedGames on
		/// Windows (like XNA), ~/Library/Application Support on macOS, the XDG data dir elsewhere.
		/// </summary>
		private static string SaveRoot
		{
			get
			{
				if (OperatingSystem.IsWindows())
				{
					return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SavedGames");
				}
				if (OperatingSystem.IsMacOS())
				{
					return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support");
				}
				string xdg = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
				if (string.IsNullOrEmpty(xdg))
				{
					xdg = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
				}
				return xdg;
			}
		}

		public void Dispose()
		{
		}
	}

	public static class LegacyStorageExtensions
	{
		/// <summary>XNA 3.0 StorageDevice.OpenContainer.</summary>
		public static LegacyStorageContainer OpenContainer(this StorageDevice device, string titleName)
		{
			return new LegacyStorageContainer(titleName);
		}
	}
}

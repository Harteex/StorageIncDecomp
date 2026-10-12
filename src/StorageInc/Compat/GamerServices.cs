// Minimal stand-in for XNA's Microsoft.Xna.Framework.GamerServices, which FNA does not
// implement. Only the surface Storage Inc uses is provided, with PC-friendly behaviour:
//   - Nobody is signed in at startup. The game signs players in and out through
//     SignedInGamers.SignIn/SignOut (Port changes): pressing Start signs in the main
//     player, and X on Game Setup toggles the other controllers, standing in for the
//     Xbox sign-in Guide. Like an Xbox profile, a sign-in survives a gamepad disconnect.
//   - Always the full version: no trial mode, no marketplace, the Guide is never visible.
//   - The storage device selector forwards to FNA's StorageDevice selector.
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Microsoft.Xna.Framework.GamerServices
{
	public static class Guide
	{
		public static bool IsVisible => false;

		public static bool IsTrialMode => false;

		public static void ShowSignIn(int paneCount, bool onlineOnly)
		{
		}

		public static void ShowMarketplace(PlayerIndex player)
		{
		}

		public static IAsyncResult BeginShowStorageDeviceSelector(PlayerIndex player, AsyncCallback callback, object state)
		{
			return StorageDevice.BeginShowSelector(player, callback, state);
		}

		public static StorageDevice EndShowStorageDeviceSelector(IAsyncResult result)
		{
			return StorageDevice.EndShowSelector(result);
		}
	}

	public abstract class Gamer
	{
		public static SignedInGamerCollection SignedInGamers { get; } = new SignedInGamerCollection();

		public string Gamertag { get; protected set; }

		public object Tag { get; set; }

		public abstract GamerProfile GetProfile();

		public override string ToString() => Gamertag;
	}

	public sealed class SignedInGamer : Gamer
	{
		public static event EventHandler<SignedInEventArgs> SignedIn;

		public static event EventHandler<SignedOutEventArgs> SignedOut;

		private readonly GamerProfile profile;

		internal SignedInGamer(PlayerIndex index)
		{
			PlayerIndex = index;
			Gamertag = "Player " + ((int)index + 1);
			profile = new GamerProfile(index);
		}

		public PlayerIndex PlayerIndex { get; }

		public bool IsGuest => false;

		public bool IsSignedInToLive => false;

		public GamerPrivileges Privileges { get; } = new GamerPrivileges();

		public GameDefaults GameDefaults { get; } = new GameDefaults();

		public override GamerProfile GetProfile() => profile;

		internal static void RaiseSignedIn(SignedInGamer gamer) => SignedIn?.Invoke(null, new SignedInEventArgs(gamer));

		internal static void RaiseSignedOut(SignedInGamer gamer) => SignedOut?.Invoke(null, new SignedOutEventArgs(gamer));
	}

	public sealed class GamerPrivileges
	{
		public bool AllowPurchaseContent => false;
	}

	/// <summary>Values match XNA (the game switches on the raw numbers).</summary>
	public enum ControllerSensitivity
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	/// <summary>Dashboard defaults of a profile; PC profiles use XNA's defaults.</summary>
	public sealed class GameDefaults
	{
		public bool InvertYAxis => false;

		public bool MoveWithRightThumbStick => false;

		public ControllerSensitivity ControllerSensitivity => ControllerSensitivity.Medium;
	}

	public sealed class GamerProfile
	{
		private static readonly Color[] PictureColors =
		{
			new Color(64, 128, 255), new Color(255, 64, 64), new Color(64, 200, 64), new Color(255, 200, 0)
		};

		private readonly PlayerIndex index;
		private Texture2D picture;

		internal GamerProfile(PlayerIndex index)
		{
			this.index = index;
		}

		/// <summary>A flat 64x64 placeholder in the player's colour (created on first use).</summary>
		public Texture2D GamerPicture
		{
			get
			{
				if (picture == null && GamerServicesComponent.Device != null)
				{
					picture = new Texture2D(GamerServicesComponent.Device, 64, 64);
					Color[] pixels = new Color[64 * 64];
					Array.Fill(pixels, PictureColors[(int)index]);
					picture.SetData(pixels);
				}
				return picture;
			}
		}
	}

	public sealed class SignedInEventArgs : EventArgs
	{
		public SignedInEventArgs(SignedInGamer gamer)
		{
			Gamer = gamer;
		}

		public SignedInGamer Gamer { get; }
	}

	public sealed class SignedOutEventArgs : EventArgs
	{
		public SignedOutEventArgs(SignedInGamer gamer)
		{
			Gamer = gamer;
		}

		public SignedInGamer Gamer { get; }
	}

	public class GamerCollection<T> : IEnumerable<T> where T : Gamer
	{
		protected readonly List<T> Items = new List<T>();

		public int Count => Items.Count;

		public T this[int index] => Items[index];

		public GamerCollectionEnumerator<T> GetEnumerator() => new GamerCollectionEnumerator<T>(Items.GetEnumerator());

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => Items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
	}

	public struct GamerCollectionEnumerator<T> : IEnumerator<T> where T : Gamer
	{
		private List<T>.Enumerator inner;

		internal GamerCollectionEnumerator(List<T>.Enumerator inner)
		{
			this.inner = inner;
		}

		public T Current => inner.Current;

		object IEnumerator.Current => inner.Current;

		public bool MoveNext() => inner.MoveNext();

		public void Reset() => throw new NotSupportedException();

		public void Dispose() => inner.Dispose();
	}

	public sealed class SignedInGamerCollection : GamerCollection<SignedInGamer>
	{
		public SignedInGamer this[PlayerIndex index] => Items.Find(g => g.PlayerIndex == index);

		/// <summary>Not in XNA: signs in a local player and raises SignedIn.</summary>
		public void SignIn(PlayerIndex index)
		{
			if (this[index] == null)
			{
				SignedInGamer gamer = new SignedInGamer(index);
				Items.Add(gamer);
				Items.Sort((a, b) => a.PlayerIndex.CompareTo(b.PlayerIndex));
				SignedInGamer.RaiseSignedIn(gamer);
			}
		}

		/// <summary>Not in XNA: signs out a local player and raises SignedOut.</summary>
		public void SignOut(PlayerIndex index)
		{
			SignedInGamer existing = this[index];
			if (existing != null)
			{
				Items.Remove(existing);
				SignedInGamer.RaiseSignedOut(existing);
			}
		}
	}

	/// <summary>Add it to Game.Components like the XNA original; it provides the device for gamer pictures.</summary>
	public class GamerServicesComponent : GameComponent
	{
		internal static GraphicsDevice Device { get; private set; }

		public GamerServicesComponent(Game game) : base(game)
		{
		}

		public override void Update(GameTime gameTime)
		{
			Device ??= Game.GraphicsDevice;
			base.Update(gameTime);
		}
	}
}

// Usage: dotnet run --project tools/ContentCheck -- [content dir]   (default: build/Content)
// Opens a small FNA window, loads every .xnb with the reader its header names, prints a
// one-line summary per asset, and exits with the number of failures.
using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

internal sealed class ContentCheck : Game
{
	private readonly string root;

	public int Failures;

	public ContentCheck(string root)
	{
		this.root = root;
		new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 320, PreferredBackBufferHeight = 180 };
	}

	protected override void LoadContent()
	{
		var content = new ContentManager(Services, root);
		foreach (string file in Directory.GetFiles(root, "*.xnb", SearchOption.AllDirectories).OrderBy(f => f))
		{
			string asset = Path.ChangeExtension(Path.GetRelativePath(root, file), null);
			string header = File.ReadAllText(file).Substring(0, Math.Min(400, (int)new FileInfo(file).Length));
			try
			{
				string info;
				if (header.Contains("ModelReader"))
				{
					Model m = content.Load<Model>(asset);
					// Read the vertices back to catch byte-order mistakes (garbage bounds / non-unit normals).
					Vector3 min = new Vector3(float.MaxValue), max = new Vector3(float.MinValue);
					float worstNormal = 0f;
					foreach (ModelMeshPart part in m.Meshes.SelectMany(x => x.MeshParts))
					{
						var verts = new VertexPositionNormalTexture[part.NumVertices];
						part.VertexBuffer.GetData(verts);
						foreach (var v in verts)
						{
							min = Vector3.Min(min, v.Position);
							max = Vector3.Max(max, v.Position);
							worstNormal = Math.Max(worstNormal, Math.Abs(v.Normal.Length() - 1f));
						}
					}
					info = $"Model {m.Meshes.Count} meshes, {m.Bones.Count} bones, {m.Meshes.Sum(x => x.MeshParts.Count)} parts, " +
						$"bounds {min} .. {max}, max |normal|-1 = {worstNormal:0.000}";
				}
				else if (header.Contains("SpriteFontReader"))
				{
					SpriteFont f = content.Load<SpriteFont>(asset);
					info = $"SpriteFont {f.Characters.Count} chars, line spacing {f.LineSpacing}";
				}
				else
				{
					Texture2D t = content.Load<Texture2D>(asset);
					info = $"Texture2D {t.Width}x{t.Height} {t.Format} mips={t.LevelCount}";
				}
				Console.WriteLine($"ok    {asset}: {info}");
			}
			catch (Exception e)
			{
				Failures++;
				Console.WriteLine($"FAIL  {asset}: {e}");
			}
		}
		Console.WriteLine($"{Failures} failures");
		Exit();
	}

	private static int Main(string[] args)
	{
		string root = args.Length > 0 ? args[0] : Path.Combine("build", "Content");
		using var game = new ContentCheck(Path.GetFullPath(root));
		game.Run();
		return game.Failures;
	}
}

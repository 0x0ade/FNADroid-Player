using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Newtonsoft.Json;
using Java.Lang;

namespace FNADroid.Player
{
	public class GameInfoListLoader : AsyncTaskLoader
	{

		public GameInfoListLoader(Context context)
			: base(context)
		{
			OnContentChanged();
		}

		protected override void OnStartLoading()
		{
			if (TakeContentChanged())
				ForceLoad();
		}

		protected override void OnStopLoading()
		{
			CancelLoad();
		}

		public override Java.Lang.Object LoadInBackground()
		{
			Java.Util.ArrayList list = new Java.Util.ArrayList();

			// Find all games and add them to a list.
			int i = -1;
			foreach (Java.IO.File root in Context.GetExternalFilesDirs(null))
			{
				string rootPath = root?.AbsolutePath;
				if (string.IsNullOrEmpty(rootPath))
					continue;

				foreach (string dir in Directory.GetDirectories(rootPath))
				{
					if (string.IsNullOrEmpty(dir))
						continue;

					string dirName = Path.GetFileName(dir);
					if (dirName?.StartsWith(".") ?? true)
						continue;

					string exe = Directory.GetFileSystemEntries(dir, "*.exe", SearchOption.TopDirectoryOnly)?.FirstOrDefault();
					if (string.IsNullOrEmpty(exe))
						continue;

					GameConfig config = null;
					string configPath = Path.Combine(dir, "fnadroid.yaml");
					if (File.Exists(configPath))
					{
						try
						{
							using (Stream stream = File.OpenRead(configPath))
							using (StreamReader reader = new StreamReader(stream))
								config = YamlHelper.Deserializer.Deserialize<GameConfig>(reader);
						}
						catch
						{
						}
					}

					if (config == null)
					{
						// No valid config found - write a default config.
						config = new GameConfig();
						config.Environment["FNADROID"] = "1";
						if (File.Exists(configPath))
							File.Delete(configPath);
						using (Stream stream = File.OpenWrite(configPath))
						using (StreamWriter writer = new StreamWriter(stream))
							YamlHelper.Serializer.Serialize(writer, config);
					}

					list.Add(new GameInfo()
					{
						Id = ++i,
						Name = dirName,
						Dir = dir,
						Exe = exe,
						Config = config
					});
				}
			}

			// If no game info was added, add a helper entry.
			// The list's spinner won't hide otherwise
			if (i == -1)
				list.Add(new GameInfo { Id = -1 });

			return list;
		}
	}
}
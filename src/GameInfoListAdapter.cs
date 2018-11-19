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

namespace FNADroid.Player
{
	public class GameInfoListAdapter : ArrayAdapter
	{

		public GameInfoListAdapter(Context context)
			: base(context, Android.Resource.Layout.SimpleListItem1, new object[0])
		{
		}

		public void Update(Java.Util.ArrayList list)
		{
			Clear();

			if (list != null)
			{
				// AddAll throws an UnsupportedOperationException
				int size = list.Size();
				for (int i = 0; i < size; i++)
					Add(list.Get(i));
			}

			NotifyDataSetChanged();
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			GameInfo info = (GameInfo) GetItem(position);
			LayoutInflater inflater = (LayoutInflater) Context.GetSystemService(Context.LayoutInflaterService);

			View view = convertView ?? inflater.Inflate(Resource.Layout.GameListItem, null);

			ImageView iv = view.FindViewById<ImageView>(Android.Resource.Id.Icon);
			iv.Drawable?.Dispose();
			iv.SetImageDrawable(null);

			TextView tvName = view.FindViewById<TextView>(Android.Resource.Id.Text1);
			TextView tvPath = view.FindViewById<TextView>(Android.Resource.Id.Text2);
			tvName.Text = "";
			tvPath.Text = "";

			if (info == null)
			{
				tvName.Text = $"WARNING: Null entry @ {position}!";
				return view;
			}
			else if (info.Id == -1)
			{
				tvName.Text = "No games found!";
				return view;
			}

			string exeImageName = Path.GetFileNameWithoutExtension(info.Exe).Replace(" ", "");
			// Find a .png with a similar name to the .exe in the game dir.
			// Alternatively, find a .bmp with a similar name to the .exe in the game dir.
			iv.SetImageBitmap(
				GetBitmap(exeImageName, Directory.GetFileSystemEntries(info.Dir, "*.png", SearchOption.TopDirectoryOnly)) ??
				GetBitmap(exeImageName, Directory.GetFileSystemEntries(info.Dir, "*.bmp", SearchOption.TopDirectoryOnly))
			);

			tvName.Text = info.Name;
			tvPath.Text = $"{Path.GetFileName(info.Dir)}/{Path.GetFileName(info.Exe)}";
			return view;
		}

		private static Android.Graphics.Bitmap GetBitmap(string name, string[] entries)
		{
			if ((entries?.Length ?? 0) == 0)
				return null;
			foreach (string path in entries)
				if (Path.GetFileNameWithoutExtension(path).Replace(" ", "") == name)
					return Android.Graphics.BitmapFactory.DecodeFile(path);
			return null;
		}

	}
}
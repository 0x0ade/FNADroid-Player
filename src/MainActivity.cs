using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Java.Lang;
using Android.Support.Design.Widget;
using Android.Database;
using Android.Provider;
using Android.Support.V4.Provider;
using System.Collections;

namespace FNADroid.Player
{
	[Activity(
		Label = "FNADroid Player",
		MainLauncher = true,
		Theme = "@style/AppTheme.Launcher",
		Icon = "@drawable/icon",
		HardwareAccelerated = true
	)]
	public class MainActivity : Android.Support.V7.App.AppCompatActivity, LoaderManager.ILoaderCallbacks
	{

		protected static IDictionary EnvironmentBackup;

		protected Android.Support.V7.Widget.Toolbar MainToolbar;
		protected FloatingActionButton MainFABRefresh;
		protected ListView MainListView;

		protected GameInfoListAdapter MainListAdapter;

		protected Dictionary<IMenuItem, Action> ContextContext = new Dictionary<IMenuItem, Action>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			// Store a clean environment.
			EnvironmentBackup = EnvironmentBackup ?? System.Environment.GetEnvironmentVariables();

			SetTheme(Resource.Style.AppTheme);
			base.OnCreate(savedInstanceState);
			ViewGroup root = FindViewById<ViewGroup>(Android.Resource.Id.Content);
			SetContentView(Resource.Layout.MainLayout);

			MainToolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.MainToolbar);
			SetSupportActionBar(MainToolbar);
			MainToolbar.SetLogo(Resource.Drawable.InAppLogo);
			SupportActionBar.Title = SupportActionBar.Title.Substring(3);

			FindViewById<TextView>(Resource.Id.MainWarning).ApplyHTML();

			MainFABRefresh = FindViewById<FloatingActionButton>(Resource.Id.MainFABRefresh);
			MainFABRefresh.Click += (s, e) =>
			{
				LoaderManager.RestartLoader(0, null, this);
			};

			MainListView = FindViewById<ListView>(Resource.Id.MainListView);

			FrameLayout.LayoutParams layoutParams;

			ProgressBar progress = new ProgressBar(this);
			layoutParams = new FrameLayout.LayoutParams(
				ViewGroup.LayoutParams.WrapContent,
				ViewGroup.LayoutParams.WrapContent,
				GravityFlags.CenterHorizontal | GravityFlags.CenterVertical
			);
			progress.LayoutParameters = layoutParams;
			progress.Indeterminate = true;
			MainListView.EmptyView = progress;
			root.AddView(progress);

			MainListAdapter = new GameInfoListAdapter(this);
			MainListView.Adapter = MainListAdapter;
			LoaderManager.InitLoader(0, null, this);

			MainListView.ItemClick += (s, e) => OpenGamePlay((GameInfo) MainListView.GetItemAtPosition(e.Position));
			RegisterForContextMenu(MainListView);
		}

		private void OpenGamePlay(GameInfo info)
		{
			if (info.Id == -1)
			{
				return;
			}

			// Before even loading the game:
			// Delete FNA.dll and FNA.dll.config if it exists, and replace it with ours.
			Unpack(info, "FNA.dll");
			Unpack(info, "FNA.dll.config");

			// Set up the game environment.
			foreach (DictionaryEntry entry in EnvironmentBackup)
			{
				try
				{
					System.Environment.SetEnvironmentVariable((string) entry.Key, (string) entry.Value, EnvironmentVariableTarget.Process);
				}
				catch
				{
				}
			}
			System.Environment.CurrentDirectory = info.Dir;
			if (info.Config.Environment != null)
				foreach (KeyValuePair<string, string> entry in info.Config.Environment)
				{
					try
					{
						System.Environment.SetEnvironmentVariable(entry.Key, entry.Value, EnvironmentVariableTarget.Process);
					}
					catch
					{
					}
				}

			Intent intent = new Intent(this, typeof(GameActivity));
			intent.PutExtra(GameActivity.ExtraName, info.Name);
			intent.PutExtra(GameActivity.ExtraExe, info.Exe);
			StartActivity(intent);
		}

		private void Unpack(GameInfo info, string name, string targetName = null)
		{
			string target = System.IO.Path.Combine(info.Dir, targetName ?? name);
			if (File.Exists(target))
				File.Delete(target);
			// TODO: Perform async, show progress.
			using (Stream streamAsset = Assets.Open(name))
			using (FileStream streamOut = File.OpenWrite(target))
				streamAsset.CopyTo(streamOut);
		}

		private void OpenSettings()
		{
			Intent intent = new Intent(this, typeof(SettingsActivity));
			StartActivity(intent);
		}

		public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			ContextContext.Clear();

			switch (v.Id)
			{
				case Resource.Id.MainListView:
					AdapterView.AdapterContextMenuInfo acmi = (AdapterView.AdapterContextMenuInfo) menuInfo;
					GameInfo info = (GameInfo) MainListView.GetItemAtPosition(acmi.Position);
					if (info.Id != -1)
					{
						menu.SetHeaderTitle(info.Name);
						ContextContext[menu.Add(Resource.String.Play)] = () => OpenGamePlay(info);
					}
					break;
			}

			base.OnCreateContextMenu(menu, v, menuInfo);
		}

		public override bool OnContextItemSelected(IMenuItem item)
		{
			Action action;
			if (ContextContext.TryGetValue(item, out action))
				action?.Invoke();
			return base.OnContextItemSelected(item);
		}

		public Loader OnCreateLoader(int id, Bundle args)
		{
			switch (id)
			{
				case 0:
					return new GameInfoListLoader(this);

				default:
					return null;
			}

		}

		public void OnLoadFinished(Loader loader, Java.Lang.Object data)
		{
			MainListAdapter.Update((Java.Util.ArrayList) data);
		}

		public void OnLoaderReset(Loader loader)
		{
			MainListAdapter.Update(null);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.MainActions, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.MainActionSettings:
					OpenSettings();
					return true;

				default:
					return base.OnOptionsItemSelected(item);
			}
		}

	}
}

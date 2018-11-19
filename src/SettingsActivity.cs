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
using Android.Preferences;

namespace FNADroid.Player
{
	[Activity(
		Label = "Settings",
		Theme = "@style/AppTheme",
		Icon = "@drawable/icon"
	)]
	public class SettingsActivity : Android.Support.V7.App.AppCompatActivity
	{

		protected ListView SettingsListView;

		protected ViewGeneratorArrayAdapter SettingsListAdapter;

		protected Dictionary<int, Action> ListActions = new Dictionary<int, Action>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ViewGroup root = FindViewById<ViewGroup>(Android.Resource.Id.Content);
			SetContentView(Resource.Layout.SettingsLayout);

			SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.SettingsToolbar));

			SettingsListView = (ListView) FindViewById(Resource.Id.SettingsListView);

			SettingsListAdapter = new ViewGeneratorArrayAdapter(
				this,

				(pos, convertView, parent) =>
				{
					View view = convertView ?? LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
					view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = "TODO: Add proper settings.";
					return view;
				},

				(pos, convertView, parent) =>
				{
					View view = convertView ?? LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
					view.FindViewById<TextView>(Android.Resource.Id.Text1).SetText(Resource.String.License);
					ListActions[pos] = () =>
					{
						AlertDialog dialog = null;
						using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this))
						{
							TextView msg = new TextView(this);
							msg.SetText(Resource.String.Licenses);
							msg.ApplyHTML();

							msg.SetPadding(20, 20, 20, 20);

							ScrollView msgScroll = new ScrollView(this);
							msgScroll.AddView(msg);

							dialogBuilder.SetTitle(Resource.String.License);
							dialogBuilder.SetView(msgScroll);
							dialogBuilder.SetCancelable(true);
							dialogBuilder.SetPositiveButton(Android.Resource.String.Ok, (s, e) =>
							{
								dialog.Dismiss();
							});
							dialog = dialogBuilder.Show();
						}
					};
					return view;
				},

				(pos, convertView, parent) =>
				{
					View view = convertView ?? LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
					TextView tv = view.FindViewById<TextView>(Android.Resource.Id.Text1);
					tv.SetText(Resource.String.ListTail);
					tv.ApplyHTML();
					return view;
				}
			);
			SettingsListView.Adapter = SettingsListAdapter;

			SettingsListView.ItemClick += (s, e) =>
			{
				Action action;
				if (ListActions.TryGetValue(e.Position, out action))
					action?.Invoke();
			};
		}

	}
}

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
	public class GameInfo : Java.Lang.Object
	{
		public int Id;
		public string Name;
		public string Dir;
		public string Exe;

		public GameConfig Config;
	}
}
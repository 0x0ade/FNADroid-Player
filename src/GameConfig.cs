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
	public class GameConfig
	{
		public Dictionary<string, string> Environment { get; set; } = new Dictionary<string, string>();
		public bool ForceFullscreen { get; set; } = false;
	}
}
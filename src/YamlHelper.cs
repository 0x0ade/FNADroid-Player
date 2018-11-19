using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using YamlDotNet.Serialization;

namespace FNADroid.Player
{
	public static class YamlHelper
	{
		public static Deserializer Deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
		public static Serializer Serializer = new SerializerBuilder().EmitDefaults().Build();
	}
}
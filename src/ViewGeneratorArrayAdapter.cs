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
using System.Collections;
using System.Reflection;

namespace FNADroid.Player
{
	public delegate View ViewGenerator(int position, View convertView, ViewGroup parent);
	public class ViewGeneratorArrayAdapter : ArrayAdapter
	{

		private readonly static MethodInfo m_GetInstance = typeof(IJavaObject).Assembly.GetType("Android.Runtime.JavaObject").GetProperty("Instance").GetGetMethod();
		private readonly static object[] EmptyObjArray = new object[0];

		public ViewGeneratorArrayAdapter(Context context, params ViewGenerator[] objects)
			: base(context, Android.Resource.Layout.SimpleListItem1, objects)
		{
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			return ((ViewGenerator) m_GetInstance.Invoke(GetItem(position), EmptyObjArray))(position, convertView, parent);
		}

	}
}
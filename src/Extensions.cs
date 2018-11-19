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

namespace FNADroid.Player
{
	public static class Extensions
	{

		public static void ApplyHTML(this TextView tv)
		{
			string text = tv.Text;
			if (!text.StartsWith(">"))
				return;
#pragma warning disable CS0618 // Type or member is obsolete
			tv.SetText(Android.Text.Html.FromHtml(
				tv.Text
				.Substring(1)
				.Trim()
			, null, null), TextView.BufferType.Spannable);
#pragma warning restore CS0618 // Type or member is obsolete
		}

	}
}
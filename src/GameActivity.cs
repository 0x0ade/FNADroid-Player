using Android.App;
using Android.Widget;
using Android.OS;
using Org.Libsdl.App;
using Android.Views;
using Android.Content.Res;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;

namespace FNADroid.Player
{
	[Activity(
		Label = "FNADroid Launcher",
		Icon = "@drawable/icon",
		HardwareAccelerated = true,
		ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape
	)]
	public class GameActivity : SDLActivity
	{

		public const string ExtraName = "FNADroid.Player.Game.NAME";
		public const string ExtraExe = "FNADroid.Player.Game.EXE";

		public string GameName;
		public string GameExe;

		public static GameActivity Instance;

		public override void LoadLibraries()
		{
			base.LoadLibraries();
			Java.Lang.JavaSystem.LoadLibrary("fnadroid-ext");
			// Give the main library something to call in Mono-Land.
			SetMain(SDL_Main);
		}

		protected override void OnStart()
		{
			base.OnStart();
			Instance = this;
			ActionBar?.Hide();

			// Load stub Steamworks.NET
			Steamworks.SteamAPI.Init();
			// Load our copy of FNA before the game gets a chance to load its copy.
			RuntimeHelpers.RunClassConstructor(typeof(Game).TypeHandle);

			Android.Content.Intent intent = Intent;
			GameName = intent?.GetStringExtra(ExtraName);
			GameExe = intent?.GetStringExtra(ExtraExe);
		}

		public override void OnWindowFocusChanged(bool hasFocus)
		{
			base.OnWindowFocusChanged(hasFocus);
			if (hasFocus)
			{
				Window.DecorView.SystemUiVisibility = (StatusBarVisibility) (
					SystemUiFlags.LayoutStable |
					SystemUiFlags.LayoutHideNavigation |
					SystemUiFlags.LayoutFullscreen |
					SystemUiFlags.HideNavigation |
					SystemUiFlags.Fullscreen |
					SystemUiFlags.ImmersiveSticky
				);
			}
		}

		public static void SDL_Main()
		{
			if (string.IsNullOrEmpty(Instance.GameExe))
			{
				AlertDialog dialog = null;
				Instance.RunOnUiThread(() =>
				{
					using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(Instance))
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("Game not found: ").AppendLine(Instance.GameName);
						stringBuilder.AppendLine(Instance.GameExe);

						dialogBuilder.SetMessage(stringBuilder.ToString().Trim());
						dialogBuilder.SetCancelable(true);
						dialogBuilder.SetPositiveButton(Android.Resource.String.Ok, (s, e) =>
						{
							dialog.Dismiss();
						});
						dialog = dialogBuilder.Show();
					}
				});

				while (dialog == null || dialog.IsShowing)
				{
					System.Threading.Thread.Sleep(0);
				}
				dialog.Dispose();
				return;
			}

			// Replace the following with whatever was in your Program.Main method.

			/*/
			using (TestGame game = new TestGame())
			{
				game.Run();
			}
			/*/

			// Assembly.LoadFrom(Instance.GameExe).EntryPoint.Invoke(null, new object[] { new string[] { /*args*/ } });
			System.AppDomainSetup domainSetup = new System.AppDomainSetup()
			{
				ApplicationName = Path.GetFileName(Instance.GameExe),
				ApplicationBase = Path.GetDirectoryName(Instance.GameExe),
			};
			System.AppDomain domain = System.AppDomain.CreateDomain(
				Path.GetDirectoryName(Instance.GameExe),
				System.AppDomain.CurrentDomain.Evidence,
				domainSetup
			);
			domain.AssemblyResolve += ChildDomainAssemblyResolve;
			domain.ExecuteAssembly(Instance.GameExe);
			System.AppDomain.Unload(domain);

			/**/
		}

		private static Assembly ChildDomainAssemblyResolve(object sender, System.ResolveEventArgs args)
		{
			try
			{
				return Assembly.Load(args.Name);
			}
			catch
			{
			}
			return null;
		}

		[DllImport("main")]
		public static extern void SetMain(System.Action main);

	}
}

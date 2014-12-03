using Android.App;
using Android.OS;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using CocosSharp;
using TwentyFortyEight.Shared;

namespace TwentyFortyEight.Droid
{
	[Activity(
		Label = "LeanKit2048",
		AlwaysRetainTaskState = true,
		Icon = "@drawable/icon",
		Theme = "@android:style/Theme.NoTitleBar",
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.Portrait,
		MainLauncher = true,
		ConfigurationChanges =  ConfigChanges.Keyboard | 
		ConfigChanges.KeyboardHidden)]
	public class MainActivity : AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			var application = new CCApplication();

			application.ApplicationDelegate = new TwentyFortyEightApplicationDelegate();
			SetContentView(application.AndroidContentView);
			application.StartGame();
		}
	}
}



using CocosSharp;
using MonoTouch.UIKit;
using TwentyFortyEight.Shared;
using MonoTouch.Foundation;

namespace TwentyFortyEight.iPhone
{
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override void FinishedLaunching (UIApplication app)
		{
			var application = new CCApplication();
			application.ApplicationDelegate = new TwentyFortyEightApplicationDelegate();
			application.StartGame();
		}
	}
}
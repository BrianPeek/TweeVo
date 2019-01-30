using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using Application=System.Windows.Application;
using MessageBox=System.Windows.MessageBox;

namespace TweeVo
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private TweeVoWindow _window;
		private NotifyIcon _notifyIcon;

		private void App_Startup(object sender, StartupEventArgs e)
		{
			// create a tray icon and setup an event handler for double-clicking it
			_notifyIcon = new NotifyIcon { Icon = TweeVo.Properties.Resources.Icon, Visible = true };
			_notifyIcon.DoubleClick += notifyIcon_DoubleClick;

			// setup 2 menu items
			MenuItem[] items = new[]
			{
				new MenuItem("&Settings", Settings_Click) { DefaultItem = true } ,
				new MenuItem("-"),
				new MenuItem("&Exit", Exit_Click)
			};
			_notifyIcon.ContextMenu = new ContextMenu(items);

			// create the window and show it if we're not configured
			_window = new TweeVoWindow();
			//_window.Show();

			if (TweeVoSettings.Default.TiVos == null || TweeVoSettings.Default.TiVos.Count == 0 || !_window.CheckAuthorization())
				_window.Show();
			else
				TiVoPoller.Start();
		}

		private void Settings_Click(object sender, EventArgs e)
		{
			_window.Show();
		}

		private void Exit_Click(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		void App_Exit(object sender, ExitEventArgs e)
		{
			// remove the icon from the tray on exit
			_notifyIcon.Dispose();
		}

		void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			// display any unhandled exception
			MessageBox.Show(e.Exception.ToString());
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			// show the window in the normal state
			_window.Show();
			_window.WindowState = WindowState.Normal;
		}

		public static void OnRequestNavigation(object sender, RoutedEventArgs e)
		{
			// all uperlinks will be passed through this handler and will start them in the default web browser
			Hyperlink hl = e.OriginalSource as Hyperlink;
			if(hl != null)
				Process.Start(hl.NavigateUri.ToString());
		}

		public void ShowBalloonTip(string msg)
		{
			_notifyIcon.ShowBalloonTip(10000, "TweeVo", msg, System.Windows.Forms.ToolTipIcon.Info);
		}
	}
}

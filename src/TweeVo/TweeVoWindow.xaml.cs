using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Win32;
using Tweetinvi;
using Tweetinvi.Models;

namespace TweeVo
{
	/// <summary>
	/// Interaction logic for TweeVoWindow.xaml
	/// </summary>
	public partial class TweeVoWindow : Window
	{
		private string TwitterAuthToken { get; set; }
		private string TwitterAuthTokenSecret { get; set; }
		public string AuthorizedScreenName { get; set; }
		public bool IsAuthorized { get; set; }

		public TweeVoWindow()
		{
			InitializeComponent();
			AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(App.OnRequestNavigation));
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TweeVoSettings.Default.Upgrade();

			TiVoBeaconListener.TiVoFound += TiVoBeaconListener_TiVoFound;
			TiVoBeaconListener.Start();

			IsAuthorized = false;

			List<PrefixItem> prefixItems = new List<PrefixItem>();
			prefixItems.Add(new PrefixItem(PrefixType.Nothing, "Nothing"));
			prefixItems.Add(new PrefixItem(PrefixType.TiVo, "TiVo>"));
			prefixItems.Add(new PrefixItem(PrefixType.Name, "(TiVo Name)>"));
			cboPrefix.ItemsSource = prefixItems;
			cboPrefix.DisplayMemberPath = "PrefixDescription";
			cboPrefix.SelectedValuePath = "PrefixType";

			List<SuggestionItem> suggestionItems = new List<SuggestionItem>();
			suggestionItems.Add(new SuggestionItem(SuggestionsType.NoShow, "Don't show"));
			suggestionItems.Add(new SuggestionItem(SuggestionsType.ShowNormal, "Tweet like a regular show"));
			suggestionItems.Add(new SuggestionItem(SuggestionsType.ShowWithPrefix, "Tweet with (S) prefix"));
			cboSuggestions.ItemsSource = suggestionItems;
			cboSuggestions.DisplayMemberPath = "SuggestionDescription";
			cboSuggestions.SelectedValuePath = "SuggestionType";

			if(TweeVoSettings.Default.TiVos != null)
			{
				lbTiVo.Items.Clear();

				foreach(TiVo tivo in TweeVoSettings.Default.TiVos.Values)
				{
					TiVo t = tivo;
					Dispatcher.BeginInvoke(new Action(() => lbTiVo.Items.Add(t)));
				}

				grid.DataContext = TweeVoSettings.Default;

				CheckAuthorization();
			}

			DisplayAuthorizationStatus();

			txtVersion.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version;
		}

		void DisplayAuthorizationStatus()
		{
			if (IsAuthorized)	
				lblAuthorized.Content = "Authorized to post to Twitter.";
			else
				lblAuthorized.Content = "This application has not been authorized.";
		}

		public bool CheckAuthorization()
		{
			IsAuthorized = false;

			TwitterAuthToken = TweeVoSettings.Default.TwitterAuthToken;
			TwitterAuthTokenSecret = TweeVoSettings.Default.TwitterAuthTokenSecret;

			if (!string.IsNullOrEmpty(TwitterAuthToken))
			{
				Auth.SetUserCredentials(TwitterConst.CONSUMER_KEY, TwitterConst.CONSUMER_SECRET, TwitterAuthToken, TwitterAuthTokenSecret);
				IsAuthorized = true;
				DisplayAuthorizationStatus();
			}
			return IsAuthorized;
		}

		void TiVoBeaconListener_TiVoFound(object sender, TiVoFoundEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() => lbTiVo.Items.Add(e.TiVo)));
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if(!VerifySettings())
				return;

			StartWithWindows(TweeVoSettings.Default.StartWithWindows);

			TweeVoSettings.Default.TwitterAuthToken = TwitterAuthToken;
			TweeVoSettings.Default.TwitterAuthTokenSecret = TwitterAuthTokenSecret;
			TweeVoSettings.Default.Save();

			MessageBox.Show("Settings saved.", "TweeVo", MessageBoxButton.OK, MessageBoxImage.Information);

			this.Hide();

			TiVoPoller.Start();
		}

		private bool SetError(IInputElement control, string msg)
		{
			MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			control.Focus();
			return false;
		}

		private bool VerifySettings()
		{
			if (!IsAuthorized)
				return SetError(btnAuthorize, "Please authorize this application with Twitter.");

			if(TweeVoSettings.Default.TiVos == null || TweeVoSettings.Default.TiVos.Count == 0)
				return SetError(lbTiVo, "Please wait for the TiVo list to populate.");
		
			if(TweeVoSettings.Default.TiVos.Count == 0)
				return SetError(lbTiVo, "Please select at least one TiVo.");

			if(string.IsNullOrEmpty(txtMAK.Text) || txtMAK.Text.Length != 10)
				return SetError(lbTiVo, "Please enter your 10 digit Media Access Key.");

			if(!(from t in TweeVoSettings.Default.TiVos.Values where t.Active select t).First().VerifyMAK(txtMAK.Text))
				return SetError(lbTiVo, "Your Media Access Key is invalid.");

			return true;
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if(WindowState == WindowState.Minimized)
				this.Hide();
		}

		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void StartWithWindows(bool start)
		{
			using (RegistryKey hkcu = Registry.CurrentUser)
			{
				using (RegistryKey runKey = hkcu.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
				{
					if(runKey == null)
						return;

					if(start)
						runKey.SetValue("TweeVo", Assembly.GetEntryAssembly().Location);
					else
					{
						if(runKey.GetValue("TweeVo") != null)
							runKey.DeleteValue("TweeVo");
					}
				}
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(MessageBox.Show("Are you sure you want to exit TweeVo?", "TweeVo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				e.Cancel = false;
			else
				e.Cancel = true;
		}

		private void btnAuthorize_Click(object sender, RoutedEventArgs e)
		{
			AuthorizeUserDlg dlg = new AuthorizeUserDlg { Owner = this };

			if (dlg.ShowDialog().Value == true)
			{
				TwitterAuthToken = dlg.Token;
				TwitterAuthTokenSecret = dlg.TokenSecret;

				IsAuthorized = true;
				DisplayAuthorizationStatus();
			}
		}
	}

	[Serializable]
	public class PrefixItem
	{
		public PrefixType PrefixType { get; set; }
		public string PrefixDescription { get; set; }

		public PrefixItem(PrefixType type, string description)
		{
			PrefixType = type;
			PrefixDescription = description;
		}
	}

	[Serializable]
	public class SuggestionItem
	{
		public SuggestionsType SuggestionType { get; set; }
		public string SuggestionDescription { get; set; }

		public SuggestionItem(SuggestionsType type, string description)
		{
			SuggestionType = type;
			SuggestionDescription = description;
		}
	}
}

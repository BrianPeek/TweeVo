using System.Diagnostics;
using System.Windows;
using Tweetinvi;
using Tweetinvi.Models;

namespace TweeVo
{
	/// <summary>
	/// Interaction logic for AuthorizeUserDlg.xaml
	/// </summary>
	public partial class AuthorizeUserDlg : Window
	{
		public string Token { get; set; }
		public string TokenSecret { get; set; }

		IAuthenticationContext authenticationContext;

		public AuthorizeUserDlg()
		{
			InitializeComponent();

			TwitterCredentials appCredentials = new TwitterCredentials(TwitterConst.CONSUMER_KEY, TwitterConst.CONSUMER_SECRET);
			// Init the authentication process and store the related `AuthenticationContext`.
			authenticationContext = AuthFlow.InitAuthentication(appCredentials);

			// Go to the URL so that Twitter authenticates the user and gives him a PIN code.
			Process.Start(authenticationContext.AuthorizationURL);
		}

		private void btnAuthorize_Click(object sender, RoutedEventArgs e)
		{
			// Ask the user to enter the pin code given by Twitter
			string pinCode = txtPin.Text;

			// With this pin code it is now possible to get the credentials back from Twitter
			ITwitterCredentials userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

			// Use the user credentials in your application
			Auth.SetCredentials(userCredentials);

			Token = userCredentials.AccessToken;
			TokenSecret = userCredentials.AccessTokenSecret;
			this.DialogResult = true;
		}
	}
}

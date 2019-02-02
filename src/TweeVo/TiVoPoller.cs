using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows;
using Tweetinvi;
using Tweetinvi.Models;

namespace TweeVo
{
	public static class TiVoPoller
	{
		private static Timer _timer;
		private static bool _polling;

		public static void Start()
		{
			Logger.Log("Starting TiVoPoller", LoggerSeverity.Info);

			if(_timer == null)
				_timer = new Timer(PollTiVo);

			// poll every 15 minutes
			_timer.Change(0, 15*60*1000);
		}

		public static void Stop()
		{
			if(_timer != null)
				_timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private static void PollTiVo(object state)
		{
			string TwitterAuthToken = TweeVoSettings.Default.TwitterAuthToken;
			string TwitterAuthTokenSecret = TweeVoSettings.Default.TwitterAuthTokenSecret;
			//Auth.SetUserCredentials(TwitterConst.CONSUMER_KEY, TwitterConst.CONSUMER_SECRET, TwitterAuthToken, TwitterAuthTokenSecret);
			int tries = 0;
			bool success = false;
	
			Logger.Log("PollTiVo Now: " + DateTime.Now, LoggerSeverity.Info);
			if(_polling)
				return;

			_polling = true;

			foreach(TiVo t in TweeVoSettings.Default.TiVos.Values)
			{
				if(t.Active)
				{
					// try to poll 3 times
					while(tries < 3 && !success)
					{
						try
						{
							List<NPLEntry> list = t.GetNowPlayingList();
							if(list != null)
							{
								foreach(NPLEntry nplEntry in list)
								{
									// if the entry is valid, tweet it
									if((!nplEntry.Suggestion || (nplEntry.Suggestion && TweeVoSettings.Default.Suggestions != SuggestionsType.NoShow)) &&
										nplEntry.CaptureDate > t.LastPolled)
									{
										string tweet = CreateTwitterString(t.Machine, nplEntry);
										try
										{
											ITweet firstTweet = Tweet.PublishTweet(tweet);
										}
										catch(Exception ex)
										{
											Logger.Log("Error posting to Twitter: " + ex, LoggerSeverity.Error);
											(Application.Current as App).ShowBalloonTip("Error posting to Twitter: " + ex.Message + "  Ensure Twitter isn't down and your credentials haven't changed.");
										}
									}
								}
							}
							success = true;
							t.LastPolled = DateTime.Now;
							Logger.Log("Completed processing " + t.Machine, LoggerSeverity.Info);
						}
						catch(Exception ex)
						{
							Logger.Log("Exception on " + t.Machine + " with ex: " + ex, LoggerSeverity.Error);
							success = false;
							tries++;

							// if we've failed 3 times, notify somebody
							if(tries == 3)
							{
								Logger.Log("3 retries on " + t.Machine + " with ex: " + ex, LoggerSeverity.Error);
								(Application.Current as App).ShowBalloonTip("Error with " + t.Machine + ": " + ex.Message + "  If this keeps happening, you may want to disable TweeVo from communicating with this TiVo.");
							}
						}
					}
					tries = 0;
				}
				success = false;
			}
			_polling = false;
			TweeVoSettings.Default.Save();
		}

		private static string GetShortUrl(string url)
		{
			// get a shortened URL using tinyurl.com
			return new WebClient().DownloadString("http://tinyurl.com/api-create.php?url=" + url);
		}

		private static string CreateTwitterString(string tivoName, NPLEntry nplEntry)
		{
			// this url will take you to the specific episode page...the "x" can be anything.

			if(nplEntry.ProgramID.Contains("-"))
				nplEntry.ProgramID = nplEntry.ProgramID.Split('-')[0];

			string progId = nplEntry.ProgramID.Substring(0, 2) + new string('0', 14 - nplEntry.ProgramID.Length) + nplEntry.ProgramID.Substring(2);
			string url = "http://tvlistings.zap2it.com/tv/x/" + progId;
			string tinyUrl = GetShortUrl(url);

			string tweet;

			string prefix = string.Empty;

			switch(TweeVoSettings.Default.TwitterPrefix)
			{
				case PrefixType.Name:
					prefix += tivoName + "> ";
					break;
				case PrefixType.TiVo:
					prefix += "TiVo> ";
					break;
				case PrefixType.Nothing:
					// nuttin'
					break;
			}

			if(nplEntry.Suggestion && TweeVoSettings.Default.Suggestions == SuggestionsType.ShowWithPrefix)
				prefix += "(S) ";

			if(string.IsNullOrEmpty(nplEntry.EpisodeTitle))
			{
				tweet = string.Format("{0}{1} at {2} on {3} {4} ",
									  prefix,
									  nplEntry.Title,
									  nplEntry.CaptureDate.ToShortTimeString(),
									  nplEntry.SourceChannel,
									  nplEntry.SourceStation);

			}
			else
			{
				tweet = string.Format("{0}{1}: \"{2}\" at {3} on {4} {5} ",
									  prefix,
									  nplEntry.Title,
									  nplEntry.EpisodeTitle, 
									  nplEntry.CaptureDate.ToShortTimeString(), 
									  nplEntry.SourceChannel, 
									  nplEntry.SourceStation);
			}

			int extra = (tweet.Length + tinyUrl.Length) - 280;
			if(extra > 0)
				tweet = tweet.Substring(0, tweet.Length-extra-4) + "... ";

			return tweet + tinyUrl;
		}
	}
}
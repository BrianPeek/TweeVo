using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace TweeVo
{
	[Serializable]
	public class TiVo
	{
		// IP of TiVo
		public IPAddress IpAddress { get; set; }

		// Name of the TiVo machine assigned on the TiVo website
		public string Machine { get; set; }

		// unique identifier for the TiVo
		public string Identity { get; set; }

		// platform of the TiVo (i.e. whether it's a TiVo or TiVo Desktop or something else)
		public string Platform { get; set; }

		// was it selected by the user to be polled
		public bool Active { get; set; }

		// time this TiVo was last polled
		public DateTime LastPolled { get; set; }

		public List<NPLEntry> GetNowPlayingList()
		{
			// pull the entire NPL
			XDocument docNowPlaying = GetNowPlayingListDocument("NowPlaying", true);

			// pull only the suggestion folder (NowPlaying/0 is the magic URL for the Suggestions folder)
			XDocument docSuggestions = GetNowPlayingListDocument("NowPlaying%2f0", false);

			if(docNowPlaying.Root == null)
				return null;

			XNamespace ns = docNowPlaying.Root.Name.Namespace;

			if(ns == null)
				return null;

			// get a list of ProgramId's for just the suggestions
			var querySuggestions = from entry in docSuggestions.Descendants(ns + "Item")
						let details = entry.Element(ns + "Details")
			            where details.Element(ns + "ProgramId") != null &&
			                  !details.Element(ns + "ProgramId").Value.StartsWith("TS") &&
			                  details.Element(ns + "CaptureDate") != null 
			            select (string)details.Element(ns + "ProgramId");
			List<string> suggestionIds = querySuggestions.ToList();

			// now pull the entire list of NPL entries that are real shows
			var query = from entry in docNowPlaying.Descendants(ns + "Item")
						let details = entry.Element(ns + "Details")
			            where details.Element(ns + "ProgramId") != null &&
			                  !details.Element(ns + "ProgramId").Value.StartsWith("TS") &&	// no downloaded TiVo videos
			                  details.Element(ns + "CaptureDate") != null 
						orderby details.Element(ns + "CaptureDate").Value ascending
			            select new NPLEntry
		                   {
			                   	Title = (string)details.Element(ns + "Title"),
			                   	EpisodeTitle = (string)details.Element(ns + "EpisodeTitle"),
			                   	SourceChannel = (string)details.Element(ns + "SourceChannel"),
			                   	SourceStation = (string)details.Element(ns + "SourceStation"),
			                   	CaptureDate = (details.Element(ns + "CaptureDate").Value).EpochToDateTime().RoundToNearestMinute(),
			                   	ProgramID = (string)details.Element(ns + "ProgramId"),
								SeriesID = (string)details.Element(ns + "SeriesId"),
								ProgramServerID = (string)details.Element(ns + "ProgramServerId"),
								SeriesServerID = (string)details.Element(ns + "SeriesServerId"),
								// if the program ID lives in the suggestion list, than mark it as a suggestion
								Suggestion = suggestionIds.Exists(s => s == (string)details.Element(ns + "ProgramId"))
		                   };

			List<NPLEntry> entries = query.ToList();
			return entries;
		}

		private XDocument GetNowPlayingListDocument(string container, bool recurse)
		{
			HttpWebRequest request;
			WebResponse response = null;
			XDocument doc;

			// pull the NPL
			string uri = $"https://{IpAddress}/TiVoConnect?Command=QueryContainer&Container=%2F{container}&Recurse={(recurse ? "Yes" : "No")}";
			Logger.Log($"Pulling {uri} from {this.ToString()}, Last polled: {LastPolled}", LoggerSeverity.Info);

			try
			{
				request = (HttpWebRequest)WebRequest.Create(uri);
				request.Credentials = new NetworkCredential("tivo", TweeVoSettings.Default.MediaAccessKey);
				// accept any ssl certificate
				ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

				response = request.GetResponse();
				Logger.Log("List retrieved", LoggerSeverity.Info);

				XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
				doc = XDocument.Load(xmlReader);
			}
			finally
			{
				if(response != null)
					response.Close();
			}

			return doc;
		}

		public bool VerifyMAK(string mak)
		{
			HttpWebRequest request;
			WebResponse response = null;

			// run the QueryServer method...returns a short document
			string uri = $"https://{IpAddress}/TiVoConnect?Command=QueryServer";
			Logger.Log($"Verifying MAK {mak} on {this.ToString()}", LoggerSeverity.Info);

			try
			{
				request = (HttpWebRequest)WebRequest.Create(uri);
				request.Credentials = new NetworkCredential("tivo", TweeVoSettings.Default.MediaAccessKey);
				ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

				// if we get this far, we had a successful request
				response = request.GetResponse();
				Logger.Log("MAK verified", LoggerSeverity.Info);
			}
			catch(WebException ex)
			{
				if(((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
				{
					Logger.Log("MAK invalid", LoggerSeverity.Info);
					return false;
				}
			}
			finally
			{
				if(response != null)
					response.Close();
			}

			return true;
		}

		public override string ToString()
		{
			return Machine + " (" + IpAddress + ")";
		}
	}
}
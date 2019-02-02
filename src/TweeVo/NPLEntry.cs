using System;

namespace TweeVo
{
	public class NPLEntry
	{
		// title of overall show (ex: Arrested Development)
		public string Title { get; set; }

		// title of specific episode (ex: Sword of Destiny)
		public string EpisodeTitle { get; set; }

		// the channel number on which it was broadcast
		public string SourceChannel { get; set; }

		// the "call letters" for said channel
		public string SourceStation { get; set; }

		// the date it was recorded
		public DateTime CaptureDate { get; set; }

		// the unique ID for this episode of this show (ex: EP005984700040)
		public string ProgramID { get; set; }
		public string SeriesID { get; set; }
		public string ProgramServerID { get; set; }
		public string SeriesServerID { get; set; }

		// whether or not it was recorded as a Suggestion
		public bool Suggestion { get; set; }
	}
}
using System;
using System.Diagnostics;
using System.IO;

namespace TweeVo
{
	public enum LoggerSeverity
	{
		Info,
		Error
	}

	public class Logger
	{
		public static void Log(string msg, LoggerSeverity severity)
		{
			Debug.WriteLine(msg);

			StreamWriter sw = File.AppendText(Path.Combine(Path.GetTempPath(), "tweevo.txt"));

			try
			{
				sw.WriteLine(String.Format("{0} - {1:G}: {2}", severity, System.DateTime.Now, msg));
			}
			finally
			{
				sw.Close();
			}
		}
	}
}
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
				sw.WriteLine($"{severity} - {DateTime.Now:G}: {msg}");
			}
			finally
			{
				sw.Close();
			}
		}
	}
}
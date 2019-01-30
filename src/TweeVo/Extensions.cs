using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace TweeVo
{
	public static class Extensions
	{
		// convert unix time to DateTime
		public static DateTime EpochToDateTime(this string date)
		{
			return new DateTime(1970, 1, 1).AddSeconds(long.Parse(date.Remove(0, 2), NumberStyles.HexNumber)).ToLocalTime();
		}

		// sometimes the start times aren't on the minute...this fixes that
		public static DateTime RoundToNearestMinute(this DateTime date)
		{
			return date.Second > 30 ? new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0).AddMinutes(1) : date;
		}

		// encryption methods for sensitive data
		public static string EncryptString(this string s)
		{
		    return string.IsNullOrEmpty(s) ? string.Empty : Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(s), null, DataProtectionScope.CurrentUser));
		}

		public static string DecryptString(this string s)
		{
		    return string.IsNullOrEmpty(s) ? string.Empty : Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(s), null, DataProtectionScope.CurrentUser));
		}
	}
}
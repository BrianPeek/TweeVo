using System.Collections.Generic;
using System.Configuration;

namespace TweeVo
{
	// custom settings for this application
	public class TweeVoSettings : ApplicationSettingsBase
	{
		private static TweeVoSettings defaultInstance = ((TweeVoSettings)(Synchronized(new TweeVoSettings())));
		
		public static TweeVoSettings Default {
			get {
				return defaultInstance;
			}
		}
		
		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		[SettingsSerializeAs(SettingsSerializeAs.Binary)]
		public Dictionary<string,TiVo> TiVos {
			get {
				return ((Dictionary<string,TiVo>)(this["TiVos"]));
			}
			set {
				this["TiVos"] = value;
			}
		}
   
		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		public PrefixType TwitterPrefix {
			get {
				return ((PrefixType)(this["TwitterPrefix"]));
			}
			set {
				this["TwitterPrefix"] = value;
			}
		}
		
		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		public SuggestionsType Suggestions {
			get {
				return ((SuggestionsType)(this["Suggestions"]));
			}
			set {
				this["Suggestions"] = value;
			}
		}
		
		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		public string MediaAccessKey {
			get {
				return ((string)(this["MediaAccessKey"]));
			}
			set {
				this["MediaAccessKey"] = value;
			}
		}

		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		public bool StartWithWindows {
			get {
				return ((bool)(this["StartWithWindows"]));
			}
			set {
				this["StartWithWindows"] = value;
			}
		}

		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		public string TwitterAuthToken
		{
			get
			{
				return ((string)(this["TwitterAuthToken"]));
			}
			set
			{
				this["TwitterAuthToken"] = value;
			}
		}

		[UserScopedSetting()]
		[System.Diagnostics.DebuggerNonUserCode()]
		[DefaultSettingValue("")]
		public string TwitterAuthTokenSecret
		{
			get
			{
				return ((string)(this["TwitterAuthTokenSecret"]));
			}
			set
			{
				this["TwitterAuthTokenSecret"] = value;
			}
		}
	}
}

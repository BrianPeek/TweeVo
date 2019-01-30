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
        
        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public Dictionary<string,TiVo> TiVos {
            get {
                return ((Dictionary<string,TiVo>)(this["TiVos"]));
            }
            set {
                this["TiVos"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string TwitterUsername {
            get {
                return ((string)(this["TwitterUsername"]));
            }
            set {
                this["TwitterUsername"] = value;
            }
        }
        
        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string TwitterPassword {
            get {
                return ((string)(this["TwitterPassword"]));
            }
            set {
                this["TwitterPassword"] = value;
            }
        }        
       
        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public PrefixType TwitterPrefix {
            get {
                return ((PrefixType)(this["TwitterPrefix"]));
            }
            set {
                this["TwitterPrefix"] = value;
            }
        }
        
        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public SuggestionsType Suggestions {
            get {
                return ((SuggestionsType)(this["Suggestions"]));
            }
            set {
                this["Suggestions"] = value;
            }
        }
        
        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string MediaAccessKey {
            get {
                return ((string)(this["MediaAccessKey"]));
            }
            set {
                this["MediaAccessKey"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public bool StartWithWindows {
            get {
                return ((bool)(this["StartWithWindows"]));
            }
            set {
                this["StartWithWindows"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
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

        [UserScopedSettingAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
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

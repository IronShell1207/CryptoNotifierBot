﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TelegramBot.Constants {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SettingsMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SettingsMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TelegramBot.Constants.SettingsMessages", typeof(SettingsMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to disabled ⛔️.
        /// </summary>
        internal static string disabled {
            get {
                return ResourceManager.GetString("disabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to enabled ✅.
        /// </summary>
        internal static string enabled {
            get {
                return ResourceManager.GetString("enabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Notifications interval for crypto monitoring setted to {0} secs!.
        /// </summary>
        internal static string SetNotifyInterval {
            get {
                return ResourceManager.GetString("SetNotifyInterval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Task status: .
        /// </summary>
        internal static string taskInfoActiveStatus {
            get {
                return ResourceManager.GetString("taskInfoActiveStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exchange platform: .
        /// </summary>
        internal static string taskInfoExchangePlatform {
            get {
                return ResourceManager.GetString("taskInfoExchangePlatform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Symbol: .
        /// </summary>
        internal static string taskInfoSymbol {
            get {
                return ResourceManager.GetString("taskInfoSymbol", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Monitoring task total info:.
        /// </summary>
        internal static string taskInfoTitle {
            get {
                return ResourceManager.GetString("taskInfoTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trigger price: .
        /// </summary>
        internal static string taskInfoTriggerPrice {
            get {
                return ResourceManager.GetString("taskInfoTriggerPrice", resourceCulture);
            }
        }
    }
}

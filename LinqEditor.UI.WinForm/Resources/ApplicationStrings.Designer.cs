﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.33440
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinqEditor.UI.WinForm.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ApplicationStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ApplicationStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LinqEditor.UI.WinForm.Resources.ApplicationStrings", typeof(ApplicationStrings).Assembly);
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
        ///   Looks up a localized string similar to Linq Editor.
        /// </summary>
        internal static string APPLICATION_TITLE {
            get {
                return ResourceManager.GetString("APPLICATION_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Executing.
        /// </summary>
        internal static string EDITOR_QUERY_EXECUTING {
            get {
                return ResourceManager.GetString("EDITOR_QUERY_EXECUTING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ready.
        /// </summary>
        internal static string EDITOR_READY {
            get {
                return ResourceManager.GetString("EDITOR_READY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading session.
        /// </summary>
        internal static string EDITOR_SESSION_LOADING {
            get {
                return ResourceManager.GetString("EDITOR_SESSION_LOADING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (cancelled after {0}ms).
        /// </summary>
        internal static string EDITOR_TIMER_QUERY_CANCELLED_AFTER {
            get {
                return ResourceManager.GetString("EDITOR_TIMER_QUERY_CANCELLED_AFTER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (completed in {0}ms).
        /// </summary>
        internal static string EDITOR_TIMER_QUERY_COMPLETED_IN {
            get {
                return ResourceManager.GetString("EDITOR_TIMER_QUERY_COMPLETED_IN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (loaded in {0}ms).
        /// </summary>
        internal static string EDITOR_TIMER_SESSION_LOADED_IN {
            get {
                return ResourceManager.GetString("EDITOR_TIMER_SESSION_LOADED_IN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not validate the connection string.
        /// </summary>
        internal static string MESSAGE_BODY_CONNECTION_STRING_PARSE_ERROR {
            get {
                return ResourceManager.GetString("MESSAGE_BODY_CONNECTION_STRING_PARSE_ERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred reading {0} file. The data will be lost if overwritten. File is located in {1}.
        /// </summary>
        internal static string MESSAGE_BODY_ERROR_LOADING_CONNECTIONS {
            get {
                return ResourceManager.GetString("MESSAGE_BODY_ERROR_LOADING_CONNECTIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string MESSAGE_CAPTION_ERROR {
            get {
                return ResourceManager.GetString("MESSAGE_CAPTION_ERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Close tab (Ctrl+W).
        /// </summary>
        internal static string TOOLTIP_CLOSE_TAB {
            get {
                return ResourceManager.GetString("TOOLTIP_CLOSE_TAB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection manager.
        /// </summary>
        internal static string TOOLTIP_CONNECTION_MANAGER {
            get {
                return ResourceManager.GetString("TOOLTIP_CONNECTION_MANAGER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Run query (F5).
        /// </summary>
        internal static string TOOLTIP_EXECUTE {
            get {
                return ResourceManager.GetString("TOOLTIP_EXECUTE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New tab (Ctrl+T).
        /// </summary>
        internal static string TOOLTIP_NEW_TAB {
            get {
                return ResourceManager.GetString("TOOLTIP_NEW_TAB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stop query (Shift+F5).
        /// </summary>
        internal static string TOOLTIP_STOP_EXECUTION {
            get {
                return ResourceManager.GetString("TOOLTIP_STOP_EXECUTION", resourceCulture);
            }
        }
    }
}
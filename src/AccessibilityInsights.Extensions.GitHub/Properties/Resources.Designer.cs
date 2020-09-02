﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccessibilityInsights.Extensions.GitHub.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AccessibilityInsights.Extensions.GitHub.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to [a-zA-Z0-9].
        /// </summary>
        internal static string AlphaNumricPattern {
            get {
                return ResourceManager.GetString("AlphaNumricPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GitHub.
        /// </summary>
        internal static string extensionName {
            get {
                return ResourceManager.GetString("extensionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}/issues/new?title={1}&amp;body={2}.
        /// </summary>
        internal static string FormattedLink {
            get {
                return ResourceManager.GetString("FormattedLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (https:/*github.com/*|https:/*www.github.com/*|www.github.com/*).
        /// </summary>
        internal static string GitHubLink {
            get {
                return ResourceManager.GetString("GitHubLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid GitHub Link Format!.
        /// </summary>
        internal static string InvalidLink {
            get {
                return ResourceManager.GetString("InvalidLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^{0}/+{1}/+{2}$.
        /// </summary>
        internal static string LinkPatttern {
            get {
                return ResourceManager.GetString("LinkPatttern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following accessibility issue needs investigation.
        ///
        ///**App:** {0}
        ///
        ///**Element path:** {1}
        ///
        ///**Issue Details:** []
        ///
        ///**How To Fix:** [].
        /// </summary>
        internal static string NoFailureIssueBody {
            get {
                return ResourceManager.GetString("NoFailureIssueBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ({0}/{1}) has an accessibility issue that needs investigation.
        /// </summary>
        internal static string NoFailureIssueTitle {
            get {
                return ResourceManager.GetString("NoFailureIssueTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/owner/repo.
        /// </summary>
        internal static string PlaceHolder {
            get {
                return ResourceManager.GetString("PlaceHolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ({0}?[_]?[-]?[.]?)+.
        /// </summary>
        internal static string RepoNamePattern {
            get {
                return ResourceManager.GetString("RepoNamePattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ...
        /// </summary>
        internal static string RepoNameSpecialCasesDoubleDot {
            get {
                return ResourceManager.GetString("RepoNameSpecialCasesDoubleDot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following accessibility issue needs investigation.
        ///
        ///**App:** {0}
        ///
        ///**Element path:** {1}
        ///
        ///**Issue Details:** {2} [{3}]({4})
        ///
        ///**How To Fix:** {5}
        ///
        ///This accessibility issue was found using Accessibility Insights for Windows, a tool that helps debug and find accessibility issues earlier. Get more information and download this tool at https://aka.ms/AccessibilityInsights..
        /// </summary>
        internal static string SingleFailureIssueBody {
            get {
                return ResourceManager.GetString("SingleFailureIssueBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: ({1}/{2}) {3}.
        /// </summary>
        internal static string SingleFailureIssueTitle {
            get {
                return ResourceManager.GetString("SingleFailureIssueTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter desired GitHub repo link.
        /// </summary>
        internal static string tbURLPlaceHolder {
            get {
                return ResourceManager.GetString("tbURLPlaceHolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}+([-]{0}+)*.
        /// </summary>
        internal static string UserNamePattern {
            get {
                return ResourceManager.GetString("UserNamePattern", resourceCulture);
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kilometros_WebGlobalization.API {
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
    public class ControllerStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ControllerStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kilometros_WebGlobalization.API.ControllerStrings", typeof(ControllerStrings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ooops! We&apos;re still developing that one... sorry.
        /// </summary>
        public static string GenericNotImplemented {
            get {
                return ResourceManager.GetString("GenericNotImplemented", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validation error: {0}.
        /// </summary>
        public static string GenericValidationError {
            get {
                return ResourceManager.GetString("GenericValidationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User is already logged in.
        /// </summary>
        public static string Warning100_CannotLoginAgain {
            get {
                return ResourceManager.GetString("Warning100_CannotLoginAgain", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User [{0}] requested could not be found, or password is incorrect.
        /// </summary>
        public static string Warning101_UserNotFound {
            get {
                return ResourceManager.GetString("Warning101_UserNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Token was deleted successfully.
        /// </summary>
        public static string Warning103_TokenDeleteOk {
            get {
                return ResourceManager.GetString("Warning103_TokenDeleteOk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User is logged in and cannot register a new account.
        /// </summary>
        public static string Warning201_CannotCreateUserWithSessionOpen {
            get {
                return ResourceManager.GetString("Warning201_CannotCreateUserWithSessionOpen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contact Information is not set yet.
        /// </summary>
        public static string Warning203_ContactInfoNotSet {
            get {
                return ResourceManager.GetString("Warning203_ContactInfoNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Physical Profile is not set yet.
        /// </summary>
        public static string Warning204_PhysicalInfoNotSet {
            get {
                return ResourceManager.GetString("Warning204_PhysicalInfoNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account Data received is invalid.
        /// </summary>
        public static string Warning501_AccountDataInvalid {
            get {
                return ResourceManager.GetString("Warning501_AccountDataInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Account Data received is not different to current data.
        /// </summary>
        public static string Warning502_AccountDataNotChanged {
            get {
                return ResourceManager.GetString("Warning502_AccountDataNotChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tip requested could not be found, or ID is invalid.
        /// </summary>
        public static string Warning801_TipNotFound {
            get {
                return ResourceManager.GetString("Warning801_TipNotFound", resourceCulture);
            }
        }
    }
}

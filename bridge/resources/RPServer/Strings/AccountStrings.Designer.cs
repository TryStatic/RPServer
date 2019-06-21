﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RPServer.Strings {
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
    internal class AccountStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccountStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RPServer.Strings.AccountStrings", typeof(AccountStrings).Assembly);
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
        ///   Looks up a localized string similar to You are already logged in..
        /// </summary>
        internal static string ErrorAlreadyLoggedIn {
            get {
                return ResourceManager.GetString("ErrorAlreadyLoggedIn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The new email address must be different than the old one..
        /// </summary>
        internal static string ErrorChangeVerificationEmailDuplicate {
            get {
                return ResourceManager.GetString("ErrorChangeVerificationEmailDuplicate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your email address is already verified..
        /// </summary>
        internal static string ErrorEmailAlreadyVerified {
            get {
                return ResourceManager.GetString("ErrorEmailAlreadyVerified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is not a valid email address..
        /// </summary>
        internal static string ErrorEmailInvalid {
            get {
                return ResourceManager.GetString("ErrorEmailInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to That email address is already taken..
        /// </summary>
        internal static string ErrorEmailTaken {
            get {
                return ResourceManager.GetString("ErrorEmailTaken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have provided invalid an invalid username/password combination..
        /// </summary>
        internal static string ErrorInvalidCredentials {
            get {
                return ResourceManager.GetString("ErrorInvalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have provided an invalid verification token..
        /// </summary>
        internal static string ErrorInvalidVerificationCode {
            get {
                return ResourceManager.GetString("ErrorInvalidVerificationCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are not logged in..
        /// </summary>
        internal static string ErrorNotLoggedIn {
            get {
                return ResourceManager.GetString("ErrorNotLoggedIn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is not a valid password..
        /// </summary>
        internal static string ErrorPasswordInvalid {
            get {
                return ResourceManager.GetString("ErrorPasswordInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You haven&apos;t verified your email address yet..
        /// </summary>
        internal static string ErrorUnverifiedEmail {
            get {
                return ResourceManager.GetString("ErrorUnverifiedEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is not a valid username..
        /// </summary>
        internal static string ErrorUsernameInvalid {
            get {
                return ResourceManager.GetString("ErrorUsernameInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We couldn&apos;t a record related to that username..
        /// </summary>
        internal static string ErrorUsernameNotExist {
            get {
                return ResourceManager.GetString("ErrorUsernameNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to That username is already taken..
        /// </summary>
        internal static string ErrorUsernameTaken {
            get {
                return ResourceManager.GetString("ErrorUsernameTaken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have changed your verification email address. Check your inbox..
        /// </summary>
        internal static string SuccessChangeVerificationEmailAddress {
            get {
                return ResourceManager.GetString("SuccessChangeVerificationEmailAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Congratulations, you have successfully verified your email address. Welcome to our server..
        /// </summary>
        internal static string SuccessEmailVerification {
            get {
                return ResourceManager.GetString("SuccessEmailVerification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have successfully logged in, welcome back..
        /// </summary>
        internal static string SuccessLogin {
            get {
                return ResourceManager.GetString("SuccessLogin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Congratulations, you have successfully registered a new account..
        /// </summary>
        internal static string SuccessRegistration {
            get {
                return ResourceManager.GetString("SuccessRegistration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We have re-sent an email containing your verification token to your inbox..
        /// </summary>
        internal static string SuccessResendVerificationEmail {
            get {
                return ResourceManager.GetString("SuccessResendVerificationEmail", resourceCulture);
            }
        }
    }
}

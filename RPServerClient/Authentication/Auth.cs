using EventNames;
using RAGE;
using RAGE.Elements;
using RPServerClient.Globals;
using Events = RAGE.Events;

// ReSharper disable CommentTypo

namespace RPServerClient.Authentication
{
    internal class Auth : Events.Script
    {
        public Auth()
        {

            #region SERVER_TO_CLIENT
            Events.Add(ServerToClient.SetLoginScreen, OnSetLoginScreen);
            Events.Add(ServerToClient.DisplayError, OnDisplayError);
            Events.Add(ServerToClient.RegistrationSuccess, OnRegistrationSuccess);
            Events.Add(ServerToClient.Show2FAbyEmailAddress, OnShow2FAbyEmailAddress);
            Events.Add(ServerToClient.Show2FAbyGoogleAuth, OnShow2FAbyGoogleAuth);
            Events.Add(ServerToClient.ShowInitialEmailVerification, OnShowInitialEmailVerification);
            Events.Add(ServerToClient.ShowChangeEmailAddress, OnShowChangeEmailAddress);
            Events.Add(ServerToClient.ShowLoginPage, OnShowLoginPage);
            Events.Add(ServerToClient.ShowQRCode, OnShowQRCode);
            Events.Add(ServerToClient.ShowQRCodeEnabled, OnShowQRCodeEnabled);
            #endregion

            #region CEF_TO_CLIENT
            // From multiple pages
            Events.Add("onBackToLogin", OnBackToLogin);
            Events.Add("onResendMail", OnResendMail);
            // From register.html
            Events.Add("onSubmitRegister", OnSubmitRegister);
            // From login.html
            Events.Add("onSubmitLogin", OnSubmitLogin);
            // From forgot.html
            Events.Add("onSubmitForgetPass", OnSubmitForgetPass);
            // From verifyemail.html
            Events.Add("onSubmitEmailToken", OnSubmitEmailToken);
            // From verifygoogleauth.html
            Events.Add("onSubmitGoogleAuthCode", OnSubmitGoogleAuthCode);
            // From newpass.html
            Events.Add("onSubmitNewPass", OnSubmitNewPass);
            // From verifyemailfirst.html
            Events.Add("onSubmitFirstEmailToken", OnSubmitFirstEmailToken);
            // From changemail.html
            Events.Add("onSubmitNewEmail", OnSubmitNewEmail);
            // From enabledgoogleauth.html
            Events.Add("onCloseWindow", OnCloseWindow);
            Events.Add("onSubmitEnableGoogleAuthCode", onSubmitEnableGoogleAuthCode);
            #endregion


        }


        private void OnShowQRCodeEnabled(object[] args)
        {
            Chat.Output("FROM:Server|OnShowInitialEmailVerification => Display:verifyemailfirst.html");
            Browser.MainBrowser.Url = "package://CEF/auth/enabledgoogleauth.html";
        }


        private void OnCloseWindow(object[] args)
        {
            Browser.DestroyBrowser(null);
        }

        private void OnShowQRCode(object[] args)
        {
            if (args[0] == null) return;
            var link = args[0].ToString();
            Chat.Output(link.Length.ToString());
            Browser.CreateBrowser(new []{ "package://CEF/auth/enablegoogleauth.html" });
            Browser.ExecuteFunction(new object[] { "addImage", link });
        }

        private void OnShowLoginPage(object[] args)
        {
            Chat.Output("FROM:Server|OnShowLoginPage => Display:login.html");
            Browser.MainBrowser.Url = "package://CEF/auth/login.html";
        }

        private void OnShowInitialEmailVerification(object[] args)
        {
            Chat.Output("FROM:Server|OnShowInitialEmailVerification => Display:verifyemailfirst.html");
            Browser.MainBrowser.Url = "package://CEF/auth/verifyemailfirst.html";
        }

        private void OnShow2FAbyGoogleAuth(object[] args)
        {
            Chat.Output("FROM:Server|OnShow2FAbyGoogleAuth => Display:verifygoogleauth.html");
            Browser.MainBrowser.Url = "package://CEF/auth/verifygoogleauth.html";
            if(args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnShow2FAbyEmailAddress(object[] args)
        {
            Chat.Output("FROM:Server|OnShow2FAbyEmailAddress => Display:verifyemail.html");
            Browser.MainBrowser.Url = "package://CEF/auth/verifyemail.html";
            if (args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnShowChangeEmailAddress(object[] args)
        {
            Chat.Output("FROM:Server|OnShow2FAbyEmailAddress => Display:verifyemail.html");
            Browser.MainBrowser.Url = "package://CEF/auth/changemail.html";
            if (args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnRegistrationSuccess(object[] args)
        {
            Chat.Output("FROM:Server|OnRegistrationSuccess => Display:login.html");
            Browser.MainBrowser.Url = "package://CEF/auth/login.html";
            if (args[0] != null) OnDisplaySuccess(new[] { args[0] });
        }


        private void onSubmitEnableGoogleAuthCode(object[] args)
        {
            Chat.Output("FROM:CEF|onSubmitEnableGoogleAuthCode => SERVERCALL: SubmitEnableGoogleAuthCode");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitEnableGoogleAuthCode, args[0].ToString());
        }

        // mp.trigger("onSubmitNewEmail", email);
        private void OnSubmitNewEmail(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitNewEmail => SERVERCALL: SubmitNewVerificationEmail");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitNewVerificationEmail, args[0].ToString());
        }

        // mp.trigger("onSubmitFirstEmailToken", token);
        private void OnSubmitFirstEmailToken(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitFirstEmailToken => SERVERCALL: SubmitFirstEmailToken");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitFirstEmailToken, args[0].ToString());
        }

        // mp.trigger("onSubmitGoogleAuthCode", code);
        private void OnSubmitGoogleAuthCode(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitGoogleAuthCode => SERVERCALL: SubmitGoogleAuthCode");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitGoogleAuthCode, args[0].ToString());
        }

        // mp.trigger("onSubmitEmailToken", user, pass);
        private void OnSubmitEmailToken(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitEmailToken => SERVERCALL: SubmitEmailToken");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitEmailToken, args[0].ToString());
        }

        // mp.trigger("onResendMail");
        private void OnResendMail(object[] args)
        {
            Chat.Output("FROM:CEF|OnResendMail => SERVERCALL: SubmitResendEmail");
            Events.CallRemote(ClientToServer.SubmitResendEmail);
        }

        // mp.trigger("onBackToLogin");
        private void OnBackToLogin(object[] args)
        {
            Chat.Output("FROM:CEF|OnBackToLogin => SERVERCALL: SubmitBackToLogin");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitBackToLogin);
        }

        // mp.trigger("onSubmitForgetPass", user, email);
        private void OnSubmitForgetPass(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitForgetPass => SERVERCALL: SubmitForgetPass");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote("SubmitForgetPass", args[0].ToString(), args[1].ToString());
        }

        // mp.trigger("onSubmitNewPass", pass);
        private void OnSubmitNewPass(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitNewPass => SERVERCALL: SubmitSubmitNewPass");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote("SubmitSubmitNewPass", args[0].ToString());
        }

        // mp.trigger("onSubmitLogin", user, pass);
        private void OnSubmitLogin(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitLogin => SERVERCALL: SubmitLoginAccount");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitLoginAccount, args[0].ToString(), args[1].ToString());
        }

        //mp.trigger("onSubmitRegister", user, email, pass);
        private void OnSubmitRegister(object[] args)
        {
            Chat.Output("FROM:CEF|OnSubmitRegister => SERVERCALL: SubmitRegisterAccount");
            Browser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitRegisterAccount, args[0].ToString(), args[1].ToString(), args[2].ToString());
        }

        private void OnDisplayError(object[] args)
        {
            Chat.Output("FROM:SERVER|OnDisplayError => CEFCALL: showSuccess");
            if (args[0] == null) return;

            var msg = args[0] as string;

            Browser.ExecuteFunction("HideLoading");
            Browser.ExecuteFunction(new object[] { "showError", msg.Replace("'", @"\'") });
        }

        private void OnDisplaySuccess(object[] args)
        {
            Chat.Output("FROM:SERVER|OnDisplaySuccess => CEFCALL: showSuccess");
            if (args[0] == null) return;

            var msg = args[0] as string;

            Browser.ExecuteFunction("HideLoading");
            Browser.ExecuteFunction(new object[] { "showSuccess", msg.Replace("'", @"\'") });
        }

        private void OnSetLoginScreen(object[] args)
        {
            var state = (bool)args[0];

            Player.LocalPlayer.FreezePosition(state);

            if (state)
            {
                Chat.Output("FROM:SERVER|OnSetLoginScreen => Display:login.html");
                Browser.CreateBrowser(new object[] { "package://CEF/auth/login.html" });
                RAGE.Game.Graphics.TransitionToBlurred(200);
                Events.CallLocal("setEnabled", false);
            }
            else
            {
                Chat.Output("FROM:SERVER|OnSetLoginScreen => CLIENT:DestroyBrowser()");
                Browser.DestroyBrowser(null);
                RAGE.Game.Graphics.TransitionFromBlurred(200);
                Events.CallLocal("setEnabled", true);
            }
        }
    }
}

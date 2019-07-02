using EventNames;
using RAGE;
using RAGE.Elements;
using RPServerClient.Globals;
using Events = RAGE.Events;

// ReSharper disable CommentTypo

namespace RPServerClient.Authentication
{
    internal class Authentication : Events.Script
    {
        private static readonly Vector3 LoginCamPos = new Vector3(148.88035583496094f, -1407.726318359375f, 156.79771423339844f);
        private static readonly Vector3 LoginCamPointAt = new Vector3(126.11740112304688f, -772.676025390625f, 155.15695190429688f);
        private static readonly CustomCamera LoginCam = new CustomCamera(LoginCamPos, LoginCamPointAt);

        public Authentication()
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
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/enabledgoogleauth.html";
        }


        private void OnCloseWindow(object[] args)
        {
            CustomBrowser.DestroyBrowser(null);
        }

        private void OnShowQRCode(object[] args)
        {
            if (args[0] == null) return;
            var link = args[0].ToString();
            CustomBrowser.CreateBrowser("package://CEF/auth/enablegoogleauth.html");
            CustomBrowser.ExecuteFunction(new object[] { "addImage", link });
        }

        private void OnShowLoginPage(object[] args)
        {
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/login.html";
        }

        private void OnShowInitialEmailVerification(object[] args)
        {
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/verifyemailfirst.html";
        }

        private void OnShow2FAbyGoogleAuth(object[] args)
        {
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/verifygoogleauth.html";
            if(args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnShow2FAbyEmailAddress(object[] args)
        {
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/verifyemail.html";
            if (args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnShowChangeEmailAddress(object[] args)
        {
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/changemail.html";
            if (args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnRegistrationSuccess(object[] args)
        {
            CustomBrowser.MainBrowser.Url = "package://CEF/auth/login.html";
            if (args[0] != null) OnDisplaySuccess(new[] { args[0] });
        }


        private void onSubmitEnableGoogleAuthCode(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitEnableGoogleAuthCode, args[0].ToString());
        }

        // mp.trigger("onSubmitNewEmail", email);
        private void OnSubmitNewEmail(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitNewVerificationEmail, args[0].ToString());
        }

        // mp.trigger("onSubmitFirstEmailToken", token);
        private void OnSubmitFirstEmailToken(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitFirstEmailToken, args[0].ToString());
        }

        // mp.trigger("onSubmitGoogleAuthCode", code);
        private void OnSubmitGoogleAuthCode(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitGoogleAuthCode, args[0].ToString());
        }

        // mp.trigger("onSubmitEmailToken", user, pass);
        private void OnSubmitEmailToken(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitEmailToken, args[0].ToString());
        }

        // mp.trigger("onResendMail");
        private void OnResendMail(object[] args)
        {
            Events.CallRemote(ClientToServer.SubmitResendEmail);
        }

        // mp.trigger("onBackToLogin");
        private void OnBackToLogin(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitBackToLogin);
        }

        // mp.trigger("onSubmitForgetPass", user, email);
        private void OnSubmitForgetPass(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote("SubmitForgetPass", args[0].ToString(), args[1].ToString());
        }

        // mp.trigger("onSubmitNewPass", pass);
        private void OnSubmitNewPass(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote("SubmitSubmitNewPass", args[0].ToString());
        }

        // mp.trigger("onSubmitLogin", user, pass);
        private void OnSubmitLogin(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitLoginAccount, args[0].ToString(), args[1].ToString());
        }

        //mp.trigger("onSubmitRegister", user, email, pass);
        private void OnSubmitRegister(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowLoading");
            Events.CallRemote(ClientToServer.SubmitRegisterAccount, args[0].ToString(), args[1].ToString(), args[2].ToString());
        }

        private void OnDisplayError(object[] args)
        {
            if (args[0] == null) return;

            var msg = args[0] as string;

            CustomBrowser.ExecuteFunction("HideLoading");
            CustomBrowser.ExecuteFunction(new object[] { "showError", msg.Replace("'", @"\'") });
        }

        private void OnDisplaySuccess(object[] args)
        {
            if (args[0] == null) return;

            var msg = args[0] as string;

            CustomBrowser.ExecuteFunction("HideLoading");
            CustomBrowser.ExecuteFunction(new object[] { "showSuccess", msg.Replace("'", @"\'") });
        }

        private void OnSetLoginScreen(object[] args)
        {
            var state = (bool) args[0];
            LoginCam.SetActive(state);

            if (state)
            {
                CustomBrowser.CreateBrowser("package://CEF/auth/login.html");
                RAGE.Game.Graphics.TransitionToBlurred(200);
                Player.LocalPlayer.FreezePosition(true);
                //Events.CallLocal("setChatState", false); TODO: UNCOMMENT WHEN DONE TESTING
                RAGE.Game.Ui.DisplayHud(false);
                RAGE.Game.Ui.DisplayRadar(false);
            }
            else
            {
                CustomBrowser.DestroyBrowser(null);
                RAGE.Game.Graphics.TransitionFromBlurred(200);
            }
        }
    }
}

using RAGE;
using RAGE.Elements;
using RPServerClient.Client;
using Events = RAGE.Events;

// ReSharper disable CommentTypo

namespace RPServerClient.Authentication
{
    internal class Authentication : Events.Script
    {
        private static CamHandler _camera;

        private static readonly Vector3 LoginCamPos = new Vector3(148.88035583496094f, -1407.726318359375f, 156.79771423339844f);
        private static readonly Vector3 LoginCamPointAt = new Vector3(126.11740112304688f, -772.676025390625f, 155.15695190429688f);

        public Authentication()
        {
            #region SERVER_TO_CLIENT
            Events.Add(Shared.Events.ServerToClient.Authentication.SetLoginScreen, OnSetLoginScreen);
            Events.Add(Shared.Events.ServerToClient.Authentication.DisplayError, OnDisplayError);
            Events.Add(Shared.Events.ServerToClient.Authentication.RegistrationSuccess, OnRegistrationSuccess);
            Events.Add(Shared.Events.ServerToClient.Authentication.Show2FAbyEmailAddress, OnShow2FAbyEmailAddress);
            Events.Add(Shared.Events.ServerToClient.Authentication.Show2FAbyGoogleAuth, OnShow2FAbyGoogleAuth);
            Events.Add(Shared.Events.ServerToClient.Authentication.ShowInitialEmailVerification, OnShowInitialEmailVerification);
            Events.Add(Shared.Events.ServerToClient.Authentication.ShowChangeEmailAddress, OnShowChangeEmailAddress);
            Events.Add(Shared.Events.ServerToClient.Authentication.ShowLoginPage, OnShowLoginPage);
            Events.Add(Shared.Events.ServerToClient.Authentication.ShowQRCode, OnShowQRCode);
            Events.Add(Shared.Events.ServerToClient.Authentication.ShowQRCodeEnabled, OnShowQRCodeEnabled);
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
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/enabledgoogleauth.html";
        }

        private void OnCloseWindow(object[] args)
        {
            BrowserHandler.DestroyBrowser(null);
        }

        private void OnShowQRCode(object[] args)
        {
            if (args[0] == null) return;
            var link = args[0].ToString();
            BrowserHandler.CreateBrowser("package://CEF/auth/enablegoogleauth.html");
            BrowserHandler.ExecuteFunction(new object[] { "addImage", link });
        }

        private void OnShowLoginPage(object[] args)
        {
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/login.html";
        }

        private void OnShowInitialEmailVerification(object[] args)
        {
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/verifyemailfirst.html";
        }

        private void OnShow2FAbyGoogleAuth(object[] args)
        {
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/verifygoogleauth.html";
            if(args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnShow2FAbyEmailAddress(object[] args)
        {
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/verifyemail.html";
            if (args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnShowChangeEmailAddress(object[] args)
        {
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/changemail.html";
            if (args[0] != null) OnDisplayError(new[] { args[0] });
        }

        private void OnRegistrationSuccess(object[] args)
        {
            BrowserHandler.BrowserHtmlWindow.Url = "package://CEF/auth/login.html";
            if (args[0] != null) OnDisplaySuccess(new[] { args[0] });
        }

        private void onSubmitEnableGoogleAuthCode(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitEnableGoogleAuthCode, args[0].ToString());
        }

        private void OnSubmitNewEmail(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitNewVerificationEmail, args[0].ToString());
        }

        private void OnSubmitFirstEmailToken(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitFirstEmailToken, args[0].ToString());
        }

        private void OnSubmitGoogleAuthCode(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitGoogleAuthCode, args[0].ToString());
        }

        private void OnSubmitEmailToken(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitEmailToken, args[0].ToString());
        }

        private void OnResendMail(object[] args)
        {
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitResendEmail);
        }

        private void OnBackToLogin(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitBackToLogin);
        }

        private void OnSubmitForgetPass(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote("SubmitForgetPass", args[0].ToString(), args[1].ToString());
        }

        private void OnSubmitNewPass(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote("SubmitSubmitNewPass", args[0].ToString());
        }

        private void OnSubmitLogin(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitLoginAccount, args[0].ToString(), args[1].ToString());
        }

        private void OnSubmitRegister(object[] args)
        {
            BrowserHandler.ExecuteFunction("ShowLoading");
            Events.CallRemote(Shared.Events.ClientToServer.Authentication.SubmitRegisterAccount, args[0].ToString(), args[1].ToString(), args[2].ToString());
        }

        private void OnDisplayError(object[] args)
        {
            if (args[0] == null) return;

            var msg = args[0] as string;

            BrowserHandler.ExecuteFunction("HideLoading");
            BrowserHandler.ExecuteFunction(new object[] { "showError", msg.Replace("'", @"\'") });
        }

        private void OnDisplaySuccess(object[] args)
        {
            if (args[0] == null) return;

            var msg = args[0] as string;

            BrowserHandler.ExecuteFunction("HideLoading");
            BrowserHandler.ExecuteFunction(new object[] { "showSuccess", msg.Replace("'", @"\'") });
        }

        private void OnSetLoginScreen(object[] args)
        {
            var state = (bool) args[0];

            if (state)
            { // Enable
                BrowserHandler.CreateBrowser("package://CEF/auth/login.html");
                RAGE.Game.Graphics.TransitionToBlurred(200);
                Player.LocalPlayer.FreezePosition(true);
                RAGE.Game.Ui.DisplayHud(false);
                RAGE.Game.Ui.DisplayRadar(false);
                _camera = new CamHandler();
                _camera.SetPos(LoginCamPos, LoginCamPointAt, true);
                RAGE.Chat.Show(false);
            }
            else
            {
                BrowserHandler.DestroyBrowser(null);
                RAGE.Game.Graphics.TransitionFromBlurred(200);
                _camera.SetActive(false);
                _camera.Destroy();
                _camera = null;
            }
        }
    }
}

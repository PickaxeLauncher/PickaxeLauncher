using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using Pickaxe.Helpers;
using XboxAuthNet.Game.Accounts;
using XboxAuthNet.Game.Msal;
using XboxAuthNet.Game.SessionStorages;

namespace Pickaxe.Controllers;

public class AccountController {
    private const string CLIENT_ID = "499c8d36-be2a-4231-9ebd-ef291b7bb64c";
    private readonly JsonXboxGameAccountManager _accountManager;
    private readonly JELoginHandler _loginHandler;
    private MSession _session;

    public AccountController() {
        _accountManager = new JsonXboxGameAccountManager(Utils.GetAppFolder("accounts.json"));
        _loginHandler = new JELoginHandlerBuilder()
            .WithAccountManager(_accountManager)
            .Build();
    }

    public string Username => _session?.Username ?? "Steve";
    public XboxGameAccountCollection Accounts => _accountManager.GetAccounts();
    public event EventHandler AccountChanged;

    public void SignOut() {
        _accountManager.ClearAccounts();
        _session = null;
        AccountChanged.Invoke(this, EventArgs.Empty);
    }

    public async Task AddAccount() {
        var app = await MsalClientHelper.BuildApplicationWithCache(CLIENT_ID);
        var authenticator = _loginHandler.CreateAuthenticatorWithNewAccount();
        authenticator.AddMsalOAuth(app, msal => msal.Interactive());
        authenticator.AddXboxAuthForJE(xbox => xbox.Basic());
        authenticator.AddForceJEAuthenticator();
        SwitchSession(await authenticator.ExecuteForLauncherAsync());
    }

    private async Task LoginToDefaultAccount() {
        var app = await MsalClientHelper.BuildApplicationWithCache(CLIENT_ID);
        var authenticator = _loginHandler.CreateAuthenticatorWithDefaultAccount();
        authenticator.AddMsalOAuth(app, msal => msal.Silent());
        authenticator.AddXboxAuthForJE(xbox => xbox.Basic());
        authenticator.AddJEAuthenticator();
        SwitchSession(await authenticator.ExecuteForLauncherAsync());
    }

    private void SwitchSession(MSession session) {
        if (session != null && session.CheckIsValid()) {
            _session = session;
            AccountChanged.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task StartupAsync() {
        if (Accounts.Count == 0) {
            await AddAccount();
        } else {
            await LoginToDefaultAccount();
        }
    }
}
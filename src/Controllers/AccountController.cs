using System.Threading.Tasks;
using CmlLib.Core.Auth.Microsoft;
using Pickaxe.Helpers;
using XboxAuthNet.Game.Accounts;
using XboxAuthNet.Game.Msal;

namespace Pickaxe.Controllers;

public class AccountController {
    private const string CLIENT_ID = "499c8d36-be2a-4231-9ebd-ef291b7bb64c";

    private readonly JsonXboxGameAccountManager _accountManager;

    public AccountController() {
        _accountManager = new JsonXboxGameAccountManager(Utils.GetAppFolder("accounts.json"));
    }

    public XboxGameAccountCollection Accounts => _accountManager.GetAccounts();

    public async Task AddAccount() {
        var loginHandler = new JELoginHandlerBuilder()
            .WithAccountManager(_accountManager)
            .Build();
        var app = await MsalClientHelper.BuildApplicationWithCache(CLIENT_ID);
        var authenticator = loginHandler.CreateAuthenticatorWithNewAccount();
        authenticator.AddMsalOAuth(app, msal => msal.Interactive());
        authenticator.AddXboxAuthForJE(xbox => xbox.Basic());
        authenticator.AddForceJEAuthenticator();
        await authenticator.ExecuteForLauncherAsync();
    }
}

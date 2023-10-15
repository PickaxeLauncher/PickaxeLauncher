using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Nickvision.Aura;
using Pickaxe.Helpers;

public class AboutDialog : GetTextMixin {
    private readonly Adw.AboutWindow _dialog = Adw.AboutWindow.New();

    public AboutDialog(Gtk.Window window) {
        var debugInfo = new StringBuilder();
        debugInfo.AppendLine(Aura.Active.AppInfo.ID);
        debugInfo.AppendLine(Aura.Active.AppInfo.Version);
        debugInfo.AppendLine(
            $"GTK {Gtk.Functions.GetMajorVersion()}.{Gtk.Functions.GetMinorVersion()}.{Gtk.Functions.GetMicroVersion()}");
        debugInfo.AppendLine(
            $"libadwaita {Adw.Functions.GetMajorVersion()}.{Adw.Functions.GetMinorVersion()}.{Adw.Functions.GetMicroVersion()}");
        if (File.Exists("/.flatpak-info")) {
            debugInfo.AppendLine("Flatpak");
        } else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SNAP"))) {
            debugInfo.AppendLine("Snap");
        }

        debugInfo.AppendLine(CultureInfo.CurrentCulture.ToString());
        var localeProcess = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "locale",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };
        try {
            localeProcess.Start();
            var localeString = localeProcess.StandardOutput.ReadToEnd().Trim();
            localeProcess.WaitForExit();
            debugInfo.AppendLine(localeString);
        } catch {
            debugInfo.AppendLine("Unknown locale");
        }

        _dialog.SetTransientFor(window);
        _dialog.SetIconName(Aura.Active.AppInfo.ID);
        _dialog.SetApplicationName(Aura.Active.AppInfo.ShortName);
        _dialog.SetApplicationIcon(Aura.Active.AppInfo.ID +
                                   (Aura.Active.AppInfo.IsDevVersion ? "-devel" : ""));
        _dialog.SetVersion(Aura.Active.AppInfo.Version);
        _dialog.SetDebugInfo(debugInfo.ToString());
        _dialog.SetComments(Aura.Active.AppInfo.Description);
        _dialog.SetDeveloperName("Nickvision");
        _dialog.SetLicenseType(Gtk.License.MitX11);
        _dialog.SetCopyright("© Nickvision 2021-2023");
        _dialog.SetWebsite("https://nickvision.org/");
        _dialog.SetIssueUrl(Aura.Active.AppInfo.IssueTracker.ToString());
        _dialog.SetSupportUrl(Aura.Active.AppInfo.SupportUrl.ToString());
        _dialog.AddLink(_("GitHub Repo"), Aura.Active.AppInfo.SourceRepo.ToString());
        foreach (var pair in Aura.Active.AppInfo.ExtraLinks) {
            _dialog.AddLink(pair.Key, pair.Value.ToString());
        }

        _dialog.SetDevelopers(
            Aura.Active.AppInfo.ConvertURLDictToArray(Aura.Active.AppInfo.Developers));
        _dialog.SetDesigners(
            Aura.Active.AppInfo.ConvertURLDictToArray(Aura.Active.AppInfo.Designers));
        _dialog.SetArtists(Aura.Active.AppInfo.ConvertURLDictToArray(Aura.Active.AppInfo.Artists));
        _dialog.SetTranslatorCredits(Aura.Active.AppInfo.TranslatorCredits);
        _dialog.SetReleaseNotes(Aura.Active.AppInfo.HTMLChangelog);
    }

    public void Present() => _dialog.Present();
}
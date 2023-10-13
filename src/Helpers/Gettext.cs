using GetText;
using System.IO;
using System.Reflection;

namespace Pickaxe.Helpers;

public static class Gettext {
    private static readonly ICatalog _catalog = new Catalog("application", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

    public static string _(string text) {
        return _catalog.GetString(text);
    }

    public static string _(string text, params object[] args) {
        return _catalog.GetString(text, args);
    }

    public static string _n(string text, string pluralText, long n) {
        return _catalog.GetPluralString(text, pluralText, n);
    }

    public static string _n(string text, string pluralText, long n, params object[] args) {
        return _catalog.GetPluralString(text, pluralText, n, args);
    }

    public static string _p(string context, string text) {
        return _catalog.GetParticularString(context, text);
    }

    public static string _p(string context, string text, params object[] args) {
        return _catalog.GetParticularString(context, text, args);
    }

    public static string _pn(string context, string text, string pluralText, long n) {
        return _catalog.GetParticularPluralString(context, text, pluralText, n);
    }

    public static string _pn(string context, string text, string pluralText, long n, params object[] args) {
        return _catalog.GetParticularPluralString(context, text, pluralText, n, args);
    }
}

public class GetTextMixin {
    public string _(string text) {
        return Gettext._(text);
    }

    public string _(string text, params object[] args) {
        return Gettext._(text, args);
    }

    public string _n(string text, string pluralText, long n) {
        return Gettext._n(text, pluralText, n);
    }

    public string _n(string text, string pluralText, long n, params object[] args) {
        return Gettext._n(text, pluralText, n, args);
    }

    public string _p(string context, string text) {
        return Gettext._p(context, text);
    }

    public string _p(string context, string text, params object[] args) {
        return Gettext._p(context, text, args);
    }

    public string _pn(string context, string text, string pluralText, long n) {
        return Gettext._pn(context, text, pluralText, n);
    }

    public string _pn(string context, string text, string pluralText, long n, params object[] args) {
        return Gettext._pn(context, text, pluralText, n, args);
    }
}
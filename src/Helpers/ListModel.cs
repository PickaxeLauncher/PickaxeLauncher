

using System;
using System.Collections.Generic;

namespace Pickaxe.Helpers;

public abstract class ListModel<T> : List<T> {
    private event EventHandler<T> _added;
    private event EventHandler<T> _removed;
    public new void Add(T item) {
        if (Contains(item)) {
            return;
        }
        _added?.Invoke(this, item);
        base.Add(item);
    }

    public new void Remove(T item) {
        if (!Contains(item)) {
            return;
        }
        _removed?.Invoke(this, item);
        base.Remove(item);
    }

    public void Connect(Gtk.ListBox widget, Func<T, Gtk.Widget> factory) {
        foreach (var item in this) {
            widget.Append(factory(item));
        }
        _added += (sender, args) => widget.Append(factory(args));
        _removed += (sender, args) => widget.Remove(factory(args));
    }

    public void Connect(Gtk.Box widget, Func<T, Gtk.Widget> factory) {
        foreach (var item in this) {
            widget.Append(factory(item));
        }
        _added += (sender, args) => widget.Append(factory(args));
        _removed += (sender, args) => widget.Remove(factory(args));
    }
}

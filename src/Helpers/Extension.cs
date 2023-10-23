using System;
using System.IO;
using System.Threading.Tasks;

namespace Pickaxe.Helpers.Extension;

public static class PixbufExtension {
    public static GdkPixbuf.Pixbuf MakeSquare(this GdkPixbuf.Pixbuf pixbuf, int iconSize) {
        var smallestSide = Math.Min(pixbuf.Width, pixbuf.Height);
        return pixbuf.NewSubpixbuf(0, 0, smallestSide, smallestSide)
            .ScaleSimple(iconSize, iconSize, GdkPixbuf.InterpType.Bilinear);
    }
}
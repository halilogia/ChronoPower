using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace ChronoPower.Services;

public static class AppIconGenerator
{
    private static Icon? _cachedIcon;

    public static Icon GetOrCreateAppIcon(string? savePath = null)
    {
        if (_cachedIcon != null && string.IsNullOrEmpty(savePath))
        {
            return _cachedIcon;
        }

        const int size = 64;
        using var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            // Arka plan koyu yuvarlak
            using var bgBrush = new LinearGradientBrush(
                new Rectangle(0, 0, size, size),
                Color.FromArgb(24, 24, 37),   // Koyu lacivert
                Color.FromArgb(49, 46, 129),  // İndigo
                LinearGradientMode.ForwardDiagonal);
            g.FillEllipse(bgBrush, 2, 2, size - 4, size - 4);

            // Parlak mor/mavi neon halka
            using var ringPen = new Pen(Color.FromArgb(99, 102, 241), 3f);
            g.DrawArc(ringPen, 16, 16, 32, 32, -60, 300);

            // Güç dikey çizgisi
            using var linePen = new Pen(Color.FromArgb(239, 68, 68), 4f);
            linePen.StartCap = LineCap.Round;
            linePen.EndCap = LineCap.Round;
            g.DrawLine(linePen, 32, 10, 32, 26);
        }

        IntPtr hIcon = bmp.GetHicon();
        var icon = Icon.FromHandle(hIcon);
        _cachedIcon = (Icon)icon.Clone();

        if (!string.IsNullOrEmpty(savePath))
        {
            try
            {
                using var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write);
                _cachedIcon.Save(fs);
            }
            catch
            {
                // İsteğe bağlı dosya kaydetme hatası yok sayılabilir
            }
        }

        return _cachedIcon;
    }
}

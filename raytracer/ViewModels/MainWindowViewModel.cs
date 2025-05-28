using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace raytracer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public event Action? Refresh;

    public IImage MainImage { get => m_MainImage; }

    public int Width { get { return m_width; } }

    public int Height { get { return m_height; } }

    private readonly WriteableBitmap m_MainImage;

    private readonly int m_width = 800;
    private readonly int m_height = 800;

    public MainWindowViewModel()
    {
        m_MainImage = new WriteableBitmap(new PixelSize(m_width, m_height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Opaque);
        //Task.Run(_updateMyBitmapColorPeriodicallyAsync);
        Scene scene = Scene.Load("scene.json");
        Raytracer r = new(scene);
        r.Start(async (x, y, c) =>
        {
            SetPixel(x, y, c);
            if (Refresh != null)
            {
                await Dispatcher.UIThread.InvokeAsync(Refresh);
            }
        }, new Size(m_width, m_height), () => { }, () => { });
    }

    private Color[] CreateColors()
    {
        int size = (int)Math.Floor(m_width / 3.0);
        List<Color> colors = [];
        for (int i = 0; i < size; i++)
        {
            colors.Add(Colors.Red);
        }
        for (int i = 0; i < size; i++)
        {
            colors.Add(Colors.Green);
        }
        for (int i = 0; i < size; i++)
        {
            colors.Add(Colors.Blue);
        }

        return [.. colors];
    }

    private async Task _updateMyBitmapColorPeriodicallyAsync()
    {
        PeriodicTimer timer = new(TimeSpan.FromMilliseconds(10));

        int position = 0;
        Color[] colors = CreateColors();
        while (await timer.WaitForNextTickAsync())
        {
            var c = colors[position % colors.Length];
            SetPixel(position, c);
            position++;
            if (position > m_width * m_height)
            {
                position = 0;
            }
            if (Refresh != null)
            {
                await Dispatcher.UIThread.InvokeAsync(Refresh);
            }
        }
    }

    public static int ConvertColor(Color color)
    {
        var col = 0;

        if (color.A != 0)
        {
            var a = color.A + 1;
            col = (color.A << 24)
                  | ((byte)((color.R * a) >> 8) << 16)
                  | ((byte)((color.G * a) >> 8) << 8)
                  | (byte)((color.B * a) >> 8);
        }

        return col;
    }

    public void SetPixel(int x, int y, Color color)
    {
        SetPixel(y * m_height + x, color);
    }

    public unsafe void SetPixel(int position, Color color)
    {
        using ILockedFramebuffer frameBuffer = m_MainImage.Lock();
        void* dataStart = (void*)frameBuffer.Address;
        Span<byte> buffer = new(dataStart, m_width * m_height * 4);
        var pixel = position * 4;
        buffer[pixel + 3] = 255;
        buffer[pixel + 2] = color.R;
        buffer[pixel + 1] = color.G;
        buffer[pixel + 0] = color.B;
    }

}

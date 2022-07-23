namespace RAGENativeUI.Elements;

using System;
using System.Drawing;

using Rage;

public class UIMenuColorPickerPanel : UIMenuPanel
{
    public delegate void ColorChangedEvent(UIMenuColorPickerPanel sender);

    private float hue = 0.0f; // 0.0-360.0
    private float saturation = 0.0f; // 0.0-1.0
    private float value = 0.0f; // 0.0-1.0
    private RectangleF saturationValueGradientBounds;
    private RectangleF hueGradientBounds;

    private enum MouseState { Released, PressedInSaturationValueGradient, PressedInHueGradient }
    private MouseState mouseState = MouseState.Released;

    public override float Height => 0.034722f * 7 + (0.034722f * 0.25f);
    //public Color CrosshairColor { get; set; } = Color.FromArgb(255, 0, 0, 0);

    /// <summary>
    /// Gets or sets the hue component from the HSV representation of the color selected by the user.
    /// <para>
    /// Valid values are 0.0 through 360.0. Values outside this range are clamped.
    /// </para>
    /// </summary>
    public float Hue
    {
        get => hue;
        set => SetAndRaiseColorChanged(ref hue, MathHelper.Clamp(value, 0.0f, 360.0f));
    }
    /// <summary>
    /// Gets or sets the saturation component from the HSV representation of the color selected by the user.
    /// <para>
    /// Valid values are 0.0 through 1.0. Values outside this range are clamped.
    /// </para>
    /// </summary>
    public float Saturation
    {
        get => saturation;
        set => SetAndRaiseColorChanged(ref saturation, MathHelper.Clamp(value, 0.0f, 1.0f));
    }
    /// <summary>
    /// Gets or sets the value component from the HSV representation of the color selected by the user.
    /// <para>
    /// Valid values are 0.0 through 1.0. Values outside this range are clamped.
    /// </para>
    /// </summary>
    public float Value
    {
        get => value;
        set => SetAndRaiseColorChanged(ref this.value, MathHelper.Clamp(value, 0.0f, 1.0f));
    }
    /// <summary>
    /// Gets or sets the color selected by the user.
    /// </summary>
    public Color Color
    {
        get => ColorFromHSV(Hue, Saturation, Value);
        set
        {
            var (h, s, v) = ColorToHSV(value);
            var changed = h != hue || s != saturation || v != this.value;
            (hue, saturation, this.value) = (h, s, v); // NOTE: not using setters to avoid triggering the ColorChanged event multiple times
            if (changed)
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Occurs when the selected color changes.
    /// </summary>
    public event ColorChangedEvent ColorChanged;

    public UIMenuColorPickerPanel()
    {
        InstructionalButtons.Add(new InstructionalButtonDynamic("Value", InstructionalKey.MouseAxisY, InstructionalKey.ControllerAxisRY));
        InstructionalButtons.Add(new InstructionalButtonDynamic("Saturation", InstructionalKey.MouseAxisX, InstructionalKey.ControllerAxisRX));
        InstructionalButtons.Add(new InstructionalButtonDynamic("Hue",
                                                                new InstructionalButtonId[] { InstructionalKey.MouseAxisY },
                                                                new InstructionalButtonId[] { InstructionalKey.ControllerRShoulder, InstructionalKey.SymbolPlus, InstructionalKey.ControllerAxisRY }));
    }

    public override bool ProcessControl()
    {
#if DEBUG
        Game.DisplaySubtitle($"H: {Hue}~n~S: {Saturation}~n~V: {Value}~n~RGB: {Color}");
        var c = Color;
        N.DrawRect(0.5f, 0.8f, 0.05f, 0.05f * N.GetAspectRatio(false), c.R, c.G, c.B, c.A);
#endif

        if (UIMenu.IsUsingController)
        {
            N.SetInputExclusive(2, GameControl.ScriptRightAxisX);
            N.SetInputExclusive(2, GameControl.ScriptRightAxisY);
            N.SetInputExclusive(2, GameControl.ScriptRB);
            var controlX = N.GetControlNormal(2, GameControl.ScriptRightAxisX);
            var controlY = N.GetControlNormal(2, GameControl.ScriptRightAxisY);
            var controlHue = Game.IsControlPressed(2, GameControl.ScriptRB);
            var frameTime = Game.FrameTime;
            if (controlHue)
            {
                Hue += controlY * frameTime * 360.0f;
            }
            else
            {
                Saturation += controlX * frameTime;
                Value -= controlY * frameTime;
            }
            return controlX != 0.0f || controlY != 0.0f;
        }

        return false;
    }

    public override bool ProcessMouse(float mouseX, float mouseY)
    {
        const int CursorFinger = 5;

        if (mouseState is MouseState.Released)
        {
            bool inSaturationValueGradientBounds = saturationValueGradientBounds.Contains(mouseX, mouseY);
            bool inHueGradientBounds = hueGradientBounds.Contains(mouseX, mouseY);
            if (inSaturationValueGradientBounds || inHueGradientBounds)
            {
                N.SetMouseCursorSprite(CursorFinger);
                if (Game.IsControlJustPressed(2, GameControl.CursorAccept))
                {
                    mouseState = inHueGradientBounds ? MouseState.PressedInHueGradient : MouseState.PressedInSaturationValueGradient;
                    return true;
                }
            }
        }
        else
        {
            N.SetMouseCursorSprite(CursorFinger);
            if (N.IsControlReleased(2, GameControl.CursorAccept))
            {
                mouseState = MouseState.Released;
            }
            else
            {
                switch (mouseState)
                {
                    case MouseState.PressedInSaturationValueGradient:
                        Saturation = (mouseX - saturationValueGradientBounds.X) / saturationValueGradientBounds.Width;
                        Value = 1.0f - (mouseY - saturationValueGradientBounds.Y) / saturationValueGradientBounds.Height;
                        break;

                    case MouseState.PressedInHueGradient:
                        Hue = 360.0f * (mouseY - hueGradientBounds.Y) / hueGradientBounds.Height;
                        break;
                }
            }

            return true;
        }

        return base.ProcessMouse(mouseX, mouseY);
    }

    protected override void DrawContents(float x, float y, float menuWidth)
    {
        var aspectRatio = N.GetAspectRatio(false);

        float svX = x + menuWidth * 0.4f;
        float svY = y + Height * 0.5f;
        float svWidth = Height * 0.5f;
        float svHeight = svWidth * aspectRatio;
        DrawSaturationValueGradient(svX, svY, svWidth, svHeight);

        float hX = x + menuWidth * 0.8f;
        float hY = y + Height * 0.5f;
        float hWidth = svWidth * 0.125f;
        float hHeight = svHeight;
        DrawHueGradient(hX, hY, hWidth, hHeight);
    }

    protected void DrawSaturationValueGradient(float x, float y, float width, float height)
    {
        var bgColor = ColorFromHSV(Hue, saturation: 1.0f, value: 1.0f);
        N.DrawRect(
            x, y,
            width, height,
            bgColor.R, bgColor.G, bgColor.B, bgColor.A);
        N.DrawSprite(
            Txd, TextureSaturationValueGradient,
            x, y,
            width, height,
            0.0f,
            255, 255, 255, 255);

        float crosshairX = x - (width * 0.5f) + (width * Saturation);
        float crosshairY = y - (height * 0.5f) + (height * (1.0f - Value));
        DrawSaturationValueCrosshair(crosshairX, crosshairY, width, height);

        // calculate gradient bounds
        saturationValueGradientBounds = Common.GetScriptGfxRect(new RectangleF(x - width * 0.5f, y - height * 0.5f, width, height));
    }

    protected void DrawSaturationValueCrosshair(float x, float y, float boxWidth, float boxHeight)
    {
        var aspectRatio = N.GetAspectRatio(false);
        var crosshairWidth = boxWidth * 0.075f;
        var crosshairHeight = crosshairWidth * aspectRatio;
        N.DrawSprite(
            Txd, TextureCrosshair,
            x, y,
            crosshairWidth, crosshairHeight,
            0.0f,
            255, 255, 255, 255);

        //const float ScaleX = 1.0f / 85.0f;
        //const float ScaleY = 1.0f / 10.0f;

        //var color = CrosshairColor;
        //var aspectRatio = N.GetAspectRatio(false);
        //var verticalBarWidth = boxWidth * ScaleX;
        //var verticalBarHeight = boxHeight * ScaleY;
        //N.DrawRect(
        //    x, y,
        //    verticalBarWidth, verticalBarHeight,
        //    color.R, color.G, color.B, color.A);
        //var horizontalBarWidth = boxWidth * ScaleY;
        //var horizontalBarHeight = boxHeight * ScaleX;
        //N.DrawRect(
        //    x, y,
        //    horizontalBarWidth, horizontalBarHeight,
        //    color.R, color.G, color.B, color.A);
    }

    protected void DrawHueGradient(float x, float y, float width, float height)
    {
        N.DrawSprite(
            Txd, TextureHueGradient,
            x, y,
            width, height,
            0.0f,
            255, 255, 255, 255);

        float crosshairX = x - (width * 0.5f) + (width * 0.5f);
        float crosshairY = y - (height * 0.5f) + (height * Hue / 360.0f);
        DrawHueCrosshair(crosshairX, crosshairY, width, height);

        // calculate gradient bounds
        hueGradientBounds = Common.GetScriptGfxRect(new RectangleF(x - width * 0.5f, y - height * 0.5f, width, height));
    }

    protected void DrawHueCrosshair(float x, float y, float boxWidth, float boxHeight)
    {
        var crosshairWidth = boxWidth;
        var crosshairHeight = boxHeight * 0.075f;
        N.DrawSprite(
            Txd, TextureCrosshair,
            x, y,
            crosshairWidth, crosshairHeight,
            0.0f,
            255, 255, 255, 255);

        //var color = CrosshairColor;
        //var aspectRatio = N.GetAspectRatio(false);
        //float dotWidth = 0.005f;
        //float dotHeight = dotWidth * aspectRatio;
        //N.DrawRect(
        //    x, y,
        //    dotWidth, dotHeight,
        //    color.R, color.G, color.B, color.A);
    }

    private void SetAndRaiseColorChanged(ref float field, float newValue)
    {
        if (field != newValue)
        {
            field = newValue;
            OnColorChanged();
        }
    }

    protected virtual void OnColorChanged()
        => ColorChanged?.Invoke(this);

    private static Color ColorFromHSV(float hue, float saturation, float value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        float f = (float)(hue / 60 - Math.Floor(hue / 60));

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }

    private static (float H, float S, float V) ColorToHSV(Color color)
    {
        var r = color.R / 255.0f;
        var g = color.G / 255.0f;
        var b = color.B / 255.0f;
        var cmax = Math.Max(Math.Max(r, g), b);
        var cmin = Math.Min(Math.Min(r, g), b);
        var d = cmax - cmin;

        var hue = 0.0f;
        if (d != 0.0f)
        {
            if (cmax == r)      hue = 60.0f * (((g - b) / d) % 6);
            else if (cmax == g) hue = 60.0f * (((b - r) / d) + 2);
            else if (cmax == b) hue = 60.0f * (((r - g) / d) + 4);
        }
        var saturation = cmax != 0.0f ? (d / cmax) : 0.0f;
        var value = cmax;

        return (hue, saturation, value);
    }

    private const string Txd = "INTERNAL_RNUI_UIMenuColorPickerPanel";
    private const string TextureHueGradient = "hue_gradient";
    private const string TextureSaturationValueGradient = "saturation_value_gradient";
    private const string TextureCrosshair = "crosshair";

    private static readonly RuntimeTextureDictionary runtimeTxd;

    static UIMenuColorPickerPanel()
    {
        if (RuntimeTextureDictionary.GetOrCreate(Txd, out runtimeTxd))
        {
            // textures from https://github.com/duckduckgo/zeroclickinfo-goodies/tree/5521881a99e0e912e076cfc50016bfb81a0bcc17/share/goodie/color_picker
            if (!runtimeTxd.HasTexture(TextureHueGradient))
            {
                runtimeTxd.AddTextureFromDDS(TextureHueGradient, UIMenuColorPickerPanel_Resources.hue_gradient);
            }

            if (!runtimeTxd.HasTexture(TextureSaturationValueGradient))
            {
                runtimeTxd.AddTextureFromDDS(TextureSaturationValueGradient, UIMenuColorPickerPanel_Resources.saturation_value_gradient);
            }

            if (!runtimeTxd.HasTexture(TextureCrosshair))
            {
                runtimeTxd.AddTextureFromDDS(TextureCrosshair, UIMenuColorPickerPanel_Resources.crosshair);
            }
        }
    }
}

namespace RAGENativeUI.Elements;

using System;
using System.Drawing;

using Rage;

public class UIMenuColorPickerPanel : UIMenuPanel
{
    private float hue = 0.0f; // 0.0-360.0
    private float saturation = 0.0f; // 0.0-1.0
    private float value = 0.0f; // 0.0-1.0
    private RectangleF saturationValueGradientBounds;
    private RectangleF hueGradientBounds;

    private enum MouseState { Released, PressedInSaturationValueGradient, PressedInHueGradient }
    private MouseState mouseState = MouseState.Released;

    public override float Height => 0.034722f * 7 + (0.034722f * 0.25f);
    public Color DotColor { get; set; } = Color.FromArgb(255, 255, 255, 255);

    public float Hue
    {
        get => hue;
        set => hue = MathHelper.Clamp(value, 0.0f, 360.0f);
    }
    public float Saturation
    {
        get => saturation;
        set => saturation = MathHelper.Clamp(value, 0.0f, 1.0f);
    }
    public float Value
    {
        get => value;
        set => this.value = MathHelper.Clamp(value, 0.0f, 1.0f);
    }
    public Color Color
    {
        get => ColorFromHSV(Hue, Saturation, Value);
        set => (Hue, Saturation, Value) = ColorToHSV(value);
    }

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
        Game.DisplaySubtitle($"H: {Hue}~n~S: {Saturation}~n~V: {Value}~n~RGB: {Color}");
        var c = Color;
        N.DrawRect(0.5f, 0.8f, 0.05f, 0.05f * N.GetAspectRatio(false), c.R, c.G, c.B, c.A);

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
        DrawSaturationValueGradient(x, y, menuWidth);
        DrawHueGradient(x, y, menuWidth);
    }

    private void DrawSaturationValueGradient(float x, float y, float menuWidth)
    {
        var aspectRatio = N.GetAspectRatio(false);
        var color = ColorFromHSV(Hue, saturation: 1.0f, value: 1.0f);
        float gradientX = x + menuWidth * 0.4f;
        float gradientY = y + Height * 0.5f;
        float gradientWidth = Height * 0.7f;
        float gradientHeight = gradientWidth * aspectRatio;
        N.DrawRect(
            gradientX, gradientY,
            gradientWidth, gradientHeight,
            color.R, color.G, color.B, color.A);
        N.DrawSprite(
            Txd, TextureSaturationValueGradient,
            gradientX, gradientY,
            gradientWidth, gradientHeight,
            0.0f,
            255, 255, 255, 255);

        color = DotColor;
        float dotX = gradientX - (gradientWidth * 0.5f) + (gradientWidth * Saturation);
        float dotY = gradientY - (gradientHeight * 0.5f) + (gradientHeight * (1.0f - Value));
        float dotWidth = 0.005f;
        float dotHeight = dotWidth * aspectRatio;
        N.DrawRect(
            dotX, dotY,
            dotWidth, dotHeight,
            color.R, color.G, color.B, color.A);

        // calculate gradient bounds
        saturationValueGradientBounds = Common.GetScriptGfxRect(new RectangleF(gradientX - gradientWidth * 0.5f, gradientY - gradientHeight * 0.5f, gradientWidth, gradientHeight));
    }

    private void DrawHueGradient(float x, float y, float menuWidth)
    {
        var aspectRatio = N.GetAspectRatio(false);
        float gradientX = x + menuWidth * 0.8f;
        float gradientY = y + Height * 0.5f;
        float gradientWidth = Height * 0.1f;
        float gradientHeight = Height * 0.7f * aspectRatio;
        N.DrawSprite(
            Txd, TextureHueGradient,
            gradientX, gradientY,
            gradientWidth, gradientHeight,
            0.0f,
            255, 255, 255, 255);

        var color = DotColor;
        float dotX = gradientX - (gradientWidth * 0.5f) + (gradientWidth * 0.5f);
        float dotY = gradientY - (gradientHeight * 0.5f) + (gradientHeight * Hue / 360.0f);
        float dotWidth = 0.005f;
        float dotHeight = dotWidth * aspectRatio;
        N.DrawRect(
            dotX, dotY,
            dotWidth, dotHeight,
            color.R, color.G, color.B, color.A);

        // calculate gradient bounds
        hueGradientBounds = Common.GetScriptGfxRect(new RectangleF(gradientX - gradientWidth * 0.5f, gradientY - gradientHeight * 0.5f, gradientWidth, gradientHeight));
    }

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

    private static readonly RuntimeTextureDictionary runtimeTxd;

    static UIMenuColorPickerPanel()
    {
        if (RuntimeTextureDictionary.TryRegister(Txd, out runtimeTxd))
        {
            // textures from https://github.com/duckduckgo/zeroclickinfo-goodies/tree/5521881a99e0e912e076cfc50016bfb81a0bcc17/share/goodie/color_picker
            runtimeTxd.AddTextureFromDDS(TextureHueGradient, UIMenuColorPickerPanel_Resources.hue_gradient);
            runtimeTxd.AddTextureFromDDS(TextureSaturationValueGradient, UIMenuColorPickerPanel_Resources.saturation_value_gradient);
        }
        else
        {
            RuntimeTextureDictionary.TryAcquire(Txd, out runtimeTxd);
        }
    }
}

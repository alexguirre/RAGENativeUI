namespace RAGENativeUI
{
    using static Rage.Native.NativeFunction;
    using Rage;
    using RAGENativeUI.Internals;
    using System;

    internal static class N
    {
        public static void GetHudColour(int colorId, out int r, out int g, out int b, out int a) => Natives.x7C9C91AB74A0360F(colorId, out r, out g, out b, out a);
        public static void ReplaceHudColourWithRgba(int colorId, int r, int g, int b, int a) => Natives.xF314CF4F0211894E(colorId, r, g, b, a);

        public static void DrawSprite(string textureDict, string textureName, float x, float y, float width, float height, float rotation, int r, int g, int b, int a)
        {
            if (DirectNatives.DrawSpriteAvailable)
            {
                DirectNatives.DrawSprite(textureDict, textureName, x, y, width, height, rotation, r, g, b, a, false);
            }
            else
            {
                Natives.xE7FFAE5EBF23D890(textureDict, textureName, x, y, width, height, rotation, r, g, b, a, false);
            }
        }

        public static void DrawRect(float x, float y, float width, float height, int r, int g, int b, int a)
        {
            if (DirectNatives.DrawRectAvailable)
            {
                DirectNatives.DrawRect(x, y, width, height, r, g, b, a, false);
            }
            else
            {
                Natives.x3A618A217E5154F0(x, y, width, height, r, g, b, a, false);
            }
        }

        public static void SetTextFont(int font) => Natives.x66E0276CC5F6B9DA(font);
        public static void SetTextScale(float scale, float size) => Natives.x07C837F9A01C34C9(scale, size);
        public static void SetTextColour(int r, int g, int b, int a) => Natives.xBE6B23FFA53FB442(r, g, b, a);
        public static void SetTextDropShadow() => Natives.x1CA3E9EAC9D93E5E();
        public static void SetTextDropshadow(int distance, int r, int g, int b, int a) => Natives.x465C84BC39F1C351(distance, r, g, b, a);
        public static void SetTextOutline() => Natives.x2513DFB0FB8400FE();
        public static void SetTextCentre(bool align) => Natives.xC02F4DBFB51D988B(align);
        public static void SetTextEdge(int p0, int r, int g, int b, int a) => Natives.x441603240D202FA6(p0, r, g, b, a);
        public static void SetTextLeading(int p0) => Natives.xA50ABC31E3CDFAFF(p0);
        public static void SetTextRightJustify(bool flag) => Natives.x6B3C4650BC8BEE47(flag);
        /// <param name="type">
        /// <para>0: Center-Justify</para>
        /// <para>1: Left-Justify</para>
        /// <para>2: Right-Justify, requires SET_TEXT_WRAP, otherwise it will draw to the far right of the screen.</para>
        /// </param>
        public static void SetTextJustification(int type) => Natives.x4E096588B13FFECA(type);
        /// <summary>
        /// It sets the text in a specified box and wraps the text if it exceeds the boundaries.
        /// Both values are for X axis. Useful when positioning text set to center or aligned to the right.  
        /// </summary>
        /// <param name="start">Left boundary on screen position (0.0 - 1.0).</param>
        /// <param name="end">Right boundary on screen position (0.0 - 1.0).</param>
        public static void SetTextWrap(float start, float end) => Natives.x63145D9C883A1A70(start, end);
        public static float GetTextHeight(float scale, int font) => Natives.xDB88A37483346780<float>(scale, font);
        public static void BeginTextCommandDisplayText(string format) => Natives.x25FBB336DF1804CB(format);
        public static void EndTextCommandDisplayText(float x, float y) => Natives.xCD015E5BB0D96A57(x, y);
        public static float GetTextScaleHeight(float scale, int font) => Natives.xDB88A37483346780<float>(scale, font);
        public static void BeginTextCommandGetWidth(string format) => Natives.x54CE8AC98E120CAB(format);
        public static float EndTextCommandGetWidth(bool p0) => Natives.x85F061DA64ED2F67<float>(p0);
        public static void BeginTextCommandGetLineCount(string format) => Natives.x521FB041D93DD0E4(format);
        public static int EndTextCommandGetLineCount(float x, float y) => Natives.x9040DFB09BE75706<int>(x, y);
        public static void BeginTextCommandPrint(string format) => Natives.xB87A37EEB7FAA67D(format);
        public static void EndTextCommandPrint(int duration, bool drawImmediately) => Natives.x9D77056A530643F6(duration, drawImmediately);
        public static void BeginTextCommandScaleformString(string format) => Natives.x80338406F3475E55(format);
        public static void EndTextCommandScaleformString() => Natives.x362E2D3FE93A9959();
        public static void AddTextComponentFloat(float value, int decimalPlaces) => Natives.xE7DCB5B874BCD96E(value, decimalPlaces);
        public static void AddTextComponentInteger(int value) => Natives.x03B504CF259931BC(value);
        public static void AddTextComponentFormattedInteger(int value, bool commaSeparated) => Natives.x0E4C749FF9DE9CC4(value, commaSeparated);
        public static void AddTextComponentSubstringBlipName(uint blip) => Natives.x80EAD8E2E1D5D52E(blip);
        public static void AddTextComponentSubstringPlayerName(string text) => Natives.x6C188BE134E074AA(text);
        public static void AddTextComponentSubstringPlayerName(System.IntPtr text) => Natives.x6C188BE134E074AA(text);
        public static void AddTextComponentSubstringTextLabel(string labelName) => Natives.xC63CD5D2920ACBE7(labelName);
        public static void AddTextComponentSubstringTextLabelHashKey(uint labelHash) => Natives.x17299B63C7683A2B(labelHash);
        public static void AddTextComponentSubstringTime(int timestamp, int flags) => Natives.x1115F16B8AB9E8BF(timestamp, flags);

        // Same as GET_SCREEN_RESOLUTION (0x888D57E407E63624) 
        public static void GetScreenResolution(out int x, out int y) { x = 1280; y = 720; }
        public static void GetActiveScreenResolution(out int x, out int y) => Natives.x873C9F3104101DD3(out x, out y);
        public static float GetAspectRatio(bool b) => Natives.xF1307EF624A80D87<float>(b);
        public static bool GetIsWidescreen() => Natives.x30CF4BDA4FCB1905<bool>();

        /// <summary>
        /// This function anchors script draws to a side of the safe zone. This needs to be called to make the interface independent of the player's safe zone configuration.
        /// </summary>
        /// <param name="alignX">
        /// The horizontal alignment. This can be 67 ('C'), 76 ('L'), or 82 ('R').
        /// <para>
        /// C (67) - Center: DRAW_TEXT starts in the middle of the screen, while DRAW_RECT starts on the right; both move with the right side of the screen.
        /// </para>
        /// <para>
        /// L (76) - Left: Anchors to the left side, DRAW_RECT starts on the left side of the screen, same as DRAW_TEXT when centered.
        /// </para>
        /// <para>
        /// R (82) - Right: DRAW_TEXT starts on the left side(normal 0,0), while DRAW_RECT starts some short distance away from the right side of the screen, both move with the right side of the screen.
        /// </para>
        /// </param>
        /// <param name="alignY">
        /// The vertical alignment.This can be 67 ('C'), 66 ('B'), or 84 ('T').
        /// <para>
        /// B (66) - Bottom: DRAW_RECT starts about as far as the middle of the map from the bottom, while DRAW_TEXT is about rather centered.
        /// </para>
        /// <para>
        /// C (67) - Center: It starts at a certain distance from the bottom, but the distance is fixed, the distance is different from 66.
        /// </para>
        /// <para>
        /// T (84) - Top: Anchors to the top, DRAW_RECT starts on the top of the screen, DRAW_TEXT just below it.
        /// </para>
        /// </param>
        /// <remarks>
        /// Using any other value (including 0) will result in the safe zone not being taken into account for this draw. The canonical value for this is 'I' (73).
        /// </remarks>
        public static void SetScriptGfxAlign(char alignX, char alignY) => Natives.xB8A850F20A067EB6((int)alignX, (int)alignY);
        /// <summary>
        /// Sets the draw offset/calculated size for SET_SCRIPT_GFX_ALIGN. 
        /// If using any alignment other than left/top, the game expects the width/height to be configured 
        /// using this native in order to get a proper starting position for the draw command.
        /// </summary>
        /// <param name="x">The X offset for the item to be drawn.</param>
        /// <param name="y">The Y offset for the item to be drawn.</param>
        /// <param name="w">The width of the item to be drawn.</param>
        /// <param name="h">The height of the item to be drawn.</param>
        public static void SetScriptGfxAlignParams(float x, float y, float w, float h) => Natives.xF5A2C681787E579D(x, y, w, h);
        public static void ResetScriptGfxAlign() => Natives.xE3A3DB414A373DAB();
        public static void SetScriptGfxDrawBehindPausemenu(bool flag) => Natives.xC6372ECD45D73BCD(flag);
        public static void SetScriptGfxDrawOrder(int order) => Natives.x61BB1D9B3A95D802(order);
        public static void GetScriptGfxPosition(float x, float y, out float newX, out float newY) => Natives.x6DD8F5AA635EB4B2(x, y, out newX, out newY);

        public static Vector3 GetTextureResolution(string textureDict, string textureName) => Natives.x35736EE65BD00C11<Vector3>(textureDict, textureName);

        public static bool HasStreamedTextureDictLoaded(string name) => Natives.HasStreamedTextureDictLoaded<bool>(name);
        public static void RequestStreamedTextureDict(string name) => Natives.RequestStreamedTextureDict(name, 0);

        public static void HideHudComponentThisFrame(int id) => Natives.x6806C51AD12B83B8(id);

        public static bool BusySpinnerIsOn() => Natives.xD422FCC5F239A915<bool>();

        public static void EnableControlAction(int index, GameControl control) => Natives.EnableControlAction(index, (int)control, true);
        public static void DisableControlAction(int index, GameControl control)
        {
            if (DirectNatives.DisableControlActionAvailable)
            {
                DirectNatives.DisableControlAction(index, (int)control, true);
            }
            else
            {
                Natives.DisableControlAction(index, (int)control, true);
            }
        }
        public static void EnableAllControlActions(int index) => Natives.EnableAllControlActions(index);
        public static void DisableAllControlActions(int index) => Natives.DisableAllControlActions(index);
        public static float GetControlNormal(int index, GameControl control) => Natives.GetControlNormal<float>(index, (int)control);
        public static void SetInputExclusive(int index, GameControl control) => Natives.xEDE476E5EE29EDB1(index, (int)control);
        public static string GetControlInstructionalButton(int index, GameControl control) => Natives.x0499D7B09FC9B407<string>(index, (int)control, true);

        public static void SetMouseCursorActiveThisFrame() => Natives.xAAE7CE1D63167423();
        public static void SetMouseCursorSprite(int spriteId) => Natives.x8DB8CFFD58B62552(spriteId);
        public static void SetMouseCursorLocation(float x, float y) => Natives.xFC695459D4D0E219(x, y);

        public static float GetGameplayCamRelativeHeading() => Natives.GetGameplayCamRelativeHeading<float>();
        public static void SetGameplayCamRelativeHeading(float heading) => Natives.SetGameplayCamRelativeHeading(heading);

        public static bool IsInputDisabled(int index) => Natives.xA571D46727E2B718<bool>(index);
        public static bool HasInputJustChanged(int index) => Natives.x6CD79468A1E595C6<bool>(index);

        /// <summary>
        /// If true, in multiple-screens setups, limits the range of GameControl.CursorX to only the main screen.
        /// </summary>
        public static void x5B73C77D9EB66E24(bool value) => Natives.x5B73C77D9EB66E24(value);

        public static int GetNumberOfReferencesOfScript(uint nameHash) => Natives.x2C83A9DA6BFFC4F9<int>(nameHash);

        public static bool HasScaleformMovieLoaded(int handle) => Natives.x85F01B8D5B90570E<bool>(handle);
        public static int RequestScaleformMovie(string name) => Natives.x11FE353CF9733E6F<int>(name);
        public static int EndScaleformMovieMethodReturnValue() => Natives.xC50AA39A577AF886<int>();
        public static bool IsScaleformMovieMethodReturnValueReady(int returnValueId) => Natives.x768FF8961BA904D6<bool>(returnValueId);
        public static int GetScaleformMovieMethodReturnValueInt(int returnValueId) => Natives.x2DE7EFA66B906036<int>(returnValueId);

        public static void SetPlayerControl(Player player, bool hasControl, uint flags) => Natives.x8D32347D6D4C40A2(player, hasControl, flags);

        public static void AnimPostFxPlay(string effectName, int duration, bool looped) => Natives.x2206bf9a37b7f724(effectName, duration, looped);
        public static void AnimPostFxStop(string effectName) => Natives.x068E835A1D0DC0E3(effectName);

        public static bool IsControlReleased(int index, GameControl control) => Natives.x648EE3E7F38877DD<bool>(index, (int)control);

        public static bool DoesTextLabelExist(string str) => Natives.xAC09CA973C564252<bool>(str);
        public static IntPtr GetLabelText(string str) => Natives.x7B5280EBA9840C72<IntPtr>(str);
        public static int GetSystemLanguage() => Natives.x497420E022796B3F<int>();
        public static int GetCurrentLanguage() => Natives.x2BDD44CC428A7EAE<int>();
        public static bool ShouldUseMetricMeasurements() => Natives.xD3D15555431AB793<bool>();
    }
}


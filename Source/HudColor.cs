namespace RAGENativeUI
{
    using System.Drawing;

    using Rage.Native;

    /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColorExtensions/Doc/*' />
    public static class HudColorExtensions
    {
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColorExtensions/Member[@name="GetColor"]/*' />
        public static Color GetColor(this HudColor hudColor)
        {
            NativeFunction.Natives.GetHudColour((int)hudColor, out int r, out int g, out int b, out int a);
            return Color.FromArgb(a, r, g, b);
        }

        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColorExtensions/Member[@name="SetColor"]/*' />
        public static void SetColor(this HudColor hudColor, Color color)
        {
            NativeFunction.Natives.xF314CF4F0211894E((int)hudColor, color.R, color.G, color.B, color.A); // _SET_HUD_COLOUR
        }

        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColorExtensions/Member[@name="GetName"]/*' />
        public static string GetName(this HudColor hudColor)
        {
            int i = (int)hudColor;
            Throw.IfOutOfRange(i, 0, KnownNames.HudColors.Array.Length - 1, nameof(hudColor));

            return KnownNames.HudColors.Array[i];
        }
    }

    /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Doc/*' />
    public enum HudColor
    {
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PureWhite"]/*' />
        PureWhite = 0,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="White"]/*' />
        White = 1,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Black"]/*' />
        Black = 2,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Grey"]/*' />
        Grey = 3,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GreyLight"]/*' />
        GreyLight = 4,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GreyDark"]/*' />
        GreyDark = 5,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Red"]/*' />
        Red = 6,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="RedLight"]/*' />
        RedLight = 7,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="RedDark"]/*' />
        RedDark = 8,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Blue"]/*' />
        Blue = 9,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="BlueLight"]/*' />
        BlueLight = 10,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="BlueDark"]/*' />
        BlueDark = 11,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Yellow"]/*' />
        Yellow = 12,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="YellowLight"]/*' />
        YellowLight = 13,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="YellowDark"]/*' />
        YellowDark = 14,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Orange"]/*' />
        Orange = 15,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="OrangeLight"]/*' />
        OrangeLight = 16,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="OrangeDark"]/*' />
        OrangeDark = 17,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Green"]/*' />
        Green = 18,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GreenLight"]/*' />
        GreenLight = 19,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GreenDark"]/*' />
        GreenDark = 20,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Purple"]/*' />
        Purple = 21,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PurpleLight"]/*' />
        PurpleLight = 22,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PurpleDark"]/*' />
        PurpleDark = 23,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Pink"]/*' />
        Pink = 24,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="RadarHealth"]/*' />
        RadarHealth = 25,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="RadarArmour"]/*' />
        RadarArmour = 26,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="RadarDamage"]/*' />
        RadarDamage = 27,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer1"]/*' />
        NetPlayer1 = 28,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer2"]/*' />
        NetPlayer2 = 29,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer3"]/*' />
        NetPlayer3 = 30,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer4"]/*' />
        NetPlayer4 = 31,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer5"]/*' />
        NetPlayer5 = 32,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer6"]/*' />
        NetPlayer6 = 33,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer7"]/*' />
        NetPlayer7 = 34,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer8"]/*' />
        NetPlayer8 = 35,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer9"]/*' />
        NetPlayer9 = 36,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer10"]/*' />
        NetPlayer10 = 37,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer11"]/*' />
        NetPlayer11 = 38,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer12"]/*' />
        NetPlayer12 = 39,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer13"]/*' />
        NetPlayer13 = 40,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer14"]/*' />
        NetPlayer14 = 41,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer15"]/*' />
        NetPlayer15 = 42,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer16"]/*' />
        NetPlayer16 = 43,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer17"]/*' />
        NetPlayer17 = 44,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer18"]/*' />
        NetPlayer18 = 45,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer19"]/*' />
        NetPlayer19 = 46,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer20"]/*' />
        NetPlayer20 = 47,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer21"]/*' />
        NetPlayer21 = 48,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer22"]/*' />
        NetPlayer22 = 49,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer23"]/*' />
        NetPlayer23 = 50,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer24"]/*' />
        NetPlayer24 = 51,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer25"]/*' />
        NetPlayer25 = 52,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer26"]/*' />
        NetPlayer26 = 53,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer27"]/*' />
        NetPlayer27 = 54,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer28"]/*' />
        NetPlayer28 = 55,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer29"]/*' />
        NetPlayer29 = 56,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer30"]/*' />
        NetPlayer30 = 57,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer31"]/*' />
        NetPlayer31 = 58,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer32"]/*' />
        NetPlayer32 = 59,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="SimpleBlipDefault"]/*' />
        SimpleBlipDefault = 60,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuBlue"]/*' />
        MenuBlue = 61,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuGreyLight"]/*' />
        MenuGreyLight = 62,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuBlueExtraDark"]/*' />
        MenuBlueExtraDark = 63,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuYellow"]/*' />
        MenuYellow = 64,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuYellowDark"]/*' />
        MenuYellowDark = 65,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuGreen"]/*' />
        MenuGreen = 66,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuGrey"]/*' />
        MenuGrey = 67,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuGreyDark"]/*' />
        MenuGreyDark = 68,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuHighlight"]/*' />
        MenuHighlight = 69,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuStandard"]/*' />
        MenuStandard = 70,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuDimmed"]/*' />
        MenuDimmed = 71,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MenuExtraDimmed"]/*' />
        MenuExtraDimmed = 72,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="BriefTitle"]/*' />
        BriefTitle = 73,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MidGreyMultiplayer"]/*' />
        MidGreyMultiplayer = 74,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer1Dark"]/*' />
        NetPlayer1Dark = 75,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer2Dark"]/*' />
        NetPlayer2Dark = 76,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer3Dark"]/*' />
        NetPlayer3Dark = 77,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer4Dark"]/*' />
        NetPlayer4Dark = 78,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer5Dark"]/*' />
        NetPlayer5Dark = 79,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer6Dark"]/*' />
        NetPlayer6Dark = 80,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer7Dark"]/*' />
        NetPlayer7Dark = 81,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer8Dark"]/*' />
        NetPlayer8Dark = 82,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer9Dark"]/*' />
        NetPlayer9Dark = 83,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer10Dark"]/*' />
        NetPlayer10Dark = 84,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer11Dark"]/*' />
        NetPlayer11Dark = 85,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer12Dark"]/*' />
        NetPlayer12Dark = 86,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer13Dark"]/*' />
        NetPlayer13Dark = 87,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer14Dark"]/*' />
        NetPlayer14Dark = 88,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer15Dark"]/*' />
        NetPlayer15Dark = 89,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer16Dark"]/*' />
        NetPlayer16Dark = 90,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer17Dark"]/*' />
        NetPlayer17Dark = 91,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer18Dark"]/*' />
        NetPlayer18Dark = 92,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer19Dark"]/*' />
        NetPlayer19Dark = 93,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer20Dark"]/*' />
        NetPlayer20Dark = 94,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer21Dark"]/*' />
        NetPlayer21Dark = 95,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer22Dark"]/*' />
        NetPlayer22Dark = 96,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer23Dark"]/*' />
        NetPlayer23Dark = 97,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer24Dark"]/*' />
        NetPlayer24Dark = 98,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer25Dark"]/*' />
        NetPlayer25Dark = 99,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer26Dark"]/*' />
        NetPlayer26Dark = 100,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer27Dark"]/*' />
        NetPlayer27Dark = 101,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer28Dark"]/*' />
        NetPlayer28Dark = 102,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer29Dark"]/*' />
        NetPlayer29Dark = 103,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer30Dark"]/*' />
        NetPlayer30Dark = 104,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer31Dark"]/*' />
        NetPlayer31Dark = 105,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NetPlayer32Dark"]/*' />
        NetPlayer32Dark = 106,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Bronze"]/*' />
        Bronze = 107,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Silver"]/*' />
        Silver = 108,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Gold"]/*' />
        Gold = 109,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Platinum"]/*' />
        Platinum = 110,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Gang1"]/*' />
        Gang1 = 111,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Gang2"]/*' />
        Gang2 = 112,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Gang3"]/*' />
        Gang3 = 113,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Gang4"]/*' />
        Gang4 = 114,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="SameCrew"]/*' />
        SameCrew = 115,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Freemode"]/*' />
        Freemode = 116,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PauseBackground"]/*' />
        PauseBackground = 117,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Friendly"]/*' />
        Friendly = 118,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Enemy"]/*' />
        Enemy = 119,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Location"]/*' />
        Location = 120,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Pickup"]/*' />
        Pickup = 121,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PauseSingleplayer"]/*' />
        PauseSingleplayer = 122,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="FreemodeDark"]/*' />
        FreemodeDark = 123,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="InactiveMission"]/*' />
        InactiveMission = 124,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Damage"]/*' />
        Damage = 125,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PinkLight"]/*' />
        PinkLight = 126,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PmMItemHighlight"]/*' />
        PmMItemHighlight = 127,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ScriptVariable"]/*' />
        ScriptVariable = 128,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Yoga"]/*' />
        Yoga = 129,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Tennis"]/*' />
        Tennis = 130,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Golf"]/*' />
        Golf = 131,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ShootingRange"]/*' />
        ShootingRange = 132,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="FlightSchool"]/*' />
        FlightSchool = 133,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NorthBlue"]/*' />
        NorthBlue = 134,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="SocialClub"]/*' />
        SocialClub = 135,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PlatformBlue"]/*' />
        PlatformBlue = 136,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PlatformGreen"]/*' />
        PlatformGreen = 137,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PlatformGrey"]/*' />
        PlatformGrey = 138,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="FacebookBlue"]/*' />
        FacebookBlue = 139,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="InGameBackground"]/*' />
        InGameBackground = 140,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Darts"]/*' />
        Darts = 141,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Waypoint"]/*' />
        Waypoint = 142,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Michael"]/*' />
        Michael = 143,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Franklin"]/*' />
        Franklin = 144,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Trevor"]/*' />
        Trevor = 145,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GolfP1"]/*' />
        GolfP1 = 146,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GolfP2"]/*' />
        GolfP2 = 147,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GolfP3"]/*' />
        GolfP3 = 148,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="GolfP4"]/*' />
        GolfP4 = 149,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="WaypointLight"]/*' />
        WaypointLight = 150,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="WaypointDark"]/*' />
        WaypointDark = 151,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PanelLight"]/*' />
        PanelLight = 152,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="MichaelDark"]/*' />
        MichaelDark = 153,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="FranklinDark"]/*' />
        FranklinDark = 154,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="TrevorDark"]/*' />
        TrevorDark = 155,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ObjectiveRoute"]/*' />
        ObjectiveRoute = 156,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PauseMapTint"]/*' />
        PauseMapTint = 157,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PauseDeselect"]/*' />
        PauseDeselect = 158,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PmWeaponsPurchasable"]/*' />
        PmWeaponsPurchasable = 159,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PmWeaponsLocked"]/*' />
        PmWeaponsLocked = 160,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="EndScreenBackground"]/*' />
        EndScreenBackground = 161,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Chop"]/*' />
        Chop = 162,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="PauseMapTintHalf"]/*' />
        PauseMapTintHalf = 163,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="NorthBlueOfficial"]/*' />
        NorthBlueOfficial = 164,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ScriptVariable2"]/*' />
        ScriptVariable2 = 165,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="H"]/*' />
        H = 166,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="HDark"]/*' />
        HDark = 167,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="T"]/*' />
        T = 168,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="TDark"]/*' />
        TDark = 169,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="HShard"]/*' />
        HShard = 170,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ControllerMichael"]/*' />
        ControllerMichael = 171,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ControllerFranklin"]/*' />
        ControllerFranklin = 172,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ControllerTrevor"]/*' />
        ControllerTrevor = 173,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="ControllerChop"]/*' />
        ControllerChop = 174,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorVideo"]/*' />
        VideoEditorVideo = 175,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorAudio"]/*' />
        VideoEditorAudio = 176,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorText"]/*' />
        VideoEditorText = 177,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="HbBlue"]/*' />
        HbBlue = 178,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="HbYellow"]/*' />
        HbYellow = 179,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorScore"]/*' />
        VideoEditorScore = 180,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorAudioFadeout"]/*' />
        VideoEditorAudioFadeout = 181,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorTextFadeout"]/*' />
        VideoEditorTextFadeout = 182,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorScoreFadeout"]/*' />
        VideoEditorScoreFadeout = 183,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="HeistBackground"]/*' />
        HeistBackground = 184,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorAmbient"]/*' />
        VideoEditorAmbient = 185,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="VideoEditorAmbientFadeout"]/*' />
        VideoEditorAmbientFadeout = 186,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Gb"]/*' />
        Gb = 187,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G"]/*' />
        G = 188,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="B"]/*' />
        B = 189,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="LowFlow"]/*' />
        LowFlow = 190,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="LowFlowDark"]/*' />
        LowFlowDark = 191,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G1"]/*' />
        G1 = 192,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G2"]/*' />
        G2 = 193,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G3"]/*' />
        G3 = 194,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G4"]/*' />
        G4 = 195,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G5"]/*' />
        G5 = 196,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G6"]/*' />
        G6 = 197,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G7"]/*' />
        G7 = 198,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G8"]/*' />
        G8 = 199,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G9"]/*' />
        G9 = 200,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G10"]/*' />
        G10 = 201,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G11"]/*' />
        G11 = 202,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G12"]/*' />
        G12 = 203,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G13"]/*' />
        G13 = 204,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G14"]/*' />
        G14 = 205,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="G15"]/*' />
        G15 = 206,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Adversary"]/*' />
        Adversary = 207,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="DegenRed"]/*' />
        DegenRed = 208,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="DegenYellow"]/*' />
        DegenYellow = 209,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="DegenGreen"]/*' />
        DegenGreen = 210,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="DegenCyan"]/*' />
        DegenCyan = 211,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="DegenBlue"]/*' />
        DegenBlue = 212,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="DegenMagenta"]/*' />
        DegenMagenta = 213,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Stunt1"]/*' />
        Stunt1 = 214,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Stunt2"]/*' />
        Stunt2 = 215,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="SpecialRaceSeries"]/*' />
        SpecialRaceSeries = 216,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="SpecialRaceSeriesDark"]/*' />
        SpecialRaceSeriesDark = 217,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="Cs"]/*' />
        Cs = 218,
        /// <include file='..\Documentation\RAGENativeUI.HudColor.xml' path='D/HudColor/Member[@name="CsDark"]/*' />
        CsDark = 219,
    }
}


namespace RNUIExamples.Showcase
{
    using Rage;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal sealed class LocalizationMenu : UIMenu
    {
        UIMenuItem language, systemLanguage;
        string inputLabelId = "";

        public LocalizationMenu() : base(Plugin.MenuTitle, "LOCALIZATION")
        {
            Plugin.Pool.Add(this);

            CreateMenuItems();

            Localization.SetText("RNUI_TEST_LABEL", "[RNUI_TEST_LABEL] Hello, World! From RAGENativeUI áéíóú äëïöü ç");
            Localization.SetText("EXIT_SURE_2", "[EXIT_SURE_2] Replaced");
            Localization.SetText("RNUI_TEST_LABEL_1", "[RNUI_TEST_LABEL_1] Hello, World! From RAGENativeUI áéíóú äëïöü ç");
            Localization.SetText("RNUI_TEST_LABEL_2", "[RNUI_TEST_LABEL_2] Hello, World! From RAGENativeUI áéíóú äëïöü ç");
            Localization.SetText("RNUI_TEST_LABEL_3", "[RNUI_TEST_LABEL_3] Hello, World! From RAGENativeUI áéíóú äëïöü ç");
            Localization.SetText("RNUI_TEST_LABEL_4", "[RNUI_TEST_LABEL_4] Hello, World! From RAGENativeUI áéíóú äëïöü ç");
            Localization.SetText("RNUI_TEST_LABEL", "[RNUI_TEST_LABEL] Overwritten");
        }

        private void CreateMenuItems()
        {
            language = new UIMenuItem("Language", $"Gets the ~b~{nameof(Localization)}.{nameof(Localization.Language)}~s~ property.");
            systemLanguage = new UIMenuItem("System Language", $"Gets the ~b~{nameof(Localization)}.{nameof(Localization.SystemLanguage)}~s~ property.");
            var resultExists = new UIMenuItem(
                "    Exists",
                $"Whether the input label ID exists, using ~b~{nameof(Localization)}.{nameof(Localization.DoesTextExist)}~s~.");
            var resultString = new UIMenuItem(
                "    Text",
                $"Value of the input label ID, using ~b~{nameof(Localization)}.{nameof(Localization.GetText)}~s~.");
            var labelId = new UIMenuItem("Label ID", $"Select to change the input label ID.")
                .WithTextEditing(() => inputLabelId,
                                 s =>
                                 {
                                     inputLabelId = s;

                                     bool exists;
                                     string str;
                                     if (s.StartsWith("0x") && uint.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out uint hash))
                                     {
                                         exists = Localization.DoesTextExist(hash);
                                         str = Localization.GetText(hash);
                                     }
                                     else
                                     {
                                         exists = Localization.DoesTextExist(inputLabelId);
                                         str = Localization.GetText(inputLabelId);
                                     }

                                     resultExists.RightLabel = exists ? "Yes" : "No";
                                     resultString.RightLabel = str.Replace("~", "\\~"); // basic token escaping
                                 });

            AddItems(language, systemLanguage, labelId, resultExists, resultString);
        }

        protected override void MenuOpenEv()
        {
            language.RightLabel = Localization.Language.ToString();
            systemLanguage.RightLabel = Localization.SystemLanguage.ToString();

            base.MenuOpenEv();
        }
    }
}
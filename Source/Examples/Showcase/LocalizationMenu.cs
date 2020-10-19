namespace RNUIExamples.Showcase
{
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal sealed class LocalizationMenu : UIMenu
    {
        UIMenuItem language, systemLanguage, prefersMetricMeasurements;
        string inputLabelId = "";

        public LocalizationMenu() : base(Plugin.MenuTitle, "LOCALIZATION")
        {
            Plugin.Pool.Add(this);

            CreateMenuItems();
        }

        private void CreateMenuItems()
        {
            language = new UIMenuItem("Language", $"Gets the ~b~{nameof(Localization)}.{nameof(Localization.Language)}~s~ property.");
            systemLanguage = new UIMenuItem("System Language", $"Gets the ~b~{nameof(Localization)}.{nameof(Localization.SystemLanguage)}~s~ property.");
            prefersMetricMeasurements = new UIMenuItem("Prefers Metric Measurements", $"Gets the ~b~{nameof(Localization)}.{nameof(Localization.PrefersMetricMeasurements)}~s~ property.");
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

            var overwriteTitle = new UIMenuCheckboxItem(
                "Overwrite Pause Menu Title",
                false,
                $"Overwrite or reset the pause menu title text label, `PM_PAUSE_HDR`, using ~b~{nameof(Localization)}.{nameof(Localization.SetText)}~s~/~b~{nameof(Localization.ClearTextOverride)}~s~.");
            overwriteTitle.CheckboxEvent += (s, isChecked) =>
            {
                if (isChecked)
                {
                    Localization.SetText("PM_PAUSE_HDR", "RAGENativeUI");
                }
                else
                {
                    Localization.ClearTextOverride("PM_PAUSE_HDR");
                }
            };

            AddItems(language, systemLanguage, prefersMetricMeasurements, labelId, resultExists, resultString, overwriteTitle);
        }

        protected override void MenuOpenEv()
        {
            language.RightLabel = Localization.Language.ToString();
            systemLanguage.RightLabel = Localization.SystemLanguage.ToString();
            prefersMetricMeasurements.RightLabel = Localization.PrefersMetricMeasurements ? "Yes" : "No";

            base.MenuOpenEv();
        }
    }
}
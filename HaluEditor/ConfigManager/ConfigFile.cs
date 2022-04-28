namespace ngprojects.HaluEditor.ConfigManager
{
    public class ConfigFile
    {
        public string AutocompleteForegroundColor { get; set; }
        public string AutocompleteForegroundReturntypeColor { get; set; }
        public string AutocompleteWindowColor { get; set; }
        public double CursorBlinkInMs { get; set; }
        public string CursorColor { get; set; }
        public double CursorWidth { get; set; }
        public string EditorColor { get; set; }
        public double FontSize { get; set; }
        public string LineNumberColor { get; set; }
        public string LineNumberTextColor { get; set; }
        public string RowHighlightColor { get; set; }
        public string RowHighlightOutlineColor { get; set; }
        public double RowHighlightOutlineWidth { get; set; }
        public string SelectionColor { get; set; }
        public string SelectionOutlineColor { get; set; }

        /// <summary>
        /// Größe des Outlines bei der Selektion
        /// </summary>
        public double SelectionOutlineWidth { get; set; }

        /// <summary>
        /// Farbe des Standardtextes
        /// </summary>
        public string TextColor { get; set; }
    }
}
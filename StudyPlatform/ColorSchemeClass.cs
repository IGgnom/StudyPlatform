using System.Drawing;
using System.Windows.Forms;

namespace StudyPlatform
{
    public static class ColorSchemeClass
    {
        //PublicSettings
        public static readonly BorderStyle TextBoxBorderStyle = BorderStyle.FixedSingle;
        public static readonly DataGridViewHeaderBorderStyle DataGridHeaderBorder = DataGridViewHeaderBorderStyle.None;
        public static readonly DataGridViewCellBorderStyle DataGridCellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        public static readonly DataGridViewTriState DataGridWrap = DataGridViewTriState.True;
        public static bool EnadleHeadersVisuals = false;
        public static bool LigthTheme = true;

        //DarkTheme
        public static readonly Color FormBackColor = Color.FromArgb(13, 20, 28);
        public static readonly Color ButtonBackColor = Color.FromArgb(184, 31, 34);
        public static readonly Color ButtonForeColor = Color.FromArgb(255, 255, 255);
        public static readonly Color ButtonMouseOver = Color.FromArgb(163, 31, 34);
        public static readonly Color ButtonMouseDown = Color.FromArgb(173, 31, 34);
        public static readonly Color LabelForeColor = Color.FromArgb(255, 255, 255);
        public static readonly Color TextBoxBackColor = Color.FromArgb(12, 16, 25);
        public static readonly Color TextBoxForeColor = Color.FromArgb(255, 255, 255);
        public static readonly Color FormButtonMouseOver = Color.FromArgb(13, 16, 20);
        public static readonly Color FormButtonMouseDown = Color.FromArgb(13, 25, 36);
        public static readonly Color DataGridSelectionColor = Color.FromArgb(23, 35, 61);

        //LightTheme
        public static readonly Color FormBackColorLight = Color.FromArgb(231, 231, 222);
        public static readonly Color ButtonBackColorLight = Color.FromArgb(74, 118, 168);
        public static readonly Color ButtonForeColorLight = Color.FromArgb(0, 0, 0);
        public static readonly Color ButtonMouseOverLight = Color.FromArgb(64, 108, 158);
        public static readonly Color ButtonMouseDownLight = Color.FromArgb(54, 98, 148);
        public static readonly Color LabelForeColorLight = Color.FromArgb(0, 0, 0);
        public static readonly Color TextBoxBackColorLight = Color.FromArgb(221, 221, 212);
        public static readonly Color TextBoxForeColorLight = Color.FromArgb(0, 0, 0);
        public static readonly Color FormButtonMouseOverLight = Color.FromArgb(221, 221, 212);
        public static readonly Color FormButtonMouseDownLight = Color.FromArgb(211, 211, 202);
        public static readonly Color DataGridSelectionColorLight = Color.FromArgb(221, 221, 212);
    }
}

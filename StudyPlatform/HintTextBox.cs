using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace StudyPlatform
{
    class HintTextBox : TextBox
    {
        [Localizable(true)]
        public string Hint { get { return ModifedHint; } set { ModifedHint = value; UpdateHint(); } }
        private string ModifedHint;

        private void UpdateHint()
        {
            if (this.IsHandleCreated && ModifedHint != null)
            {
                SendMessage(this.Handle, 0x1501, (IntPtr)1, ModifedHint);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateHint();
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Messsageg, IntPtr WP, string LP);
    }
}

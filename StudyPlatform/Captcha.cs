using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudyPlatform
{
    //Класс капчи
    public partial class Captcha : Form
    {
        private string CaptchaText { get; set; }

        public Captcha()
        {
            InitializeComponent();
            Colorize();
        }

        private void Captcha_Load(object sender, EventArgs e)
        {
            CaptchaPictureBox.Image = CreateImage(CaptchaPictureBox.Width, CaptchaPictureBox.Height);
        }

        private Bitmap CreateImage(int Width, int Height)
        {
            CaptchaText = null;

            Random Randomize = new Random();            
            Bitmap Result = new Bitmap(Width, Height);
            Graphics Graph = Graphics.FromImage(Result);

            string Alphabet = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            int Xpos = Randomize.Next(0, Width - 50);
            int Ypos = Randomize.Next(15, Height - 15);

            Graph.Clear(Color.FromArgb(92, 92, 92));

            for (int i = 0; i < 5; ++i)
                CaptchaText += Alphabet[Randomize.Next(Alphabet.Length)];

            Graph.DrawString(CaptchaText, new Font("Calibri", 25), Brushes.Gray, new PointF(Xpos, Ypos));

            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (Randomize.Next() % 20 == 0)
                        Result.SetPixel(i, j, Color.FromArgb(48, 48, 48));

            return Result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (EnterCapthaTextBox.Text.ToUpper() == CaptchaText)
            {
                Authorization.IsCaptchaTrue = true;
                this.Close();
            }
            else
            {
                CaptchaPictureBox.Image = CreateImage(CaptchaPictureBox.Width, CaptchaPictureBox.Height);
                EnterCapthaTextBox.Text = null;
            }    
        }

        private void EnterCapthaTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (EnterCapthaTextBox.Text.ToUpper() == CaptchaText)
                {
                    Authorization.IsCaptchaTrue = true;
                    this.Close();
                }
                else
                {
                    CaptchaPictureBox.Image = CreateImage(CaptchaPictureBox.Width, CaptchaPictureBox.Height);
                    EnterCapthaTextBox.Text = null;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CaptchaPictureBox.Image = CreateImage(CaptchaPictureBox.Width, CaptchaPictureBox.Height);
            EnterCapthaTextBox.Text = null;
        }

        private void Colorize()
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    BackColor = ColorSchemeClass.FormBackColor;
                    EnterCapthaTextBox.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    EnterCapthaTextBox.BackColor = ColorSchemeClass.TextBoxBackColor;
                    EnterCapthaTextBox.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    button1.BackColor = ColorSchemeClass.ButtonBackColor;
                    button2.BackColor = ColorSchemeClass.ButtonBackColor;
                    button3.BackColor = ColorSchemeClass.ButtonBackColor;
                    button1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button2.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button3.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button1.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button2.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button3.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button1.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button2.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button3.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    break;
                case "Light":
                    BackColor = ColorSchemeClass.FormBackColorLight;
                    EnterCapthaTextBox.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    EnterCapthaTextBox.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    EnterCapthaTextBox.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    button1.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button2.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button3.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button2.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button3.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button1.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button2.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button3.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button1.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button2.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button3.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    break;
            }
        }
    }
}

using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SecurityLib;

namespace StudyPlatform
{
    //Класс авторизации
    public partial class Authorization : Form
    {
        DatabaseLinkDataContext DatabaseLinkData = new DatabaseLinkDataContext();

        Security PasswordDecryption = new Security();

        public static int NumberOfAttempts { get; set; }
        public static bool IsCaptchaTrue { get; set; }
        public static string Profile { get; set; }
        public static string Theme { get; set; }
        public static string LogFilePath { get; set; }

        public Authorization()
        {
            InitializeComponent();
            SetTheme();
            SetLogsFile();
            Colorize();            
        }

        private void Authorization_Load(object sender, EventArgs e)
        {
            label2.Text = null;
            NumberOfAttempts = 3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string DecodedPassword = null;

            if (NumberOfAttempts > 0)
            {
                var GetUser = DatabaseLinkData.Users.Where(u => u.Username == LoginTextBox.Text).Select(u => new { u.Password, u.RndKey });

                if (GetUser.Count() > 0)
                {
                    DecodedPassword = PasswordDecryption.Decode(GetUser.First().Password, LoginTextBox.Text, GetUser.First().RndKey);
                }

                IQueryable<string> AuthorizationResult = DatabaseLinkData.Users.Where(u => u.Username == LoginTextBox.Text && DecodedPassword == PasswordTextBox.Text).Select(u => u.Role);

                if (AuthorizationResult.Count() > 0)
                {
                    Profile = AuthorizationResult.First().ToString();

                    this.Hide();
                    Administrator AdministratorForm = new Administrator(Profile);
                    AdministratorForm.Show();

                    label2.Text = null;
                    LoginTextBox.Text = null;
                    PasswordTextBox.Text = null;
                }
                else
                {
                    this.Height = 290;
                    label2.Text = $"Неверное имя пользователя или пароль.\n                 Осталось попыток: {NumberOfAttempts--}";
                }
            }
            else
            {
                label2.Text = "  Вы превысили допустимое количество \n                    попыток входа!";
                Captcha CapthcaForm = new Captcha();
                CapthcaForm.ShowDialog();
                if (IsCaptchaTrue)
                {
                    NumberOfAttempts = 3;
                    label2.Text = null;
                    this.Height = 250;
                    IsCaptchaTrue = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SetTheme()
        {
            string ProgramPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + @"StudyPlatform";
            DirectoryInfo ThemeDirectoryInfo = new DirectoryInfo(ProgramPath);
            if (!ThemeDirectoryInfo.Exists)
            {
                ThemeDirectoryInfo.Create();
            }

            string ThemePath = ProgramPath + @"/Theme.conf";
            FileInfo ThemeFileInfo = new FileInfo(ThemePath);
            if (ThemeFileInfo.Exists)
            {
                try
                {
                    using (StreamReader Reader = new StreamReader(ThemePath))
                    {
                        Theme = Reader.ReadLine();
                    }
                }
                catch (Exception FileException)
                {
                    MessageBox.Show(FileException.Message, "Произошла ошибка!");
                }
            }
            else
            {
                File.WriteAllText(ThemePath, "Dark");
                Theme = "Dark";
            }
        }

        private void SetLogsFile()
        {
            string ProgramPath = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)) + @"StudyPlatform";
            DirectoryInfo LogDirectoryInfo = new DirectoryInfo(ProgramPath);
            if (!LogDirectoryInfo.Exists)
            {
                LogDirectoryInfo.Create();
            }

            LogFilePath = ProgramPath + @"/Errors.log";
            FileInfo LogFileInfo = new FileInfo(LogFilePath);
            if (!LogFileInfo.Exists)
            {
                LogFileInfo.Create();
            }
        }

        private void Colorize()
        {
            switch (Theme)
            {
                case "Dark":
                    BackColor = ColorSchemeClass.FormBackColor;
                    LoginTextBox.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    LoginTextBox.BackColor = ColorSchemeClass.TextBoxBackColor;
                    LoginTextBox.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    PasswordTextBox.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    PasswordTextBox.BackColor = ColorSchemeClass.TextBoxBackColor;
                    PasswordTextBox.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    button1.BackColor = ColorSchemeClass.ButtonBackColor;
                    button2.BackColor = ColorSchemeClass.ButtonBackColor;
                    button1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button2.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button1.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button2.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button1.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button2.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    label1.ForeColor = ColorSchemeClass.LabelForeColor;
                    label2.ForeColor = ColorSchemeClass.LabelForeColor;
                    label3.ForeColor = ColorSchemeClass.LabelForeColor;
                    break;
                case "Light":
                    BackColor = ColorSchemeClass.FormBackColorLight;
                    LoginTextBox.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    LoginTextBox.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    LoginTextBox.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    PasswordTextBox.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    PasswordTextBox.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    PasswordTextBox.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    button1.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button2.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button2.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button1.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button2.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button1.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button2.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    label1.ForeColor = ColorSchemeClass.LabelForeColorLight;
                    label2.ForeColor = ColorSchemeClass.LabelForeColorLight;
                    label3.ForeColor = ColorSchemeClass.LabelForeColorLight;
                    break;
            } 
        }

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                button1_Click(sender, e);
            }
        }
    }
}

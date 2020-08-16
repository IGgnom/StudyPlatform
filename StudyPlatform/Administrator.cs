using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using SecurityLib;

namespace StudyPlatform
{
    //Класс формы пользователя
    public partial class Administrator : Form
    {
        DatabaseLinkDataContext DatabaseLinkData = new DatabaseLinkDataContext();

        Security PasswordEncryption = new Security();

        public static byte PressedBtn { get; set; }
        public static string EditUsername { get; set; }
        public static string CoordsOfTable { get; set; }
        public static string DocName { get; set; }
        public static int XCoordsOfTable { get; set; }
        public static int YCoordsOfTable { get; set; }
        public static DateTime DocumentDate { get; set; }

        public Administrator(string CurrentUser)
        {
            InitializeComponent();
            Colorize();
            UserCustomization(CurrentUser);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                IQueryable<string> CheckQuery = DatabaseLinkData.Users.Where(u => u.Username == hintTextBox1.Text).Select(u => u.Username);
                if (CheckQuery.Count() == 0)
                {
                    if (hintTextBox1.Text != null && hintTextBox1.Text != "" && hintTextBox2.Text != null && hintTextBox2.Text != "" && hintTextBox3.Text != null && hintTextBox3.Text != "")
                    {
                        string RndKey = null;
                        byte RndNum;

                        Random RandomChar = new Random();

                        for (int i = 0; i < 16; i++)
                        {
                            RndNum = (byte)RandomChar.Next(33, 122);
                            RndKey += (char)RndNum;
                        }

                        Users NewUser = new Users
                        {
                            Username = hintTextBox1.Text,
                            Password = PasswordEncryption.Encode(hintTextBox2.Text, hintTextBox1.Text, RndKey),
                            RndKey = @RndKey,
                            Role = hintTextBox3.Text
                        };
                        DatabaseLinkData.Users.InsertOnSubmit(NewUser);
                        DatabaseLinkData.SubmitChanges();
                        this.usersTableAdapter.Fill(this.usersDataSet.Users);
                    }
                }
            }
            catch (Exception AddException)
            {
                MessageBox.Show(AddException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {AddException.Message}");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                this.usersTableAdapter.Fill(this.usersDataSet.Users);
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void Administrator_Load(object sender, EventArgs e)
        {
            try
            {
                switch (Authorization.Profile)
                {
                    case "Administrator":
                        this.Text = "ИС \"Учебное отделение\" | Администратор";
                        break;
                    case "Teacher":
                        this.Text = "ИС \"Учебное отделение\" | Преподаватель";
                        break;
                }

                timer1.Enabled = true;
                timer1.Interval = 100;

                this.usersTableAdapter.Fill(this.usersDataSet.Users);
                this.documentsTableAdapter.Fill(this.studyPlatformDataSet.Documents);
                this.studentsTableAdapter.Fill(this.studentsDataSet.Students);
                this.teachersTableAdapter.Fill(this.teachersDataSet.Teachers);

                comboBox1.Items.AddRange(DatabaseLinkData.Groups.Select(g => g.GroupName).ToArray());
                comboBox2.Items.AddRange(DatabaseLinkData.Teachers.Select(t => t.TeachersName).ToArray());
                comboBox3.Items.AddRange(DatabaseLinkData.Cabinets.Select(c => c.CabinetNumber).ToArray());
                comboBox4.Items.AddRange(DatabaseLinkData.Disciplines.Select(d => d.Discipline).ToArray());
                comboBox5.Items.AddRange(DatabaseLinkData.Disciplines.Select(d => d.Discipline).ToArray());
                comboBox6.Items.AddRange(DatabaseLinkData.Disciplines.Select(d => d.Discipline).ToArray());
                comboBox7.Items.AddRange(DatabaseLinkData.Groups.Select(g => g.GroupName).ToArray());
                comboBox9.Items.AddRange(DatabaseLinkData.Cabinets.Select(c => c.CabinetNumber).ToArray());
                comboBox13.Items.AddRange(DatabaseLinkData.Groups.Select(g => g.GroupName).ToArray());
                comboBox10.Items.AddRange(DatabaseLinkData.Groups.Select(g => g.GroupName).ToArray());
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (DateTime.Now.ToShortTimeString().Length == 4)
                    label17.Text = "0" + DateTime.Now.ToShortTimeString();
                else
                    label17.Text = DateTime.Now.ToShortTimeString();
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                hintTextBox1.Text = null;
                hintTextBox2.Text = null;
                hintTextBox3.Text = null;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            } 
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                EditUsername = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                hintTextBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                hintTextBox2.Text = PasswordEncryption.Decode(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                hintTextBox3.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            } 
            catch (Exception UknownException)
            {
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }          
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (hintTextBox1.Text != null && hintTextBox1.Text != "" && hintTextBox2.Text != null && hintTextBox2.Text != "" && hintTextBox3.Text != null && hintTextBox3.Text != "")
                {
                    Users DeleteUser = DatabaseLinkData.Users.Single(u => u.Username == hintTextBox1.Text);
                    DatabaseLinkData.Users.DeleteOnSubmit(DeleteUser);
                    DatabaseLinkData.SubmitChanges();
                    this.usersTableAdapter.Fill(this.usersDataSet.Users);
                    button6_Click(sender, e);
                }
            }
            catch (Exception DeleteException)
            {
                MessageBox.Show(DeleteException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {DeleteException.Message}");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (hintTextBox1.Text != null && hintTextBox1.Text != "" && hintTextBox2.Text != null && hintTextBox2.Text != "" && hintTextBox3.Text != null && hintTextBox3.Text != "" && EditUsername != null)
                {
                    string RndKey = null;
                    byte RndNum;

                    Random RandomChar = new Random();

                    for (int i = 0; i < 16; i++)
                    {
                        RndNum = (byte)RandomChar.Next(33, 122);
                        RndKey += (char)RndNum;
                    }

                    Users UpdateUser = DatabaseLinkData.Users.Single(u => u.Username == EditUsername);
                    UpdateUser.Password = PasswordEncryption.Encode(hintTextBox2.Text, hintTextBox1.Text, RndKey);
                    UpdateUser.RndKey = @RndKey;
                    UpdateUser.Role = hintTextBox3.Text;
                    DatabaseLinkData.SubmitChanges();
                    this.usersTableAdapter.Fill(this.usersDataSet.Users);
                    button6_Click(sender, e);
                }
            }
            catch (Exception UpdateException)
            {
                MessageBox.Show(UpdateException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UpdateException.Message}");
            }
        }

        public void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                XCoordsOfTable = e.ColumnIndex;
                YCoordsOfTable = e.RowIndex;
                CoordsOfTable = e.RowIndex.ToString() + e.ColumnIndex.ToString();
                if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "" && dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "1" && dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "2" && dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "3" && dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "4")
                    {
                        string[] Words = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(new char[] { '\n' });
                        Words[2] = Words[2].Trim();
                        Words[2] = Words[2].Remove(Words[2].IndexOf("Кабинет"), 8);
                        comboBox2.Text = Words[0];
                        comboBox4.Text = Words[1];
                        comboBox3.Text = Words[2];
                        switch (Authorization.Theme)
                        {
                            case "Dark":
                                comboBox2.ForeColor = ColorSchemeClass.ButtonForeColor;
                                comboBox3.ForeColor = ColorSchemeClass.ButtonForeColor;
                                comboBox4.ForeColor = ColorSchemeClass.ButtonForeColor;
                                break;
                            case "Light":
                                comboBox2.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                                comboBox3.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                                comboBox4.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                                break;
                        }
                    }
                }
                hintTextBox4.Text = null;
                hintTextBox5.Text = null;
                hintTextBox6.Text = null;
            }
            catch (Exception UknownException)
            {
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        //Процедура заполнения расписания
        private void button12_Click(object sender, EventArgs e)
        {
            bool IsNull = false;
            if (comboBox2.Text == "" & comboBox3.Text == "" & comboBox4.Text == "")
                IsNull = true;
            try
            {
                Timetable EditTimetable = DatabaseLinkData.Timetable.Single(t => t.Group == DatabaseLinkData.Groups.Where(g => g.GroupName == comboBox1.Text).Select(g => g.GroupId).First());

                switch (CoordsOfTable)
                {
                    case "01":
                        if (IsNull)
                            EditTimetable.L11 = "";
                        else
                            EditTimetable.L11 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "02":
                        if (IsNull)
                            EditTimetable.L21 = "";
                        else
                            EditTimetable.L21 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "03":
                        if (IsNull)
                            EditTimetable.L31 = "";
                        else
                            EditTimetable.L31 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "04":
                        if (IsNull)
                            EditTimetable.L41 = "";
                        else
                            EditTimetable.L41 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "05":
                        if (IsNull)
                            EditTimetable.L51 = "";
                        else
                            EditTimetable.L51 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "11":
                        if (IsNull)
                            EditTimetable.L12 = "";
                        else
                            EditTimetable.L12 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "12":
                        if (IsNull)
                            EditTimetable.L22 = "";
                        else
                            EditTimetable.L22 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "13":
                        if (IsNull)
                            EditTimetable.L32 = "";
                        else
                            EditTimetable.L32 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "14":
                        if (IsNull)
                            EditTimetable.L42 = "";
                        else
                            EditTimetable.L42 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "15":
                        if (IsNull)
                            EditTimetable.L52 = "";
                        else
                            EditTimetable.L52 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "21":
                        if (IsNull)
                            EditTimetable.L13 = "";
                        else
                            EditTimetable.L13 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "22":
                        if (IsNull)
                            EditTimetable.L23 = "";
                        else
                            EditTimetable.L23 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "23":
                        if (IsNull)
                            EditTimetable.L33 = "";
                        else
                            EditTimetable.L33 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "24":
                        if (IsNull)
                            EditTimetable.L43 = "";
                        else
                            EditTimetable.L43 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "25":
                        if (IsNull)
                            EditTimetable.L53 = "";
                        else
                            EditTimetable.L53 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "31":
                        if (IsNull)
                            EditTimetable.L14 = "";
                        else
                            EditTimetable.L14 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "32":
                        if (IsNull)
                            EditTimetable.L24 = "";
                        else
                            EditTimetable.L24 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "33":
                        if (IsNull)
                            EditTimetable.L34 = "";
                        else
                            EditTimetable.L34 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "34":
                        if (IsNull)
                            EditTimetable.L44 = "";
                        else
                            EditTimetable.L44 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                    case "35":
                        if (IsNull)
                            EditTimetable.L54 = "";
                        else
                            EditTimetable.L54 = $"{comboBox2.Text}\n{comboBox4.Text}\n      Кабинет {comboBox3.Text}";
                        break;
                }
                DatabaseLinkData.SubmitChanges();
                CreateTimetable();
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            } 
        }

        //Процедура проверки расписания
        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                hintTextBox4.Text = null;
                hintTextBox5.Text = null;
                hintTextBox6.Text = null;

                List<string> CheckList = new List<string>();
                int Count = 1;

                switch (CoordsOfTable)
                {
                    case "01":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L11).ToList();
                        break;
                    case "02":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L21).ToList();
                        break;
                    case "03":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L31).ToList();
                        break;
                    case "04":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L41).ToList();
                        break;
                    case "05":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L51).ToList();
                        break;
                    case "11":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L12).ToList();
                        break;
                    case "12":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L22).ToList();
                        break;
                    case "13":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L32).ToList();
                        break;
                    case "14":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L42).ToList();
                        break;
                    case "15":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L52).ToList();
                        break;
                    case "21":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L13).ToList();
                        break;
                    case "22":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L23).ToList();
                        break;
                    case "23":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L33).ToList();
                        break;
                    case "24":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L43).ToList();
                        break;
                    case "25":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L53).ToList();
                        break;
                    case "31":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L14).ToList();
                        break;
                    case "32":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L23).ToList();
                        break;
                    case "33":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L34).ToList();
                        break;
                    case "34":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L44).ToList();
                        break;
                    case "35":
                        CheckList = DatabaseLinkData.Timetable.Select(c => c.L54).ToList();
                        break;
                }

                foreach (string CheckLesson in CheckList)
                {
                    if (dataGridView2.Rows[YCoordsOfTable].Cells[XCoordsOfTable].Value.ToString() != CheckLesson && CheckLesson != null && CheckLesson != "")
                    {
                        string[] Words = CheckLesson.Split(new char[] { '\n' });
                        Words[2] = Words[2].Trim();
                        Words[2] = Words[2].Remove(Words[2].IndexOf("Кабинет"), 8);

                        if (Words[0] == comboBox2.Text || Words[2] == comboBox3.Text)
                        {
                            hintTextBox4.Text = DatabaseLinkData.Groups.Where(g => g.GroupId == Count).Select(g => g.GroupName).First();
                            hintTextBox5.Text = Words[0];
                            hintTextBox6.Text = Words[2];
                            break;
                        }
                    }
                    Count++;
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ChooseXlsx = new OpenFileDialog();
                ChooseXlsx.Title = "Выбрать Excel файл";
                ChooseXlsx.Filter = "Excel файлы |*.xlsx|Все файлы |*.*";
                if (ChooseXlsx.ShowDialog() == DialogResult.OK)
                    hintTextBox7.Text = ChooseXlsx.FileName;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ChooseDocx = new OpenFileDialog();
                ChooseDocx.Title = "Выбрать шаблон выписки";
                ChooseDocx.Filter = "Word файлы |*.docx|Все файлы |*.*";
                if (ChooseDocx.ShowDialog() == DialogResult.OK)
                    hintTextBox8.Text = ChooseDocx.FileName;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            } 
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog ChooseFolder = new FolderBrowserDialog();
                ChooseFolder.Description = "Выбрать папку сохранения";
                if (ChooseFolder.ShowDialog() == DialogResult.OK)
                    hintTextBox9.Text = ChooseFolder.SelectedPath;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }  
        }

        //Процедура типового заполнения выписки оценок
        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                if ((hintTextBox7.Text == null || hintTextBox7.Text == "") || (hintTextBox8.Text == null || hintTextBox8.Text == "") || (hintTextBox9.Text == null || hintTextBox9.Text == "") || (hintTextBox10.Text == null || hintTextBox10.Text == ""))
                {
                    MessageBox.Show("Заполните все поля для корректной работы!", "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Excel.Application ExcelApp = new Excel.Application();
                Excel.Workbook ExcelBook = ExcelApp.Workbooks.Open(hintTextBox7.Text);
                Excel.Worksheet ExcelSheet = ExcelBook.Worksheets[Convert.ToInt32(hintTextBox10.Text)];

                string Addr = ExcelSheet.UsedRange.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Address.ToString();
                bunifuProgressBar1.MaximumValue = Convert.ToInt32(Addr.Remove(0, Addr.LastIndexOf('$') + 1)) - 4;
                bunifuProgressBar1.Value = 0;

                Excel.Range ExcelRange = ExcelSheet.Range["B5", Addr];

                foreach (Excel.Range UsedRow in ExcelRange.Rows)
                {
                    Word.Application WordApp = new Word.Application();
                    Word.Document WordDoc = WordApp.Documents.Open(hintTextBox8.Text);
                    WordDoc.Activate();
                    int MarkCount = 1;
                    int ColumnIndex = 1;
                    foreach (Excel.Range UsedCell in UsedRow.Cells)
                    {
                        try
                        {
                            switch (UsedCell.Value2.ToString())
                            {
                                case "3":
                                    WordDoc.Bookmarks[MarkCount++].Range.Text = "удовлетворительно";
                                    break;
                                case "4":
                                    WordDoc.Bookmarks[MarkCount++].Range.Text = "хорошо";
                                    break;
                                case "5":
                                    WordDoc.Bookmarks[MarkCount++].Range.Text = "отлично";
                                    break;
                                case "зачет":
                                    WordDoc.Bookmarks[MarkCount++].Range.Text = "зачтено";
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch { }
                    }
                    Excel.Range Name = UsedRow.Cells[1, ColumnIndex++];
                    WordDoc.SaveAs2(hintTextBox9.Text + @"\" + Name.Value2.ToString() + ".docx");
                    bunifuProgressBar1.Value++;
                    WordDoc = null;
                    WordApp.Quit();
                }
                bunifuProgressBar1.Value = 0;
                MessageBox.Show($"Автозаполнение дипломов для группы \"{ExcelSheet.Name}\" прошло успешно!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ExcelBook = null;
                ExcelApp.Quit();
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void Administrator_Resize(object sender, EventArgs e)
        {
            try
            {
                if (this.Size.Height <= 465)
                    label17.Visible = false;
                else
                    label17.Visible = true;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.RowCount > 0)
                    for (int i = 0; i < 4; i++)
                        for (int j = 1; j <= 5; j++)
                            dataGridView2.Rows[i].Cells[j].Value = null;

                switch (Authorization.Theme)
                {
                    case "Dark":
                        comboBox1.ForeColor = ColorSchemeClass.ButtonForeColor;
                        break;
                    case "Light":
                        comboBox1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        break;
                }

                comboBox2.Text = null;
                comboBox3.Text = null;
                comboBox4.Text = null;
                CreateTimetable();
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                hintTextBox4.Text = null;
                hintTextBox5.Text = null;
                hintTextBox6.Text = null;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (Authorization.Theme)
                {
                    case "Dark":
                        (sender as ComboBox).ForeColor = ColorSchemeClass.ButtonForeColor;
                        break;
                    case "Light":
                        (sender as ComboBox).ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        break;
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox5.Text != null && comboBox6.Text != null && comboBox7.Text != null && comboBox8.Text != null && comboBox9.Text != null && bunifuDatepicker1.Value != null)
                {
                    Replacements NewReplacement = new Replacements
                    {
                        Date = bunifuDatepicker1.Value,
                        LessonNumber = Convert.ToInt32(comboBox8.Text),
                        Group = comboBox7.Text,
                        DisciplineOld = comboBox6.Text,
                        DisciplineNew = comboBox5.Text,
                        CabinetNumber = comboBox9.Text

                    };
                    DatabaseLinkData.Replacements.InsertOnSubmit(NewReplacement);
                    DatabaseLinkData.SubmitChanges();
                    bunifuDatepicker1_onValueChanged(sender, e);
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox5.Text != null && comboBox6.Text != null && comboBox7.Text != null && comboBox8.Text != null && comboBox9.Text != null && bunifuDatepicker1.Value != null)
                {
                    Replacements DeleteReplacement = DatabaseLinkData.Replacements.Single(r => r.Date == bunifuDatepicker1.Value && r.LessonNumber == Convert.ToInt32(comboBox8.Text) && r.Group == comboBox7.Text && r.DisciplineOld == comboBox6.Text && r.DisciplineNew == comboBox5.Text && r.CabinetNumber == comboBox9.Text);
                    DatabaseLinkData.Replacements.DeleteOnSubmit(DeleteReplacement);
                    DatabaseLinkData.SubmitChanges();
                    bunifuDatepicker1_onValueChanged(sender, e);
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            } 
        }

        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                bunifuDatepicker1_onValueChanged(sender, e);
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox5.Text = null;
                comboBox6.Text = null;
                comboBox7.Text = null;
                comboBox8.Text = null;
                comboBox9.Text = null;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void bunifuDatepicker1_onValueChanged(object sender, EventArgs e)
        {
            dataGridView3.RowCount = 0;
            try
            {
                var RefreshQuery = DatabaseLinkData.Replacements.Where(d => d.Date == bunifuDatepicker1.Value).Select(r => new { r.LessonNumber, r.Group, r.DisciplineOld, r.DisciplineNew, r.CabinetNumber });

                if (RefreshQuery.Count() > 0)
                {
                    foreach (var RefreshRow in RefreshQuery)
                    {
                        dataGridView3.RowCount++;
                        dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[0].Value = RefreshRow.LessonNumber;
                        dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[1].Value = RefreshRow.Group;
                        dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[2].Value = RefreshRow.DisciplineOld;
                        dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[3].Value = RefreshRow.DisciplineNew;
                        dataGridView3.Rows[dataGridView3.RowCount - 1].Cells[4].Value = RefreshRow.CabinetNumber;
                    }
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                switch (Authorization.Theme)
                {
                    case "Dark":
                        comboBox5.ForeColor = ColorSchemeClass.ButtonForeColor;
                        comboBox6.ForeColor = ColorSchemeClass.ButtonForeColor;
                        comboBox7.ForeColor = ColorSchemeClass.ButtonForeColor;
                        comboBox8.ForeColor = ColorSchemeClass.ButtonForeColor;
                        comboBox9.ForeColor = ColorSchemeClass.ButtonForeColor;
                        break;
                    case "Light":
                        comboBox5.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        comboBox6.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        comboBox7.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        comboBox8.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        comboBox9.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                        break;
                }

                comboBox5.Text = dataGridView3.Rows[e.RowIndex].Cells[3].Value.ToString();
                comboBox6.Text = dataGridView3.Rows[e.RowIndex].Cells[2].Value.ToString();
                comboBox7.Text = dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString();
                comboBox8.Text = dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString();
                comboBox9.Text = dataGridView3.Rows[e.RowIndex].Cells[4].Value.ToString();
            }
            catch (Exception UknownException)
            {
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void bunifuSwitch1_Click(object sender, EventArgs e)
        {
            try
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
                    if (bunifuSwitch1.Value)
                    {
                        Authorization.Theme = "Light";
                        bunifuDatepicker1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    }
                    else
                    {
                        Authorization.Theme = "Dark";
                        bunifuDatepicker1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    }
                    File.WriteAllText(ThemePath, Authorization.Theme);
                }
                else
                {
                    ThemeFileInfo.Create();
                    if (bunifuSwitch1.Value)
                    {
                        Authorization.Theme = "Light";
                        bunifuDatepicker1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    }
                    else
                    {
                        Authorization.Theme = "Dark";
                        bunifuDatepicker1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    }
                    File.WriteAllText(ThemePath, Authorization.Theme);
                }
                Colorize();
                button14_Click(sender, e);
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ChooseDocument = new OpenFileDialog();
                ChooseDocument.Title = "Выбрать документ";
                ChooseDocument.Filter = "Все файлы |*.*";
                if (ChooseDocument.ShowDialog() == DialogResult.OK)
                {
                    byte[] DocumentData;
                    using (FileStream DocumentFileStream = new FileStream(ChooseDocument.FileName, FileMode.Open))
                    {
                        DocumentData = new byte[DocumentFileStream.Length];
                        DocumentFileStream.Read(DocumentData, 0, DocumentData.Length);
                    }

                    try
                    {
                        Documents NewDocument = new Documents
                        {
                            LoadingDate = DateTime.Now,
                            DocumentName = ChooseDocument.FileName.Remove(0, ChooseDocument.FileName.LastIndexOf(@"\") + 1),
                            Document = DocumentData
                        };
                        DatabaseLinkData.Documents.InsertOnSubmit(NewDocument);
                        DatabaseLinkData.SubmitChanges();
                        this.documentsTableAdapter.Fill(this.studyPlatformDataSet.Documents);
                    }
                    catch (Exception AddDocumentExeption)
                    {
                        MessageBox.Show(AddDocumentExeption.Message, "Ошибка при добавлении документа!");
                    }
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            try
            {
                this.documentsTableAdapter.Fill(this.studyPlatformDataSet.Documents);
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                Documents DeleteDocument = DatabaseLinkData.Documents.Single(d => d.LoadingDate == DocumentDate && d.DocumentName == DocName);
                DatabaseLinkData.Documents.DeleteOnSubmit(DeleteDocument);
                DatabaseLinkData.SubmitChanges();
                this.documentsTableAdapter.Fill(this.studyPlatformDataSet.Documents);
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DocName = dataGridView4.Rows[e.RowIndex].Cells[1].Value.ToString();
                DocumentDate = Convert.ToDateTime(dataGridView4.Rows[e.RowIndex].Cells[0].Value);
            }
            catch (Exception UknownException)
            {
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog ChooseDocumentFolder = new FolderBrowserDialog();
                ChooseDocumentFolder.Description = "Выбрать папку сохранения";
                if (ChooseDocumentFolder.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] DownloadDocument = DatabaseLinkData.Documents.Where(d => d.LoadingDate == DocumentDate && d.DocumentName == DocName).Select(d => d.Document).First().ToArray();

                        using (FileStream DownloadStream = new FileStream(ChooseDocumentFolder.SelectedPath + @"\" + DocName, FileMode.OpenOrCreate))
                        {
                            DownloadStream.Write(DownloadDocument, 0, DownloadDocument.Length);
                        }
                    }
                    catch (Exception UknownException)
                    {
                        File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
                    }
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            try
            {
                if (hintTextBox11.Text != null && hintTextBox12.Text != null && hintTextBox13.Text != null && hintTextBox14.Text != null && hintTextBox15.Text != null && hintTextBox16.Text != null && hintTextBox17.Text != null && comboBox13.Text != null)
                {
                    Students NewStudent = new Students
                    {
                        Group = DatabaseLinkData.Groups.Where(g => g.GroupName == comboBox13.Text).Select(g => g.GroupId).First(),
                        FirstName = hintTextBox14.Text,
                        SecondName = hintTextBox15.Text,
                        LastName = hintTextBox16.Text,
                        PassportSerial = hintTextBox13.Text,
                        PassportNumber = hintTextBox12.Text,
                        Address = hintTextBox11.Text,
                        Email = hintTextBox17.Text
                    };
                    DatabaseLinkData.Students.InsertOnSubmit(NewStudent);
                    DatabaseLinkData.SubmitChanges();
                }
                this.studentsTableAdapter.Fill(this.studentsDataSet.Students);
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            try
            {
                if (hintTextBox11.Text != null && hintTextBox12.Text != null && hintTextBox13.Text != null && hintTextBox14.Text != null && hintTextBox15.Text != null && hintTextBox16.Text != null && hintTextBox17.Text != null && comboBox13.Text != null)
                {
                    Students DeleteStrudent = DatabaseLinkData.Students.Single(s => s.PassportSerial == hintTextBox13.Text && s.PassportNumber == hintTextBox12.Text);
                    DatabaseLinkData.Students.DeleteOnSubmit(DeleteStrudent);
                    DatabaseLinkData.SubmitChanges();
                    this.studentsTableAdapter.Fill(this.studentsDataSet.Students);
                    button30_Click(sender, e);
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox13.Text = null;
                hintTextBox14.Text = null;
                hintTextBox15.Text = null;
                hintTextBox16.Text = null;
                hintTextBox13.Text = null;
                hintTextBox12.Text = null;
                hintTextBox11.Text = null;
                hintTextBox17.Text = null;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                comboBox13.Text = DatabaseLinkData.Groups.Where(g => g.GroupId == DatabaseLinkData.Students.Where(s => s.PassportSerial == dataGridView5.Rows[e.RowIndex].Cells[3].Value.ToString() && s.PassportNumber == dataGridView5.Rows[e.RowIndex].Cells[4].Value.ToString()).Select(s => s.Group).First()).Select(g => g.GroupName).First();
                hintTextBox14.Text = dataGridView5.Rows[e.RowIndex].Cells[0].Value.ToString();
                hintTextBox15.Text = dataGridView5.Rows[e.RowIndex].Cells[1].Value.ToString();
                hintTextBox16.Text = dataGridView5.Rows[e.RowIndex].Cells[2].Value.ToString();
                hintTextBox13.Text = dataGridView5.Rows[e.RowIndex].Cells[3].Value.ToString();
                hintTextBox12.Text = dataGridView5.Rows[e.RowIndex].Cells[4].Value.ToString();
                hintTextBox11.Text = dataGridView5.Rows[e.RowIndex].Cells[5].Value.ToString();
                hintTextBox17.Text = dataGridView5.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
            catch (Exception UknownException)
            {
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            try
            {
                if (hintTextBox21.Text != null && hintTextBox20.Text != null && hintTextBox24.Text != null && hintTextBox23.Text != null && hintTextBox19.Text != null && hintTextBox22.Text != null && hintTextBox18.Text != null && comboBox10.Text != null)
                {
                    Teachers NewTeacher = new Teachers
                    {
                        Group = DatabaseLinkData.Groups.Where(g => g.GroupName == comboBox10.Text).Select(g => g.GroupId).First(),
                        TeachersName = hintTextBox21.Text,
                        PhoneNumber = hintTextBox20.Text,
                        Username = hintTextBox19.Text,
                        PassportSerial = hintTextBox24.Text,
                        PassportNumber = hintTextBox23.Text,
                        Address = hintTextBox22.Text,
                        Email = hintTextBox18.Text
                    };
                    DatabaseLinkData.Teachers.InsertOnSubmit(NewTeacher);
                    DatabaseLinkData.SubmitChanges();
                }
                this.teachersTableAdapter.Fill(this.teachersDataSet.Teachers);
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(DatabaseLinkData.Teachers.Select(t => t.TeachersName).ToArray());
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            try
            {
                if (hintTextBox21.Text != null && hintTextBox20.Text != null && hintTextBox24.Text != null && hintTextBox23.Text != null && hintTextBox19.Text != null && hintTextBox22.Text != null && hintTextBox18.Text != null && comboBox10.Text != null)
                {
                    Teachers DeleteTeacher = DatabaseLinkData.Teachers.Single(t => t.PassportSerial == hintTextBox24.Text && t.PassportNumber == hintTextBox23.Text);
                    DatabaseLinkData.Teachers.DeleteOnSubmit(DeleteTeacher);
                    DatabaseLinkData.SubmitChanges();
                    this.teachersTableAdapter.Fill(this.teachersDataSet.Teachers);
                    button33_Click(sender, e);
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox10.Text = null;
                hintTextBox21.Text = null;
                hintTextBox20.Text = null;
                hintTextBox24.Text = null;
                hintTextBox23.Text = null;
                hintTextBox19.Text = null;
                hintTextBox22.Text = null;
                hintTextBox18.Text = null;
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            } 
        }

        private void dataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                comboBox10.Text = DatabaseLinkData.Groups.Where(g => g.GroupId == DatabaseLinkData.Teachers.Where(t => t.PassportSerial == dataGridView6.Rows[e.RowIndex].Cells[2].Value.ToString() && t.PassportNumber == dataGridView6.Rows[e.RowIndex].Cells[3].Value.ToString()).Select(t => t.Group).First()).Select(g => g.GroupName).First();
                hintTextBox21.Text = dataGridView6.Rows[e.RowIndex].Cells[0].Value.ToString();
                hintTextBox20.Text = dataGridView6.Rows[e.RowIndex].Cells[1].Value.ToString();
                hintTextBox24.Text = dataGridView6.Rows[e.RowIndex].Cells[2].Value.ToString();
                hintTextBox23.Text = dataGridView6.Rows[e.RowIndex].Cells[3].Value.ToString();
                hintTextBox19.Text = dataGridView6.Rows[e.RowIndex].Cells[4].Value.ToString();
                hintTextBox22.Text = dataGridView6.Rows[e.RowIndex].Cells[5].Value.ToString();
                hintTextBox18.Text = dataGridView6.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
            catch (Exception UknownException)
            {
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            this.Hide();
            Authorization ReAuthorization = new Authorization();
            ReAuthorization.Show();
        }

        private void Administrator_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        //Процедура создания расписания
        private void CreateTimetable()
        {
            try
            {
                var CreateTimetableQuery = DatabaseLinkData.Timetable.Where(t => t.Group.Equals(DatabaseLinkData.Groups.Where(g => g.GroupName == comboBox1.Text).Select(g => g.GroupId).First())).Select(t => new { t.L11, t.L12, t.L13, t.L14, t.L21, t.L22, t.L23, t.L24, t.L31, t.L32, t.L33, t.L34, t.L41, t.L42, t.L43, t.L44, t.L51, t.L52, t.L53, t.L54, });

                dataGridView2.RowCount = 4;
                dataGridView2.Rows[0].Cells[0].Value = 1.ToString();
                dataGridView2.Rows[1].Cells[0].Value = 2.ToString();
                dataGridView2.Rows[2].Cells[0].Value = 3.ToString();
                dataGridView2.Rows[3].Cells[0].Value = 4.ToString();

                if (CreateTimetableQuery.Count() > 0)
                {
                    dataGridView2.Rows[0].Cells[1].Value = CreateTimetableQuery.First().L11;
                    dataGridView2.Rows[0].Cells[2].Value = CreateTimetableQuery.First().L21;
                    dataGridView2.Rows[0].Cells[3].Value = CreateTimetableQuery.First().L31;
                    dataGridView2.Rows[0].Cells[4].Value = CreateTimetableQuery.First().L41;
                    dataGridView2.Rows[0].Cells[5].Value = CreateTimetableQuery.First().L51;
                    dataGridView2.Rows[1].Cells[1].Value = CreateTimetableQuery.First().L12;
                    dataGridView2.Rows[1].Cells[2].Value = CreateTimetableQuery.First().L22;
                    dataGridView2.Rows[1].Cells[3].Value = CreateTimetableQuery.First().L32;
                    dataGridView2.Rows[1].Cells[4].Value = CreateTimetableQuery.First().L42;
                    dataGridView2.Rows[1].Cells[5].Value = CreateTimetableQuery.First().L52;
                    dataGridView2.Rows[2].Cells[1].Value = CreateTimetableQuery.First().L13;
                    dataGridView2.Rows[2].Cells[2].Value = CreateTimetableQuery.First().L23;
                    dataGridView2.Rows[2].Cells[3].Value = CreateTimetableQuery.First().L33;
                    dataGridView2.Rows[2].Cells[4].Value = CreateTimetableQuery.First().L43;
                    dataGridView2.Rows[2].Cells[5].Value = CreateTimetableQuery.First().L53;
                    dataGridView2.Rows[3].Cells[1].Value = CreateTimetableQuery.First().L14;
                    dataGridView2.Rows[3].Cells[2].Value = CreateTimetableQuery.First().L24;
                    dataGridView2.Rows[3].Cells[3].Value = CreateTimetableQuery.First().L34;
                    dataGridView2.Rows[3].Cells[4].Value = CreateTimetableQuery.First().L44;
                    dataGridView2.Rows[3].Cells[5].Value = CreateTimetableQuery.First().L54;
                }
            }
            catch (Exception UknownException)
            {
                MessageBox.Show(UknownException.Message, "Произошла ошибка!");
                File.WriteAllText(Authorization.LogFilePath, $"[{DateTime.Now}]: {UknownException.Message}");
            }
        }

        //Процедура перерисовки формы, в зависимости от темы оформления
        private void Colorize()
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel1.BackColor = ColorSchemeClass.FormBackColor;
                    panel2.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel6.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[0].BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[1].BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[2].BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[3].BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[4].BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[5].BackColor = ColorSchemeClass.FormBackColor;
                    tabControl1.TabPages[6].BackColor = ColorSchemeClass.FormBackColor;
                    button1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button1.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button1.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button2.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button2.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button2.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button3.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button3.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button3.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button4.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button4.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button4.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button10.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button10.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button10.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button13.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button13.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button13.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button14.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button14.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button14.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button15.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button15.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button15.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button16.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button16.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button16.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    button17.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button17.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOver;
                    button17.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDown;
                    label1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label5.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label6.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label7.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label8.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label9.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label10.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label11.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label12.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label13.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label14.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label15.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label16.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label17.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label18.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label19.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label20.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label21.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label22.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label23.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label24.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label25.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label26.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label27.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label28.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label29.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label30.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label31.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label32.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label33.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label34.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label35.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label36.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label37.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label38.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label39.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label40.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label41.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label42.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label43.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label44.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label45.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label46.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label47.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label48.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label49.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label50.ForeColor = ColorSchemeClass.ButtonForeColor;
                    label51.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView1.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColor;
                    dataGridView1.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView1.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView1.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView1.BackgroundColor = ColorSchemeClass.FormBackColor;
                    dataGridView1.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView1.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView1.GridColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView2.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView2.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColor;
                    dataGridView2.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView2.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView2.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView2.BackgroundColor = ColorSchemeClass.FormBackColor;
                    dataGridView2.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView2.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView2.GridColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView2.DefaultCellStyle.WrapMode = ColorSchemeClass.DataGridWrap;
                    dataGridView3.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView3.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView3.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColor;
                    dataGridView3.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView3.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView3.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView3.BackgroundColor = ColorSchemeClass.FormBackColor;
                    dataGridView3.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView3.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView3.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView3.DefaultCellStyle.WrapMode = ColorSchemeClass.DataGridWrap;
                    dataGridView3.GridColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView4.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView4.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView4.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColor;
                    dataGridView4.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView4.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView4.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView4.BackgroundColor = ColorSchemeClass.FormBackColor;
                    dataGridView4.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView4.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView4.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView4.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView4.GridColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView5.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView5.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView5.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColor;
                    dataGridView5.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView5.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView5.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView5.BackgroundColor = ColorSchemeClass.FormBackColor;
                    dataGridView5.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView5.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView5.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView5.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView5.GridColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView6.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView6.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView6.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColor;
                    dataGridView6.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView6.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDown;
                    dataGridView6.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView6.BackgroundColor = ColorSchemeClass.FormBackColor;
                    dataGridView6.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView6.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView6.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColor;
                    dataGridView6.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColor;
                    dataGridView6.GridColor = ColorSchemeClass.FormButtonMouseDown;
                    button5.BackColor = ColorSchemeClass.ButtonBackColor;
                    button5.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button5.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button5.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button6.BackColor = ColorSchemeClass.ButtonBackColor;
                    button6.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button6.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button6.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button7.BackColor = ColorSchemeClass.ButtonBackColor;
                    button7.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button7.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button7.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button8.BackColor = ColorSchemeClass.ButtonBackColor;
                    button8.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button8.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button8.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button9.BackColor = ColorSchemeClass.ButtonBackColor;
                    button9.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button9.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button9.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button11.BackColor = ColorSchemeClass.ButtonBackColor;
                    button11.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button11.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button11.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button12.BackColor = ColorSchemeClass.ButtonBackColor;
                    button12.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button12.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button12.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button18.BackColor = ColorSchemeClass.ButtonBackColor;
                    button18.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button18.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button18.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button20.BackColor = ColorSchemeClass.ButtonBackColor;
                    button20.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button20.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button20.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button22.BackColor = ColorSchemeClass.ButtonBackColor;
                    button22.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button22.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button22.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button23.BackColor = ColorSchemeClass.ButtonBackColor;
                    button23.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button23.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button23.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button24.BackColor = ColorSchemeClass.ButtonBackColor;
                    button24.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button24.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button24.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button25.BackColor = ColorSchemeClass.ButtonBackColor;
                    button25.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button25.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button25.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button19.BackColor = ColorSchemeClass.ButtonBackColor;
                    button19.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button19.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button19.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button26.BackColor = ColorSchemeClass.ButtonBackColor;
                    button26.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button26.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button26.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button27.BackColor = ColorSchemeClass.ButtonBackColor;
                    button27.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button27.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button27.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button21.BackColor = ColorSchemeClass.ButtonBackColor;
                    button21.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button21.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button21.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button28.BackColor = ColorSchemeClass.ButtonBackColor;
                    button28.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button28.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button28.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button30.BackColor = ColorSchemeClass.ButtonBackColor;
                    button30.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button30.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button30.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button31.BackColor = ColorSchemeClass.ButtonBackColor;
                    button31.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button31.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button31.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button32.BackColor = ColorSchemeClass.ButtonBackColor;
                    button32.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button32.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button32.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button33.BackColor = ColorSchemeClass.ButtonBackColor;
                    button33.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button33.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button33.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button34.BackColor = ColorSchemeClass.ButtonBackColor;
                    button34.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button34.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button34.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    button35.BackColor = ColorSchemeClass.ButtonBackColor;
                    button35.ForeColor = ColorSchemeClass.ButtonForeColor;
                    button35.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOver;
                    button35.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDown;
                    hintTextBox1.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox1.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox1.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox2.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox2.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox2.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox3.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox3.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox3.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox4.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox4.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox4.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox5.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox5.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox5.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox6.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox6.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox6.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox7.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox7.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox7.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox8.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox8.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox8.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox9.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox9.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox9.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox10.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox10.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox10.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox11.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox11.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox11.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox12.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox12.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox12.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox13.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox13.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox13.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox14.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox14.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox14.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox15.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox15.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox15.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox16.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox16.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox16.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox17.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox17.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox17.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox18.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox18.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox18.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox19.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox19.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox19.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox20.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox20.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox20.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox21.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox21.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox21.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox22.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox22.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox22.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox23.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox23.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox23.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    hintTextBox24.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox24.BackColor = ColorSchemeClass.TextBoxBackColor;
                    hintTextBox24.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    comboBox1.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox2.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox3.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox4.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox5.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox6.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox7.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox8.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox9.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox13.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox13.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    comboBox10.BackColor = ColorSchemeClass.TextBoxBackColor;
                    comboBox10.ForeColor = ColorSchemeClass.TextBoxForeColor;
                    bunifuProgressBar1.BackColor = ColorSchemeClass.TextBoxBackColor;
                    bunifuProgressBar1.ProgressColor = ColorSchemeClass.ButtonMouseDown;
                    bunifuDatepicker1.BackColor = ColorSchemeClass.TextBoxBackColor; 
                    bunifuSwitch1.Oncolor = ColorSchemeClass.ButtonBackColor;
                    break;
                case "Light":
                    panel1.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel2.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel6.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[0].BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[1].BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[2].BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[3].BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[4].BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[5].BackColor = ColorSchemeClass.FormBackColorLight;
                    tabControl1.TabPages[6].BackColor = ColorSchemeClass.FormBackColorLight;
                    button1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button1.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button1.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button2.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button2.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button2.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button3.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button3.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button3.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button4.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button4.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button4.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button10.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button10.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button10.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button13.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button13.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button13.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button14.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button14.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button14.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button15.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button15.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button15.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button16.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button16.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button16.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button17.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button17.FlatAppearance.MouseOverBackColor = ColorSchemeClass.FormButtonMouseOverLight;
                    button17.FlatAppearance.MouseDownBackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    label1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label5.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label6.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label7.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label8.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label9.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label10.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label11.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label12.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label13.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label14.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label15.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label16.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label17.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label18.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label19.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label20.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label21.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label22.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label23.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label24.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label25.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label26.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label27.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label28.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label29.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label30.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label31.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label32.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label33.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label34.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label35.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label36.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label37.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label38.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label39.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label40.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label41.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label42.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label43.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label44.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label45.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label46.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label47.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label48.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label49.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label50.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    label51.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView1.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColorLight;
                    dataGridView1.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView1.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView1.BackgroundColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView1.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView1.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView1.GridColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView2.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView2.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColorLight;
                    dataGridView2.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView2.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView2.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView2.BackgroundColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView2.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView2.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView2.GridColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView2.DefaultCellStyle.WrapMode = ColorSchemeClass.DataGridWrap;
                    dataGridView3.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView3.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView3.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColorLight;
                    dataGridView3.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView3.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView3.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView3.BackgroundColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView3.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView3.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView3.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView3.GridColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView3.DefaultCellStyle.WrapMode = ColorSchemeClass.DataGridWrap;
                    dataGridView4.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView4.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView4.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColorLight;
                    dataGridView4.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView4.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView4.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView4.BackgroundColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView4.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView4.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView4.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView4.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView4.GridColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView5.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView5.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView5.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColorLight;
                    dataGridView5.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView5.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView5.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView5.BackgroundColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView5.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView5.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView5.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView5.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView5.GridColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView6.CellBorderStyle = ColorSchemeClass.DataGridCellBorderStyle;
                    dataGridView6.AlternatingRowsDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView6.DefaultCellStyle.SelectionBackColor = ColorSchemeClass.DataGridSelectionColorLight;
                    dataGridView6.DefaultCellStyle.SelectionForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView6.DefaultCellStyle.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                    dataGridView6.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView6.BackgroundColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView6.EnableHeadersVisualStyles = ColorSchemeClass.EnadleHeadersVisuals;
                    dataGridView6.ColumnHeadersBorderStyle = ColorSchemeClass.DataGridHeaderBorder;
                    dataGridView6.ColumnHeadersDefaultCellStyle.BackColor = ColorSchemeClass.FormBackColorLight;
                    dataGridView6.ColumnHeadersDefaultCellStyle.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    dataGridView6.GridColor = ColorSchemeClass.FormButtonMouseDownLight;
                    button5.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button5.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button5.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button5.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button6.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button6.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button6.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button6.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button7.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button7.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button7.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button7.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button8.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button8.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button8.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button8.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button9.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button9.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button9.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button9.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button11.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button11.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button11.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button11.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button12.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button12.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button12.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button12.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button18.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button18.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button18.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button18.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button20.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button20.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button20.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button20.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button22.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button22.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button22.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button22.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button23.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button23.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button23.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button23.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button24.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button24.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button24.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button24.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button25.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button25.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button25.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button25.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button28.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button28.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button28.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button28.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button30.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button30.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button30.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button30.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button31.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button31.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button31.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button31.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button32.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button32.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button32.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button32.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button33.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button33.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button33.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button33.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button34.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button34.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button34.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button34.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button35.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button35.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button35.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button35.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button19.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button19.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button19.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button19.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button26.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button26.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button26.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button26.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button27.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button27.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button27.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button27.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    button21.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    button21.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    button21.FlatAppearance.MouseOverBackColor = ColorSchemeClass.ButtonMouseOverLight;
                    button21.FlatAppearance.MouseDownBackColor = ColorSchemeClass.ButtonMouseDownLight;
                    hintTextBox1.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox1.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox1.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox2.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox2.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox2.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox3.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox3.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox3.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox4.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox4.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox4.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox5.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox5.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox5.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox6.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox6.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox6.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox7.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox7.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox7.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox8.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox8.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox8.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox9.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox9.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox9.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox10.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox10.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox10.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox11.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox11.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox11.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox12.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox12.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox12.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox13.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox13.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox13.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox14.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox14.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox14.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox15.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox15.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox15.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox16.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox16.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox16.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox17.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox17.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox17.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox18.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox18.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox18.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox19.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox19.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox19.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox20.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox20.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox20.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox21.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox21.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox21.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox22.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox22.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox22.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox23.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox23.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox23.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    hintTextBox24.BorderStyle = ColorSchemeClass.TextBoxBorderStyle;
                    hintTextBox24.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    hintTextBox24.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    comboBox1.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox2.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox3.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox4.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox5.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox6.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox7.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox8.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox9.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox13.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox13.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    comboBox10.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    comboBox10.ForeColor = ColorSchemeClass.TextBoxForeColorLight;
                    bunifuProgressBar1.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    bunifuProgressBar1.ProgressColor = ColorSchemeClass.ButtonMouseDownLight;
                    bunifuDatepicker1.BackColor = ColorSchemeClass.TextBoxBackColorLight;
                    bunifuDatepicker1.ForeColor = ColorSchemeClass.ButtonForeColorLight;
                    bunifuSwitch1.Oncolor = ColorSchemeClass.ButtonBackColorLight;
                    bunifuSwitch1.Value = ColorSchemeClass.LigthTheme;
                    break;
            }
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 1)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel3.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 1)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel3.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel3.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                } 
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 1)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel3.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel3.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 1)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel3.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel3.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }      
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.FormBackColor;
                    panel3.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel3.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    break;
            }
            
            PressedBtn = 1;
            tabControl1.SelectTab(PressedBtn);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 0)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel2.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel2.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }  
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 0)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel2.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel2.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 0)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel2.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 0)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel2.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel2.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    break;
            }
            
            PressedBtn = 0;
            tabControl1.SelectTab(PressedBtn);
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            if(PressedBtn != 2)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel4.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel4.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 2)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel4.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel4.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 2)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel4.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 2)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel4.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel4.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.FormBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    break;
            }
            
            PressedBtn = 2;
            tabControl1.SelectTab(PressedBtn);
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 3)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel5.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel5.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 3)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel5.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel5.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 3)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel5.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 3)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel5.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel5.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.FormBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    break;
            }
            
            PressedBtn = 3;
            tabControl1.SelectTab(PressedBtn);
        }

        private void button13_MouseDown(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 4)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel10.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel10.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }
        }

        private void button13_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 4)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel10.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel10.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }      
        }

        private void button13_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 4)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel10.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }           
        }

        private void button13_MouseUp(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 4)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel10.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel10.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }           
        }

        private void button13_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.FormBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    break;
            }
            
            PressedBtn = 4;
            tabControl1.SelectTab(PressedBtn);
        }

        private void button14_MouseDown(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 5)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel11.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel11.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }  
        }

        private void button14_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 5)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel11.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel11.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }          
        }

        private void button14_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 5)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel11.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }          
        }

        private void button14_MouseMove(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 5)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel11.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel11.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }          
        }

        private void button14_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.FormBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.ButtonBackColor;
                    panel12.BackColor = ColorSchemeClass.FormBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                    break;
            }
            
            PressedBtn = 5;
            tabControl1.SelectTab(PressedBtn);
        }

        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 6)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel12.BackColor = ColorSchemeClass.FormButtonMouseDown;
                        break;
                    case "Light":
                        panel12.BackColor = ColorSchemeClass.FormButtonMouseDownLight;
                        break;
                }         
        }

        private void button10_MouseEnter(object sender, EventArgs e)
        {
            if (PressedBtn != 6)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel12.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel12.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }          
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            if (PressedBtn != 6)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel12.BackColor = ColorSchemeClass.FormBackColor;
                        break;
                    case "Light":
                        panel12.BackColor = ColorSchemeClass.FormBackColorLight;
                        break;
                }           
        }

        private void button10_MouseUp(object sender, MouseEventArgs e)
        {
            if (PressedBtn != 6)
                switch (Authorization.Theme)
                {
                    case "Dark":
                        panel12.BackColor = ColorSchemeClass.FormButtonMouseOver;
                        break;
                    case "Light":
                        panel12.BackColor = ColorSchemeClass.FormButtonMouseOverLight;
                        break;
                }            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            switch (Authorization.Theme)
            {
                case "Dark":
                    panel2.BackColor = ColorSchemeClass.FormBackColor;
                    panel3.BackColor = ColorSchemeClass.FormBackColor;
                    panel4.BackColor = ColorSchemeClass.FormBackColor;
                    panel5.BackColor = ColorSchemeClass.FormBackColor;
                    panel10.BackColor = ColorSchemeClass.FormBackColor;
                    panel11.BackColor = ColorSchemeClass.FormBackColor;
                    panel12.BackColor = ColorSchemeClass.ButtonBackColor;
                    break;
                case "Light":
                    panel2.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel3.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel4.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel5.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel10.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel11.BackColor = ColorSchemeClass.FormBackColorLight;
                    panel12.BackColor = ColorSchemeClass.ButtonBackColorLight;
                    break;
            }
            
            PressedBtn = 6;
            tabControl1.SelectTab(PressedBtn);
        }

        private void UserCustomization(string CurrentUser)
        {
            if (CurrentUser == "Teacher")
            {
                button2_Click(null, null);
                button1.Visible = false;
                button4.Visible = false;
                button10.Visible = false;
                button13.Visible = false;
                panel2.Visible = false;
                panel5.Visible = false;
                panel10.Visible = false;
                panel12.Visible = false;
                button2.Left = -1;
                button2.Top = 55;
                panel3.Left = 1;
                panel3.Top = 56;
                button3.Left = -1;
                button3.Top = 103;
                panel4.Left = 1;
                panel4.Top = 104;
                button14.Left = -1;
                button14.Top = 151;
                panel11.Left = 1;
                panel11.Top = 152;
                label12.Visible = false;
                label13.Visible = false;
                label14.Visible = false;
                label15.Visible = false;
                hintTextBox4.Visible = false;
                hintTextBox5.Visible = false;
                hintTextBox6.Visible = false;
                button11.Visible = false;
                button12.Visible = false;
                button20.Visible = false;
                button22.Visible = false;
                button23.Visible = false;
                button24.Visible = false;
                button25.Visible = false;
            }
        }
    }
}

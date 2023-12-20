using MongoDB.Driver;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MongoDB.Bson;

namespace FrnClubRegistration
{
    public partial class MainWindow : Window
    {
        private string connectionString = "mongodb+srv://jasonm2822:JasMalC2822@cluster0.tva1uxn.mongodb.net/";
        private readonly MongoClient client;
        public MainWindow()
        {
            InitializeComponent();
            client = new MongoClient(connectionString);
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            var database = client.GetDatabase("ClubRegistrationQuery");
            var collection = database.GetCollection<ClubMembers>("ClubRegistration");

            var clubMembersList = collection.Find(_ => true).ToList();

            foreach (var member in clubMembersList)
            {
                ClubMemberTable.Items.Add(member);
                FrmUpdateMember frmUpdateMemberInstance = new FrmUpdateMember(client);

                frmUpdateMemberInstance.studentCB.Items.Add(member.StudentID);
            }
        }

        private bool IsSpecialCharacter(Key key)
        {
            var specialCharacters = new[]
            {
                Key.Oem1, Key.Oem2, Key.Oem3, Key.Oem4, Key.Oem5,
                Key.Oem6, Key.Oem7, Key.Oem8, Key.Oem102,Key.OemPeriod,
                Key.OemComma, Key.OemPlus, Key.OemQuotes, Key.OemOpenBrackets,
                Key.OemCloseBrackets, Key.OemBackslash,Key.DeadCharProcessed,
                Key.ImeProcessed, Key.System, Key.OemMinus
            };

            return specialCharacters.Contains(key);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ListOfProgram = new string[]
{
                "BS Information Technology",
                "BS Computer Science",
                "BS Information System",
                "BS in Accountancy",
                "BS in Hospitality Management",
                "BS in Tourism Management"
};
            for (int i = 0; i < 6; i++)
            {
                ProgramCb.Items.Add(ListOfProgram[i].ToString());
            }

            string[] ListOfGender = new string[]
            {
                "Male",
                "Female",
                "Non-Binary"
            };
            for (int g = 0; g < 3; g++)
            {
                GenderCB.Items.Add(ListOfGender[g].ToString());
            }
        }

        private void regiterBtn_Click(object sender, RoutedEventArgs e)
        {
            string studentId = studentIdTb.Text;
            string firstName = firstNameTb.Text;
            string middleName = middleNameTb.Text;
            string lastName = lastNameTb.Text;
            string ageText = ageTb.Text.Trim();

            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(middleName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(ageText) || GenderCB.SelectedItem == null || ProgramCb.SelectedItem == null)
            {
                notifierTb.Text = "Please Fill Up The Form!";
                notifierTb.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                notifierTb.Visibility = Visibility.Collapsed;
            }

            if (!int.TryParse(ageText, out int age))
            {
                return;
            }

            string gender = GenderCB.SelectedItem.ToString();
            string program = ProgramCb.SelectedItem.ToString();

            var database = client.GetDatabase("ClubRegistrationQuery");
            var collection = database.GetCollection<ClubMembers>("ClubRegistration");

            var existingStudentId = collection.Find(x => x.StudentID == long.Parse(studentId)).FirstOrDefault();

            if (existingStudentId != null)
            {
                notifierTb.Text = "Student ID Already Exists!";
                notifierTb.Visibility = Visibility.Visible;
                return;
            }

            var newStudent = new ClubMembers
            {
                StudentID = long.Parse(studentId),
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Age = age,
                Gender = gender,
                Program = program
            };

            collection.InsertOne(newStudent);
            ClubMemberTable.Items.Add(newStudent);

            studentIdTb.Clear();
            firstNameTb.Clear();
            middleNameTb.Clear();
            lastNameTb.Clear();
            ageTb.Clear();
            GenderCB.SelectedIndex = -1;
            ProgramCb.SelectedIndex = -1;
            notifierTb.Visibility = Visibility.Collapsed;
        }
        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            FrmUpdateMember frmUpdateMember = new FrmUpdateMember(client);
            frmUpdateMember.ShowDialog();
        }  
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ClubMemberTable.Items.Clear();
            LoadDataFromDatabase();
        }
        private void studentIdTb_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z))
            {
                e.Handled = true;
            }
            else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else if (IsSpecialCharacter(e.Key))
            {
                e.Handled = true;
            }
        }

        private void lastNameTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z))
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = true;
            }
            else if (IsSpecialCharacter(e.Key))
            {
                e.Handled = true;
            }
        }

        private void ageTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z))
            {
                e.Handled = true;
            }
            else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else if (IsSpecialCharacter(e.Key))
            {
                e.Handled = true;
            }
        }

        private void firstNameTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z))
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = true;
            }
            else if (IsSpecialCharacter(e.Key))
            {
                e.Handled = true;
            }
        }

        private void middleNameTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z))
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = true;
            }
            else if (IsSpecialCharacter(e.Key))
            {
                e.Handled = true;
            }
        }
    }
}

public class ClubMembers
{
    public ObjectId _id {  get; set; }
    public long StudentID { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string Program { get; set; }
}
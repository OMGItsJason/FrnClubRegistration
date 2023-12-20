using MongoDB.Driver;
using System.Linq;
using System.Windows;

namespace FrnClubRegistration
{

    public partial class FrmUpdateMember : Window
    {
        private MongoClient client;
        public FrmUpdateMember(MongoClient mongoClient)
        {
            client = mongoClient;
            InitializeComponent();
            LoadStudentIdsFromDatabase();
        }

        private void LoadStudentIdsFromDatabase()
        {
            var database = client.GetDatabase("ClubRegistrationQuery");
            var collection = database.GetCollection<ClubMembers>("ClubRegistration");

            var studentIds = collection.Find(_ => true).ToList().Select(member => member.StudentID);

            foreach (var studentId in studentIds)
            {
                studentCB.Items.Add(studentId);
            }
        }

        private void studentCB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (long.TryParse(studentCB.SelectedItem?.ToString(), out long selectedStudentId))
            {
                UpdateStudentInformation(selectedStudentId);
            }
        }

        private void UpdateStudentInformation(long selectedStudentId)
        {
            var database = client.GetDatabase("ClubRegistrationQuery");
            var collection = database.GetCollection<ClubMembers>("ClubRegistration");

            var selectedStudent = collection.Find(s => s.StudentID == selectedStudentId).FirstOrDefault();

            if (selectedStudent != null)
            {
                lastNameTb.Text = selectedStudent.LastName;
                firstNameTb.Text = selectedStudent.FirstName;
                middleNameTb.Text = selectedStudent.MiddleName;
                ageTb.Text = selectedStudent.Age.ToString();
                GenderCB.SelectedItem = selectedStudent.Gender;
                programCB.SelectedItem = selectedStudent.Program;
            }
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
                programCB.Items.Add(ListOfProgram[i].ToString());
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

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedStudentId = studentCB.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedStudentId))
            {
                noti.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                noti.Visibility = Visibility.Collapsed;
            }

            if (!int.TryParse(ageTb.Text, out int updatedAge))
            {
                return;
            }

            string updatedProgram = programCB.SelectedItem?.ToString();

            var database = client.GetDatabase("ClubRegistrationQuery");
            var collection = database.GetCollection<ClubMembers>("ClubRegistration");

            var filter = Builders<ClubMembers>.Filter.Eq("StudentID", selectedStudentId);
            var update = Builders<ClubMembers>.Update
                .Set("Age", updatedAge)
                .Set("Program", updatedProgram);

            var result = collection.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                updated.Visibility = Visibility.Visible;

                UpdateStudentInformation(long.Parse(selectedStudentId));
                studentCB.SelectedIndex = -1;
                firstNameTb.Clear();
                middleNameTb.Clear();
                lastNameTb.Clear();
                ageTb.Clear();
                GenderCB.SelectedIndex = -1;
                programCB.SelectedIndex = -1;
            }
            else
            {
                return;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

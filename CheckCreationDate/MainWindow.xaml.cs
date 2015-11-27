using System;
using System.Windows;

namespace CheckCreationDate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string dbPath = "";
        string path = "";
        int checkedHour = 0;

        public MainWindow()
        {
            InitializeComponent();

            dbPath = getDbPath();

            if (didDBExist())
            {
                loadDB();
                checkDate();
                Environment.Exit(0);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (validateData() == false)
            {
                MessageBox.Show("Du lieu chua dung");
                return;
            }

            saveDb();
            checkDate();
            Environment.Exit(0);
        }

        void loadDB()
        {
            var content = System.IO.File.ReadAllLines(dbPath);
            path = content[0];

            int.TryParse(content[1], out checkedHour);

            btnPath.Content = path;
            txtCheckedDate.Text = content[1];
        }

        string[] splitDateString(string dateString)
        {
            var separation = new char[] { '/', ' ' };
            var dateElements = dateString.Split(separation);
            return dateElements;
        }

        bool didExist(string path)
        {
            return System.IO.File.Exists(path);
        }

        bool didDBExist()
        {
            return didExist(dbPath);
        }

        bool didFileExist()
        {
            return didExist(path);
        }

        DateTime getFileCreationDate()
        {
            return System.IO.File.GetCreationTime(path);
        }

        void checkDate()
        {
            DateTime creationDate = getFileCreationDate();

            if ((DateTime.Now - creationDate).TotalHours > checkedHour)
            {
                createFile();
            }
        }

        void saveDb()
        {
            dbPath = getDbPath();

            var content = new string[] { btnPath.Content.ToString(), txtCheckedDate.Text};
            System.IO.File.WriteAllLines(dbPath, content);
        }

        string getDbPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "\\db.dat"; ;
        }

        bool validateData()
        {
            if (path.Trim() == "")
            {
                return false;
            }

            if (!tryParse(txtCheckedDate.Text, out checkedHour))
            {
                return false;
            }
            
            return true;
        }

        bool tryParse(string numberToParse, out int number)
        {
            return int.TryParse(numberToParse, out number);
        }

        void createFile()
        {
            string fileName = "OK.txt";

            string okFilePath = System.IO.Path.GetFullPath(path).Replace(System.IO.Path.GetFileName(path), "");

            okFilePath = System.IO.Path.Combine(okFilePath, fileName);

            var content = prepareTextContent();

            System.IO.File.WriteAllText(okFilePath, content);

        }

        string prepareTextContent()
        {
            string content = "OK:" + path + Environment.NewLine;

            var createdDate = getFileCreationDate();
            content += "Ngày tạo: " + createdDate + Environment.NewLine;

            content += "Ngày kiểm tra: " + DateTime.Now + Environment.NewLine;

            content += "Số giờ tồn tại: " + (DateTime.Now - createdDate).TotalHours + Environment.NewLine;

            return content;
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
            open.ShowDialog();
            path = open.FileName;
            btnPath.Content = path;
        }
    }
}

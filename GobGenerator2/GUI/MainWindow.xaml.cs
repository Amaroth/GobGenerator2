using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using GobGenerator2.Core;

namespace GobGenerator2.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserSettings usi = UserSettings.Instance;
        CoreController core = CoreController.Instance;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads values into components according to saved/default user settings.
        /// </summary>
        internal void LoadSettings()
        {
            listfileBox.Text = usi.listfilePath;
            dbcBox.Text = usi.dbcPath;
            if (usi.exportM2)
            {
                if (usi.exportWMO)
                    bothRadio.IsChecked = true;
                else
                    m2Radio.IsChecked = true;
            }
            else if (usi.exportWMO)
                wmoRadio.IsChecked = true;

            hostBox.Text = usi.host;
            loginBox.Text = Utilities.ToInsecureString(usi.login);
            passBox.Password = Utilities.ToInsecureString(usi.password);
            savePassBox.IsChecked = usi.savePassword;
            databaseBox.Text = usi.database;
            tableBox.Text = usi.table;
            portBox.Value = usi.port;

            displayIDBox.Value = usi.startDisplayID;
            entryBox.Value = usi.baseEntry;
            prefixBox.Text = usi.prefix;
            postfixBox.Text = usi.postfix;
            insertRadio.IsChecked = usi.useInsert;
            replaceRadio.IsChecked = !usi.useInsert;
            avoidDuplicatesBox.IsChecked = usi.avoidDuplicates;

            minDisplayIDBox.Value = usi.minDisplayID;
            maxDisplayIDBox.Value = usi.maxDisplayID;
        }

        #region Button event handlers...
        private void generateButt_Click(object sender, RoutedEventArgs e)
        {
            core.Generate();
            displayIDBox.Value = core.SuggestStartDisplayID();
        }

        private void listfileButt_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if ((bool)ofd.ShowDialog())
                listfileBox.Text = ofd.FileName;
        }

        private void dbcButt_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "DBC files (*.dbc)|*.dbc|DB2 files (*.db2)|*.db2";
            if ((bool)ofd.ShowDialog())
                dbcBox.Text = ofd.FileName;
        }

        private void testButt_Click(object sender, RoutedEventArgs e)
        {
            core.TestConnection();
        }

        private void saveButt_Click(object sender, RoutedEventArgs e)
        {
            core.SaveUserSettings();
        }

        private void helpButt_Click(object sender, RoutedEventArgs e)
        {
            core.Help();
        }

        private void collisionButt_Click(object sender, RoutedEventArgs e)
        {
            core.CheckForCollisions();
        }

        private void syncButt_Click(object sender, RoutedEventArgs e)
        {
            core.DisplayIDToDB();
        }
        #endregion

        #region Checkbox and radio event handlers...
        private void avoidDuplicatesBox_Checked(object sender, RoutedEventArgs e)
        {
            usi.avoidDuplicates = (bool)avoidDuplicatesBox.IsChecked;
        }

        private void fileTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            usi.exportM2 = (bool)m2Radio.IsChecked || (bool)bothRadio.IsChecked;
            usi.exportWMO = (bool)wmoRadio.IsChecked || (bool)bothRadio.IsChecked;
        }

        private void insertRadio_Checked(object sender, RoutedEventArgs e)
        {
            usi.useInsert = (bool)insertRadio.IsChecked;
        }

        private void savePassBox_Checked(object sender, RoutedEventArgs e)
        {
            usi.savePassword = (bool)savePassBox.IsChecked;
        }
        #endregion

        #region TextBox event handlers...
        private void listfileBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.listfilePath = listfileBox.Text;
        }

        private void dbcBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.dbcPath = dbcBox.Text;
            displayIDBox.Value = core.SuggestStartDisplayID();
        }

        private void hostBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.host = hostBox.Text;
        }

        private void loginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.login = Utilities.ToSecureString(loginBox.Text);
        }

        private void passBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            usi.password = Utilities.ToSecureString(passBox.Password);
        }

        private void databaseBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.database = databaseBox.Text;
        }

        private void tableBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.table = tableBox.Text;
        }

        private void prefixBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.prefix = prefixBox.Text;
        }

        private void postfixBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            usi.postfix = postfixBox.Text;
        }
        #endregion

        #region IntegerUpDown event handlers...
        private void portBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            usi.port = (int)portBox.Value;
        }

        private void displayIDBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            usi.startDisplayID = (int)displayIDBox.Value;
        }

        private void entryBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            usi.baseEntry = (int)entryBox.Value;
        }

        private void minDisplayIDBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            usi.minDisplayID = (int)minDisplayIDBox.Value;
        }

        private void maxDisplayIDBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            usi.maxDisplayID = (int)maxDisplayIDBox.Value;
        }
        #endregion

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1)
            {
                if (System.IO.Path.GetExtension(files[0]).ToLower() == ".dbc")
                    dbcBox.Text = files[0];
                else
                    listfileBox.Text = files[0];
            }
        }
    }
}

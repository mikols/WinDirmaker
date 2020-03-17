using System.Collections.Generic;
using System;
using System.Windows;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;

namespace WpfDirMaker
{
    


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public char[] mFileNameSeparators = null;
        private List<MyTuple> mFileOrDirList = new List<MyTuple>();
        private List<MyTuple> mAllFilesOrDirsDatagridList = new List<MyTuple>();
        bool AutoStart = false;
        private int nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor = 2;
        private MyIO mIO = null;
        private Interpreter mInterpreter = null;

        public bool ShowDirs
        {
            get => mIO.ShowDirs;
            set => mIO.ShowDirs = value;
        }

        public bool FilterBýName
        {
            get => mIO.FilterByName;
            set => mIO.FilterByName = value;
        }

        /// <summary>
        /// CONSTRUCTOR - Konstruerar fönstret
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this.Title = "Version " + MyConstants.FileVersion + " (" + MyConstants.AssemblyVersion + ";  " + MyConstants.AssemblyLocation + ")";
                statusItemFileVer.Content = MyConstants.FileVersion;
                statusItemAssemblyVer.Content = MyConstants.AssemblyVersion + ", Built: " + MyConstants.FileVersionCommentBuildDate;
                statusItemLocation.Content = MyConstants.AssemblyLocation;
                statusItemTime.Content = DateTime.Now.ToString("yyyy-MM-dd  HH:mm");

                mIO = new MyIO();
                mInterpreter = new Interpreter(mIO);

                CheckBoxFilterDirFolders.DataContext = this;
                CheckBoxFilterByName.DataContext = this;


                textBoxSourceFolder.Text = mIO.FolderSourceRoot;
                if (textBoxSourceFolder.Text.Length == 0)
                {
                    string str2 = Directory.GetCurrentDirectory();//System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
                    textBoxSourceFolder.Text = System.IO.Path.GetDirectoryName(str2);
                }

                textBoxDestinationFolder.Text = mIO.FolderDestinationRoot;
                if (textBoxDestinationFolder.Text.Length == 0)
                {
                    textBoxDestinationFolder.Text = textBoxSourceFolder.Text;
                }

                //buttonSearch.IsEnabled = false;
                setMainDir();

                dataGridFiles.ItemsSource = mFileOrDirList;
                dataGridDirs.ItemsSource = mAllFilesOrDirsDatagridList;

                setDestinationDir();
                textBoxSearchForFileTypes.Text = "*.jpg";

                textBoxTOConvert.Text = ConfigurationManager.AppSettings["TestTnName"].ToString();

                var str = ConfigurationManager.AppSettings["Auto"].ToString();
                if (int.TryParse(str, out int nr))
                {
                    AutoStart = (nr == 1);
                }

                this.DataContext = mIO;
            }
            catch (Exception e)
            {
                labelSysMessage.Content = "MainWindow(): " + e.Message;
            }
        }

        /// <summary>
        /// When Window Is ACTIVATED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Activated(object sender, EventArgs e)
        {
            var str = ConfigurationManager.AppSettings["Auto"].ToString();
            if (int.TryParse(str, out int nr))
            {
                AutoStart = (nr == 1);
            }

            if (AutoStart)
            {
                try
                {
                    this.buttonPastePatToDestinationFolder_Click(sender, null);
                    this.buttonSearchForFilesInSourceDirectory_Click(sender, null);

                    this.buttonPasteTorr_Click(sender, null);
                }
                catch (Exception ex)
                {
                    labelSysMessage.Content = "Window_Activated: " + ex.Message;
                }
            }
        }


        #region SEARCH SETTINGS (TOP WINDOW)

        /// <summary>
        /// Clear searchTable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearItemsInSearchTable_Click(object sender, RoutedEventArgs e)
        {
            clearSearchTable();
        }

        /// <summary>
        /// Open Source Folder (... - button)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            if (mIO != null)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = mIO.FolderSourceRoot;
                process.Start();
            }
        }

        /// <summary>
        /// Copy Path from Source -> Destination
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCopyPathFromSourceToDestination_Click(object sender, RoutedEventArgs e)
        {
            textBoxDestinationFolder.Text = textBoxSourceFolder.Text;
            setDestinationDir();
        }

        /// <summary>
        /// Close WIndow - Save data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            SaveBeforeClosing();
            Close();

        }

        /// <summary>
        /// Open Destination Folder in Explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = mIO.FolderDestinationRoot;
                process.Start();
            }
            catch (Exception excep)
            {
                labelSysMessage.Content = "buttonOpenDestinationFolder_Click: " + excep.Message;
            }
        }

        /// <summary>
        /// Paste string in Clipboard to Destination Folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPastePatToDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strDirectory = Clipboard.GetData(DataFormats.Text) as string;
                if (strDirectory == null)
                    return;

                if (strDirectory.Length > 120)
                    strDirectory.Substring(0, 120);
                if (Directory.Exists(strDirectory))
                {
                    textBoxDestinationFolder.Text = strDirectory;
                    mIO.FolderDestinationRoot = textBoxDestinationFolder.Text;

                    buttonSearchForFilesInSourceDirectory_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                labelSysMessage.Content = "buttonPasteFolder_Click: " + ex.Message;
            }
        }

        /// <summary>
        /// Select Destination FOlder (... button)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectDestinationFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog =
            new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Välj Destinations folder! ",
                ShowNewFolderButton = true,
                SelectedPath = textBoxDestinationFolder.Text
            };
            //      dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            // dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxDestinationFolder.Text = dialog.SelectedPath;
        } // buttonSelectDestinationFolder_Click

        /// <summary>
        /// Välj ROOTBILBIOTEK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSelectSourceFolder2_Click(object sender, RoutedEventArgs e)
        {
            clearSearchTable();
            var selectedDir = OpenDirectorySelectionDialog(textBoxSourceFolder.Text, "Välj ROOT folder! ");
            if (string.IsNullOrEmpty(selectedDir))
                return;

            textBoxSourceFolder.Text = selectedDir;
            if (Directory.Exists(selectedDir))
                setMainDir();
        }

        /// <summary>
        /// SÖK IGENOM BIBLIOTEKET EFTER FILER AV TYPEN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearchForFilesInSourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mIO.FolderDestinationRoot = textBoxDestinationFolder.Text;
                mIO.FolderSourceRoot = textBoxSourceFolder.Text;
                mIO.WriteInitFile();
                setMainDir();
                setDestinationDir();

                clearSearchTable();

                if (!mIO.IsSourcePath())
                {
                    labelSysMessage.Content = "Inget källbibliotek";
                    return;
                }
                //getNameSeparators();

                char[] separator = { ';' };
                string[] strSuffixes;
                if (textBoxSearchForFileTypes.Text.Trim().Length == 0)
                {
                    strSuffixes = new string[1];
                    strSuffixes[0] = "*.*";
                }
                else
                    strSuffixes = textBoxSearchForFileTypes.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                DirectoryInfo MainDir = null;
                MainDir = new DirectoryInfo(mIO.FolderSourceRoot);
                // Lista filer      
                for (int i = 0; i < strSuffixes.Length; i++)
                {
                    foreach (FileInfo fi in MainDir.GetFiles(strSuffixes[i]))
                    {
                        MyTuple aTuple = new MyTuple(fi);
                        mFileOrDirList.Add(aTuple);
                    }
                }
                dataGridFiles.ItemsSource = mFileOrDirList;
                dataGridFiles.Items.Refresh();
                labelSearchInformation.Content = "Hittade " + mFileOrDirList.Count.ToString() + " st filer";
                buttonSkapaDir.IsEnabled = (mFileOrDirList.Count > 0);
            }
            catch (Exception ex)
            {
                labelSysMessage.Content = "buttonPasteFolder_Click: " + ex.Message;
                labelSearchInformation.Content = "Oj! Något gick fel! Kolla sökfunktionen.";
            }
        }

        private void TextBoxSourceFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setMainDir();
        }

        private void TextBoxDestinationFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setDestinationDir();
        }


        #endregion


        #region FLIK: MOVE FILES AND RENAME ----------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreateTvixJpg_Click(object sender, RoutedEventArgs e)
        {
            string message = "Bakgrundsfil på väg att skapas. {0}Vill du fortsätta";
            var res = MessageBox.Show(string.Format(message, "\r\n"), "Fråga", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
                return;

            string bgFileMall = MyConstants.PictureFileBackground();
            if (File.Exists(bgFileMall))
            {
              setDestinationDir();
                try
                {
                    string filedest = System.IO.Path.Combine(mIO.FolderDestinationRoot, "tvix.jpg");
                    File.Copy(bgFileMall, filedest);
                    labelMessageBgFile.Content = "Filen: " + filedest + " har skapats!";
                    labelMessageBgFile.Background = System.Windows.Media.Brushes.DarkGreen;
                    labelMessageBgFile.Foreground = System.Windows.Media.Brushes.LightGreen;

                }
                catch (Exception ex)
                {
                    labelMessageBgFile.Content = "Filen existerar redan! - " + ex.Message;
                }
            }
            else
            {
                labelMessageBgFile.Content = "Filen: " + bgFileMall + " existerar EJ!";
                labelMessageBgFile.Background = System.Windows.Media.Brushes.DarkRed;
                labelMessageBgFile.Foreground = System.Windows.Media.Brushes.Orange;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNumberedFileNames_Click(object sender, RoutedEventArgs e)
        {
            string strSourceFile, strDestFile, strFileName;
            int nr = 0;
            setMainDir();
            setDestinationDir();
            foreach (MyTuple aTuple in mFileOrDirList)
            {
                strSourceFile = System.IO.Path.Combine(mIO.FolderSourceRoot, aTuple.Name);
                strFileName = aTuple.Name.ToLower();
                if ((strFileName != "src.jpg") && (strFileName != "s.jpg") && (strFileName != "s0.jpg") && (strFileName != "s1.jpg") &&
                    (strFileName != "s2.jpg") && (strFileName != "s3.jpg") && (strFileName != "s4.jpg") && (strFileName != "s5.jpg") && (strFileName != "tvix.jpg"))
                {
                    int tryCopy = -1;
                    strDestFile = mIO.CreateNewDestinationFileName(textBoxFilenamePrefix.Text.Trim(), nr, textBoxFilenameSufffix.Text.Trim(), aTuple.Suffix);

                    tryCopy = mIO.CopyFile(strSourceFile, strDestFile);
                    nr++;

                    if (tryCopy == -1)
                        return;
                    else if (tryCopy == 0)
                    {
                        int tryAgain = 0;
                        while (tryAgain < 35)
                        {
                            // Försök med att inkrementera nummret
                            strDestFile = mIO.CreateNewDestinationFileName(textBoxFilenamePrefix.Text.Trim(), nr, textBoxFilenameSufffix.Text.Trim(), aTuple.Suffix);
                            tryCopy = mIO.CopyFile(strSourceFile, strDestFile);
                            nr++;
                            if (tryCopy == 0)
                                tryAgain++; // Prova med annat löpnummer.
                            else if (tryCopy == -1)
                                return;
                            else
                                tryAgain = 999; // Success! Lämna loopen
                        }
                    }

                    try
                    {
                        // Kopieringen lyckades! Ta bort filen fr source.        
                        File.Delete(strSourceFile);
                    }
                    catch (Exception exc)
                    {
                        labelSysMessage.Content = "buttonNumberedFileNames_Click: " + exc.Message + "\n\n" + "Kunde ej RADERA följande fil: " + "\n" + strSourceFile;
                    }
                }
            }
            labelMessage.Content = nr.ToString() + " filer har flyttats till: " + mIO.FolderDestinationRoot;
        }

        private void buttonCutTheCrapInterpret_Click(object sender, RoutedEventArgs e)
        {
            textBoxConverted.Text = mInterpreter.InterpretDottedString(textBoxTOConvert.Text, nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor);
            Clipboard.SetData(DataFormats.Text, textBoxConverted.Text);
        }

        private void buttonCutTheCrap_Click(object sender, RoutedEventArgs e)
        {
            textBoxConverted.Text = mInterpreter.CutTheCrap(textBoxTOConvert.Text);
            Clipboard.SetData(DataFormats.Text, textBoxConverted.Text);
        }

        private void buttonPasteTorr_Click(object sender, RoutedEventArgs e)
        {
            textBoxTOConvert.Text = mIO.GetClipboardString();
            buttonCutTheCrapInterpret_Click(this, null);
        }

        /// <summary>
        /// Convert to Title Case 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCamelcase_Click(object sender, RoutedEventArgs e)
        {
            var strConv = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(textBoxConverted.Text.ToLower());
            textBoxConverted.Text = strConv;

            var strToConv = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(textBoxTOConvert.Text.ToLower());
            textBoxTOConvert.Text = strToConv;
        }

        /// <summary>
        /// Convert to Upper case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUppercase_Click(object sender, RoutedEventArgs e)
        {
            textBoxConverted.Text = textBoxConverted.Text.ToUpper();
            textBoxTOConvert.Text = textBoxTOConvert.Text.ToUpper();

        }

        #endregion

        #region FLIK: DIRCREATOR ----------------------------------------------------

        private void buttonCreateNewDirectoryAndMoveFilesIntoIt_Click(object sender, RoutedEventArgs e)
        {
            TextBoxCreatedDirectories.Text = mIO.CreateNewDirectoryAndMoveFilesIntoIt(textBoxSourceDirMoveFiles.Text);
        }

        private void buttonSourceDir_Click(object sender, RoutedEventArgs e)
        {
            textBoxSourceDirMoveFiles.Text = mIO.GetClipboardString().Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSetMyPicturesFolder_Click(object sender, RoutedEventArgs e)
        {
            textBoxSourceFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            setMainDir();
        }

        #endregion

        #region FLIK TESTING - CHANGE FILENAMES

        /// <summary>
        /// Select Filename to be changed later in the process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectFileToChangeItsName_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                InitialDirectory = mIO.FolderDestinationRoot,
                DefaultExt = "*.*",
                Filter = "All Files (*.*)|*.*"
            };
            //        dlg.Filter = "JPG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                textBoxOriginalFileName.Text = filename;
            }
        }

        /// <summary>
        /// Create new Filename Based on old filename
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreateNewBasedOnOldNameFileName_Click(object sender, RoutedEventArgs e)
        {
            string strSourceFile = textBoxOriginalFileName.Text;

            DirectoryInfo MainDir = null;
            MainDir = new DirectoryInfo(mIO.FolderSourceRoot);
            FileInfo fi = new FileInfo(strSourceFile);
            string strNewName = fi.Name;

            if (strNewName.LastIndexOf('.') < 0)
                return;

            strNewName = strNewName.Substring(0, strNewName.LastIndexOf('.'));
            // Läs in namnseparatorn
            loadNameSeparators();
            for (int i = 0; i < mFileNameSeparators.Length; i++)
            {
                strNewName = strNewName.Replace(mFileNameSeparators[i], ' ');
            }
            if ((textBoxPrefix.Text.Trim()).Length > 1)
                strNewName = textBoxPrefix.Text.Trim() + " - " + strNewName.Trim();
            if ((textBoxSuffix.Text.Trim()).Length > 1)
                strNewName = strNewName + " - " + textBoxSuffix.Text.Trim();
            if (ComboType.Text.Length > 0)
                strNewName = strNewName + " " + ComboType.Text;
            textBoxNewFileName.Text = System.IO.Path.Combine(fi.DirectoryName, strNewName) + fi.Extension;
        }

        /// <summary>
        /// Change filename permanently (COMMENTED)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChangeNamePermanent_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(textBoxNewFileName.Text, "Ok?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    File.Copy(textBoxOriginalFileName.Text, textBoxNewFileName.Text);
                }
                catch (Exception ee)
                {
                    labelSysMessage.Content = "buttonChangeNamePermanent_Click" + ee.Message;
                    return;
                }
            }

            try
            {
                //File.Delete(textBoxOriginalFileName.Text);
                MessageBox.Show("buttonChangeNamePermanent_Click - Fakeradering av " + textBoxOriginalFileName.Text);
            }
            catch (Exception ee)
            {
                labelSysMessage.Content = "buttonChangeNamePermanent_Click: " + ee.Message;
                return;
            }
        }

        #endregion

        #region FLIK TESTING - CHANGE DIR NAMES

        /// <summary>
        /// Create Subdirectories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreateSubDirectories_Click(object sender, RoutedEventArgs e)
        {
            //labelResult.Content = "....väntar....";
            if (dataGridFiles.Items.Count == 0)
            {
                labelSysMessage.Content = "Inga filer hittades!\nProgramfel?!";
                return;
            }
            if (!setDestinationDir())
            {
                labelSysMessage.Content = "Inget Destinationsbibliotek!";
                return;
            }

            int iNrMoved = 0;
            string theName = "";
            string strNewDirName = "";
            for (int i = 0; i < dataGridFiles.Items.Count; i++)
            {
                var rowPost = mFileOrDirList[i];
                string strFileName = rowPost.Name.ToString();
                //string strFileName = dataGridFiles.ItemsSource[i].
                if (createNewName(strFileName, out theName, out strNewDirName))
                {
                    strNewDirName = System.IO.Path.Combine(theName, strNewDirName);
                    DirectoryInfo DestinationDir = new DirectoryInfo(mIO.FolderDestinationRoot);
                    DestinationDir.CreateSubdirectory(strNewDirName);
                    string strSourceFileName = mIO.FolderSourceRoot + "\\" + strFileName;
                    string strDestFileName = DestinationDir.FullName + "\\" + strNewDirName + "\\" + strFileName;
                    try
                    {
                        File.Move(strSourceFileName, strDestFileName);
                    }
                    catch (Exception ee)
                    {
                        labelSysMessage.Content =
                            "Kopiera\n" + strSourceFileName + "\ntill\n" +
                            strDestFileName + "\n\n GICK ÅT HELVETE!\n\nErrmsg: " + ee.Message;
                        return;
                    }
                    iNrMoved++;
                }
            }
            labelSysMessage.Content = "Flyttat : " + iNrMoved.ToString() + " st filer!";
        } 

        /// <summary>
        /// Add suffix to old directory name. SHow in table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRenameDirectorySuggestion_Click(object sender, RoutedEventArgs e)
        {
            buttonOkRename.IsEnabled = true;

            if (textBoxEnd.Text.Length == 0)
            {
                MessageBox.Show("Ingen text inskriven. Detta ger inga ändringar!");
            }

            if (Directory.Exists(textBoxSourceFolder.Text))
            {
                clearSearchTable();

                var strList = Directory.GetDirectories(textBoxSourceFolder.Text);
                var strIgnore = "]";
                var strNewName = "";
                foreach (var strOldName in strList)
                {
                    var idx = strOldName.LastIndexOf(strIgnore, StringComparison.CurrentCultureIgnoreCase);
                    strNewName = strOldName + textBoxEnd.Text;
                    if (idx > 0 && strOldName.Trim().Length == idx + 1)
                    {
                        strNewName = "";
                    }
                    MyTuple aTuple = new MyTuple(strOldName, "", strNewName, false, textBoxEnd.Text, "");
                    mFileOrDirList.Add(aTuple);
                }
                dataGridFiles.ItemsSource = mFileOrDirList;
                dataGridFiles.Items.Refresh();
                labelSearchInformation.Content = "Hittade " + mFileOrDirList.Count.ToString() + " st filer";
            }
        }

        /// <summary>
        /// Add suffix to every dirname as a suggestion only . shown in SearchTable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPreviewAddSuffixToEveryDirShownInTable_Click(object sender, RoutedEventArgs e)
        {
            var strNewDirName = "";
            var strOldDirName = "";
            foreach (var strItem in mFileOrDirList)
            {
                try
                {
                    strNewDirName = strItem.Name;
                    strOldDirName = strItem.DirectoryName;
                    if (Directory.Exists(strNewDirName) && (strNewDirName != strOldDirName) && (strOldDirName.Trim().Length > 0))
                        Directory.Move(strNewDirName, strOldDirName);
                }
                catch (Exception exc)
                {
                    labelSysMessage.Content = "buttonOkRename_Click ERROR: " + exc.Message.ToString();
                    return;
                }
            }
            clearSearchTable();

            var strList = Directory.GetDirectories(textBoxSourceFolder.Text);
            foreach (var strName in strList)
            {
                MyTuple aTuple = new MyTuple(strName, "", "Ändrat!", false, textBoxEnd.Text, "");
                mFileOrDirList.Add(aTuple);
            }
            dataGridFiles.ItemsSource = mFileOrDirList;
            dataGridFiles.Items.Refresh();
            labelSearchInformation.Content = "Hittade " + mFileOrDirList.Count.ToString() + " st filer";
            buttonOkRename.IsEnabled = false;
        }

        #endregion


        /// <summary>
        /// SEARCH : Use Nameseparators
        /// </summary>
        /// <param name="filename"></param>
        private void loadNameSeparators()
        {
            if (textBoxSearchSeparator.Text.Trim().Length == 0)
            return;
            char[] sep = {';'};
            string[] separators = textBoxSearchSeparator.Text.Trim().Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (separators.Length>0)
            {
                char[] tmp = new char[separators.Length]; 
                for (int i = 0; i<separators.Length; i++)
                {
                    tmp[i]  = separators[i][0];
                }
                mFileNameSeparators = tmp;
            }
        } 

        /// <summary>
        /// Sätt källbibliotek
        /// </summary>
        /// <returns></returns>
        private bool setMainDir()
        {
            if (Directory.Exists(textBoxSourceFolder.Text))
            {
                mIO.FolderSourceRoot = textBoxSourceFolder.Text;
                try
                {
                    Directory.SetCurrentDirectory(mIO.FolderSourceRoot);
                }
                catch
                {
                    labelSysMessage.Content = "setMainDir(): Ogiltigt bibliotek. Kanske felaktiga rättigheter.";
                    Directory.SetCurrentDirectory("C:\\");
                }
            }
            return buttonSearch.IsEnabled;
        } // setMainDir

        /// <summary>
        /// Sätt destinationsbibliotek
        /// </summary>
        /// <returns></returns>
        private bool setDestinationDir()
        {
            if (Directory.Exists(textBoxDestinationFolder.Text))
            {
                mIO.FolderDestinationRoot = textBoxDestinationFolder.Text;
                return true;
            }
            else
            {
                mIO.FolderDestinationRoot = "C:\\";
            }
          return false;
        } // setDestinationDir

        /// <summary>
        /// Create New Name
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        private bool createNewName(string strFileName, out string TheName, out string strNewFolderName)
        {
            strNewFolderName = "";
            TheName = "";
            string fileNameWithoutSuffix = "";
            bool bSuccess = false;
            if (File.Exists(strFileName))
            {
                fileNameWithoutSuffix = strFileName.Substring(0, strFileName.LastIndexOf('.'));
                // Läs in namnseparatorn
                loadNameSeparators();
                string[] strs = fileNameWithoutSuffix.Split(mFileNameSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length > 0)
                    TheName = strs[0];

                string strMiddle = "";
                if (textBoxMiddle.Text.Trim().Length > 0)
                    strMiddle = " " + textBoxMiddle.Text;

                string strEnd = "";
                if (textBoxEnd.Text.Trim().Length > 0)
                    strEnd = " " + textBoxEnd.Text;

                if (strs.Length == 1)
                {
                    strNewFolderName = TheName;
                }
                else if (strs.Length == 2)
                {
                    strNewFolderName =
                    string.Format("{0} - {1}{2}{3}", TheName, strs[1].Trim(), strMiddle, strEnd);
                }
                else if (strs.Length == 3)
                {
                    strNewFolderName = string.Format("{0} - {1}{2} {3}{4}", TheName, strs[1], strMiddle, strs[2], strEnd);
                }
            }
            return bSuccess;
        }


        private void clearSearchTable()
        {
            mFileOrDirList.Clear();
            dataGridFiles.Items.Refresh();
            labelSearchInformation.Content = " - ";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveBeforeClosing();
        }

        private void SaveBeforeClosing()
        {
            mIO.WriteInitFile();
        }

        #region buttonprio clicked
        private void btnPrio0_Click(object sender, RoutedEventArgs e)
        {
            tbPrio0.Text = tbPrio1.Text;
            tbPrio1.Text = tbPrio2.Text;
            tbPrio2.Text = tbPrio3.Text;
            tbPrio3.Text = tbPrio4.Text;
            tbPrio4.Text = tbPrio5.Text;
            tbPrio5.Text = tbPrio6.Text;
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio1_Click(object sender, RoutedEventArgs e)
        {
            tbPrio1.Text = tbPrio2.Text;
            tbPrio2.Text = tbPrio3.Text;
            tbPrio3.Text = tbPrio4.Text;
            tbPrio4.Text = tbPrio5.Text;
            tbPrio5.Text = tbPrio6.Text;
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio2_Click(object sender, RoutedEventArgs e)
        {
            tbPrio2.Text = tbPrio3.Text;
            tbPrio3.Text = tbPrio4.Text;
            tbPrio4.Text = tbPrio5.Text;
            tbPrio5.Text = tbPrio6.Text;
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio3_Click(object sender, RoutedEventArgs e)
        {
            tbPrio3.Text = tbPrio4.Text;
            tbPrio4.Text = tbPrio5.Text;
            tbPrio5.Text = tbPrio6.Text;
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio4_Click(object sender, RoutedEventArgs e)
        {
            tbPrio4.Text = tbPrio5.Text;
            tbPrio5.Text = tbPrio6.Text;
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio5_Click(object sender, RoutedEventArgs e)
        {
            tbPrio5.Text = tbPrio6.Text;
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio6_Click(object sender, RoutedEventArgs e)
        {
            tbPrio6.Text = tbPrio7.Text;
            tbPrio7.Text = " ";
        }

        private void btnPrio7_Click(object sender, RoutedEventArgs e)
        {
            tbPrio7.Text = " ";
        }

        private void RbOneName_Click(object sender, RoutedEventArgs e)
        {
            nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor = 1;
            buttonCutTheCrapInterpret_Click(sender, e);
        }

        private void RbTwoNames_Click(object sender, RoutedEventArgs e)
        {
            nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor = 2;
            buttonCutTheCrapInterpret_Click(sender, e);
        }

        private void RbThreeNames_Click(object sender, RoutedEventArgs e)
        {
            nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor = 3;
            buttonCutTheCrapInterpret_Click(sender, e);
        }

        #endregion

        private void ButtonLoadErrors_Click(object sender, RoutedEventArgs e)
        {
            if (mIO.ErrorLog.Count == 0)
            {
                ListBoxErrors.Items.Add("Log is empty!");
                return;
            }

            for (int i = 0; i < mIO.ErrorLog.Count; i++)
            {
                ListBoxErrors.Items.Add(mIO.ErrorLog[i]);
            }
        }

        private void ButtonClearErrors_Click(object sender, RoutedEventArgs e)
        {
            mIO.ErrorLog.Clear();
            ListBoxErrors.Items.Add("Log was Cleared!");
        }
    }




}

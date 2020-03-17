using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using System.Configuration;

namespace WpfDirMaker
{
    public class MyIO
    {
        private string mFolderSource = "";
        private string mFolderDestination = "";

        public static string _resolution1;
        public static string _resolution2;
        public static string _resolution3;
        public static string _resolution4;
        public static string _resolution5;
        public static string _resolution6;
        public static string _resolution7;
        public static string _resolution8;

        public List<string> RemoveShitList;
        public List<string> ErrorLog = new List<string>();
        public string RemoveShitString
        {
            get
            {
                if (RemoveShitList != null)
                    return string.Join(",", RemoveShitList);
                return "";
            }
            set
            {
                RemoveShitList = value.Split(',').ToList();
            }
        }

        public static string _AND;

        public string Priority1 { get; set; }
        public string Priority2 { get; set; }
        public string Priority3 { get; set; }
        public string Priority4 { get; set; }
        public string Priority5 { get; set; }
        public string Priority6 { get; set; }
        public string Priority7 { get; set; }
        public string Priority8 { get; set; }
        public string Priority9 { get; set; }

        public string SearchDir1 { get; set; }
        public string SearchDir2 { get; set; }
        public string SearchDir3 { get; set; }
        public string SearchDir4 { get; set; }
        public string SearchDir5 { get; set; }
        public string SearchDir6 { get; set; }
        public string SearchDir7 { get; set; }
        public string SearchDir8 { get; set; }

        public string ExcludedFileTypes { get; set; } // ".jpg;.bmp;.png;.gif;.jpeg" 
        public string SearchPattern { get; set; }

        public List<MyTuple> mCacheListOfAllSearchedDirsAndFiles = new List<MyTuple>();
        public List<MyTuple> mCacheListOfAllSearchedDirsAndFilesOrdered = new List<MyTuple>();

        public bool ShowDirs = true;
        public bool FilterByName = true;

        // Sökvägar och filnamn till diverse INI och TXT(resultat) filer i CAT
        public string FolderSourceRoot
        {
            get { return mFolderSource; }
            set { mFolderSource = value; }
        }

        public string FolderDestinationRoot
        {
            get { return mFolderDestination; }
            set { mFolderDestination = value; }
        }

        public string FilePathDirmakerXML
        {
            get
            {
                return System.IO.Path.Combine(MyConstants.AssemblyLocation, MyConstants.cFileDirmakerXML);
            }
        }

        public MyIO()
        {
            if (ReadAppConfig())
                ReadInitFile();
        }


        public bool IsSourcePath()
        {
            return ((mFolderSource != "") && (Directory.Exists(mFolderSource)));
        }

        public bool IsDestinationPath()
        {
            return ((mFolderDestination != "") && (Directory.Exists(mFolderDestination)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CopyFile(string strSourceFile, string strDestFile)
        {
            try
            {
                MyConstants.FullPathSplit(strSourceFile, out string path, out string srcname, out string suffix);
                MyConstants.FullPathSplit(strDestFile, out path, out string destname, out suffix);
                if (destname != srcname)
                    File.Copy(strSourceFile, strDestFile, false);
                else
                    return 0;
            }
            catch (PathTooLongException pathTooLong)
            {
                MessageBox.Show(pathTooLong.Message + "\n\n" + "För många tecken i ngn sökväg! ");
                return -1;
            }
            catch (FileNotFoundException fileNotFound)
            {
                MessageBox.Show(fileNotFound.Message + "\n\n" + "Källfil kunde ej hittas! " + "\n" + strSourceFile);
                return -1;
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                MessageBox.Show(dirNotFound.Message + "\n\n" + "Käll och/eller Dest Bibliotek kunde ej hittas! ");
                return -1;
            }
            catch (UnauthorizedAccessException unauthorized)
            {
                MessageBox.Show(unauthorized.Message + "\n\n" + "Felaktiga rättigheter! " + "\n" + strSourceFile);
                return -1;
            }
            catch (IOException copyError)
            {
                // Catch exception if the file was already copied.
                return 0;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message + "\n\n" + "Kunde ej kopiera/flytta följande fil: " + "\n" + strSourceFile);
                return -1;
            }
            return 1;
        }

        public string CreateNewDestinationFileName(string pre, int nr, string post, string suffix)
        {
            string newName = "";
            string strName = nr.ToString();
            if (strName.Length == 1)
                strName = "0" + strName;

            if (pre.Trim().Length > 0)
                strName = pre.Trim() + strName;
            if (post.Trim().Length > 0)
                strName = strName + post.Trim();

            newName = System.IO.Path.Combine(FolderDestinationRoot, strName + suffix);

            return newName;
        }

        public bool CreateNewSubDirectory(string rootDir, string name, out string newlyCreatedDir)
        {
            newlyCreatedDir = "";
            if (!Directory.Exists(rootDir))
                return false;

            if (string.IsNullOrEmpty(name))
                return false;

            try
            {
                DirectoryInfo DestinationDir = new DirectoryInfo(rootDir);
                DirectoryInfo di = DestinationDir.CreateSubdirectory(name);
                newlyCreatedDir = di.FullName;
            }
            catch (Exception ex)
            {
                LogError("CreateNewSubDirectory(): " + ex.Message);
                return false;
            }
            return true;
        }

        public string GetClipboardString()
        {
            try
            {
                string strToName = Clipboard.GetData(DataFormats.Text) as string;
                if (strToName == null)
                    return "";

                if (strToName.Length > 120)
                    strToName.Substring(0, 120);
                return strToName;
            }
            catch (Exception ex)
            {
                return "buttonPasteFolder_Click: " + ex.Message;
            }
        }

        public int LoadListOfAllSearchedDirsAndFiles()
        {
            mCacheListOfAllSearchedDirsAndFiles.Clear();
            var searchPattern = "*";
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir1, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir2, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir3, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir4, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir5, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir6, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir7, searchPattern);
            FindAndAddFoundDirectories(ref mCacheListOfAllSearchedDirsAndFiles, SearchDir8, searchPattern);
            return mCacheListOfAllSearchedDirsAndFiles.Count;
        }

        public int NrOfItemsInCache()
        {
            if (mCacheListOfAllSearchedDirsAndFiles == null)
                return 0;
            return mCacheListOfAllSearchedDirsAndFiles.Count;
        }

        public bool FilterList(ref List<MyTuple> dataGridList, bool findDuplicates)
        {
            // FILTRERA DÅ!
            if (findDuplicates)
                FindDuplicates(ref dataGridList, SearchPattern);
            else
                FindAndAddFoundDirectoriesFromCache(ref dataGridList, SearchPattern);
            return (dataGridList.Count > 0);
        }

        private bool IsPostsEqual(MyTuple e0, MyTuple e1)
        {
            if (FilterByName)
            {
                return e1.Name.ToUpper() == e0.Name.ToUpper();
                //return e1.Name.ToUpper().Contains(e0.Name.ToUpper()/*.CutStr(40)*/);
            }

            return (e1.Size == e0.Size);
        }

        private bool FindDuplicates(ref List<MyTuple> resultSearchList, string SearchPattern)
        {
            if (mCacheListOfAllSearchedDirsAndFiles == null)
                return false;
            if (mCacheListOfAllSearchedDirsAndFiles.Count == 0)
                return false;
            if (mCacheListOfAllSearchedDirsAndFilesOrdered == null)
                mCacheListOfAllSearchedDirsAndFilesOrdered = new List<MyTuple>();
            mCacheListOfAllSearchedDirsAndFilesOrdered.Clear();
            
            // Order list of dirs and files
            if (mCacheListOfAllSearchedDirsAndFilesOrdered.Count == 0)
            {

                if (FilterByName)
                {
                    mCacheListOfAllSearchedDirsAndFilesOrdered
                        .AddRange(mCacheListOfAllSearchedDirsAndFiles
                        .OrderBy(q => q.Name));
                }
                else
                {
                    mCacheListOfAllSearchedDirsAndFilesOrdered
                        .AddRange(mCacheListOfAllSearchedDirsAndFiles
                        .OrderBy(q => q.Size));
                }
            }

            // Check if we want to show Directories
            if (!ShowDirs)
                mCacheListOfAllSearchedDirsAndFilesOrdered = 
                    mCacheListOfAllSearchedDirsAndFilesOrdered.Where(e => !e.IsDirectory()).ToList();
            
            // Show only duplicates, by name or by size
            resultSearchList.Clear();
            var lastInserted = "";
            for (int i = 0; i <  mCacheListOfAllSearchedDirsAndFilesOrdered.Count; i++)
            {
                if (i>0)
                {
                    var myTupleElement1 = mCacheListOfAllSearchedDirsAndFilesOrdered[i];
                    var myTupleElement0 = mCacheListOfAllSearchedDirsAndFilesOrdered[i-1];
                    if (IsPostsEqual(myTupleElement0, myTupleElement1))
                    {
                        lastInserted = myTupleElement0.Name;
                        resultSearchList.Add(myTupleElement0);
                        // if last post is duplicate - add the last post too to resultList
                        if (i == mCacheListOfAllSearchedDirsAndFilesOrdered.Count - 1)
                            resultSearchList.Add(myTupleElement1);
                    }
                    else
                    {
                        if (lastInserted.Length > 0)
                            resultSearchList.Add(myTupleElement0);
                        lastInserted = "";
                    }
                }
            }

            return (resultSearchList.Count > 0);
        }


        private bool FindAndAddFoundDirectoriesFromCache(ref List<MyTuple> resultSearchList, string SearchPattern)
        {
            if (mCacheListOfAllSearchedDirsAndFiles == null)
                return false;
            if (mCacheListOfAllSearchedDirsAndFiles.Count == 0)
                return false;
            if (resultSearchList == null)
                resultSearchList = new List<MyTuple>();
            resultSearchList.Clear();


            if (SearchPattern == "*")
                resultSearchList.AddRange(mCacheListOfAllSearchedDirsAndFiles.ToList());
            else
                resultSearchList.AddRange(
                    mCacheListOfAllSearchedDirsAndFiles
                        .Where(a => a.FullName.ToUpper().Contains(SearchPattern.ToUpper())).ToList()
                );

            // Check if we want to show Directories
            if (!ShowDirs)
                resultSearchList =
                    resultSearchList.Where(e => !e.IsDirectory()).ToList();

            return (resultSearchList.Count > 0);
        }

        private bool FindAndAddFoundDirectories(ref List<MyTuple> resultSearchList, string path, string searchPattern)
        {
            var result = false;
            if (!Directory.Exists(path))
            {
                return false;
            }

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(searchPattern))
                return result;     // Empty list

            if (!Directory.Exists(path))
                return result;     // Empty list
            var excludedStringsList = ExcludedFileTypes.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            List<string> listOfAllDirectories = new List<string>();
            int counter = 0;
            try
            {
                // Get all directories except excluded ones
                listOfAllDirectories = Directory.GetDirectories(path, searchPattern, SearchOption.AllDirectories).ToList();

                listOfAllDirectories.ExcludeItemsInList(excludedStringsList);
                listOfAllDirectories.Sort();
                if (listOfAllDirectories != null && listOfAllDirectories.Count > 0)
                {
                    foreach (var dir in listOfAllDirectories) // loop directories
                    {
                        // Hämta filer i subbibliotek
                        //var _TempListOfFilesInSearchedDirectory = Directory.GetFiles(dir).ToList();
                        var _TempListOfFilesInSearchedDirectory = new List<string>();
                        try
                        {
                            _TempListOfFilesInSearchedDirectory = Directory.EnumerateFiles(dir).ToList();
                        }
                        catch (Exception e)
                        {
                            LogError("FindAndAddFoundDirectories(): " + e.Message);
                            continue;
                        }
                        resultSearchList.Add(new MyTuple(dir.GetFileNameFromPath(), dir, true, "", ""));
                        foreach (var fileName in _TempListOfFilesInSearchedDirectory)
                        {
                            counter++;
                            if (fileName.Length > 250)
                            {
                                LogError("FindAndAddFoundDirectories(): Too long filename: " + fileName);
                                continue;
                            }
                            bool containsExcludedString = excludedStringsList.Exists(w => fileName.ToUpper().Contains(w.ToUpper()));
                            if (!containsExcludedString)
                            {
                                FileInfo fi = new FileInfo(fileName);
                                resultSearchList.Add(
                                    new MyTuple(
                                        fileName.GetFileNameFromPath(),
                                        fileName.ToString(),
                                        "",
                                        false,
                                        fileName.GetSuffixFromPath(),
                                        fi.Length.ToString()));
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException uaec)
            {
                MessageBox.Show(uaec.Message);
                return false;
            }
            catch (Exception e)
            {
                LogError("FindAndAddFoundDirectories(): : "  + e.Message);
                return false;
            }
            return result;
        }

        public bool MoveFile(string file, string sourceDir, string destinationDir)
        {
            string sourceFile = Path.Combine(sourceDir, file);
            string destFile = Path.Combine(destinationDir, file);

            if (File.Exists(destFile))
            {
                var strUniqueSufffix = $@"{DateTime.Now.Ticks}";
                destFile += "." + strUniqueSufffix + ".BCK";
            }

            if (File.Exists(sourceFile) && Directory.Exists(destinationDir))
            {
                File.Move(sourceFile, destFile);
                return true;
            }
            return false;
        }

        public string CreateNewDirectoryAndMoveFilesIntoIt(string sourceDirMoveFiles)
        {
            string statusString;
            int counter = 0;
            if (Directory.Exists(sourceDirMoveFiles))
            {
                string[] listOfStrings = { ".wmv", ".mkv", ".avi", ".mp4", ".mpg", ".flv", ".m4v" };
                string[] filesToBeMoved = new string[1];
                filesToBeMoved[0] = "*.*";

                DirectoryInfo MainDir = null;
                MainDir = new DirectoryInfo(sourceDirMoveFiles);
                // Lista filer      
                for (int i = 0; i < filesToBeMoved.Length; i++)
                {
                    foreach (FileInfo fi in MainDir.GetFiles(filesToBeMoved[i]))
                    {
                        bool found = (listOfStrings.Where(suffix => fi.Extension.EndsWith(suffix)).FirstOrDefault() != null);
                        if (!found)
                        {
                            continue;
                        }

                        string newDir = "";
                        string strDirNameWithoutPath = fi.Name.Substring(0, fi.Name.LastIndexOf('.')).Replace(".", " ");
                        if (CreateNewSubDirectory(sourceDirMoveFiles, strDirNameWithoutPath, out newDir))
                        {
                            if (MoveFile(fi.Name, sourceDirMoveFiles, newDir))
                            {
                                counter++;
                            }
                        }
                    }
                }
                statusString = "Flyttade " + counter + " filer till subdirs i " + sourceDirMoveFiles + ".";
            }
            else
            {
                statusString = "FAILURE";
            }
            return statusString;
        }


        #region InitFile Handling

        public bool ReadAppConfig()
        {
            try
            {
                _resolution1 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution1].ToString();
                _resolution2 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution2].ToString();
                _resolution3 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution3].ToString();
                _resolution4 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution4].ToString();
                _resolution5 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution5].ToString();
                _resolution6 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution6].ToString();
                _resolution7 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution7].ToString();
                _resolution8 = ConfigurationManager.AppSettings[MyConstants.cFileTagResolution8].ToString();

                _AND = ConfigurationManager.AppSettings[MyConstants.cExtra1].ToString();
            }
            catch (Exception ex)
            {
                LogError("ERROR! Bad App Config file: " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Läs användardata/Inställningar från dirmaker.Xml. 
        /// </summary>
        /// <returns>true om filskrivning skedde, annars false</returns>
        public bool ReadInitFile()
        {
            if (!File.Exists(FilePathDirmakerXML))
                return false;

            try
            {
                XDocument doc = XDocument.Load(FilePathDirmakerXML);
                FolderSourceRoot = (from dirRoot in doc.Descendants(MyConstants.cFileTagSourceRootFolder)
                                select (string)dirRoot.Value).First();

                var result = (from query in doc.Elements()
                                select new
                                {
                                    folderSourceRoot = query.Element(MyConstants.cFileTagSourceRootFolder).Value,
                                    folderDestinationRoot = query.Element(MyConstants.cFileTagDestinationRootFolder).Value,
                                    priority1 = query.Element(MyConstants.cPriorityWork1).Value,
                                    priority2 = query.Element(MyConstants.cPriorityWork2).Value,
                                    priority3 = query.Element(MyConstants.cPriorityWork3).Value,
                                    priority4 = query.Element(MyConstants.cPriorityWork4).Value,
                                    priority5 = query.Element(MyConstants.cPriorityWork5).Value,
                                    priority6 = query.Element(MyConstants.cPriorityWork6).Value,
                                    priority7 = query.Element(MyConstants.cPriorityWork7).Value,
                                    priority8 = query.Element(MyConstants.cPriorityWork8).Value,
                                    searchDir1 = query.Element(MyConstants.cSearchDirTag1).Value,
                                    searchDir2 = query.Element(MyConstants.cSearchDirTag2).Value,
                                    searchDir3 = query.Element(MyConstants.cSearchDirTag3).Value,
                                    searchDir4 = query.Element(MyConstants.cSearchDirTag4).Value,
                                    searchDir5 = query.Element(MyConstants.cSearchDirTag5).Value,
                                    searchDir6 = query.Element(MyConstants.cSearchDirTag6).Value,
                                    searchDir7 = query.Element(MyConstants.cSearchDirTag7).Value,
                                    searchDir8 = query.Element(MyConstants.cSearchDirTag8).Value,
                                    excludedFileTypes = query.Element(MyConstants.cExcludedFileTypes).Value,
                                    removeThisShit = query.Element(MyConstants.cRemoveThisShit).Value
                                }).FirstOrDefault();

                if (result != null)
                {
                    FolderSourceRoot = result.folderSourceRoot;
                    FolderDestinationRoot = result.folderDestinationRoot;
                    Priority1 = result.priority1;
                    Priority2 = result.priority2;
                    Priority3 = result.priority3;
                    Priority4 = result.priority4;
                    Priority5 = result.priority5;
                    Priority6 = result.priority6;
                    Priority7 = result.priority7;
                    Priority8 = result.priority8;

                    SearchDir1 = result.searchDir1;
                    SearchDir2 = result.searchDir2;
                    SearchDir3 = result.searchDir3;
                    SearchDir4 = result.searchDir4;
                    SearchDir5 = result.searchDir5;
                    SearchDir6 = result.searchDir6;
                    SearchDir7 = result.searchDir7;
                    SearchDir8 = result.searchDir8;
                    ExcludedFileTypes = result.excludedFileTypes;
                    SearchPattern = "*";
                    if (result.removeThisShit.Length > 0)
                    {
                        RemoveShitString = result.removeThisShit;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError("ReadInitFile(): " + ex.Message);
                return false;
            }

            return false;
        }

        private void LogError(string message)
        {
            MessageBox.Show(message);
            ErrorLog.Add(message);
        }

        /// <summary>
        /// Skapa/Ändra inställningar dirmaker.XML
        /// </summary>
        /// <returns></returns>
        public bool WriteInitFile()
        {
            bool retval = true;

            //mFolderSource = sourceFile;
            //mFolderDestination = destFile;
            if (IsSourcePath())
            {
                // Skriv till XML
                System.Xml.Linq.XDocument xdoc = new System.Xml.Linq.XDocument(new System.Xml.Linq.XComment("Inställningar för Dirmaker"));
                System.Xml.Linq.XElement xmlroot = new System.Xml.Linq.XElement("Dirmaker");

                xmlroot.Add(
                    new XElement("Version", MyConstants.FileVersion),
                    new XElement(MyConstants.cFileTagSourceRootFolder, mFolderSource),
                    new XElement(MyConstants.cFileTagDestinationRootFolder, mFolderDestination),
                    new XElement(MyConstants.cPriorityWork1, Priority1),
                    new XElement(MyConstants.cPriorityWork2, Priority2),
                    new XElement(MyConstants.cPriorityWork3, Priority3),
                    new XElement(MyConstants.cPriorityWork4, Priority4),
                    new XElement(MyConstants.cPriorityWork5, Priority5),
                    new XElement(MyConstants.cPriorityWork6, Priority6),
                    new XElement(MyConstants.cPriorityWork7, Priority7),
                    new XElement(MyConstants.cPriorityWork8, Priority8),
                    new XElement(MyConstants.cSearchDirTag1, SearchDir1),
                    new XElement(MyConstants.cSearchDirTag2, SearchDir2),
                    new XElement(MyConstants.cSearchDirTag3, SearchDir3),
                    new XElement(MyConstants.cSearchDirTag4, SearchDir4),
                    new XElement(MyConstants.cSearchDirTag5, SearchDir5),
                    new XElement(MyConstants.cSearchDirTag6, SearchDir6),
                    new XElement(MyConstants.cSearchDirTag7, SearchDir7),
                    new XElement(MyConstants.cSearchDirTag8, SearchDir8),
                    new XElement(MyConstants.cExcludedFileTypes, ExcludedFileTypes),
                    new XElement(MyConstants.cRemoveThisShit, RemoveShitString)
                   );
                xdoc.Add(xmlroot);
                xdoc.Save(FilePathDirmakerXML);
            }
            return retval;
        }
        #endregion
    }
}

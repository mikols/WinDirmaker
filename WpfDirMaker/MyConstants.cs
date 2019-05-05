using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfDirMaker
{
    static public class MyConstants
    {
        #region Constants:
        //
        // Standardfil för att minnas inställningar fr tidigare körningar
        public const string cFileDirmakerXML = "Dirmaker.XML";

        public const string cFileTagVersion = "Version";
        public const string cFileTagAssemblyVersion = "AssemblyVersion";
        public const string cFileTagSourceRootFolder = "SourceRootFolder";
        public const string cFileTagDestinationRootFolder = "DestinationRootFolder";

        public const string cFileTagResolution1 = "Res1";
        public const string cFileTagResolution2 = "Res2";
        public const string cFileTagResolution3 = "Res3";
        public const string cFileTagResolution4 = "Res4";
        public const string cFileTagResolution5 = "Res5";
        public const string cFileTagResolution6 = "Res6";
        public const string cFileTagResolution7 = "Res7";
        public const string cFileTagResolution8 = "Res8";

        

        public const string cExtra1 = "Extra1";

        public const string cPriorityWork1 = "Priority1";
        public const string cPriorityWork2 = "Priority2";
        public const string cPriorityWork3 = "Priority3";
        public const string cPriorityWork4 = "Priority4";
        public const string cPriorityWork5 = "Priority5";
        public const string cPriorityWork6 = "Priority6";
        public const string cPriorityWork7 = "Priority7";
        public const string cPriorityWork8 = "Priority8";

        public const string cSearchDirTag1 = "SearchDir1";
        public const string cSearchDirTag2 = "SearchDir2";
        public const string cSearchDirTag3 = "SearchDir3";
        public const string cSearchDirTag4 = "SearchDir4";
        public const string cSearchDirTag5 = "SearchDir5";
        public const string cSearchDirTag6 = "SearchDir6";
        public const string cSearchDirTag7 = "SearchDir7";
        public const string cSearchDirTag8 = "SearchDir8";

        public const string cExcludedFileTypes = "ExcludedFileTypes";
        public const string cRemoveThisShit = "RemoveThisShit";


        // Folder där externa program ligger under programmets root bibliotek
        public const string cFolderBinary = "Bin";
        public const string cFolderData = "Data";
        public const string cFileTagErrorLogFile = "ERROR_LOG_FILE";
        public const string cFileErrorLog = "Error.log";
        //
        // Filsuffix Polyamp
        public const string cBinaryMeasureFileSuffix = "udasdata";
        public const string cBinaryMeasureFileSuffix2 = "SweMeasTurn";

        public const string cPictureFileBackground = "bg\\background.jpg";

        #endregion


        #region Statiska Metoder:

        /// <summary>
        /// Returnera Assemblyversionsnummer
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}.{2}.{3}",
                            version.Major.ToString(),
                            version.Minor.ToString(),
                            version.Build.ToString(),
                            version.Revision.ToString());
            }
        } // AssemblyVersion

        /// <summary>
        /// Returnera Assemblyns folder
        /// </summary>
        public static string AssemblyLocation
        {
            get
            {
                string str2 = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
                return System.IO.Path.GetDirectoryName(str2);
            }
        } // AssemblyLocation

        /// <summary>
        /// Returnera Filversionsnummer
        /// </summary>
        public static string FileVersion
        {
            get
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location);
                return String.Format("{0}.{1}.{2}", fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart);
            }
        } // FileVersion

        /// <summary>
        /// Splittar upp filnamn och sökväg i mindre beståndsdelar
        /// </summary>
        /// <param name="indata">Hela sökvägen + filnamn</param>
        /// <param name="path">Endast sökväg</param>
        /// <param name="name">Endast filnamn</param>
        /// <param name="suffix">Endast suffix</param>
        public static void FullPathSplit(string indata, out string path, out string name, out string suffix)
        {
            path = ""; name = ""; suffix = "";
            if (indata.Length <= 3)
                return;

            FileInfo fi = new FileInfo(indata);
            int index = fi.Name.IndexOf('.');
            name = fi.Name.Substring(0, index);
            suffix = fi.Name.Substring(index);
            path = fi.DirectoryName;
            return;
        } // FullPathSplit

        public static string PictureFileBackground()
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            return System.IO.Path.Combine(str, cPictureFileBackground);
        }

        #endregion
    }
}

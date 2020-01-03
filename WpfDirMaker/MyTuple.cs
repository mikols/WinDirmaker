using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfDirMaker
{


    public class MyTuple
    {
        const string cDirectory = "Dir";
        public MyTuple()
        {

        }
        public MyTuple(string name, string fullName, bool isDirectory, string suffix, string size)
        {
            Name = name;
            FullName = fullName;
            if (isDirectory)
            {
                Rest = cDirectory;
                DirectoryName = fullName;
            }
            else
            {
                Rest = "";
                DirectoryName = "";
            }
            Suffix = suffix;
            Size = size;
        }

        public MyTuple(string name, string fullName, string dirName, bool isDirectory, string suffix, string size)
        {
            Name = name;
            FullName = fullName;
            DirectoryName = dirName;
            if (isDirectory)
                Rest = cDirectory;
            else
                Rest = "";
            Suffix = suffix;
            Size = size;
        }

        public MyTuple (FileInfo fi)
        {
            Name = fi.Name;
            FullName = fi.FullName;
            DirectoryName = fi.DirectoryName;
            Rest = "";
            Suffix = fi.Extension;
            Size = fi.Length.ToString();
        }
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string Size { get; private set; }
        public string Suffix { get; private set; }
        public string DirectoryName { get; private set; }
        public string Rest { get; private set; }

        public bool IsDirectory()
        {
            return (Rest == cDirectory);
        }
    }

}

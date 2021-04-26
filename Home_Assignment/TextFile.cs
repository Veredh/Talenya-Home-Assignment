using System;
using System.Collections.Generic;
using System.Linq;
namespace Home_Assignment
{
    /// <summary>
    /// This class is responsible for gathering the data of a text file
    /// </summary>
    public class TextFile
    {
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private string fileContent;

        public string FileContent
        {
            get { return fileContent; }
            set { fileContent = value; }
        }
    }
}

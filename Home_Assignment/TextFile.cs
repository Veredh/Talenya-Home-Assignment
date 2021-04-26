using System;
using System.Collections.Generic;
using System.Linq;
namespace Home_Assignment
{
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

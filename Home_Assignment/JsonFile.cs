using System.Collections.Generic;

namespace Home_Assignment
{
    /// <summary>
    /// This class is responsible for gathering the relevant data from a json file
    /// </summary>
    public class JsonFile
    {
        public string FileName { get; set; }

        public string PositionTitle { get; set; }

        public List<string> Skills { get; set; }
    }
}

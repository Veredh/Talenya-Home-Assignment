using System.Collections.Generic;

namespace Home_Assignment
{
    /// <summary>
    /// This class is responsible for gathering the data for the json output file
    /// </summary>
    public class JsonOutput
    {
        public string filename { get; set; }
        public string jobTitle { get; set; }
        public List<object> Skills { get; set; }
    }
}

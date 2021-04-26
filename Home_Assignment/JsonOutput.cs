using System.Collections.Generic;

namespace Home_Assignment
{
    public class JsonOutput
    {
        public string filename { get; set; }
        public string jobTitle { get; set; }
        public List<Dictionary<string, string>> Skills { get; set; }
    }
}

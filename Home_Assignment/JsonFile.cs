using Newtonsoft.Json.Linq;

namespace Home_Assignment
{
    public class JsonFile
    {
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private JObject jsonFileContent;

        public JObject JsonFileContent
        {
            get { return jsonFileContent; }
            set { jsonFileContent = value; }
        }
    }
}

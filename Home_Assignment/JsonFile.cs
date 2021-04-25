using Newtonsoft.Json.Linq;

namespace Home_Assignment
{
    public class JsonFile
    {
        public string fileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        
        public JObject jsonObject
        {
            get { return jsonObject; }
            set { jsonObject = value; }
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Home_Assignment
{
    public class Handler
    {
        Dictionary<string, Dictionary<string, string>> JsonAndTxtFileNameAndContent = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, string> jsonFileContent = new Dictionary<string, string>();
        string jsonContent = string.Empty;
        string fileName = string.Empty;

        public void HandleFolder(string sourceDirectory)
        {
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");
            var jsonFiles = Directory.EnumerateFiles(sourceDirectory, "*.json");

            handleTxtFile(sourceDirectory, txtFiles);
            handleJsonFile(sourceDirectory, txtFiles, jsonFiles);
        }

        private void handleTxtFile(string sourceDirectory, IEnumerable<string> txtFiles)
        {

            foreach (string currentFile in txtFiles)
            {
                Dictionary<string, string> fileContent = new Dictionary<string, string>();

                fileName = Path.GetFileNameWithoutExtension(currentFile);
                fileContent.Add("Text File Content", File.ReadAllText(currentFile));
                JsonAndTxtFileNameAndContent.Add(fileName, fileContent);
            }
        }

        private void handleJsonFile(string sourceDirectory, IEnumerable<string> txtFiles, IEnumerable<string> jsonFiles)
        {

            foreach (string currentFile in jsonFiles)
            {
                fileName = Path.GetFileNameWithoutExtension(currentFile);

                using (StreamReader streamReader = new StreamReader(currentFile))
                {
                    var jsonFile = streamReader.ReadToEnd();
                    var jsonObject = JObject.Parse(jsonFile);

                    jsonContent = jsonObject.ToString();
                    jsonFileContent.Add(fileName, jsonContent);
                }
            }
        }

        /*private static string removeDigitsAndBracketsFromString(string key)
        {
            string result = Regex.Replace(key, @"\d", "");
            return result.Replace("(", "").Replace(")", "").Trim();
        }*/
    }
}

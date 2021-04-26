using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Home_Assignment
{
    public class Handler
    {
        /*private Dictionary<string, Dictionary<string, string>> jsonAndTxtFileNameAndContent = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> jsonFileContent = new Dictionary<string, string>();
        private string jsonContent = string.Empty;
        private string fileName = string.Empty;*/

        private Dictionary<TextFile, JsonFile> txtAndMatchingJsonFiles = new Dictionary<TextFile, JsonFile>();
        private List<TextFile> textFilesList = new List<TextFile>();
        private List<JsonFile> jsonFilesList = new List<JsonFile>();

        public void HandleFolder(string sourceDirectory)
        {
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");
            var jsonFiles = Directory.EnumerateFiles(sourceDirectory, "*.json");

            handleTxtFile(sourceDirectory, txtFiles);
            handleJsonFile(sourceDirectory, jsonFiles);
            zipListsToDictionary();
            countNumberOfAppearance();
        }

        private void handleTxtFile(string sourceDirectory, IEnumerable<string> txtFiles)
        {

            foreach (string currentFile in txtFiles)
            {
                /*Dictionary<string, string> fileContent = new Dictionary<string, string>();
                fileName = Path.GetFileNameWithoutExtension(currentFile);
                fileContent.Add("Text File Content", File.ReadAllText(currentFile));
                jsonAndTxtFileNameAndContent.Add(fileName, fileContent);*/

                TextFile textFile = new TextFile
                {
                    FileName = Path.GetFileNameWithoutExtension(currentFile),
                    FileContent = File.ReadAllText(currentFile)
                };

                textFilesList.Add(textFile);
            }
        }

        private void handleJsonFile(string sourceDirectory, IEnumerable<string> jsonFiles)
        {

            foreach (string currentFile in jsonFiles)
            {
                /*fileName = Path.GetFileNameWithoutExtension(currentFile);

                using (StreamReader streamReader = new StreamReader(currentFile))
                {
                    var jsonFile = streamReader.ReadToEnd();
                    var jsonObject = JObject.Parse(jsonFile);

                    jsonContent = jsonObject.ToString();
                    jsonFileContent.Add(fileName, jsonContent);
                }*/

                JsonFile jsonFile = new JsonFile
                {
                    FileName = Path.GetFileNameWithoutExtension(currentFile)
                };

                using (StreamReader streamReader = new StreamReader(currentFile))
                {
                    jsonFile.JsonFileContent = JObject.Parse(streamReader.ReadToEnd());
                }

                jsonFilesList.Add(jsonFile);
            }
        }

        private void zipListsToDictionary()
        {
            txtAndMatchingJsonFiles = textFilesList.Zip(jsonFilesList, (k, v) => new { Key = k, Value = v })
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private void countNumberOfAppearance()
        {
            foreach(KeyValuePair<TextFile, JsonFile> pair in txtAndMatchingJsonFiles)
            {
                JArray competencyArray = new JArray();
                List<string> skillList = new List<string>();
                JsonFile jsonFile = pair.Value;
                TextFile textFile = pair.Key;
                int i = 0;

                var resultyArray = jsonFile.JsonFileContent.Children<JProperty>().FirstOrDefault(x => x.Name == "result").Value;

                foreach(var item in resultyArray.Children())
                {
                    var itemProperties = item.Children<JProperty>().FirstOrDefault(x => x.Name == "_value").Value;
                    foreach(var child in itemProperties.Children())
                    {
                        if(child["_name"].ToString().Equals("Competency"))
                        {
                            competencyArray = (JArray)child["_value"];
                        }
                    }
                }

                foreach(var item in competencyArray.Children().Children())
                {
                    if (item["_name"].ToString().Equals("skillName"))
                    {
                        skillList.Add(item["_value"].ToString());
                    }
                }
            }
        }
    }
}

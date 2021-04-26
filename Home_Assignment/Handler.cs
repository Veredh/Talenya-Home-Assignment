using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Home_Assignment
{
    /// <summary>
    /// This class contains all the methods for counting the number of appearences of each skill from a json file in the matching text file
    /// </summary>
    public class Handler
    {
        private Dictionary<TextFile, JsonFile> txtAndMatchingJsonFiles = new Dictionary<TextFile, JsonFile>();
        private List<TextFile> textFilesList = new List<TextFile>();
        private List<JsonFile> jsonFilesList = new List<JsonFile>();
        private List<JsonOutput> jsonOutput = new List<JsonOutput>();
        private string jsonResult = string.Empty;

        /// <summary>
        /// This method is responsible for running all the other methods in the class
        /// </summary>
        /// <param name="sourceDirectory">The path to the folder where the text and the json files are at</param>
        public void HandleFolder(string sourceDirectory)
        {
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");
            var jsonFiles = Directory.EnumerateFiles(sourceDirectory, "*.json");

            handleTxtFile(sourceDirectory, txtFiles);
            handleJsonFile(jsonFiles);
            zipListsToDictionary();
            createJsonOutput();
            createJsonFileInSourceDirectory(sourceDirectory);
        }

        private void createJsonFileInSourceDirectory(string sourceDirectory)
        {
            using (var tw = new StreamWriter(string.Format($"{sourceDirectory}\\output.json"), true))
            {
                tw.WriteLine(jsonResult.ToString());
                tw.Close();
            }
        }

        private void handleJsonFile(IEnumerable<string> jsonFiles)
        {
            string jsonString;
            Dictionary<string,object> deserializedJsonFile;
            List<object> resultList;
            Dictionary<string, object> valueInResultList;
            List<object> valueList;

            foreach (string currentFile in jsonFiles)
            {
                JsonFile jsonFile = new JsonFile
                {
                    FileName = Path.GetFileNameWithoutExtension(currentFile)
                };

                jsonString = File.ReadAllText(currentFile);
                deserializedJsonFile = (Dictionary<string, object>) deserializeToDictionaryOrList(jsonString);
                resultList = (List<object>)deserializedJsonFile["result"];
                valueInResultList = (Dictionary<string, object>)resultList[0];
                valueList = (List<object>)valueInResultList["_value"];

                foreach (Dictionary<string, object> dict in valueList)
                {
                    if (dict["_name"].ToString().Equals("PositionTitle"))
                    {
                        jsonFile.PositionTitle = dict["_value"].ToString();
                    }

                    if (dict["_name"].ToString().Equals("Competency"))
                    {
                        jsonFile.Skills = getSkillsFromJson((List<object>)dict["_value"]);
                    }
                }

                jsonFilesList.Add(jsonFile);
            }
        }

        private List<string> getSkillsFromJson(List<object> competencyList)
        {
            List<string> skillsList = new List<string>();

            foreach(List<object> competencySubList in competencyList)
            {
                foreach(Dictionary<string, object> dict in competencySubList)
                {
                    if (dict["_name"].ToString().Equals("skillName"))
                    {
                        skillsList.Add(dict["_value"].ToString());
                        break;
                    }
                }             
            }

            return skillsList;
        }

        /// <summary>
        /// This method is responsible for creating an instance of the "TextFile" class for each file in "txtFiles", and adding it to a designated list
        /// </summary>
        /// <param name="sourceDirectory">The path to the folder where the text files are at</param>
        /// <param name="txtFiles">The text files in the given path</param>
        private void handleTxtFile(string sourceDirectory, IEnumerable<string> txtFiles)
        {

            foreach (string currentFile in txtFiles)
            {
                TextFile textFile = new TextFile
                {
                    FileName = Path.GetFileNameWithoutExtension(currentFile),
                    FileContent = File.ReadAllText(currentFile)
                };

                textFilesList.Add(textFile);
            }
        }

        private object deserializeToDictionaryOrList(string jsonObject, bool isArray = false)
        {
            if (!isArray)
            {
                isArray = jsonObject.Substring(0, 1) == "[";
            }
            if (!isArray)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject);
                var values2 = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> d in values)
                {
                    if (d.Value is JObject)
                    {
                        values2.Add(d.Key, deserializeToDictionaryOrList(d.Value.ToString()));
                    }
                    else if (d.Value is JArray)
                    {
                        values2.Add(d.Key, deserializeToDictionaryOrList(d.Value.ToString(), true));
                    }
                    else
                    {
                        values2.Add(d.Key, d.Value);
                    }
                }
                return values2;
            }
            else
            {
                var values = JsonConvert.DeserializeObject<List<object>>(jsonObject);
                var values2 = new List<object>();

                foreach (var d in values)
                {
                    if (d is JObject)
                    {
                        values2.Add(deserializeToDictionaryOrList(d.ToString()));
                    }
                    else if (d is JArray)
                    {
                        values2.Add(deserializeToDictionaryOrList(d.ToString(), true));
                    }
                    else
                    {
                        values2.Add(d);
                    }
                }
                return values2;
            }
        }

        /// <summary>
        /// This method is responsible for combining the two lists (of json files and text files) to one dictionary
        /// </summary>
        private void zipListsToDictionary()
        {
            txtAndMatchingJsonFiles = textFilesList.Zip(jsonFilesList, (k, v) => new { Key = k, Value = v })
                .ToDictionary(x => x.Key, x => x.Value);
        }

 
        private void createJsonOutput()
        {
            foreach(KeyValuePair<TextFile, JsonFile> pair in txtAndMatchingJsonFiles)
            {
                Dictionary<string, int> skillAndItsOccurrences = new Dictionary<string, int>();
                JsonOutput jsonOutput = new JsonOutput();
                JsonFile jsonFile = pair.Value;
                TextFile textFile = pair.Key;

                jsonOutput.filename = textFile.FileName;
                jsonOutput.jobTitle = jsonFile.PositionTitle;
                jsonFile.Skills = sortSkillOccurences(jsonFile.Skills);
                countSkillOccurrences(textFile, jsonFile.Skills, skillAndItsOccurrences);
                createSkillDictionaryForJsonOutput(skillAndItsOccurrences, jsonOutput);
                jsonResult += JsonConvert.SerializeObject(jsonOutput, Formatting.Indented);
                jsonResult += Environment.NewLine;
            }
        }

        private List<string> sortSkillOccurences(List<string> skills)
        {
            return skills.OrderByDescending(x => x).Reverse().ToList();
        }

        private void createSkillDictionaryForJsonOutput(Dictionary<string, int> skillAndItsOccurrences, JsonOutput jsonOutput)
        {
            jsonOutput.Skills = new List<Dictionary<string, string>>();

            foreach (KeyValuePair<string, int> keyValuePair in skillAndItsOccurrences)
            {
                jsonOutput.Skills.Add(new Dictionary<string, string>
                {
                    {string.Format($"name: {keyValuePair.Key}"), string.Format($"count: {keyValuePair.Value}") }
                });
            }
        }

        /// <summary>
        /// This method is responsible for counting the number of apperences of each skill fro the json file in the text file
        /// </summary>
        /// <param name="textFile">The text file name and content</param>
        /// <param name="skillList">The list of all the skills in the json file</param>
        /// <param name="numOfOccur">The result dictionary of skill as key and it's occurrences as value</param>
        private void countSkillOccurrences(TextFile textFile, List<string> skillList, Dictionary<string, int> numOfOccur)
        {
            int counter = 0;

            foreach (string skill in skillList)
            {
                foreach (Match match in Regex.Matches(textFile.FileContent, skill))
                {
                    counter++;
                }

                numOfOccur.Add(skill, counter);
                counter = 0;
            }
        }
    }
}

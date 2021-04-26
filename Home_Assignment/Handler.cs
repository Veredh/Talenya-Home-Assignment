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
    /// This class contains all the methods for counting the number of appearences of each skill from a json file in the matching text file,
    /// and creating a summery json file of all the collected data.
    /// </summary>
    public class Handler
    {
        private Dictionary<TextFile, JsonFile> txtAndMatchingJsonFiles = new Dictionary<TextFile, JsonFile>();
        private List<TextFile> textFilesList = new List<TextFile>();
        private List<JsonFile> jsonFilesList = new List<JsonFile>();
        private string jsonResult = string.Empty;

        /// <summary>
        /// This method is responsible for running all the other methods in the class
        /// </summary>
        /// <param name="sourceDirectory"> The path to the folder where the text and the json files are at </param>
        public void HandleFolder(string sourceDirectory)
        {
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.txt");
            var jsonFiles = Directory.EnumerateFiles(sourceDirectory, "*.json");

            handleTxtFile(txtFiles);
            handleJsonFile(jsonFiles);
            zipListsToDictionary();
            createJsonOutput();
            createJsonFileInSourceDirectory(sourceDirectory);
        }

        /// <summary>
        /// This method is responsible for creating a new json file in the source directory with the collected data.
        /// </summary>
        /// <param name="sourceDirectory"> The path to the folder where the text and the json files are at </param>
        private void createJsonFileInSourceDirectory(string sourceDirectory)
        {
            using (var streamWriter = new StreamWriter(string.Format($"{sourceDirectory}\\output.json"), true))
            {
                streamWriter.WriteLine(jsonResult.ToString());
                streamWriter.Close();
            }
        }

        /// <summary>
        /// This method is responsible for retreiving the data from the json files in the source directory
        /// </summary>
        /// <param name="jsonFiles"> A list of all the full paths to the json files in the source directory </param>
        private void handleJsonFile(IEnumerable<string> jsonFiles)
        {
            Dictionary<string,object> deserializedJsonFile;
            Dictionary<string, object> valueInResultList;
            List<object> resultList;
            List<object> valueList;
            string jsonString;

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

        /// <summary>
        /// This method is responsible for retrieving the skills from a json file.
        /// </summary>
        /// <param name="competencyList"> A competency list from a specific json file </param>
        /// <returns> 
        /// A list of the skills in the json file
        /// </returns>
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
        /// This method is responsible for retrieving the data from a text file in the source directory
        /// </summary>
        /// <param name="txtFiles"> The text files in the given path </param>
        private void handleTxtFile(IEnumerable<string> txtFiles)
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

        /// <summary>
        /// This method is responsible for retrieving a json file data to a complex structure combined of Dictionarys and Lists
        /// </summary>
        /// <param name="jsonFileContent"> Json file content </param>
        /// <param name="isArray"> Boolean for differentiating between JObject structure then JArray structure in the json file </param>
        /// <returns> 
        /// An object contains the data from a json file 
        /// </returns>
        private object deserializeToDictionaryOrList(string jsonFileContent, bool isArray = false)
        {
            if (!isArray)
            {
                isArray = jsonFileContent.Substring(0, 1) == "[";
            }
            if (!isArray)
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonFileContent);
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
                var values = JsonConvert.DeserializeObject<List<object>>(jsonFileContent);
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

        /// <summary>
        /// This methos is responsible for creating the json output
        /// </summary>
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
                createSkillListForJsonOutput(jsonOutput, skillAndItsOccurrences);
                jsonResult += JsonConvert.SerializeObject(jsonOutput, Formatting.Indented);
                jsonResult += Environment.NewLine;
            }
        }

        /// <summary>
        /// This method is responsible for initializing the skill list for the json output
        /// </summary>
        /// <param name="jsonOutput"> The intialized json output </param>
        /// <param name="skillAndItsOccurrences"> A dictionary of skills as keys and the number of their occurrences in a text file as value </param>
        private void createSkillListForJsonOutput(JsonOutput jsonOutput, Dictionary<string, int> skillAndItsOccurrences)
        {
            List<object> skillList = new List<object>();
            object skill;

            foreach(KeyValuePair<string, int> pair in skillAndItsOccurrences)
            {
                skill = "{name: " + pair.Key + ", count: " + pair.Value + "}";
                skillList.Add(skill);
            }

            jsonOutput.Skills = skillList;
        }

        /// <summary>
        /// This method is responsible for sorting the skills list in an acending order
        /// </summary>
        /// <param name="skills"> The list of skills from a json file </param>
        /// <returns> 
        /// An acending sorted skill list 
        /// </returns>
        private List<string> sortSkillOccurences(List<string> skills)
        {
            return skills.OrderByDescending(x => x).Reverse().ToList();
        }

        /// <summary>
        /// This method is responsible for counting how much times a skill from a json file apperes in the matching text file
        /// </summary>
        /// <param name="textFile"> The text file content </param>
        /// <param name="skillList"> The list of all the skills in a json file </param>
        /// <param name="numOfOccur"> The result dictionary of skill as key and it's occurrences as value </param>
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

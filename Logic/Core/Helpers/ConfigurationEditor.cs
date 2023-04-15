using Logic.Core.DataType;
using Logic.Core.Models;
using OneOf;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Logic.Core.Helpers
{
    public class ConfigurationEditor: IConfigurationEditor
    {
        Freezable<string> SettingsFilePath { get; set; }
        Freezable<JsonObject> Json { get; set; }
        public ConfigurationEditor()
        {
            
        }
        public OneOf<Ok, Exception> Parse(string settingsFilePath)
        {
            SettingsFilePath = settingsFilePath;
            if (!File.Exists(SettingsFilePath))
            {
                return new FileNotFoundException($"Couldn't find file {SettingsFilePath}");
            }
            try
            {
                var json = JsonNode.Parse(File.ReadAllText(SettingsFilePath));
                if (json is null)
                {
                    return new Exception("Failed to parse");
                }
                Json = json.AsObject();
            }
            catch (Exception ex)
            {
                return ex;
            }
            
            return new Ok();
        }
        /// <summary>
        /// update value of configuration file
        /// </summary>
        /// <param name="key">the name of the value, you can also use : as separator between key names if the value is nested</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public OneOf<Ok,Exception> SetValue<DataType>(string key, DataType value) where DataType :class 
        {
            if (Json.Value is null)
            {
                return new NullReferenceException("no loaded json");
            }
            try
            {
                var keys = key.Split(":"); // if the key is nested it will be separated by :
                                           // and if we need to reach it value we need to navigate
                                           // and so we need to convert the key into string of keys for navigation
                if (keys.Length==0)
                {
                    return new Exception($" Key is Empty");
                }

                EditJson(keys, value);
                SaveJson();
            }
            catch (Exception ex) 
            {
                return ex;
            }
            return new Ok();
        }
        OneOf<Ok, Exception> EditJson<DataType>(string[] keys, DataType value) where DataType : class
        {
            // we loop through nodes in the json try
            // we check if reached the last key in our keys array
            // ( which mean we reached the goal as the last key is what we need to edit its value)
            //if its we remove the node that hold the key then we replace it with another one
            try
            {
                var node = Json.Value.AsObject().FirstOrDefault(x => x.Key == keys.First()).Value;
                if (node is null)
                {
                    return new NullReferenceException("first node is null");
                }
                for (int i = 1; i < keys.Length; i++)
                {
                    var _key = keys[i];
                    var current = node.AsObject().FirstOrDefault(x => x.Key == _key);
                    if (current.Value is null)
                    {
                        return new NullReferenceException($"{_key} is null");
                    }
                    if (current.Key == keys.Last())
                    {
                        node.AsObject().Remove(current.Key);
                        node.AsObject().Add(new KeyValuePair<string, JsonNode?>(_key, JsonValue.Create(value)));
                        return new Ok();
                    }
                    node = current.Value;
                }
                return new Exception("Key not found");
            }
            catch (Exception ex)
            {
                return ex;
            }        
      
        }
        OneOf<Ok, Exception> SaveJson()
        {
            try
            {
                var json=  JsonSerializer.Serialize(Json.Value);
                File.WriteAllText(SettingsFilePath.Value, json);
                return new Ok();
            }
            catch (Exception ex)
            {
                return ex;
            }           
        }

    }
}

using System.IO;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameDevTV.Saving
{
    [CreateAssetMenu(menuName = "SavingStrategies/Bson", fileName ="BsonStrategy")]
    public class BsonStrategy : SavingStrategy
    {
        
        public override void SaveToFile(string saveFile, JObject state)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log($"Saving BSon to {path}");
            using (FileStream fileStream = File.Open(path, FileMode.Create))
            {
                using (BsonWriter writer = new BsonWriter(fileStream))
                {
                    state.WriteTo(writer);
                }
            }
        }

        public override JObject LoadFromFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log($"Loading BSon from {path}");
            if (!
                File.Exists(path))
            {
                return new JObject();
            }

            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                using (BsonReader reader = new BsonReader(fileStream))
                {
                    return (JObject) JToken.ReadFrom(reader);
                }
            }
        }

        public override string GetExtension()
        {
            return ".bson";
        }
    }
}
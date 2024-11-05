using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameBase.Managers
{
    static public class SaveLoadSystem
    {
        static BinaryFormatter formatter = new BinaryFormatter();

        // Update is called once per frame
        static public void SaveData(GameDataOrigin gameDataOrigin)
        {
            string folderName = "SaveData";
            string fileName = "Data";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            FileStream dataFile = File.Create(folderName + "/" + fileName + ".bin");

            formatter.Serialize(dataFile, gameDataOrigin);
            dataFile.Close();
        }

        static public void LoadData(GameDataOrigin gameDataOrigin)
        {
            string folderName = "SaveData";
            string fileName = "Data";

            if (!Directory.Exists(folderName))
            {
                Logger.Log("Don't have data to load");
                return;
            }

            FileStream dataFile = File.Open(folderName + "/" + fileName + ".bin", FileMode.Open);
            gameDataOrigin = (GameDataOrigin)formatter.Deserialize(dataFile);

            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            dataFile.Close();
        }
    }
}

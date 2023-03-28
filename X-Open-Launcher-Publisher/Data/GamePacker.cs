using LiteDB;

namespace X_Open_Launcher_Publisher.Data
{
    public class Test
    {
        public string A { get; set; }
        public string B { get; set; }
    }
    public static class GamePacker
    {
        public static readonly LiteDatabase GameDB = new LiteDatabase(GameDBPath);

        public static string DataBaseName = "GameCash";
        public static string GameDBPath => Path.Combine(FileDialog.Path, $"{DataBaseName}DB.db");

        public static void WriteFolderToDb(string folderPath) =>
            Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).ToList().ForEach(x =>
            {
                string relativePath = Path.GetRelativePath(folderPath, x);
                WriteToDb(x, relativePath);
            });

        public static void WriteToDb(string filePath, string relativePath)
        {
            ILiteStorage<string> storage = GameDB.GetStorage<string>("GameFiles", "GameFileChunks");
            string id = Guid.NewGuid().ToString();
            LiteFileInfo<string> uploadedFile = storage.Upload(id, filePath);
            BsonDocument doc = new BsonDocument
            {
                { "relativePath", relativePath }
            };
            storage.SetMetadata(uploadedFile.Id, doc);
        }
    }
}

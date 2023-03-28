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

        public static void SplitFile(string sourceFile, string destinationDirectory, long chunkSize = 2L * 1024L * 1024L * 1024L)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                byte[] buffer = new byte[chunkSize];
                int bytesRead;

                for (int i = 0; (bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0; i++)
                {
                    string destinationFile = Path.Combine(destinationDirectory, $"part_{i}.dat");

                    using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create))
                    {
                        destinationStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        public static void CombineFile(string sourceDirectory, string destinationFile)
        {
            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create))
            {
                int i = 0;
                foreach (string sourceFile in Directory.GetFiles(sourceDirectory))
                {
                    FileInfo fileInfo = new FileInfo(sourceFile);
                    if (fileInfo.Exists && fileInfo.Extension == ".dat" && fileInfo.Name == $"part_{i}")
                    {
                        using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
                        {
                            byte[] buffer = new byte[sourceStream.Length];
                            int bytesRead;

                            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                destinationStream.Write(buffer, 0, bytesRead);
                            }
                        }
                        i++;
                    }
                }
            }
        }
    }
}

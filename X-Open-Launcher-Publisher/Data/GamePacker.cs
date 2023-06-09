﻿using LiteDB;


public class Test
{
    public string A { get; set; }
    public string B { get; set; }
}
public static class GamePacker
{
    public static string DataBaseName = "GameCash";
    public static string DataBasePatchName = "GamePatch";
    public static string GameDBPath => Path.Combine(FileDialog.Path, $"{DataBaseName}DB.db");
    public static string PatchDBPath => Path.Combine(FileDialog.Path, $"{DataBasePatchName}DB.db");
    public static LiteDatabase GameDB = new LiteDatabase(GameDBPath);
    public static LiteDatabase PatchDB = new LiteDatabase(PatchDBPath);

    public static void WriteFolderToDb(string folderPath)
    {
       var Files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).ToList();
        
        Files.ForEach(x =>
        {
            string relativePath = Path.GetRelativePath(folderPath, x);
            WriteToDb(x, relativePath);
        });
        EditorPublisher.InfoDB.Checkpoint();
    }

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

    public static void SplitFile(string sourceFile, string destinationDirectory, int chunkSize = System.Int32.MaxValue - 100)
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

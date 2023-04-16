using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InfoClass
{
    [BsonId]
    public ObjectId UpdateID { get; set; } = ObjectId.NewObjectId();
    public string GameName { get; set; } = " ";
    public string Discription { get; set; } = " ";
    public string Media { get; set; } =  " " ;
    public string Tags { get; set; } =  " " ;
    public string DISCLAIMER_url { get; set; } = " " ;
    public string Exe { get; set; } = " " ;
}

public static class EditorPublisher
{
    public static InfoClass infoClass { get; set; }
    public static readonly LiteDatabase InfoDB = new LiteDatabase(PublisherInfoPath());
    public static ILiteCollection<InfoClass> cl = InfoDB.GetCollection<InfoClass>("Info");
    public static string PublisherInfoPath()
    {
        return Path.Combine(FileDialog.Path, "InfoDB.db");
    }
    public static async Task LoadInfo() 
    {
        var infos = cl.FindAll();
        infoClass = infos.FirstOrDefault();
        if (infoClass == null) 
        {
            infoClass = new InfoClass();
        } 
    }
    public static async Task SaveInfo()
    {
        if (cl.Exists(x => x.UpdateID == infoClass.UpdateID))
        {
            cl.Update(infoClass);
        }
        else
        {
            cl.Insert(infoClass);
            cl.EnsureIndex(x => x.UpdateID);
        }
        EditorPublisher.InfoDB.Checkpoint();
    }
}


using System.IO;
using System.Text;
using UnityEngine;

public class FileUtils
{
    public static string GetPathToPersistent(string localPath)
    {
        return Path.Combine(Application.persistentDataPath, localPath);
    }

    public static string ReadFileToString(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using FileStream fs = File.OpenRead(path);
        byte[] b = new byte[1024];
        UTF8Encoding temp = new UTF8Encoding(true);
        string str = "";
        while (fs.Read(b, 0, b.Length) > 0)
        {
            str += temp.GetString(b);
        }
        return str;
    }

    public static void WriteStringToFile(string path, string data)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        using (FileStream fs = File.Create(path))
        {
            string s = data;
            AddText(fs, s);
        }
    }

    private static void AddText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
}
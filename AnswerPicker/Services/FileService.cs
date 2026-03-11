using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnswerPicker.Services;

public class FileService
{
    private readonly string classesFolder;

    public FileService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        classesFolder = Path.Combine(appData, "Classes");
        if (!Directory.Exists(classesFolder))
            Directory.CreateDirectory(classesFolder);
    }

    public List<string> GetClassList()
    {
        var files = Directory.GetFiles(classesFolder, "*.txt");
        return files.Select(f => Path.GetFileNameWithoutExtension(f)).OrderBy(n => n).ToList();
    }

    public List<string> LoadClass(string className)
    {
        var path = GetClassPath(className);
        if (!File.Exists(path)) return new List<string>();
        var lines = File.ReadAllLines(path);
        return lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList();
    }

    public void SaveClass(string className, IEnumerable<string> students)
    {
        var path = GetClassPath(className);
        File.WriteAllLines(path, students.Select(s => s ?? string.Empty));
    }

    public void DeleteClass(string className)
    {
        var path = GetClassPath(className);
        if (File.Exists(path)) File.Delete(path);
    }

    private string GetClassPath(string className)
    {
        var safeName = MakeSafeFileName(className);
        return Path.Combine(classesFolder, safeName + ".txt");
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}

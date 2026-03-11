using AnswerPicker.Models;
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

    public List<Student> LoadClass(string className)
    {
        var path = GetClassPath(className);
        if (!File.Exists(path)) return new List<Student>();

        var lines = File.ReadAllLines(path);
        var result = new List<Student>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            var student = new Student(parts[0].Trim());

            if (parts.Length > 1 && bool.TryParse(parts[1], out bool isAbsent) && isAbsent)
            {
                student.SetAbsent();
            }

            if (parts.Length > 2 && bool.TryParse(parts[2], out bool isLucky) && isLucky)
            {
                student.SetLucky(true);
            }

            result.Add(student);
        }

        return result;
    }

    public void SaveClass(string className, IEnumerable<Student> students)
    {
        var path = GetClassPath(className);
        var lines = students.Select(s => $"{s.Name}|{s.IsAbsent}|{s.IsLucky}");
        File.WriteAllLines(path, lines);
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

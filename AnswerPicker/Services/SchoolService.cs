using AnswerPicker.Models;
using System.Text;

namespace AnswerPicker.Services;

public class SchoolService
{
    public async Task SaveSchoolAsync(
        SchoolModel school,
        string filePath)
    {
        StringBuilder builder = new();

        foreach (var schoolClass in school.Classes)
        {
            builder.AppendLine($"[{schoolClass.Key}]");

            foreach (string student in schoolClass.Value)
            {
                builder.AppendLine(student);
            }

            builder.AppendLine();
        }

        await File.WriteAllTextAsync(
            filePath,
            builder.ToString());
    }

    public async Task<SchoolModel> LoadSchoolAsync(
        string filePath)
    {
        SchoolModel school = new();

        if (!File.Exists(filePath))
        {
            return school;
        }

        string[] lines =
            await File.ReadAllLinesAsync(filePath);

        string currentClass = string.Empty;

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("[") &&
                line.EndsWith("]"))
            {
                currentClass =
                    line[1..^1];

                if (!school.Classes.ContainsKey(currentClass))
                {
                    school.Classes.Add(
                        currentClass,
                        new List<string>());
                }

                continue;
            }

            if (!string.IsNullOrWhiteSpace(currentClass))
            {
                school.Classes[currentClass]
                    .Add(line);
            }
        }

        return school;
    }

    public string DrawStudent(
        List<string> students)
    {
        if (students.Count == 0)
        {
            return string.Empty;
        }

        Random random = new();

        return students[random.Next(students.Count)];
    }
}
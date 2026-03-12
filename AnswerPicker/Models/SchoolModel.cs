using System;
using System.Collections.Generic;
using System.Linq;

namespace AnswerPicker.Models
{
    public class Student
    {
        public string Name { get; set; } = string.Empty;
        public bool IsAbsent { get; set; }
        public bool IsLucky { get; set; }

        public Student() { }

        public Student(string name)
        {
            Name = name;
            IsAbsent = false;
            IsLucky = false;
        }

        public void SetAbsent() { IsAbsent = true; }
        public void SetPresent() { IsAbsent = false; }
        public void SetLucky(bool lucky) { IsLucky = lucky; }
    }

    public class ClassModel
    {
        public string Name { get; set; } = string.Empty;
        public List<Student> Students { get; set; } = new List<Student>();

        public ClassModel() { }

        public ClassModel(string name)
        {
            Name = name;
        }
    }

    public class SchoolModel
    {
        public List<ClassModel> Classes { get; set; } = new List<ClassModel>();

        public SchoolModel() { }

        public void AddClass(ClassModel cls)
        {
            if (!Classes.Any(c => c.Name == cls.Name))
                Classes.Add(cls);
        }

        public void RemoveClass(string name)
        {
            var cls = Classes.FirstOrDefault(c => c.Name == name);
            if (cls != null) Classes.Remove(cls);
        }

        public ClassModel? GetClass(string name)
        {
            return Classes.FirstOrDefault(c => c.Name == name);
        }
    }
}

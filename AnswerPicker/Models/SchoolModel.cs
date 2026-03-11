using System.Collections.Generic;

namespace AnswerPicker.Models
{
    public class SchoolModel
    {
        public List<ClassModel> Classes { get; set; } = new List<ClassModel>();

        public SchoolModel() { }

        public void AddClass(ClassModel classModel)
        {
            if (!Classes.Exists(c => c.Name == classModel.Name))
                Classes.Add(classModel);
        }

        public void RemoveClass(string className)
        {
            var existing = Classes.Find(c => c.Name == className);
            if (existing != null) Classes.Remove(existing);
        }

        public ClassModel? GetClass(string className)
        {
            return Classes.Find(c => c.Name == className);
        }
    }
}
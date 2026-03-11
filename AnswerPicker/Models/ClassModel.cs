using System.Collections.Generic;

namespace AnswerPicker.Models
{
    public class ClassModel
    {
        public string Name { get; set; } = string.Empty;
        public List<Student> Students { get; set; } = new List<Student>();

        public ClassModel() { }
        public ClassModel(string name) { Name = name; }
    }
}

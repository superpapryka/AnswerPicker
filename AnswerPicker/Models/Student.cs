namespace AnswerPicker.Models
{
    public class Student
    {
        public Student() { }

        public Student(string name)
        {
            Name = name;
            IsAbsent = false;
            IsLucky = false;
        }

        public string Name { get; set; } = string.Empty;
        public bool IsAbsent { get; set; }
        public bool IsLucky { get; set; }

        public override string ToString() => Name;

        public void TogglePresence()
        {
            IsAbsent = !IsAbsent;
        }

        public void SetLucky(bool lucky)
        {
            IsLucky = lucky;
        }
        public void SetAbsent()
        {
            IsAbsent = true;
            Name += " (Absent)";
        }

        public void SetPresent()
        {
            IsAbsent = false;
            Name = Name.Replace(" (Absent)", "");
        }
    }
}
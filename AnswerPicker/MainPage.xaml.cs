using System.Collections.ObjectModel;
using AnswerPicker.Models;
using AnswerPicker.Services;

namespace AnswerPicker;

public partial class MainPage : ContentPage
{
    private readonly SchoolService schoolService;

    private SchoolModel school;

    private readonly ObservableCollection<string> students;

    private string currentClass = string.Empty;

    private readonly string saveFilePath;

    public MainPage()
    {
        InitializeComponent();

        schoolService = new SchoolService();

        school = new SchoolModel();

        students = new ObservableCollection<string>();

        StudentsCollectionView.ItemsSource = students;

        saveFilePath =
            Path.Combine(
                FileSystem.Current.AppDataDirectory,
                "SchoolData.txt");
    }

    private void RefreshClassPicker()
    {
        ClassPicker.Items.Clear();

        foreach (string className in school.Classes.Keys)
        {
            ClassPicker.Items.Add(className);
        }
    }

    private void AddClass_Clicked(
        object sender,
        EventArgs e)
    {
        string className =
            ClassNameEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(className))
        {
            return;
        }

        if (school.Classes.ContainsKey(className))
        {
            return;
        }

        school.Classes.Add(
            className,
            new List<string>());

        RefreshClassPicker();

        ClassPicker.SelectedItem = className;

        ClassNameEntry.Text = string.Empty;
    }

    private void DeleteClass_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentClass))
        {
            return;
        }

        school.Classes.Remove(currentClass);

        students.Clear();

        currentClass = string.Empty;

        RefreshClassPicker();
    }

    private void ClassPicker_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        if (ClassPicker.SelectedItem == null)
        {
            return;
        }

        currentClass =
            ClassPicker.SelectedItem.ToString() ?? "";

        students.Clear();

        foreach (string student in school.Classes[currentClass])
        {
            students.Add(student);
        }
    }

    private void AddStudent_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentClass))
        {
            return;
        }

        string studentName =
            StudentEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(studentName))
        {
            return;
        }

        school.Classes[currentClass]
            .Add(studentName);

        students.Add(studentName);

        StudentEntry.Text = string.Empty;
    }

    private async void EditStudent_Clicked(
        object sender,
        EventArgs e)
    {
        Button button = (Button)sender;

        string oldName =
            button.BindingContext?.ToString() ?? "";

        string newName =
            await DisplayPromptAsync(
                "Edit Student",
                "Enter new name:",
                initialValue: oldName);

        if (string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        int index =
            school.Classes[currentClass]
            .IndexOf(oldName);

        if (index >= 0)
        {
            school.Classes[currentClass][index] =
                newName;
        }

        int observableIndex =
            students.IndexOf(oldName);

        if (observableIndex >= 0)
        {
            students[observableIndex] =
                newName;
        }
    }

    private void DeleteStudent_Clicked(
        object sender,
        EventArgs e)
    {
        Button button = (Button)sender;

        string student =
            button.BindingContext?.ToString() ?? "";

        school.Classes[currentClass]
            .Remove(student);

        students.Remove(student);
    }

    private async void Save_Clicked(
        object sender,
        EventArgs e)
    {
        await schoolService.SaveSchoolAsync(
            school,
            saveFilePath);

        await DisplayAlert(
            "Success",
            $"Saved to:\n{saveFilePath}",
            "OK");
    }

    private async void Load_Clicked(
        object sender,
        EventArgs e)
    {
        school =
            await schoolService.LoadSchoolAsync(
                saveFilePath);

        students.Clear();

        currentClass = string.Empty;

        RefreshClassPicker();

        await DisplayAlert(
            "Success",
            "Data loaded.",
            "OK");
    }

    private async void DrawStudent_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(currentClass))
        {
            await DisplayAlert(
                "Error",
                "Select a class.",
                "OK");

            return;
        }

        string student =
            schoolService.DrawStudent(
                school.Classes[currentClass]);

        if (string.IsNullOrWhiteSpace(student))
        {
            await DisplayAlert(
                "Error",
                "No students in class.",
                "OK");

            return;
        }

        ResultLabel.Text =
            $"Selected: {student}";
    }
}
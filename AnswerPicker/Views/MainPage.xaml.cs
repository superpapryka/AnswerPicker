using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using AnswerPicker.Services;
using AnswerPicker.Models;

namespace AnswerPicker.Views;

public partial class MainPage : ContentPage
{
    private readonly FileService fileService;
    private readonly Random rng = new Random();

    private ObservableCollection<string> classes = new ObservableCollection<string>();
    private ObservableCollection<Student> students = new ObservableCollection<Student>();

    public MainPage(FileService fileService)
    {
        InitializeComponent();

        this.fileService = fileService;

        ClassesPicker.ItemsSource = classes;
        StudentsCollection.ItemsSource = students;

        RefreshClasses();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshClasses();
    }

    private void RefreshClasses()
    {
        classes.Clear();
        foreach (var c in fileService.GetClassList())
            classes.Add(c);
    }

    private void OnAddClassClicked(object sender, EventArgs e)
    {
        var name = NewClassEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return;
        if (!classes.Contains(name))
        {
            classes.Add(name);
            ClassesPicker.SelectedItem = name;
            students.Clear();
            NewClassEntry.Text = string.Empty;

            fileService.SaveClass(name, new List<Student>());
            RefreshClasses();
        }
    }

    private void OnLoadClassClicked(object sender, EventArgs e)
    {
        var selected = ClassesPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selected)) return;

        var list = fileService.LoadClass(selected);
        students.Clear();
        foreach (var s in list)
        {
            students.Add(s);
        }

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = students;

        PickedLabel.Text = string.Empty;
    }

    private void OnSaveClassClicked(object sender, EventArgs e)
    {
        var selected = ClassesPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selected)) return;

        fileService.SaveClass(selected, students.ToList());

        var previousSelection = selected;
        RefreshClasses();
        ClassesPicker.SelectedItem = previousSelection;
    }

    private async void OnDeleteClassClicked(object sender, EventArgs e)
    {
        var selected = ClassesPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selected)) return;

        var answer = await DisplayAlert("Confirm", $"Delete class '{selected}'?", "Yes", "No");
        if (!answer) return;

        fileService.DeleteClass(selected);
        RefreshClasses();
        students.Clear();
        ClassesPicker.SelectedItem = null;
        PickedLabel.Text = string.Empty;
    }

    private void OnAddStudentClicked(object sender, EventArgs e)
    {
        var name = NewStudentEntry.Text?.Trim();
        var selected = ClassesPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(selected)) return;
        students.Add(new Student(name));
        NewStudentEntry.Text = string.Empty;
    }

    private void OnDeleteStudentClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Student st)
        {
            if (students.Contains(st)) students.Remove(st);
        }
        else if (sender is Button btn2 && btn2.CommandParameter is string sname)
        {

            var found = students.FirstOrDefault(x => x.Name == sname);
            if (found != null) students.Remove(found);
        }
    }

    private void OnPickStudentClicked(object sender, EventArgs e)
    {
        var availableStudents = students.Where(s => !s.IsAbsent && !s.IsLucky).ToList();

        if (availableStudents.Count == 0)
        {
            PickedLabel.Text = "No students available";
            return;
        }

        var index = rng.Next(availableStudents.Count);
        PickedLabel.Text = availableStudents[index].Name;
    }

    public void OnTogglePresenceClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Student student)
        {
            if (student.IsAbsent)
            {
                student.SetPresent();
            }
            else
            {
                student.SetAbsent();
            }

            btn.Text = student.IsAbsent ? "Present" : "Absent";

            StudentsCollection.ItemsSource = null;
            StudentsCollection.ItemsSource = students;
        }
    }

    public void OnLuckyNumberClicked(object sender, EventArgs e)
    {
        if (students.Count == 0)
        {
            return;
        }

        foreach (var student in students)
        {
            student.SetLucky(false);
        }

        var luckyStudent = students[rng.Next(students.Count)];
        luckyStudent.SetLucky(true);

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = students;
    }
    public void OnResetLuckyNumberClicked(object sender, EventArgs e)
    {
        if (students == null || students.Count == 0)
            return;

        foreach (var s in students)
        {
            s.IsLucky = false;
        }

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = students;
    }
}

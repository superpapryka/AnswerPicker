using System;
using System.Linq;
using Microsoft.Maui.Controls;
using AnswerPicker.Services;
using AnswerPicker.Models;

namespace AnswerPicker.Views;

public partial class MainPage : ContentPage
{
    private readonly FileService fileService;
    private readonly Random rng = new Random();
    private readonly SchoolModel school = new SchoolModel();
    private ClassModel? currentClass;

    public MainPage(FileService fileService)
    {
        InitializeComponent();
        this.fileService = fileService;

        // Picker klas i CollectionView dla uczniów
        ClassesPicker.ItemsSource = school.Classes.Select(c => c.Name).ToList();
        StudentsCollection.ItemsSource = null;

        LoadAllClasses();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadAllClasses();
    }

    // Wczytanie wszystkich klas z plików
    private void LoadAllClasses()
    {
        school.Classes.Clear();
        var classNames = fileService.GetClassList();

        foreach (var name in classNames)
        {
            var students = fileService.LoadClass(name);
            var cls = new ClassModel(name) { Students = students };
            school.AddClass(cls);
        }

        RefreshClassesPicker();
    }

    private void RefreshClassesPicker()
    {
        ClassesPicker.ItemsSource = school.Classes.Select(c => c.Name).ToList();
        if (currentClass != null)
            ClassesPicker.SelectedItem = currentClass.Name;

        StudentsCollection.ItemsSource = currentClass?.Students;
    }

    private void OnAddClassClicked(object sender, EventArgs e)
    {
        var name = NewClassEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return;
        if (school.Classes.Any(c => c.Name == name)) return;

        var newClass = new ClassModel(name);
        school.AddClass(newClass);
        currentClass = newClass;

        fileService.SaveClass(name, newClass.Students);
        RefreshClassesPicker();
        NewClassEntry.Text = string.Empty;
    }

    private void OnLoadClassClicked(object sender, EventArgs e)
    {
        var selected = ClassesPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(selected)) return;

        var cls = school.GetClass(selected);
        if (cls == null) return;

        currentClass = cls;
        StudentsCollection.ItemsSource = currentClass.Students;
        PickedLabel.Text = string.Empty;
    }

    private void OnSaveClassClicked(object sender, EventArgs e)
    {
        if (currentClass == null) return;

        fileService.SaveClass(currentClass.Name, currentClass.Students);
        LoadAllClasses(); // odświeżenie pickera i zachowanie stanu klas
    }

    private async void OnDeleteClassClicked(object sender, EventArgs e)
    {
        if (currentClass == null) return;

        var answer = await DisplayAlert("Confirm", $"Delete class '{currentClass.Name}'?", "Yes", "No");
        if (!answer) return;

        fileService.DeleteClass(currentClass.Name);
        school.RemoveClass(currentClass.Name);
        currentClass = null;
        LoadAllClasses();
        PickedLabel.Text = string.Empty;
    }

    private void OnAddStudentClicked(object sender, EventArgs e)
    {
        if (currentClass == null) return;

        var name = NewStudentEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return;

        var newStudent = new Student(name);
        currentClass.Students.Add(newStudent);

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = currentClass.Students;

        NewStudentEntry.Text = string.Empty;
    }

    private void OnDeleteStudentClicked(object sender, EventArgs e)
    {
        if (currentClass == null) return;

        if (sender is Button btn && btn.CommandParameter is Student student)
            currentClass.Students.Remove(student);

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = currentClass.Students;
    }

    private void OnTogglePresenceClicked(object sender, EventArgs e)
    {
        if (currentClass == null) return;

        if (sender is Button btn && btn.CommandParameter is Student student)
        {
            if (student.IsAbsent)
                student.SetPresent();
            else
                student.SetAbsent();

            btn.Text = student.IsAbsent ? "Present" : "Absent";

            StudentsCollection.ItemsSource = null;
            StudentsCollection.ItemsSource = currentClass.Students;
        }
    }

    private void OnPickStudentClicked(object sender, EventArgs e)
    {
        if (currentClass == null) return;

        var available = currentClass.Students.Where(s => !s.IsAbsent && !s.IsLucky).ToList();
        if (available.Count == 0)
        {
            PickedLabel.Text = "No students available";
            return;
        }

        var picked = available[rng.Next(available.Count)];
        PickedLabel.Text = picked.Name;
    }

    private void OnLuckyNumberClicked(object sender, EventArgs e)
    {
        if (currentClass == null || currentClass.Students.Count == 0) return;

        foreach (var s in currentClass.Students)
            s.SetLucky(false);

        var lucky = currentClass.Students[rng.Next(currentClass.Students.Count)];
        lucky.SetLucky(true);

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = currentClass.Students;
    }

    private void OnResetLuckyNumberClicked(object sender, EventArgs e)
    {
        if (currentClass == null || currentClass.Students.Count == 0) return;

        foreach (var s in currentClass.Students)
            s.SetLucky(false);

        StudentsCollection.ItemsSource = null;
        StudentsCollection.ItemsSource = currentClass.Students;
    }
}

using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private readonly string _notesDir = Path.Combine(AppContext.BaseDirectory, "notatki");

    public MainWindow()
    {
        InitializeComponent();
        Directory.CreateDirectory(_notesDir);
        RefreshNotesList();
    }

    private void RefreshNotesList()
    {
        NotesList.Items.Clear();
        if (Directory.Exists(_notesDir))
        {
            var files = Directory.GetFiles(_notesDir, "*.txt").OrderByDescending(f => f);
            foreach (var file in files)
            {
                NotesList.Items.Add(Path.GetFileName(file));
            }
        }
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        var title = TitleBox.Text ?? "Bez tytulu";
        var content = ContentBox.Text ?? string.Empty;

        var fileName = $"notatka_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var filePath = Path.Combine(_notesDir, fileName);

        File.WriteAllText(filePath, $"{title}\n\n{content}");

        TitleBox.Text = string.Empty;
        ContentBox.Text = string.Empty;

        RefreshNotesList();
    }
}
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private readonly string _notesFilePath = ResolveNotesFilePath();

    public MainWindow()
    {
        InitializeComponent();
        RefreshNotesList();
    }

    private static string ResolveNotesFilePath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        if (File.Exists(Path.Combine(currentDir, "notatnik.csproj")))
        {
            return Path.Combine(currentDir, "notatki.md");
        }
        if (Directory.Exists(Path.Combine(currentDir, "notatnik")))
        {
            return Path.Combine(currentDir, "notatnik", "notatki.md");
        }
        var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        if (File.Exists(Path.Combine(projectDir, "notatnik.csproj")))
        {
            return Path.Combine(projectDir, "notatki.md");
        }
        return Path.Combine(AppContext.BaseDirectory, "notatki.md");
    }

    private void RefreshNotesList()
    {
        NotesList.Items.Clear();
        if (!File.Exists(_notesFilePath))
        {
            return;
        }

        var lines = File.ReadAllLines(_notesFilePath, Encoding.UTF8);
        foreach (var line in lines)
        {
            if (line.StartsWith("## "))
            {
                NotesList.Items.Add(line.Substring(3).Trim());
            }
        }
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        var title = (TitleBox.Text ?? "Bez tytułu").Trim();
        var content = (ContentBox.Text ?? string.Empty).Trim();

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        var newEntry = $"## {timestamp} — {title}\n{content}";

        var existingContent = File.Exists(_notesFilePath)
            ? File.ReadAllText(_notesFilePath, Encoding.UTF8).Trim()
            : string.Empty;

        var fullContent = string.IsNullOrEmpty(existingContent)
            ? newEntry
            : $"{newEntry}\n\n{existingContent}";
        try
        {
            File.WriteAllText(_notesFilePath, fullContent, Encoding.UTF8);
            StatusText.Text = "Zapisano";

            TitleBox.Text = string.Empty;
            ContentBox.Text = string.Empty;

            RefreshNotesList();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Nie udało się zapisać: {ex.Message}";
        }
    }
}
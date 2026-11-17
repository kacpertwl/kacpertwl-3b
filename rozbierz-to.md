# Rozbierz to — analiza fragmentu kodu agenta

Fragment kodu poddany analizie pochodzi z lekcji o weryfikacji kodu generowanego przez modele AI:

```csharp
// NotesWindow.axaml.cs — fragment wygenerowany przez agenta
private System.Timers.Timer autosaveTimer; 

public NotesWindow() 
{ 
    InitializeComponent(); 
    autosaveTimer = new System.Timers.Timer(60000); 
    autosaveTimer.Elapsed += (s, e) => StatusText.Text = "Zapisano"; 
    autosaveTimer.Start(); 
} 

// Zapisuje notatkę do pliku wybranego przez użytkownika. 
// Obsługa błędów zapisu jest w środku. 
private async void SaveButton_Click(object sender, RoutedEventArgs e) 
{ 
    var dialog = new SaveFileDialog(); 
    dialog.InitialFileName = "notatki.md"; 
    var path = await dialog.ShowAsync(this); 
    File.WriteAllText(path, NoteBox.Text); 
}
```

Kod na pierwszy rzut oka wygląda podręcznikowo: nazwy są czytelne, wcięcia równe, a komentarz uprzejmie opisuje działanie. Zawiera jednak cztery poważne błędy.

Pierwszy błąd został omówiony w lekcji: klasa `SaveFileDialog` została oznaczona jako przestarzała w Avalonia 11 i całkowicie usunięta w Avalonia 12 na rzecz interfejsu `IStorageProvider` i metody `SaveFilePickerAsync`. To halucynacja typu **„Coś, czego nie ma”**, która uniemożliwia zbudowanie projektu (`dotnet build` zakończy się błędem kompilacji).

Poniżej znajduje się szczegółowy rozbiór trzech pozostałych błędów.

---

## Błąd 1: Zegar tykający poza wątkiem interfejsu (`System.Timers.Timer`)

- **Na czym polega:** Użyto klasy `System.Timers.Timer`, której zdarzenie `Elapsed` jest wywoływane w tle na wątku z puli wątków systemowych (`ThreadPool`), a nie na głównym wątku UI. W obsłudze zdarzenia następuje bezpośrednia próba modyfikacji kontrolki interfejsu użytkownika: `StatusText.Text = "Zapisano"`. W bibliotece Avalonia (oraz w większości technologii UI, takich jak WPF czy WinUI) obowiązuje reguła powiązania z wątkiem (thread affinity) — kontrolki interfejsu mogą być modyfikowane wyłącznie przez wątek dyspozytora UI. Wywołanie tej instrukcji z innego wątku skutkuje natychmiastowym błędem współbieżności i awarią aplikacji. Dodatkowo sam timer w ogóle nie realizuje zapisu notatki, a jedynie po 60 sekundach fałszywie sygnalizuje użytkownikowi, że zapis nastąpił. Prawidłowym rozwiązaniem w Avalonia jest użycie `Avalonia.Threading.DispatcherTimer` lub opakowanie zmiany w `Dispatcher.UIThread.Post(...)`.
- **Kształt halucynacji:** **Coś, co istnieje, użyte źle**. Klasa `System.Timers.Timer` istnieje w .NET, ale została wstawiona w niewłaściwym miejscu architektonicznym aplikacji okienkowej.
- **Kiedy się ujawni:** Przy uruchomieniu aplikacji / w trakcie działania (dokładnie po 60 sekundach od otwarcia okna, gdy timer po raz pierwszy wywoła zdarzenie `Elapsed`). Kompilator nie zgłasza żadnego błędu ani ostrzeżenia podczas budowania.
- **Czym bym to sprawdził:** Uruchomieniem aplikacji poleceniem `dotnet run` i odczekaniem 60 sekund (lub testem jednostkowym wywołującym zdarzenie timera), co wywoła wyjątek w runtime i natychmiast wykaże naruszenie dostępu do wątku UI w logach lub debugerze.
- **Stan w naszym notatniku:** Mój notatnik nie ma tego błędu: nie używa klasy `System.Timers.Timer` ani żadnego timera w tle, a kontrolka `StatusText` jest aktualizowana wyłącznie synchronicznie w głównym wątku UI podczas obsługi kliknięcia przycisku `OnSaveClicked`, co ustaliłem przeszukując cały kod `MainWindow.axaml.cs` pod kątem słowa kluczowego `Timer` i analizując przepływ wywołań.

---

## Błąd 2: Ciche kasowanie danych użytkownika (`File.WriteAllText`)

- **Na czym polega:** Wywołanie `File.WriteAllText(path, NoteBox.Text)` bezwzględnie zastępuje całą zawartość wskazanego pliku wyłącznie bieżącą treścią kontrolki `NoteBox`. Zamiast dopisywać nową notatkę z nagłówkiem Markdown na początku lub końcu pliku (zgodnie z wymaganiami z Rundy 2–4 i specyfikacją akumulowania notatek), kod przy każdym zapisie bezpowrotnie niszczy wszystkie wcześniej zapisane notatki. Dodatkowo jeśli użytkownik w oknie dialogowym kliknie „Anuluj”, zmienna `path` otrzyma wartość `null`, co sprawi, że `File.WriteAllText` rzuci `ArgumentNullException` i zawiesi program.
- **Kształt halucynacji:** **Coś, co istnieje, użyte źle** w kontekście domeny biznesowej. Metoda biblioteki standardowej jest w pełni poprawna technicznie i kompiluje się bez zastrzeżeń, ale realizuje operację destrukcyjną, sprzeczną z podstawowym celem programu.
- **Kiedy się ujawni:** Dopiero po kilku zapisach. Pierwszy zapis sprawia pozorne wrażenie sukcesu, ale już przy zapisaniu drugiej notatki użytkownik odkrywa, że pierwsza bezpowrotnie zniknęła z pliku.
- **Czym bym to sprawdził:** Testem sekwencyjnego zapisu: uruchomieniem aplikacji, zapisaniem pierwszej notatki, zapisaniem drugiej notatki, a następnie inspekcją zawartości pliku na dysku (np. poleceniem `Get-Content notatki.md`), weryfikując, czy plik zawiera obie notatki, czy tylko ostatnią.
- **Stan w naszym notatniku:** Mój notatnik nie ma tego błędu: przed zapisem odczytuje dotychczasową zawartość pliku przez `File.ReadAllText(_notesFilePath, Encoding.UTF8)` i dokleja nowy wpis na samej górze (`$"{newEntry}\n\n{existingContent}"`), co ustaliłem weryfikacją pliku `notatki.md` zawierającego nienaruszoną historię wszystkich kolejno tworzonych wpisów.

---

## Błąd 3: Kłamliwy komentarz o obsłudze błędów

- **Na czym polega:** Komentarz w kodzie bezpośrednio obiecuje: `// Obsługa błędów zapisu jest w środku.` Wewnątrz metody `SaveButton_Click` nie ma jednak żadnego bloku `try-catch`, sprawdzania uprawnień do pliku, obsługi atrybutu tylko do odczytu, braku miejsca na dysku ani zabezpieczenia przed wartością `null` po anulowaniu okna wyboru pliku. Każda nieprzewidziana sytuacja wejścia-wyjścia skutkuje natychmiastowym nieobsłużonym wyjątkiem. Ponieważ metoda ma sygnaturę `async void`, nieobsłużony wyjątek powoduje bezpośredni crash całego procesu aplikacji bez możliwości jego przechwycenia w wyższych warstwach.
- **Kształt halucynacji:** **Zdanie o kodzie, które nie jest prawdą**. Asystent AI wygenerował deklarację o obecności mechanizmu ochronnego, którego faktycznie nie zaimplementował w kodzie.
- **Kiedy się ujawni:** W trakcie działania aplikacji w momencie wystąpienia błędu wejścia/wyjścia (anulowanie dialogu, brak uprawnień zapisu do ścieżki docelowej, zablokowany plik). Kompilator ignoruje komentarze, więc na etapie budowania błąd jest całkowicie niewykrywalny.
- **Czym bym to sprawdził:** Inspekcją kodu źródłowego (sprawdzenie obecności bloku `try-catch`) oraz testem negatywnym w runtime: nadaniem plikowi docelowemu atrybutu tylko do odczytu (`attrib +r notatki.md`) i próbą zapisu, aby zweryfikować, czy program obsłuży błąd w interfejsie, czy się wyłączy.
- **Stan w naszym notatniku:** Mój notatnik nie ma tego błędu: metoda `OnSaveClicked` zawiera rzeczywisty blok `try ... catch (Exception ex)`, który w razie błędu wejścia-wyjścia wyświetla czytelny komunikat w kontrolce `StatusText` i nie dopuszcza do awarii aplikacji, co ustaliłem inspekcją kodu oraz testem z plikiem `notatki.md` ustawionym z atrybutem tylko do odczytu.
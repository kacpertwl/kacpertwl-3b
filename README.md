# Zadanie 3.2: Notatnik i cztery rundy próśb

Projekt aplikacji notatnika desktopowego w technologii **Avalonia .NET (C#)** stworzony w ramach ćwiczenia z inżynierii promptów i iteracyjnej pracy z asystentem AI.

---

## 1. Przegląd rund i eksperymentów

W repozytorium znajduje się pełna historia commitów i dokumentacja wyników kolejnych etapów w folderze [`rundy/`](rundy/):

| Runda | Nazwa pliku | Typ promptu | Opis prośby i zachowanie modelu |
|---|---|---|---|
| **Runda 1** | [`rundy/runda-1.md`](rundy/runda-1.md) | Zero-shot (sama prośba) | Jednozdaniowa prośba: *„Zrób notatnik: pole na tytuł, pole na treść, przycisk zapisu i lista zapisanych notatek. Notatki mają być zapisywane do pliku.”* Model sam dopowiedział format i stworzył osobne pliki `.txt` z datą i godziną w nazwie (`notatka_YYYYMMDD_HHMMSS.txt`). |
| **Runda 2** | [`rundy/runda-2.md`](rundy/runda-2.md) | Zero-shot + format | Doprecyzowano format: jeden plik `notatki.md`, format nagłówka `## YYYY-MM-DD HH:mm — Tytuł`, najnowsza notatka na górze, zachowanie polskich znaków (UTF-8). |
| **Runda 3** | [`rundy/runda-3.md`](rundy/runda-3.md) | One-shot | Do prośby dołączono jeden gotowy wzorzec pliku `notatki.md`. Model bez wahania odwzorował strukturę nagłówka, pustych linii i kolejności. |
| **Runda 4** | [`rundy/runda-4.md`](rundy/runda-4.md) | Few-shot | Dołączono trzy zróżnicowane przykłady: wpis jednolinijkowy, wielolinijkowy z dodatkowym myślnikiem w tytule oraz wpis z pełnym zestawem polskich znaków diakrytycznych. Format stał się w 100% jednoznaczny. |
| **Runda 5** | [`rundy/runda-5.md`](rundy/runda-5.md) | Meta-prompt | Wysłano prośbę o przeanalizowanie prośby z Rundy 1 i wskazanie braków. Model wskazał luki w specyfikacji (kodowanie, lokalizacja, sortowanie, parsowanie) i zaproponował gotowy, rozszerzony prompt. |
| **Powtórka**| [`rundy/runda-1-powtorka.md`](rundy/runda-1-powtorka.md) | Test niedeterminizmu | Uruchomienie Rundy 1 po raz drugi bez żadnych zmian w zapytaniu. |

---

## 2. Runda powtórzona — badanie niedeterminizmu

- **Co się różni:** W pierwszym uruchomieniu Rundy 1 model wygenerował mechanizm zapisu każdej notatki do osobnego pliku tekstowego z datą w nazwie (`notatki/notatka_YYYYMMDD_HHMMSS.txt`). W powtórzonym uruchomieniu (plik [`rundy/runda-1-powtorka.md`](rundy/runda-1-powtorka.md)) ten sam model przy identycznej prośbie stworzył pojedynczy plik `notes.json` ze strukturą tablicy obiektów JSON.
- **Wniosek:** *Aż do kilkudziesięciu procent różnic obserwowanych w rundach pozbawionych ścisłego formatu (zwłaszcza Runda 1 vs Runda 2) może wynikać ze zwykłego niedeterminizmu modelu i losowości próbkowania (temperature/top-p), a nie ze zmiany stopnia „zrozumienia” intencji użytkownika.*

---

## 3. Zalecenie dotyczące promptów od producenta modelu (Google Gemini)

- **Rada:** *Używanie jednoznacznych separatorów i tagów semantycznych (np. bloków Markdown lub znaczników XML `<instructions>`, `<format_specification>`, `<examples>`) do ścisłego oddzielania poleceń operacyjnych od danych wejściowych i wzorców formatowania.*
- **Źródło i data odczytania:** [Google Gemini Prompting Strategies Guide](https://ai.google.dev/gemini-api/docs/prompting-strategies) (odczytano: 27 października 2026).
- **Wpływ na notatnik:** *Wprowadzenie wyraźnych znaczników blokowych wokół przykładowego pliku markdown zagwarantowało, że model nie zinterpretował przykładowego tekstu notatki jako kodu C# ani stałych wpisanych na sztywno w logikę kontrolera.*

---

## 4. Sprawdź się — odpowiedzi na pytania z lekcji

1. **Trzy rzeczy, które siedzą w jednej prośbie do modelu:**
   - **Zadanie** — co ma się stać (cel biznesowy/funkcjonalny, np. „zapisuj notatki do pliku”).
   - **Format** — jak dokładnie ma wyglądać wynik na dysku lub w odpowiedzi (struktura nagłówka, podział na linie, kolejność, rozszerzenie pliku).
   - **Ograniczenia** — czego nie wolno lub jakie reguły techniczne obowiązują (brak zewnętrznych bibliotek, kodowanie UTF-8, konwencje nazewnicze).

2. **Czym różni się zero-shot od few-shot:**
   - **Zero-shot** to polecenie, w którym zadanie i format opisujemy wyłącznie słownie, nie podając żadnego gotowego przykładu oczekiwanego wyniku.
   - **Few-shot** polega na dołączeniu do promptu kilku (zwykle 2–5) konkretnych, różnorodnych przykładów wejścia i wyjścia, co pozwala modelowi bezbłędnie wychwycić stałe elementy wzorca i skrajne przypadki.

3. **Jedna sytuacja, w której proszenie o rozumowanie krok po kroku (Chain-of-Thought) jest złym pomysłem:**
   - Gdy pracujemy z nowoczesnymi modelami wyposażonymi w natywny mechanizm myślenia (ang. *thinking models*), takimi jak Gemini 2.0/3.0 Flash Thinking czy Claude 3.7 Sonnet Thinking, które same wewnętrznie planują kroki przed udzieleniem odpowiedzi. Narzucenie im sztywnej, sztucznej listy kroków w prompcie degraduje jakość ich własnego procesu dedukcyjnego.

---

## 5. Własną ręką: obsługa błędu zapisu (try / catch)

W ramach części samodzielnej zabezpieczono operację zapisu do pliku przed awarią:
- **Zmiany w XAML:** W pliku `MainWindow.axaml` pod przyciskiem zapisu dodano kontrolkę `<TextBlock Name="StatusText" Margin="0,5,0,0" TextWrapping="Wrap" />`.
- **Zmiany w C#:** W metodzie `OnSaveClicked` operację `File.WriteAllText` ujęto w blok `try-catch (Exception ex)`:
  ```csharp
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
  ```
- **Test z plikiem tylko do odczytu (read-only):**
  Po nadaniu plikowi `notatki.md` atrybutu *tylko do odczytu* (Read-Only) i kliknięciu „Zapisz”, aplikacja nie uległa awarii ani nie zamknęła okna. W kontrolce `StatusText` natychmiast wyświetlił się komunikat systemowy:
  `Nie udało się zapisać: Access to the path '...\notatki.md' is denied.`
  Po zdjęciu atrybutu ponowny zapis zakończył się sukcesem i komunikatem `Zapisano`.
- **Czas i trudność:** Realizacja zajęła około 8 minut; kluczowe było upewnienie się, że pola tekstowe nie czyszczą się, dopóki zapis do pliku nie zakończy się powodzeniem.

---

## 6. Trwałość danych (warunek 5 notatek)

Format z Rundy 4 gwarantuje, że przy każdym zapisie odczytywana jest cała dotychczasowa zawartość pliku, nowy wpis dodawany jest na samą górę, a całość zapisywana jest w UTF-8. Dzięki temu po dopisaniu 5 notatek żadna wcześniejsza notatka nie zostaje ucięta ani utracona.

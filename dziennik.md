# Dziennik weryfikacji

## 2026-10-27 — Notatnik z budowy 2 (weryfikacja wsteczna po rundach 1–5)
- Sprawdzone: `dotnet build` w katalogu `notatnik` → 0 błędów, 0 ostrzeżeń.
- Sprawdzone: zapis 5 kolejnych notatek w aplikacji → plik `notatki.md` ma 5 wpisów, najnowszy na górze, nagłówki w formacie `## YYYY-MM-DD HH:mm — Tytuł`.
- Sprawdzone: obsługa błędu zapisu przy pliku z atrybutem tylko do odczytu (`attrib +r notatki.md`) → aplikacja nie uległa awarii, wyświetliła w `StatusText` komunikat `Nie udało się zapisać: Access to the path ... is denied.`
- Nie wiadomo: co się stanie, gdy użytkownik nie poda tytułu notatki (przyjęto na wiarę kod agenta `(TitleBox.Text ?? "Bez tytułu").Trim()`, nie weryfikując, że dla pustego pola `TextBox.Text` zwraca pusty ciąg `""`, a nie `null`, przez co notatka zapisywała się z pustym tytułem po myślniku).
- Nie wiadomo: co się stanie, gdy treść notatki zawiera nagłówek markdown zaczynający się od `## ` (metoda `RefreshNotesList()` traktuje każdą linię z `## ` jako osobną notatkę).
- Nie wiadomo: jak zachowuje się aplikacja przy bardzo dużej liczbie notatek (brak wirtualizacji i paginacji listy).

## 2026-11-17 — Weryfikacja notatnika po zakończeniu modułu
- Sprawdzone: czy się buduje → `dotnet build` w katalogu `notatnik` → kompilacja zakończona sukcesem, 0 błędów, 0 ostrzeżeń.
- Sprawdzone: czy się uruchamia → `dotnet run` → okno `MainWindow` (700x500 px) uruchamia się poprawnie, poprawnie inicjalizuje kontrolki oraz motyw graficzny FluentTheme zgodnie z bieżącym motywem systemowym.
- Sprawdzone: czy zapisuje dokładnie tak, jak mówi opis notatnika → zapis notatki z polskimi znakami diakrytycznymi („Zażółć gęślą jaźń”) i wielolinijkową treścią dodaje wpis z nagłówkiem `## YYYY-MM-DD HH:mm — Tytuł` na samą górę pliku `notatki.md`, zachowuje całą dotychczasową zawartość pliku w kodowaniu UTF-8, czyści formularz i odświeża listę `NotesList`. Dodatkowo zweryfikowano poprawkę fallbacku: pusty tytuł zapisuje się jako „Bez tytułu”.
- Nie wiadomo: co się stanie, gdy w trakcie zapisu inny proces zablokuje plik `notatki.md` z blokadą wyłączną (`FileShare.None`).

---

### Sprawdzenie nazwy z notatnika w dokumentacji
- **Nazwa:** `ActualThemeVariant` (właściwość z przestrzeni nazw `Avalonia.Styling` w klasie `StyledElement`, użyta w `MainWindow.axaml.cs`: `ThemeToggle.IsChecked = Application.Current.ActualThemeVariant == ThemeVariant.Dark;`).
- **Link do dokumentacji:** https://docs.avaloniaui.net/docs/styling/theme-variants
- **Data odczytania:** 17 listopada 2026 r.
- **Co robi:** `ActualThemeVariant` to właściwość tylko do odczytu, która zwraca wariant motywu faktycznie renderowany przez dany element lub całą aplikację (`ThemeVariant.Light`, `ThemeVariant.Dark` lub wariant niestandardowy) po uwzględnieniu hierarchii dziedziczenia stylów w drzewie kontrolek i motywu systemu operacyjnego.
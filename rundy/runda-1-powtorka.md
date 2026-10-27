# Wynik rundy 1 — powtórka (badanie niedeterminizmu)

## Warunki eksperymentu
Uruchomiono dokładnie tę samą prośbę co w Rundzie 1, bez ani jednej zmiany w promptcie:
> "Zrób notatnik: pole na tytuł, pole na treść, przycisk zapisu i lista zapisanych notatek. Notatki mają być zapisywane do pliku."

## Co powstało tym razem
Podczas pierwszego uruchomienia Rundy 1 model wygenerował mechanizm tworzenia osobnych plików `.txt` w podfolderze `notatki/` z datą w nazwie pliku.

W powtórzonym uruchomieniu (Runda 1 powtórka) model zinterpretował „zapisuj do pliku” zupełnie inaczej:
Zamiast osobnych plików tekstowych, utworzył pojedynczy plik `notes.json` w formacie tablicy obiektów JSON z właściwościami `Title`, `Content` i `CreatedAt`.

Oto dokładny stan pliku `notes.json` powstałego na dysku:

```json
[
  {
    "Title": "Zakupy",
    "Content": "mleko\nchleb",
    "CreatedAt": "2026-10-27T14:47:01"
  },
  {
    "Title": "Pomysł na projekt",
    "Content": "minutnik z historią sesji",
    "CreatedAt": "2026-10-27T14:47:35"
  }
]
```

## Obserwacje
1. **Niedeterminizm przy braku ograniczeń:** Przy niedookreślonym prompcie model ma zbyt dużą swobodę. Losowość w procesie generowania sprawiła, że za pierwszym razem powstał zestaw plików `.txt`, a za drugim pojedynczy plik `notes.json`.
2. **Wpływ na różnice między rundami:** Pokazuje to, że część różnic w zachowaniu modelu (szczególnie w rundach bez określonego formatu) nie wynika ze „zrozumienia” zadania, lecz z czystego przypadku i losowości wyboru ścieżki implementacyjnej. Dopiero ścisły format (Rundy 2–4) redukuje tę wariancję do zera.

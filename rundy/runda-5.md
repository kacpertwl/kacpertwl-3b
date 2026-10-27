# Wynik rundy 5 (meta-prompt)

## 1. Zastosowany meta-prompt
Do modelu wysłano zapytanie:
> "To jest moja prośba do agenta kodującego:
> *'Zrób notatnik: pole na tytuł, pole na treść, przycisk zapisu i lista zapisanych notatek. Notatki mają być zapisywane do pliku.'*
> Nie wykonuj jej. Wypisz, czego w niej brakuje, żeby wynik był jednoznaczny, i zaproponuj poprawioną wersję."

## 2. Co wytknął model (czego brakowało w prośbie z rundy 1):
1. **Lokalizacja i nazwa pliku:** Nie określono, czy plik ma być jeden (np. `notatki.md` w katalogu projektu), czy osobne pliki w podkatalogu.
2. **Format zapisu:** Brak definicji formatu (Markdown, JSON, surowy tekst .txt, CSV).
3. **Struktura pojedynczego wpisu:** Brak informacji, jak łączyć tytuł i treść oraz czy dołączać znacznik czasu (timestamp).
4. **Kolejność wpisów:** Brak określenia, czy dopisujemy na końcu (append), czy nowsze notatki lądują na początku (prepend).
5. **Kodowanie:** Brak wymuszenia UTF-8 (ryzyko utraty polskich znaków diakrytycznych).
6. **Interakcja z UI:** Czy kliknięcie notatki na liście ma podglądać/edytować treść, jak odświeżać listę.
7. **Niezamówione dodatki:** Model często proponuje też rzeczy nadmiarowe: pole ID, statusy, walidację pustych pól, potwierdzenia dialogowe.

## 3. Poprawiona wersja prośby wygenerowana przez model:
> "Stwórz aplikację notatnika w Avalonia .NET. Interfejs powinien zawierać: pole tekstowe na tytuł, wieloliniowe pole na treść, przycisk 'Zapisz' oraz listę zapisanych notatek po prawej stronie.
> Notatki zapisuj do jednego pliku `notatki.md` w głównym folderze projektu w kodowaniu UTF-8.
> Format każdego wpisu to:
> `## YYYY-MM-DD HH:mm — [Tytuł]`
> `[Treść notatki]`
> Nowe wpisy wstawiaj zawsze na samej górze pliku, oddzielając je podwójnym znakiem nowej linii (pustą linią).
> Przy uruchomieniu i po każdym zapisie lista powinna wyświetlać nagłówki wszystkich notatek z pliku.
> Zadbaj o bezbłędną obsługę polskich znaków (ą, ę, ś, ć, ż, ź, ó, ł, ń)."

## 4. Wynikowy plik na dysku (`notatki.md`):
```markdown
## 2026-10-27 14:53 — Porównanie prośby ręcznej i meta-promptu
Meta-prompt precyzyjnie wydobył wszystkie luki specyfikacji, które człowiek musiałby wymyślać ręcznie.
Jednocześnie widać tendencję do proponowania nadmiarowych funkcji, jeśli nie zostaną one odrzucone.

## 2026-10-27 14:48 — Pomysł na projekt
minutnik z historią sesji

## 2026-10-27 14:47 — Zakupy
mleko
chleb
```

## 5. Wnioski: Runda 2 vs Runda 5
- Runda 2 (ręcznie dopisany format) była minimalistyczna i osiągnęła cel w 4 linijkach.
- Runda 5 (meta-prompt) automatycznie zidentyfikowała niejednoznaczności techniczne (kodowanie, kolejność, parsowanie), ale miała tendencję do rozbudowywania specyfikacji o elementy, o które nikt nie prosił. Jest to doskonała technika przygotowawcza, pod warunkiem krytycznej selekcji propozycji.

# Wynik rundy 4 (few-shot)

W rundzie 4 do prośby dołączono trzy różnorodne przykłady (few-shot):
1. Wpis jednolinijkowy.
2. Wpis wielolinijkowy z myślnikiem w tytule.
3. Wpis z polskimi znakami diakrytycznymi.

Przykłady te jednoznacznie zademonstrowały modelowi:
- Że tytuł może zawierać dodatkowe myślniki, a separatorem daty i tytułu jest ` — ` (em-dash lub en-dash z odstępami).
- Że treść może mieć jedną linijkę lub wiele linijek bez łamania struktury oddzielającej wpisy pustą linią.
- Że polskie znaki są w pełni wspierane (UTF-8).
- Że każda kolejna notatka trafia na samą górę pliku, nie usuwając poprzednich wpisów.

Oto stan pliku `notatki.md` na dysku po zapisaniu kolejnych notatek:

```markdown
## 2026-10-27 14:52 — Zażółć gęślą jaźń
Wpisałem tę notatkę, aby sprawdzić kodowanie znaków: ą, ć, ę, ł, ń, ó, ś, ź, ż.

## 2026-10-27 14:51 — Zadania domowe — część 1
zrobić pranie
posprzątać pokój
ugotować obiad

## 2026-10-27 14:50 — Zakupy
mleko
```

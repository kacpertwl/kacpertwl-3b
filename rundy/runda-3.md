# Wynik rundy 3 (one-shot)

W rundzie 3 do prośby dołączono jeden gotowy przykład (one-shot):
> "Plik ma wyglądać dokładnie tak:
> ## 2026-09-08 14:32 — Zakupy
> mleko
> chleb
> 
> ## 2026-09-07 09:15 — Pomysł na projekt
> minutnik z historią sesji"

Dzięki pokazaniu przykładu zamiast samego opisu słownego, model natychmiast odwzorował dokładną strukturę: podwójny znak `#`, spację, datę w formacie `YYYY-MM-DD HH:mm`, pauzę `—`, tytuł, a pod spodem treść i odstęp pustą linią.

Oto stan pliku `notatki.md` po zapisaniu dwóch notatek:

```markdown
## 2026-10-27 14:49 — Pomysł na projekt
minutnik z historią sesji

## 2026-10-27 14:47 — Zakupy
mleko
chleb
```

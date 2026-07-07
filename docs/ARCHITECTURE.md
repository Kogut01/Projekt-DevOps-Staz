### Architektura

W projekcie są trzy mikroserwisy i każdy ma swoją prostą rolę.

- `hello_service` zwraca zwykłą odpowiedź `Hello, World!`, przez `GET /hello` i ma endpoint `GET /health` zwracający tekst `Healthy`.
- `user_service` trzyma listę użytkowników w pamięci, obsługuje `GET /users`, `POST /users` oraz `GET /health`.
- `audit_service` zapisuje wpisy audytowe, zwraca je przez `GET /audit`. Obsługuje również `POST /audit` oraz `GET /health`.

<br>

### Komunikacja między usługami

Najważniejsza komunikacja jest między `user_service` i `audit_service`. Gdy dodawany jest nowy użytkownik, `user_service` wysyła do `audit_service` wpis audytowy na adres `POST /audit`.

`hello_service` nie łączy się z innymi usługami, działa samodzielnie.

W Dockerze wszystkie usługi są w jednej sieci, więc `user_service` może wołać `audit_service` po nazwie kontenera: `http://audit_service/audit`.

Lokalnie każda usługa działa na swoim porcie z `launchSettings.json`, ale przepływ ten jest prostszy do sprawdzenia w kontenerach.

<br>

### Diagram przedstawiający architekturę znajduje się pod nazwą: [ARCHITECTURE.png](./ARCHITECTURE.png)
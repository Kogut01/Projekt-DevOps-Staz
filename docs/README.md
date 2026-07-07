<div align="center">

# DevOps - Projekt

To jest prosty projekt mikroserwisów zrobiony w ASP.NET Core. Pokazuje podstawową komunikację między usługami i proste uruchamianie lokalnie oraz w Dockerze.

![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0089D6?style=flat-square&logo=microsoft-azure&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker&logoColor=white)
[![License](https://img.shields.io/github/license/Kogut01/dotfiles?style=flat-square&color=A3BE8C)](../LICENSE)

</div>

<br>

### Co jest w środku

- `hello_service` zwraca `Hello, World!` i ma `GET /health`.
- `user_service` trzyma użytkowników w pamięci, obsługuje `GET /users` i `POST /users`.
- `audit_service` zapisuje wpisy audytowe i udostępnia je przez `GET /audit`.

Najważniejsze jest to, że `user_service` wysyła wpis audytowy do `audit_service` przy dodawaniu użytkownika.

<br>

### 💻 Uruchomienie lokalne

Każdy mikroserwis uruchom osobno:

```bash
dotnet run --project services/hello_service/hello_service.csproj
```

```bash
dotnet run --project services/user_service/user_service.csproj
```

```bash
dotnet run --project services/audit_service/audit_service.csproj
```

Usługi działają wtedy na portach z plików `launchSettings.json`:

- `hello_service` - `http://localhost:5169`
- `user_service` - `http://localhost:5008`
- `audit_service` - `http://localhost:5268`

<br>

### 🐳 Uruchomienie w Dockerze

Każdy serwis ma własny `Dockerfile`. Jeśli uruchamiasz je osobno, `user_service` i `audit_service` muszą być w tej samej sieci, żeby audyt działał po nazwie kontenera.

Najprościej użyć `docker compose`:

```bash
docker compose up -d
```

Przed startem upewnij się, że masz plik `.env` z portami, na przykład:

```env
AUDIT_SERVICE_PORT=12345
HELLO_SERVICE_PORT=12346
USER_SERVICE_PORT=12347
```

Po uruchomieniu usługi będą dostępne na portach z `.env`.

Na koniec zatrzymaj środowisko:

```bash
docker compose down
```

---

<div align="center">

### 📜 Licencja 📜

Projekt jest udostępniany na licencji [MIT](../LICENSE).

Copyright © 2026 [Kogut01](https://github.com/Kogut01)

</div>

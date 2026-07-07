### Deployment

W tym projekcie CI/CD jest zrobione osobno dla każdej usługi. Pipeline CI najpierw sprawdza, czy aplikacja się buduje, potem uruchamia prostą kontrolę jakości kodu, buduje obraz Dockera i robi skan bezpieczeństwa obrazu. Dzięki temu przed wdrożeniem można szybciej wyłapać podstawowe błędy.

Pipeline CD uruchamia się po zmianach na gałęzi `main` dla danej usługi. Jego zadanie to zbudowanie obrazu, wysłanie go do Azure Container Registry, a potem aktualizacja aplikacji w Azure Container Apps.

W praktyce wygląda to następująco:

1. kod trafia do repozytorium,
2. otwierany jest nowy pull request,
3. pipeline CI sprawdza build i jakość,
4. obraz jest budowany i skanowany,
5. po mergu na `main` lub comicie pipeline CD zaczyna działanie,
6. pipeline CD wysyła obraz do ACR,
7. aktualizuje się Azure Container Apps przy pomocy az containerapp update,
8. Azure Container Apps dostaje nową wersję usługi.

W tym projekcie ważne jest też to, że usługi działają w jednej sieci kontenerowej, więc `user_service` może komunikować się z `audit_service` po nazwie kontenera.

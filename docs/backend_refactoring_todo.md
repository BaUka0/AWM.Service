# Backend Refactoring & Production Readiness (Roadmap)

Этот документ содержит список задач для доведения Backend-части AWM.Service до уровня промышленной эксплуатации (Production-Ready).

## 1. 🛠️ Оставшийся Архитектурный Рефакторинг (Stage 3)
Задачи по улучшению внутренней структуры приложения.

- [ ] **ICacheableQuery & CachingBehavior**:
    - Внедрить интерфейс-маркер для запросов, требующих кэширования.
    - Добавить MediatR Pipeline Behavior для автоматического кэширования (Memory/Redis).
- [ ] **Domain Event Handlers**:
    - Реализовать обработчики (`INotificationHandler`) для опубликованных событий (например, отправка уведомлений при создании темы или изменении статуса).
- [ ] **Domain Value Objects**:
    - Заменить примитивные типы (`string email`) на объекты-значения (`EmailAddress`) для инкапсуляции валидации в доменном слое.

---

## 2. 🛡️ Безопасность (Security)
Критически важные настройки для работы в открытой сети.

- [ ] **CORS Configuration**:
    - Настроить политики доступа для фронтенд-приложений.
- [ ] **Rate Limiting**:
    - Ограничить количество запросов к API для защиты от перегрузок и брутфорса.
- [ ] **Security Headers**:
    - Добавить заголовки HSTS, X-Content-Type-Options, Content-Security-Policy.

---

## 3. 🚦 Наблюдаемость и Мониторинг (Observability)
Чтобы понимать, что происходит с приложением на сервере.

- [ ] **Serilog Integration**:
    - Внедрить структурированное логирование в формате JSON.
- [ ] **Health Checks**:
    - Добавить `/health` endpoint для проверки состояния API и базы данных.

---

## 4. 🚀 Отказоустойчивость и Перфоманс (Resilience)
- [ ] **EF Core Resiliency**:
    - Настроить `EnableRetryOnFailure` в `DbContextOptions`.
- [ ] **Redis for Distributed Caching**:
    - Заменить In-Memory кэш на распределенный (Redis) для возможности масштабирования.
- [ ] **Data Protection API**:
    - Настроить хранение ключей шифрования вне контейнера, чтобы сессии/токены не инвалидировались при рестартах.

---

## 5. 🗄️ Миграции и Инфраструктура
- [ ] **Production Database Strategy**:
    - Отказаться от `EnsureDeleted/Created` в пользу автоматизированных миграций (`dotnet ef migrations`).
- [ ] **Dockerization**:
    - Создать промышленный мульти-стейдж `Dockerfile`.
- [ ] **Environment Settings**:
    - Тщательно настроить `appsettings.Production.json` (отключение Swagger, Secrets management).

---

## Статус Бэкенда
- [x] Чистая Архитектура (Core)
- [x] Аудит (Interceptor)
- [x] CQRS (MediatR)
- [x] Маппинг ответов (Mapster)
- [ ] Тестовое покрытие (Unit/Integration Tests) — *Pending*
- [ ] Production-Ready Configuration — *In Progress*

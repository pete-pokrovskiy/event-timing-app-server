Все команды :
https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet
https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/

Добавление новой миграции через cli и обовление БД:
1. Перейти в проект, где лежат миграции через cd (Полезное расширение, чтобы открывать консоль прямо из студии - https://marketplace.visualstudio.com/items?itemName=MadsKristensen.OpenCommandLine)
2.1 Предварительно нужно зачекаутить файл <context>ModelSnapshot.cs, тк cli сам этого делать не умеет - в противном случае будет Access.. is denied
2.2 Сформировавть миграцию с указанным стартап проектом - dotnet ef migrations add Initial --startup-project "../EventTiming.API"
3. Добавить новые файлы миграций в TFS (если по умолчанию они не добавились)

Опционально (почему опциаонально - миграции автоматически накатятся при первом запуске проекта)
(4. Обновить БД вручную - dotnet ef database update --startup-project "../EventTiming.API"   - обновление будет для той бд, которая указана в конфиге appsettings.json по умолчанию. 
Для обновления бд на других стендах нужно выставить на локальной машине переменную окружения ASPNETCORE_ENVIRONMENT с соответствующим значением)

Удаление БД - dotnet ef database drop --startup-project "../EventTiming.API"


--------------------------------------------------------------------------------------------------------------------

Для ef 3.1

- dotnet tool install --global dotnet-ef --version 3.0.0
- nugets for startup project :
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
Microsoft.EntityFrameworkCore.Tools

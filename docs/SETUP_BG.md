# Стартиране в Cursor / Windows

Копирай ZIP съдържанието в E:\GitHub\selenium-csharp-ui-tests. Запази .git и LICENSE. Замени README и .gitignore.

Няма Python .venv. Използваме dotnet restore и dotnet test според README. Chrome трябва да е инсталиран. При първо стартиране Selenium Manager може да изтегли подходящ driver.

За видим браузър: $env:HEADLESS = "false". За headless: $env:HEADLESS = "true".

Тестовете сами стартират локалното приложение. Не отваряй HTML файла директно с file:// — тестовете ползват HTTP сървър.

Ако има NuGet, browser или driver грешка, запази точния текст. Не приемай build success за test success. Преди Git commit настрой локалния user.name и точния GitHub noreply user.email.

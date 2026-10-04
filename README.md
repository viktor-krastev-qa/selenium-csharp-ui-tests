# Selenium C# UI Tests

A QA Automation portfolio project using C#, Selenium WebDriver, NUnit and Page Object Model. It tests a bundled local TaskBoard demonstration application through Chrome.

## Quick start
Requires .NET 9 SDK and Google Chrome. Initial NuGet restore and Selenium Manager driver resolution require internet access.
```powershell
git clone https://github.com/viktor-krastev-qa/selenium-csharp-ui-tests.git
cd selenium-csharp-ui-tests
dotnet restore tests/UiTests/UiTests.csproj
dotnet test tests/UiTests/UiTests.csproj --logger "trx;LogFileName=ui-tests.trx" --results-directory TestResults
```
If already cloned, skip clone/cd. The test fixture starts an embedded localhost HTTP server automatically, chooses an available port, creates a fresh Chrome session for every test and stops the server afterward. No separate Python, Docker or external website is required.

To see the browser:
```powershell
$env:HEADLESS = "false"
dotnet test tests/UiTests/UiTests.csproj
```
The default is headless. Selenium Manager resolves the Chrome driver; no ChromeDriver NuGet binary package is pinned.

## Coverage
- Login: valid, invalid and missing credentials; logout and refreshed logged-out state.
- Empty board, task creation and validation; 1/70-character boundaries, 71-character rejection and trimming.
- Case-insensitive partial search, no-match state and clearing search.
- Deleting one or the final task; persistence after refresh.
- HTML-like task title rendered as text (one focused regression, not a complete XSS assessment).

All 23 NUnit UI test cases passed locally on Windows with Chrome and .NET 9.

- [Local HTML report](examples/local/report.html) — download and open.
- [Local TRX results](examples/local/ui-tests.trx).
- See [validation details](docs/VALIDATION.md).

## Design
`Pages/` encapsulates locators/actions; assertions belong to `Tests/`. Explicit WebDriverWait is used for action completion; implicit wait is zero. The demo updates synchronously. There are no Thread.Sleep calls. Stable IDs/data-testid selectors support readable tests.

`Infrastructure/DemoServer.cs` hosts only three allowlisted static files. `UiTestBase.cs` owns browser lifecycle and captures PNG/HTML on assertion failure without replacing the original failure if evidence capture fails. NUnit runs this fixture sequentially. A new browser profile isolates sessionStorage/localStorage across tests. Tests interact through WebDriver, not injected JavaScript.

## Results
TRX results are written to `TestResults/`. To generate HTML from actual results (Python 3 required):
```powershell
py -3.13 scripts/trx_to_html.py
Start-Process TestResults/report.html
```
 Failure screenshots and page source are attached through NUnit and stored under the test working directory's `artifacts/`. GitHub Actions uploads available evidence even on failure. Local Chrome execution is verified; GitHub Actions validation is pending.

## Demo limitations
The bundled app is authored alongside the tests. It is a small controlled test target, not an independent production system. The public credentials `qa_user` / `QaDemo123!` are synthetic client-side demo values. Authentication is not secure, no backend authorization is implemented, and task persistence exists only in the browser profile. There is no API, database, account registration, edit operation or multi-user isolation.

## .NET lifecycle
This version targets the user's installed .NET 9 SDK. Microsoft support for .NET 9 ends November 10, 2026. Before that date, migrate TargetFramework to net10.0, update CI to 10.0.x, install the .NET 10 SDK and rerun all tests. Source: https://dotnet.microsoft.com/en-us/platform/support/policy

## References
- https://www.selenium.dev/documentation/selenium_manager/
- https://www.selenium.dev/documentation/webdriver/waits/
- https://docs.nunit.org/articles/nunit/getting-started/installation.html

## License
MIT — see [LICENSE](LICENSE).

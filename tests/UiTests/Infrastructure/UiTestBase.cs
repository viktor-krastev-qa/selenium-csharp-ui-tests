using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UiTests.Pages;

namespace UiTests.Infrastructure;

public abstract class UiTestBase
{
    private DemoServer? server;
    protected IWebDriver Driver = null!;
    protected LoginPage Login = null!;
    protected BoardPage Board = null!;

    [OneTimeSetUp]
    public void StartServer() => server = new DemoServer();

    [SetUp]
    public void OpenBrowser()
    {
        var options = new ChromeOptions();
        if (Environment.GetEnvironmentVariable("HEADLESS") != "false") options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1400,1000");
        options.AddArgument("--disable-dev-shm-usage");
        if (OperatingSystem.IsLinux()) options.AddArgument("--no-sandbox");
        Driver = new ChromeDriver(options);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        Driver.Navigate().GoToUrl(server!.Url);
        Login = new LoginPage(Driver); Board = new BoardPage(Driver);
    }

    [TearDown]
    public void CloseBrowser()
    {
        try
        {
            if (Driver is not null && TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                try
                {
                    var folder = Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts");
                    Directory.CreateDirectory(folder);
                    var prefix = Path.Combine(folder, "failure-" + Guid.NewGuid().ToString("N"));
                    var screenshot = prefix + ".png";
                    ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(screenshot);
                    TestContext.AddTestAttachment(screenshot, "Failure screenshot");
                    File.WriteAllText(prefix + ".html", Driver.PageSource);
                }
                catch (Exception ex) { TestContext.Progress.WriteLine("Artifact capture failed: " + ex.GetType().Name); }
            }
        }
        finally { Driver?.Quit(); Driver?.Dispose(); }
    }

    [OneTimeTearDown]
    public void StopServer() => server?.Dispose();
}

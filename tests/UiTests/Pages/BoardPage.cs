using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UiTests.Pages;

public sealed class BoardPage(IWebDriver driver)
{
    private readonly WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));
    private static readonly By Rows = By.CssSelector("[data-testid='task-row']");
    public bool IsVisible => driver.FindElement(By.Id("board-view")).Displayed;
    public string Welcome => driver.FindElement(By.Id("welcome")).Text;
    public string Error => driver.FindElement(By.Id("task-error")).Text;
    public bool EmptyVisible => driver.FindElement(By.Id("empty-state")).Displayed;
    public string[] Titles => driver.FindElements(By.CssSelector("[data-testid='task-name']")).Select(e => e.Text).ToArray();
    public void Create(string title)
    {
        var input = driver.FindElement(By.Id("task-title")); input.Clear(); input.SendKeys(title);
        driver.FindElement(By.Id("create-button")).Click();
        wait.Until(d => d.FindElement(By.Id("task-title")).GetAttribute("value") == "" || Error.Length > 0);
    }
    public void Search(string query)
{
    var input = driver.FindElement(By.Id("search"));

    input.Click();
    input.SendKeys(Keys.Control + "a");
    input.SendKeys(Keys.Backspace);

    if (!string.IsNullOrEmpty(query))
    {
        input.SendKeys(query);
    }
}
    public void Delete(string title)
    {
        var row = driver.FindElements(Rows).Single(e => e.FindElement(By.CssSelector("[data-testid='task-name']")).Text == title);
        row.FindElement(By.CssSelector("[data-testid='delete-task']")).Click();
        wait.Until(d => !Titles.Contains(title));
    }
    public void Logout()
    {
        driver.FindElement(By.Id("logout-button")).Click();
        wait.Until(d => d.FindElement(By.Id("login-view")).Displayed);
    }
}

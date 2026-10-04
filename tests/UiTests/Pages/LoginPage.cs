using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UiTests.Pages;

public sealed class LoginPage(IWebDriver driver)
{
    private readonly WebDriverWait wait = new(driver, TimeSpan.FromSeconds(5));
    public bool IsVisible => driver.FindElement(By.Id("login-view")).Displayed;
    public string Error => driver.FindElement(By.Id("login-error")).Text;
    public void SignIn(string username = "qa_user", string password = "QaDemo123!")
    {
        var user = wait.Until(d => d.FindElement(By.Id("username")));
        user.Clear(); user.SendKeys(username);
        var pass = driver.FindElement(By.Id("password")); pass.Clear(); pass.SendKeys(password);
        driver.FindElement(By.Id("login-button")).Click();
        wait.Until(d => d.FindElement(By.Id("board-view")).Displayed || d.FindElement(By.Id("login-error")).Text.Length > 0);
    }
}

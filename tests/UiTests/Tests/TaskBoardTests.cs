using NUnit.Framework;
using UiTests.Infrastructure;

namespace UiTests.Tests;

[TestFixture, NonParallelizable]
public class TaskBoardTests : UiTestBase
{
    [Test]
    public void LoginPage_HasExpectedTitle()
    {
        Assert.That(Driver.Title, Is.EqualTo("TaskBoard QA Demo"));
        Assert.That(Login.IsVisible, Is.True);
        Assert.That(Board.IsVisible, Is.False);
    }

    [Test]
    public void ValidLogin_ShowsBoardAndWelcome()
    {
        Login.SignIn();
        Assert.That(Board.IsVisible, Is.True);
        Assert.That(Board.Welcome, Is.EqualTo("Welcome, qa_user!"));
    }

    [TestCase("wrong_user", "QaDemo123!")]
    [TestCase("qa_user", "wrong_password")]
    public void InvalidCredentials_KeepUserLoggedOut(string user, string password)
    {
        Login.SignIn(user,password);
        Assert.That(Login.Error, Is.EqualTo("Invalid username or password."));
        Assert.That(Board.IsVisible, Is.False);
    }

    [TestCase("", "")]
    [TestCase("qa_user", "")]
    [TestCase("", "QaDemo123!")]
    public void MissingCredentials_ShowValidation(string user, string password)
    {
        Login.SignIn(user,password);
        Assert.That(Login.Error, Is.EqualTo("Username and password are required."));
        Assert.That(Login.IsVisible, Is.True);
    }

    [Test]
    public void Logout_ReturnsToLogin()
    {
        Login.SignIn(); Board.Logout();
        Assert.That(Login.IsVisible, Is.True);
        Assert.That(Board.IsVisible, Is.False);
        Driver.Navigate().Refresh();
        Assert.That(Login.IsVisible, Is.True);
    }

    [Test]
    public void NewSession_HasEmptyBoard()
    {
        Login.SignIn();
        Assert.That(Board.Titles, Is.Empty);
        Assert.That(Board.EmptyVisible, Is.True);
    }

    [Test]
    public void CreateTask_ShowsTitle()
    {
        Login.SignIn(); Board.Create("Write UI tests");
        Assert.That(Board.Titles, Is.EqualTo(new[]{"Write UI tests"}));
        Assert.That(Board.EmptyVisible, Is.False);
    }

    [TestCase(1)]
    [TestCase(70)]
    public void TitleBoundary_IsAccepted(int length)
    {
        var title = new string('a',length);
        Login.SignIn(); Board.Create(title);
        Assert.That(Board.Titles, Is.EqualTo(new[]{title}));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void BlankTitle_IsRejected(string title)
    {
        Login.SignIn(); Board.Create(title);
        Assert.That(Board.Error, Is.EqualTo("Title must contain 1 to 70 characters."));
        Assert.That(Board.Titles, Is.Empty);
    }

    [Test]
    public void TitleOverMaximum_IsRejected()
    {
        Login.SignIn(); Board.Create(new string('a',71));
        Assert.That(Board.Error, Is.EqualTo("Title must contain 1 to 70 characters."));
        Assert.That(Board.Titles, Is.Empty);
    }

    [Test]
    public void Title_IsTrimmed()
    {
        Login.SignIn(); Board.Create("  Review tests  ");
        Assert.That(Board.Titles, Is.EqualTo(new[]{"Review tests"}));
    }

    [Test]
    public void Search_IsCaseInsensitiveAndPartial()
    {
        Login.SignIn(); Board.Create("Review API tests"); Board.Create("Write UI tests");
        Board.Search("api");
        Assert.That(Board.Titles, Is.EqualTo(new[]{"Review API tests"}));
    }

    [Test]
    public void SearchWithNoMatch_ShowsEmptyState()
    {
        Login.SignIn(); Board.Create("Write UI tests"); Board.Search("missing");
        Assert.That(Board.Titles, Is.Empty);
        Assert.That(Board.EmptyVisible, Is.True);
    }

    [Test]
    public void ClearingSearch_RestoresTasks()
    {
        Login.SignIn(); Board.Create("First task"); Board.Create("Second task");
        Board.Search("First"); Board.Search("");
        Assert.That(Board.Titles, Is.EqualTo(new[]{"First task","Second task"}));
    }

    [Test]
    public void Delete_RemovesOnlySelectedTask()
    {
        Login.SignIn(); Board.Create("Keep task"); Board.Create("Delete task"); Board.Delete("Delete task");
        Assert.That(Board.Titles, Is.EqualTo(new[]{"Keep task"}));
    }

    [Test]
    public void DeleteLastTask_ShowsEmptyState()
    {
        Login.SignIn(); Board.Create("Delete task"); Board.Delete("Delete task");
        Assert.That(Board.Titles, Is.Empty);
        Assert.That(Board.EmptyVisible, Is.True);
    }

    [Test]
    public void Refresh_PreservesTaskAndSession()
    {
        Login.SignIn(); Board.Create("Persistent task"); Driver.Navigate().Refresh();
        Assert.That(Board.IsVisible, Is.True);
        Assert.That(Board.Titles, Is.EqualTo(new[]{"Persistent task"}));
    }

    [Test]
    public void HtmlLikeTitle_IsRenderedAsText()
    {
        var title = "<img src=x onerror=alert(1)>";
        Login.SignIn(); Board.Create(title);
        Assert.That(Board.Titles, Is.EqualTo(new[]{title}));
        Assert.That(Driver.FindElements(OpenQA.Selenium.By.CssSelector("#task-list img")), Is.Empty);
    }
}

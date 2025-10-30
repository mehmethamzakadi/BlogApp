using BlogApp.Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

[TestFixture]
public class UserNameTests
{
    [Test]
    public void Create_WithValidUserName_ShouldSucceed()
    {
        var validUserName = "testuser";
        var userName = UserName.Create(validUserName);
        Assert.That(userName.Value, Is.EqualTo(validUserName));
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void Create_WithEmptyUserName_ShouldThrowException(string invalidUserName)
    {
        Assert.Throws<ArgumentException>(() => UserName.Create(invalidUserName));
    }

    [Test]
    public void Create_WithNullUserName_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => UserName.Create(null));
    }

    [Test]
    [TestCase("ab")]
    public void Create_WithTooShortUserName_ShouldThrowException(string shortUserName)
    {
        Assert.Throws<ArgumentException>(() => UserName.Create(shortUserName));
    }

    [Test]
    public void Create_WithTooLongUserName_ShouldThrowException()
    {
        var longUserName = new string('a', 51);
        Assert.Throws<ArgumentException>(() => UserName.Create(longUserName));
    }

    [Test]
    [TestCase("user name")]
    [TestCase("user!name")]
    [TestCase("user#name")]
    public void Create_WithInvalidCharacters_ShouldThrowException(string invalidUserName)
    {
        Assert.Throws<ArgumentException>(() => UserName.Create(invalidUserName));
    }

    [Test]
    public void Equals_WithSameUserName_ShouldReturnTrue()
    {
        var userName1 = UserName.Create("testuser");
        var userName2 = UserName.Create("testuser");
        Assert.That(userName1, Is.EqualTo(userName2));
    }
}

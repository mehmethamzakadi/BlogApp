using BlogApp.Domain.Exceptions;
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
    [TestCase("abc")]
    [TestCase("user_name")]
    [TestCase("user-name")]
    [TestCase("User123")]
    public void Create_WithValidUserNames_ShouldSucceed(string validUserName)
    {
        var userName = UserName.Create(validUserName);
        Assert.That(userName.Value, Is.EqualTo(validUserName));
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void Create_WithEmptyUserName_ShouldThrowDomainValidationException(string invalidUserName)
    {
        Assert.Throws<DomainValidationException>(() => UserName.Create(invalidUserName));
    }

    [Test]
    public void Create_WithNullUserName_ShouldThrowDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() => UserName.Create(null!));
    }

    [Test]
    [TestCase("ab")]
    public void Create_WithTooShortUserName_ShouldThrowDomainValidationException(string shortUserName)
    {
        Assert.Throws<DomainValidationException>(() => UserName.Create(shortUserName));
    }

    [Test]
    public void Create_WithTooLongUserName_ShouldThrowDomainValidationException()
    {
        var longUserName = new string('a', 51);
        Assert.Throws<DomainValidationException>(() => UserName.Create(longUserName));
    }

    [Test]
    [TestCase("user name")]
    [TestCase("user!name")]
    [TestCase("user#name")]
    [TestCase("user@name")]
    public void Create_WithInvalidCharacters_ShouldThrowDomainValidationException(string invalidUserName)
    {
        Assert.Throws<DomainValidationException>(() => UserName.Create(invalidUserName));
    }

    [Test]
    public void Equals_WithSameUserName_ShouldReturnTrue()
    {
        var userName1 = UserName.Create("testuser");
        var userName2 = UserName.Create("testuser");
        Assert.That(userName1, Is.EqualTo(userName2));
    }

    [Test]
    public void Equals_WithDifferentCaseUserName_ShouldReturnTrue()
    {
        var userName1 = UserName.Create("TestUser");
        var userName2 = UserName.Create("testuser");
        Assert.That(userName1, Is.EqualTo(userName2));
    }

    [Test]
    public void Equals_WithDifferentUserName_ShouldReturnFalse()
    {
        var userName1 = UserName.Create("testuser1");
        var userName2 = UserName.Create("testuser2");
        Assert.That(userName1, Is.Not.EqualTo(userName2));
    }

    [Test]
    public void ToString_ShouldReturnValue()
    {
        var userName = UserName.Create("testuser");
        Assert.That(userName.ToString(), Is.EqualTo("testuser"));
    }

    [Test]
    public void ImplicitConversion_ShouldReturnValue()
    {
        var userName = UserName.Create("testuser");
        string userNameString = userName;
        Assert.That(userNameString, Is.EqualTo("testuser"));
    }
}

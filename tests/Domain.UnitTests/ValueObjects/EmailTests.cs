using BlogApp.Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

[TestFixture]
public class EmailTests
{
    [Test]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        var validEmail = "test@example.com";
        var email = Email.Create(validEmail);
        Assert.That(email.Value, Is.EqualTo(validEmail));
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public void Create_WithEmptyEmail_ShouldThrowException(string invalidEmail)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Test]
    public void Create_WithNullEmail_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => Email.Create(null));
    }

    [Test]
    [TestCase("invalid")]
    [TestCase("invalid@")]
    [TestCase("@example.com")]
    public void Create_WithInvalidFormat_ShouldThrowException(string invalidEmail)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Test]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");
        Assert.That(email1, Is.EqualTo(email2));
    }
}

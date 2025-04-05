using NUnit.Framework;
using SafeVault.Services;

namespace SafeVault.Tests;

public class TestInputValidation
{
    private UserService _service;

    [SetUp]
    public void Setup()
    {
        _service = new UserService("Data Source=:memory:");
    }

    [Test]
    public void TestForSQLInjection()
    {
        var result = _service.SanitizeInput("Robert'); DROP TABLE Users;--");
        Assert.That(result.Contains("DROP TABLE"), Is.False);
    }

    [Test]
    public void TestForXSS()
    {
        var result = _service.SanitizeInput("<script>alert('xss')</script>");
        Assert.That(result.Contains("<script>"), Is.False);
    }
}

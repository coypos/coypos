using FluentAssertions;
using NUnit.Framework;

namespace CoyposServer;

[TestFixture]
public class SampleTest
{
    [Test]
    public void Test()
    {
        var kajetanFajny = true;
        kajetanFajny.Should().BeTrue();
    }
}
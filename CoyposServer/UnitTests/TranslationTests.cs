using CoyposServer.Utils.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class TranslationTests
{
	[SetUp]
	public async Task Setup()
	{
		LanguageHelpers.DefaultLanguage = "pl";
	}
    
	[Test]
	public void CaseA()
	{
		var input = "test";
		var output = LanguageHelpers.Translate(input, "pl");
		input.Should().Be(output);
	}
	
	[Test]
	public void CaseB()
	{
		var input = "pl:test|en:test2";
		var output = LanguageHelpers.Translate(input, "pl");
		output.Should().Be("test");
	}
	
	[Test]
	public void CaseC()
	{
		var input = "en:test2|pl:test";
		var output = LanguageHelpers.Translate(input, "");
		output.Should().Be("test");
	}
	
	[Test]
	public void CaseD()
	{
		var input = "pl:test|en:test2|it:";
		var output = LanguageHelpers.Translate(input, "");
		output.Should().Be("test");
	}
	
	[Test]
	public void CaseE()
	{
		var input = "test|en:test2|it:";
		var output = LanguageHelpers.Translate(input, "it");
		output.Should().Be("test");
	}
	
	[Test]
	public void CaseF()
	{
		var input = "pl:test|en:test2|it:testino";
		var output = LanguageHelpers.Translate(input, "en");
		output.Should().Be("test2");
	}
}
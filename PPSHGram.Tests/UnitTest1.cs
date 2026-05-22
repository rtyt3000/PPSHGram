using FluentAssertions;

namespace PPSHGram.Tests;

using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Add_ShouldReturnSum()
    {

        var result = 2 + 3;

        result.Should().Be(5);
    }
}
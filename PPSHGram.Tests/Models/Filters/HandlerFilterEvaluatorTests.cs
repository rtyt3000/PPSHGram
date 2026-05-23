using FluentAssertions;
using PPSHGram.Core.Models.Context;
using PPSHGram.Core.Models.Filters;
using PPSHGram.Telegram;
using Xunit;

namespace PPSHGram.Tests;

public class HandlerFilterEvaluatorTests
{
    [Fact]
    public void Matches_returns_true_without_filters()
    {
        HandlerFilterEvaluator.Matches(new TestContext(), [])
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Matches_combines_plain_filters_with_and()
    {
        var filters = new HandlerFilterAttribute[]
        {
            new AlwaysMatchAttribute(),
            new NeverMatchAttribute()
        };

        HandlerFilterEvaluator.Matches(new TestContext(), filters)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Matches_combines_filters_inside_same_group_with_or()
    {
        var filters = new HandlerFilterAttribute[]
        {
            new AlwaysMatchAttribute(),
            new NeverMatchAttribute { Group = "chat" },
            new AlwaysMatchAttribute { Group = "chat" }
        };

        HandlerFilterEvaluator.Matches(new TestContext(), filters)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Matches_applies_negation_to_single_filter()
    {
        var filters = new HandlerFilterAttribute[]
        {
            new NeverMatchAttribute { Negate = true }
        };

        HandlerFilterEvaluator.Matches(new TestContext(), filters)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void GetFilters_combines_class_and_method_filters()
    {
        var method = typeof(FilteredHandler).GetMethod(nameof(FilteredHandler.Handle))!;

        var filters = HandlerFilterEvaluator.GetFilters(typeof(FilteredHandler), method);

        filters.Should().HaveCount(2);
        HandlerFilterEvaluator.Matches(new TestContext(), typeof(FilteredHandler), method)
            .Should()
            .BeTrue();
    }

    private sealed class TestContext : IContext
    {
        public Api Api { get; } = new("test-token");
    }

    private sealed class AlwaysMatchAttribute : HandlerFilterAttribute
    {
        public override bool Matches(IContext context) => true;
    }

    private sealed class NeverMatchAttribute : HandlerFilterAttribute
    {
        public override bool Matches(IContext context) => false;
    }

    [AlwaysMatch]
    private sealed class FilteredHandler
    {
        [NeverMatch(Negate = true)]
        public void Handle()
        {
        }
    }
}

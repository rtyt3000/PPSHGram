using FluentAssertions;
using PPSHGram.Core.Filters;
using PPSHGram.Core.Models.Context;
using PPSHGram.Telegram;
using PPSHGram.Telegram.Generated.Types;
using System.Text.RegularExpressions;

namespace PPSHGram.Tests.Filters;

public class BuiltInFilterTests
{
    [Fact]
    public void Text_filter_matches_message_text()
    {
        var context = ContextWithMessage(text: "hello world");

        new TextAttribute("hello") { Mode = TextMatchMode.StartsWith }
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Command_filter_matches_bot_command()
    {
        var context = ContextWithMessage(text: "/start payload");

        new CommandAttribute("start")
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Callback_data_filter_matches_callback_payload()
    {
        var context = new TestContext
        {
            UpdateValue = new Update
            {
                CallbackQuery = new CallbackQuery
                {
                    Id = "1",
                    From = new User { Id = 10, FirstName = "User" },
                    ChatInstance = "chat",
                    Data = "profile:10"
                }
            }
        };

        new CallbackDataAttribute("profile:") { Mode = TextMatchMode.StartsWith }
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Inline_query_filter_matches_query_text()
    {
        var context = new TestContext
        {
            UpdateValue = new Update
            {
                InlineQuery = new InlineQuery
                {
                    Id = "1",
                    From = new User { Id = 10, FirstName = "User" },
                    Query = "search cats",
                    Offset = string.Empty
                }
            }
        };

        new InlineQueryAttribute("search", TextMatchMode.StartsWith)
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Callback_query_filter_matches_callback_data()
    {
        var context = new TestContext
        {
            UpdateValue = new Update
            {
                CallbackQuery = new CallbackQuery
                {
                    Id = "1",
                    From = new User { Id = 10, FirstName = "User" },
                    ChatInstance = "chat",
                    Data = "profile:10"
                }
            }
        };

        new CallbackQueryAttribute(new Regex(@"^profile:\d+$"))
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Guest_message_filter_matches_text_or_caption()
    {
        var context = new TestContext
        {
            UpdateValue = new Update
            {
                GuestMessage = new Message
                {
                    MessageId = 1,
                    Date = 1,
                    Caption = "photo caption",
                    Chat = new Chat
                    {
                        Id = 1,
                        Type = "private"
                    }
                }
            }
        };

        new GuestMessageAttribute("caption", TextMatchMode.Contains)
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Text_filter_can_be_initialized_with_regex()
    {
        var context = ContextWithMessage(text: "hello 123");

        new TextAttribute(new Regex(@"^hello \d+$"))
            .Matches(context)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Chat_and_user_filters_use_message_metadata()
    {
        var context = ContextWithMessage(
            chatType: "private",
            userId: 42,
            username: "tester");

        new PrivateChatAttribute().Matches(context).Should().BeTrue();
        new FromUserAttribute(42).Matches(context).Should().BeTrue();
        new FromUsernameAttribute("@tester").Matches(context).Should().BeTrue();
    }

    [Fact]
    public void Date_filter_matches_message_unix_date_range()
    {
        var context = ContextWithMessage(date: 100);

        new UpdateDateAttribute(fromUnixTime: 50, toUnixTime: 150)
            .Matches(context)
            .Should()
            .BeTrue();
    }

    private static TestContext ContextWithMessage(
        string? text = null,
        string chatType = "private",
        long userId = 1,
        string? username = null,
        long date = 1)
    {
        return new TestContext
        {
            UpdateValue = new Update
            {
                Message = new Message
                {
                    MessageId = 1,
                    Date = date,
                    Text = text,
                    Chat = new Chat
                    {
                        Id = 1,
                        Type = chatType
                    },
                    From = new User
                    {
                        Id = userId,
                        FirstName = "User",
                        Username = username
                    }
                }
            }
        };
    }

    private sealed class TestContext : IContext
    {
        public Api Api { get; } = new("test-token");

        public Update Update => UpdateValue;

        public Update UpdateValue { get; init; } = new();
    }
}

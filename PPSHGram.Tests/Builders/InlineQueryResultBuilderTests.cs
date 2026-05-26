using System.Text.Json;
using FluentAssertions;
using PPSHGram.Core.Builders;
using PPSHGram.Telegram.Generated.Requests;
using PPSHGram.Telegram.Generated.Types;
using PPSHGram.Telegram.Json;

namespace PPSHGram.Tests.Builders;

public class InlineQueryResultBuilderTests
{
    [Fact]
    public void Article_builds_inline_query_result_with_text_content()
    {
        InlineQueryResult result = InlineQueryResultBuilder
            .Article("article-id", "Article title", description: "Short description")
            .Text("Hello", parseMode: "Markdown");

        result.Should().BeOfType<InlineQueryResultArticle>()
            .Which.Should().BeEquivalentTo(new InlineQueryResultArticle
            {
                Type = "article",
                Id = "article-id",
                Title = "Article title",
                Description = "Short description",
                InputMessageContent = new InputTextMessageContent
                {
                    MessageText = "Hello",
                    ParseMode = "Markdown"
                }
            });
    }

    [Fact]
    public void Build_keeps_preview_content()
    {
        InlineQueryResult result = InlineQueryResultBuilder
            .Photo("photo-id", "/images/Y.png")
            .Build();

        var photo = result.Should().BeOfType<InlineQueryResultPhoto>().Subject;
        photo.PhotoUrl.Should().Be("/images/Y.png");
        photo.ThumbnailUrl.Should().Be("/images/Y.png");
        photo.InputMessageContent.Should().BeNull();
    }

    [Fact]
    public void Result_preview_can_be_replaced_with_text_content()
    {
        InlineQueryResult result = InlineQueryResultBuilder
            .Photo("photo-id", "/images/Y.png")
            .Text("Picked photo 2!");

        var photo = result.Should().BeOfType<InlineQueryResultPhoto>().Subject;
        photo.PhotoUrl.Should().Be("/images/Y.png");
        photo.ThumbnailUrl.Should().Be("/images/Y.png");
        photo.InputMessageContent.Should().BeOfType<InputTextMessageContent>()
            .Which.MessageText.Should().Be("Picked photo 2!");
    }

    [Fact]
    public void Result_preview_can_be_replaced_with_location_content()
    {
        InlineQueryResult result = InlineQueryResultBuilder
            .Photo("photo-id", "/images/Y.png")
            .Location(10.5, 20.5, livePeriod: 60);

        result.Should().BeOfType<InlineQueryResultPhoto>()
            .Which.InputMessageContent.Should().BeOfType<InputLocationMessageContent>()
            .Which.LivePeriod.Should().Be(60);
    }

    [Fact]
    public void Interface_typed_results_serialize_with_derived_properties()
    {
        var request = new AnswerInlineQueryRequest
        {
            InlineQueryId = "query-id",
            Results =
            [
                InlineQueryResultBuilder.Article("article-id", "Article title").Text("Hello")
            ]
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(request, TelegramJsonSerializerOptions.Default));
        var result = document.RootElement.GetProperty("results")[0];

        result.GetProperty("type").GetString().Should().Be("article");
        result.GetProperty("id").GetString().Should().Be("article-id");
        result.GetProperty("input_message_content").GetProperty("message_text").GetString().Should().Be("Hello");
    }
}

using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core.Builders;

public static class InlineQueryResultBuilder
{
    public static InlineQueryResultContentBuilder<InlineQueryResultArticle> Article(
        string id,
        string title,
        InlineKeyboardMarkup? replyMarkup = null,
        string? url = null,
        string? description = null,
        string? thumbnailUrl = null,
        long? thumbnailWidth = null,
        long? thumbnailHeight = null) =>
        new(inputMessageContent => new InlineQueryResultArticle
        {
            Id = id,
            Title = title,
            InputMessageContent = inputMessageContent ?? CreateTextContent(title),
            ReplyMarkup = replyMarkup,
            Url = url,
            Description = description,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailWidth = thumbnailWidth,
            ThumbnailHeight = thumbnailHeight
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultPhoto> Photo(
        string id,
        string photoUrl,
        string? thumbnailUrl = null,
        long? photoWidth = null,
        long? photoHeight = null,
        string? title = null,
        string? description = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultPhoto
        {
            Id = id,
            PhotoUrl = photoUrl,
            ThumbnailUrl = thumbnailUrl ?? photoUrl,
            PhotoWidth = photoWidth,
            PhotoHeight = photoHeight,
            Title = title,
            Description = description,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultGif> Gif(
        string id,
        string gifUrl,
        string thumbnailUrl,
        long? gifWidth = null,
        long? gifHeight = null,
        long? gifDuration = null,
        string? thumbnailMimeType = null,
        string? title = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultGif
        {
            Id = id,
            GifUrl = gifUrl,
            GifWidth = gifWidth,
            GifHeight = gifHeight,
            GifDuration = gifDuration,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailMimeType = thumbnailMimeType,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultMpeg4Gif> Mpeg4Gif(
        string id,
        string mpeg4Url,
        string thumbnailUrl,
        long? mpeg4Width = null,
        long? mpeg4Height = null,
        long? mpeg4Duration = null,
        string? thumbnailMimeType = null,
        string? title = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultMpeg4Gif
        {
            Id = id,
            Mpeg4Url = mpeg4Url,
            Mpeg4Width = mpeg4Width,
            Mpeg4Height = mpeg4Height,
            Mpeg4Duration = mpeg4Duration,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailMimeType = thumbnailMimeType,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultVideo> Video(
        string id,
        string videoUrl,
        string mimeType,
        string thumbnailUrl,
        string title,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        long? videoWidth = null,
        long? videoHeight = null,
        long? videoDuration = null,
        string? description = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultVideo
        {
            Id = id,
            VideoUrl = videoUrl,
            MimeType = mimeType,
            ThumbnailUrl = thumbnailUrl,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            VideoWidth = videoWidth,
            VideoHeight = videoHeight,
            VideoDuration = videoDuration,
            Description = description,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultAudio> Audio(
        string id,
        string audioUrl,
        string title,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        string? performer = null,
        long? audioDuration = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultAudio
        {
            Id = id,
            AudioUrl = audioUrl,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            Performer = performer,
            AudioDuration = audioDuration,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultVoice> Voice(
        string id,
        string voiceUrl,
        string title,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        long? voiceDuration = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultVoice
        {
            Id = id,
            VoiceUrl = voiceUrl,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            VoiceDuration = voiceDuration,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultDocument> Document(
        string id,
        string title,
        string documentUrl,
        string mimeType,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        string? description = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? thumbnailUrl = null,
        long? thumbnailWidth = null,
        long? thumbnailHeight = null) =>
        new(inputMessageContent => new InlineQueryResultDocument
        {
            Id = id,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            DocumentUrl = documentUrl,
            MimeType = mimeType,
            Description = description,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailWidth = thumbnailWidth,
            ThumbnailHeight = thumbnailHeight
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultLocation> Location(
        string id,
        double latitude,
        double longitude,
        string title,
        double? horizontalAccuracy = null,
        long? livePeriod = null,
        long? heading = null,
        long? proximityAlertRadius = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? thumbnailUrl = null,
        long? thumbnailWidth = null,
        long? thumbnailHeight = null) =>
        new(inputMessageContent => new InlineQueryResultLocation
        {
            Id = id,
            Latitude = latitude,
            Longitude = longitude,
            Title = title,
            HorizontalAccuracy = horizontalAccuracy,
            LivePeriod = livePeriod,
            Heading = heading,
            ProximityAlertRadius = proximityAlertRadius,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailWidth = thumbnailWidth,
            ThumbnailHeight = thumbnailHeight
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultVenue> Venue(
        string id,
        double latitude,
        double longitude,
        string title,
        string address,
        string? foursquareId = null,
        string? foursquareType = null,
        string? googlePlaceId = null,
        string? googlePlaceType = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? thumbnailUrl = null,
        long? thumbnailWidth = null,
        long? thumbnailHeight = null) =>
        new(inputMessageContent => new InlineQueryResultVenue
        {
            Id = id,
            Latitude = latitude,
            Longitude = longitude,
            Title = title,
            Address = address,
            FoursquareId = foursquareId,
            FoursquareType = foursquareType,
            GooglePlaceId = googlePlaceId,
            GooglePlaceType = googlePlaceType,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailWidth = thumbnailWidth,
            ThumbnailHeight = thumbnailHeight
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultContact> Contact(
        string id,
        string phoneNumber,
        string firstName,
        string? lastName = null,
        string? vcard = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? thumbnailUrl = null,
        long? thumbnailWidth = null,
        long? thumbnailHeight = null) =>
        new(inputMessageContent => new InlineQueryResultContact
        {
            Id = id,
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName,
            Vcard = vcard,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailWidth = thumbnailWidth,
            ThumbnailHeight = thumbnailHeight
        });

    public static InlineQueryResultBuildBuilder<InlineQueryResultGame> Game(
        string id,
        string gameShortName,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(() => new InlineQueryResultGame
        {
            Id = id,
            GameShortName = gameShortName,
            ReplyMarkup = replyMarkup
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedPhoto> CachedPhoto(
        string id,
        string photoFileId,
        string? title = null,
        string? description = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedPhoto
        {
            Id = id,
            PhotoFileId = photoFileId,
            Title = title,
            Description = description,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedGif> CachedGif(
        string id,
        string gifFileId,
        string? title = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedGif
        {
            Id = id,
            GifFileId = gifFileId,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedMpeg4Gif> CachedMpeg4Gif(
        string id,
        string mpeg4FileId,
        string? title = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedMpeg4Gif
        {
            Id = id,
            Mpeg4FileId = mpeg4FileId,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedSticker> CachedSticker(
        string id,
        string stickerFileId,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedSticker
        {
            Id = id,
            StickerFileId = stickerFileId,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedDocument> CachedDocument(
        string id,
        string title,
        string documentFileId,
        string? description = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedDocument
        {
            Id = id,
            Title = title,
            DocumentFileId = documentFileId,
            Description = description,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedVideo> CachedVideo(
        string id,
        string videoFileId,
        string title,
        string? description = null,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        bool? showCaptionAboveMedia = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedVideo
        {
            Id = id,
            VideoFileId = videoFileId,
            Title = title,
            Description = description,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedVoice> CachedVoice(
        string id,
        string voiceFileId,
        string title,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedVoice
        {
            Id = id,
            VoiceFileId = voiceFileId,
            Title = title,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    public static InlineQueryResultContentBuilder<InlineQueryResultCachedAudio> CachedAudio(
        string id,
        string audioFileId,
        string? caption = null,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? captionEntities = null,
        InlineKeyboardMarkup? replyMarkup = null) =>
        new(inputMessageContent => new InlineQueryResultCachedAudio
        {
            Id = id,
            AudioFileId = audioFileId,
            Caption = caption,
            ParseMode = parseMode,
            CaptionEntities = captionEntities,
            ReplyMarkup = replyMarkup,
            InputMessageContent = inputMessageContent
        });

    internal static InputTextMessageContent CreateTextContent(
        string messageText,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null) =>
        new()
        {
            MessageText = messageText,
            ParseMode = parseMode,
            Entities = entities,
            LinkPreviewOptions = linkPreviewOptions
        };

    internal static InputLocationMessageContent CreateLocationContent(
        double latitude,
        double longitude,
        double? horizontalAccuracy = null,
        long? livePeriod = null,
        long? heading = null,
        long? proximityAlertRadius = null) =>
        new()
        {
            Latitude = latitude,
            Longitude = longitude,
            HorizontalAccuracy = horizontalAccuracy,
            LivePeriod = livePeriod,
            Heading = heading,
            ProximityAlertRadius = proximityAlertRadius
        };

    internal static InputVenueMessageContent CreateVenueContent(
        double latitude,
        double longitude,
        string title,
        string address,
        string? foursquareId = null,
        string? foursquareType = null,
        string? googlePlaceId = null,
        string? googlePlaceType = null) =>
        new()
        {
            Latitude = latitude,
            Longitude = longitude,
            Title = title,
            Address = address,
            FoursquareId = foursquareId,
            FoursquareType = foursquareType,
            GooglePlaceId = googlePlaceId,
            GooglePlaceType = googlePlaceType
        };

    internal static InputContactMessageContent CreateContactContent(
        string phoneNumber,
        string firstName,
        string? lastName = null,
        string? vcard = null) =>
        new()
        {
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName,
            Vcard = vcard
        };

    internal static InputInvoiceMessageContent CreateInvoiceContent(
        string title,
        string description,
        string payload,
        string currency,
        IReadOnlyList<LabeledPrice> prices,
        string? providerToken = null,
        long? maxTipAmount = null,
        IReadOnlyList<long>? suggestedTipAmounts = null,
        string? providerData = null,
        string? photoUrl = null,
        long? photoSize = null,
        long? photoWidth = null,
        long? photoHeight = null,
        bool? needName = null,
        bool? needPhoneNumber = null,
        bool? needEmail = null,
        bool? needShippingAddress = null,
        bool? sendPhoneNumberToProvider = null,
        bool? sendEmailToProvider = null,
        bool? isFlexible = null) =>
        new()
        {
            Title = title,
            Description = description,
            Payload = payload,
            ProviderToken = providerToken,
            Currency = currency,
            Prices = prices,
            MaxTipAmount = maxTipAmount,
            SuggestedTipAmounts = suggestedTipAmounts,
            ProviderData = providerData,
            PhotoUrl = photoUrl,
            PhotoSize = photoSize,
            PhotoWidth = photoWidth,
            PhotoHeight = photoHeight,
            NeedName = needName,
            NeedPhoneNumber = needPhoneNumber,
            NeedEmail = needEmail,
            NeedShippingAddress = needShippingAddress,
            SendPhoneNumberToProvider = sendPhoneNumberToProvider,
            SendEmailToProvider = sendEmailToProvider,
            IsFlexible = isFlexible
        };
}

public class InlineQueryResultBuildBuilder<TResult>(Func<TResult> build)
    where TResult : InlineQueryResult
{
    public TResult Build() => build();
}

public sealed class InlineQueryResultContentBuilder<TResult>(Func<InputMessageContent?, TResult> build)
    : InlineQueryResultBuildBuilder<TResult>(() => build(null))
    where TResult : InlineQueryResult
{
    public TResult Text(
        string messageText,
        string? parseMode = null,
        IReadOnlyList<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null) =>
        build(InlineQueryResultBuilder.CreateTextContent(messageText, parseMode, entities, linkPreviewOptions));

    public TResult Location(
        double latitude,
        double longitude,
        double? horizontalAccuracy = null,
        long? livePeriod = null,
        long? heading = null,
        long? proximityAlertRadius = null) =>
        build(InlineQueryResultBuilder.CreateLocationContent(
            latitude,
            longitude,
            horizontalAccuracy,
            livePeriod,
            heading,
            proximityAlertRadius));

    public TResult Venue(
        double latitude,
        double longitude,
        string title,
        string address,
        string? foursquareId = null,
        string? foursquareType = null,
        string? googlePlaceId = null,
        string? googlePlaceType = null) =>
        build(InlineQueryResultBuilder.CreateVenueContent(
            latitude,
            longitude,
            title,
            address,
            foursquareId,
            foursquareType,
            googlePlaceId,
            googlePlaceType));

    public TResult Contact(
        string phoneNumber,
        string firstName,
        string? lastName = null,
        string? vcard = null) =>
        build(InlineQueryResultBuilder.CreateContactContent(phoneNumber, firstName, lastName, vcard));

    public TResult Invoice(
        string title,
        string description,
        string payload,
        string currency,
        IReadOnlyList<LabeledPrice> prices,
        string? providerToken = null,
        long? maxTipAmount = null,
        IReadOnlyList<long>? suggestedTipAmounts = null,
        string? providerData = null,
        string? photoUrl = null,
        long? photoSize = null,
        long? photoWidth = null,
        long? photoHeight = null,
        bool? needName = null,
        bool? needPhoneNumber = null,
        bool? needEmail = null,
        bool? needShippingAddress = null,
        bool? sendPhoneNumberToProvider = null,
        bool? sendEmailToProvider = null,
        bool? isFlexible = null) =>
        build(InlineQueryResultBuilder.CreateInvoiceContent(
            title,
            description,
            payload,
            currency,
            prices,
            providerToken,
            maxTipAmount,
            suggestedTipAmounts,
            providerData,
            photoUrl,
            photoSize,
            photoWidth,
            photoHeight,
            needName,
            needPhoneNumber,
            needEmail,
            needShippingAddress,
            sendPhoneNumberToProvider,
            sendEmailToProvider,
            isFlexible));
}

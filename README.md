# PPSHGram
.NET TelegramBotAPI Framework (currenly in development)

## Integration checks

Set `PPSHGRAM_BOT_TOKEN` to run the optional Telegram Bot API check against the default base address:

```powershell
$env:PPSHGRAM_BOT_TOKEN = "<bot token from BotFather>"
dotnet test PPSHGram.Tests\PPSHGram.Tests.csproj --filter Default_base_address_can_call_get_updates_when_token_env_is_set
```

Do not commit real bot tokens. If a token was ever hardcoded, rotate it in BotFather.

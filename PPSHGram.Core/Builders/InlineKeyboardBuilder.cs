using PPSHGram.Telegram.Generated.Types;

namespace PPSHGram.Core.Builders;

public class InlineKeyboardBuilder
{
    private InlineKeyboardButton[][] _buttons = [[]];

    public InlineKeyboardBuilder(InlineKeyboardButton[][]? buttons)
    {
        if (buttons != null) _buttons = buttons;
    }
    
    
    
    public InlineKeyboardMarkup Build() => new InlineKeyboardMarkup { InlineKeyboard = _buttons };
}
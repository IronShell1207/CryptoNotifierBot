using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CryptoApi.Objects;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBot.Constants.Commands;
using TelegramBot.Helpers;
using TelegramBot.Objects;
using TelegramBot.Static;
using TelegramBot.Static.MessageHandlers;

namespace TelegramBot.Services.MessageHandlers
{
    public sealed class RepliedMessagesHandler : TheDisposable
    {
        public async Task HandleMessage(Update update, UserConfig user = null)
        {
            if (user == null) user = await BotApi.GetUserSettings(update);
            var editPriceMsgRegex =
                RegexHelper.ConvertMessageToRegex(CultureTextRequest.GetMessageString("CPEditPair", user.Language),
                    new List<string>()
                        {@"(?<base>[a-zA-Z0-9]{2,9})(\s+|/)(?<quote>[a-zA-Z0-9]{2,6})", "(?<id>[0-9]+)"});

            if (TelegramUpdatesHelper.StripHTML(update.Message?.ReplyToMessage?.Text)
                == TelegramUpdatesHelper.StripHTML(Messages.newPairRequestingForPair))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    await msgh.SetPairSymbolStage(update, user);
            else if (update?.Message.Text == SimpleCommands.EnableCleaning)
                using (UserSettingsMsgHandler msgHandler = new())
                    await msgHandler.TurnLastMsgCleaning(update);
            else if (TelegramUpdatesHelper.StripHTML(update.Message.ReplyToMessage.Text)
                     == TelegramUpdatesHelper.StripHTML(Messages.newPairWrongPrice))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    await msgh.SetPriceStage(update, user);
            else if (TelegramUpdatesHelper.StripHTML(update.Message.ReplyToMessage.Text)
                     == TelegramUpdatesHelper.StripHTML(Messages.newPairAfterExchangeSetPrice))
                using (CryptoPairsMsgHandler msgh = new CryptoPairsMsgHandler())
                    await msgh.SetPriceStage(update, user);
            else if (TelegramUpdatesHelper.StripHTML(update.Message.ReplyToMessage.Text) ==
                     CultureTextRequest.GetMessageString("ToaddToTheBlackList", user.Language))
                using (BreakoutPairsMsgHandler msgHandler = new BreakoutPairsMsgHandler())
                    await msgHandler.AddPairToBlackListCommandHandler(update);
            else if (editPriceMsgRegex.IsMatch(update.Message.ReplyToMessage.Text))
                using (CryptoPairsMsgHandler msgHandler = new CryptoPairsMsgHandler())
                    await msgHandler.EditUserTaskReplyHandler(update, user);
        }
    }
}
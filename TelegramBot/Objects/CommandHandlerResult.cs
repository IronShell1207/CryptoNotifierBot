using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Objects
{
    public class CommandHandlerResult
    {
        public string Message { get; set; }
        public bool Result
        {
            get; set;
        }
        public CommandHandlerResult()
        {

        }
        public CommandHandlerResult(string message, bool result)
        {
            Message = message;
            Result = result;
        }
    }
}

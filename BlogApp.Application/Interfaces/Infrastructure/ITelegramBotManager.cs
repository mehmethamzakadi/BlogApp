using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Interfaces.Infrastructure
{
    public interface ITelegramBotManager
    {
        Task SendTextMessage(string message, long chatId);
    }
}

using BlazorSignalRChat.Server.Models;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace BlazorSignalRChat.Shared.Models
{
    public class ChatMessage
    {
        public long Id { get; set; } // так как сообщений может быть 10000000500000000 поэтому - long. + Microsoft так рекомендует
        public string FromUserId { get; set; } // от кого
        public string ToUserId { get; set; } // кому
        public string Message { get; set; } // само сообщение
        public DateTime CreatedDate { get; set; }

        // навигационные ссылки
        public virtual ApplicationUser FromUser { get; set; }
        public virtual ApplicationUser ToUser { get; set; }
    }
}

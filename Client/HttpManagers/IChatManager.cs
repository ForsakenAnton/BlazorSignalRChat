using BlazorSignalRChat.Server.Models;
using BlazorSignalRChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSignalRChat.Client.HttpManagers
{
    public interface IChatManager
    {
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser> GetUserDetailsAsync(string userId);
        Task SaveMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetConversationAsync(string contactId);
    }
}

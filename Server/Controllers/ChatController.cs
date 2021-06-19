using BlazorSignalRChat.Server.Data;
using BlazorSignalRChat.Server.Models;
using BlazorSignalRChat.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorSignalRChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ! атрибут Authorize !
    public class ChatController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ChatController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // Возвращаем всех пользователей, кроме себя
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            // находим текущего пользователя (себя)
            var userId = User.Claims
                .Where(a => a.Type == ClaimTypes.NameIdentifier)
                .Select(a => a.Value)
                .FirstOrDefault();

            // возвращаем список пользователей без текущего пользователя (то есть личный список пользователей чата)
            var allUsers = await _context.Users
                .Where(user => user.Id != userId)
                .ToListAsync();

            return Ok(allUsers);
        }


        // Возвращаем одного пользователя по Id
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserDetailsAsync(string userId)
        {
            var user = await _context.Users
                .Where(user => user.Id == userId)
                .FirstOrDefaultAsync();

            return Ok(user);
        }


        // Сохраняем сообщение
        [HttpPost]
        public async Task<IActionResult> SaveMessageAsync(ChatMessage message)
        {
            // находим себя
            var userId = User.Claims
                .Where(a => a.Type == ClaimTypes.NameIdentifier)
                .Select(a => a.Value)
                .FirstOrDefault();

            message.FromUserId = userId; // от себя
            message.CreatedDate = DateTime.Now;
            message.ToUser = await _context.Users // кому
                .Where(user => user.Id == message.ToUserId)
                .FirstOrDefaultAsync();

            await _context.ChatMessages.AddAsync(message);
            return Ok(await _context.SaveChangesAsync());
        }


        // Возвращаем список сообщений чата
        [HttpGet("{contactId}")]
        public async Task<IActionResult> GetConversationAsync(string contactId)
        {
            // находим себя
            var userId = User.Claims
                .Where(a => a.Type == ClaimTypes.NameIdentifier)
                .Select(a => a.Value)
                .FirstOrDefault();

            var messages = await _context.ChatMessages
                    .Where(h => (h.FromUserId == contactId && h.ToUserId == userId) // то есть отправитель
                                || (h.FromUserId == userId && h.ToUserId == contactId)) // может быть и получателем
                    .OrderBy(a => a.CreatedDate)
                    .Include(a => a.FromUser)
                    .Include(a => a.ToUser)
                    .Select(x => new ChatMessage
                    {
                        Id = x.Id,
                        FromUserId = x.FromUserId,
                        ToUserId = x.ToUserId,
                        Message = x.Message,
                        CreatedDate = x.CreatedDate,
                        FromUser = x.FromUser,
                        ToUser = x.ToUser
                    }).ToListAsync();

            return Ok(messages);
        }
    }
}

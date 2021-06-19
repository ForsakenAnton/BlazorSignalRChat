using BlazorSignalRChat.Server.Models;
using BlazorSignalRChat.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSignalRChat.Client.Pages
{
    public partial class Chat
    {
        [CascadingParameter] // параметр из MainLayout! Мы его получаем именно от туда
        public HubConnection hubConnection { get; set; }

        [Parameter] // текущее сообщение
        public string CurrentMessage { get; set; }

        [Parameter] // текущий Id юзера
        public string CurrentUserId { get; set; }

        [Parameter] // текущий Email юзера
        public string CurrentUserEmail { get; set; }

        [Parameter] // Id получателя
        public string ContactId { get; set; }

        [Parameter] // Email получателя
        public string ContactEmail { get; set; }

        // ниже списки всех юзеров и сообщений
        public List<ApplicationUser> ChatUsers = new List<ApplicationUser>();
        private List<ChatMessage> messages = new List<ChatMessage>();
        //ILogger logger = new LoggerFactory().CreateLogger<Chat>();


        // обработчик события отправки меседжа получателю
        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ContactId))
            {
                var chatMessage = new ChatMessage()
                {
                    Message = CurrentMessage,
                    ToUserId = ContactId,
                    CreatedDate = DateTime.Now
                };
                await _chatManager.SaveMessageAsync(chatMessage); // наш HttpManager
                chatMessage.FromUserId = CurrentUserId;
                await hubConnection.SendAsync("SendMessageAsync", chatMessage, CurrentUserEmail);
                CurrentMessage = string.Empty;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/signalRHub")).Build();
            }
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            // сработает после метода SubmitAsync(), который в конце вызовет метод концентратора (SignalRHub), а последний вызовет Clients.All.SendAsync("ReceiveMessage", message, userName);
            hubConnection.On<ChatMessage, string>("ReceiveMessage", async (message, userName) =>
            {
                // здесь смысл в том, что мы можем быть как отправителем, так и получателем
                if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId) || (ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                {
                    // если мы отправитель
                    if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId))
                    {
                        messages.Add(new ChatMessage 
                        { 
                            Message = message.Message,
                            CreatedDate = message.CreatedDate,
                            FromUser = new ApplicationUser() 
                            {
                                Email = CurrentUserEmail 
                            } 
                        });
                        // вызов концентратора, в котором сработает метод Clients.All.SendAsync("ReceiveChatNotification", message, receiverUserId, senderUserId),
                        // где последний вызовет hubConnection.On<string, string, string>("ReceiveChatNotification", (message, receiverUserId, senderUserId)
                        // в MainLayout
                        await hubConnection.SendAsync("ChatNotificationAsync", $"New Message From {userName}", ContactId, CurrentUserId);
                    }
                    else if ((ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                    {
                        messages.Add(new ChatMessage 
                        { 
                            Message = message.Message, 
                            CreatedDate = message.CreatedDate,
                            FromUser = new ApplicationUser() 
                            { 
                                Email = ContactEmail 
                            } 
                        });
                    }

                    await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
                    StateHasChanged(); // документация говорит, что так сработает повторный рендеринг компонента
                }
            });

            await GetUsersAsync();

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            CurrentUserId = user.Claims.Where(a => a.Type == "sub").Select(a => a.Value).FirstOrDefault();
            CurrentUserEmail = user.Claims.Where(a => a.Type == "name").Select(a => a.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(ContactId))
            {
                await LoadUserChat(ContactId);
            }
        }

        
        async Task LoadUserChat(string userId)
        {
            var contact = await _chatManager.GetUserDetailsAsync(userId);
            ContactId = contact.Id;
            ContactEmail = contact.Email;
            _navigationManager.NavigateTo($"chat/{ContactId}");
            messages = new List<ChatMessage>();
            messages = await _chatManager.GetConversationAsync(ContactId);
        }

        private async Task GetUsersAsync()
        {
            ChatUsers = await _chatManager.GetUsersAsync();
        }


        protected override void OnInitialized()
        {
             _jsRuntime.InvokeAsync<string>("OnInitialized");
        }


        // для javascript
        // прокручиваем страницу до самого последнего сообщения вниз после рендеринга
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //logger.LogInformation("=== OnAfterRenderAsync ===");
            //if(firstRender)
            await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
        }


        private async Task OnScroll()
        {
            await _jsRuntime.InvokeAsync<string>("Scroll", "textAreaContainer");
        }
    }
}

using BlazorSignalRChat.Server.Models;
using BlazorSignalRChat.Shared.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorSignalRChat.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(d => d.FromUser) // у сущности ChatMessage есть нав. свойство ApplicationUser FromUser,
                    .WithMany(p => p.ChatMessagesFromUsers) // ...у которого есть коллекция ICollection<ChatMessage>,
                    .HasForeignKey(d => d.FromUserId) // так же ChatMessage имеет внешний ключ (который ссылается на Id юзера),
                    .OnDelete(DeleteBehavior.ClientSetNull); // установить null FromUser, если юзер удалился из чата (еще помним что типы данных внешних ключей у меня - string) 

                // делаем то же, только для ToUser
                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.ChatMessagesToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
            // В принципе все операции в OnModelCreating() ради последней строчки . OnDelete(...)
        }

    }
}

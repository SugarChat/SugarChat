using Autofac;
using SugarChat.Core.IRepositories;
using SugarChat.Infrastructure.Contexts;
using SugarChat.Infrastructure.Repositories;

namespace SugarChat.WebApi
{
    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SugarChatDbContext>()
                   .SingleInstance();

            builder.RegisterType<UserRepository>()
                   .As<IUserRepository>()
                   .SingleInstance();

            builder.RegisterType<MessageRepository>()
                 .As<IMessageRepository>()
                 .SingleInstance();
        }
    }
}

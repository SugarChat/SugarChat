using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using SugarChat.Core.Cache;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Middlewares
{
    public class UserExistSpecification<TContext> : IPipeSpecification<TContext>
        where TContext : IContext<IMessage>
    {
        private readonly IUserDataProvider _userDataProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly RunTimeProvider _runtimeProvider;

        public UserExistSpecification(IUserDataProvider userDataProvider, IMemoryCache memoryCache, RunTimeProvider runtimeProvider)
        {
            _memoryCache = memoryCache;
            _userDataProvider = userDataProvider;
            _runtimeProvider = runtimeProvider;
        }

        public Task AfterExecute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public async Task BeforeExecute(TContext context, CancellationToken cancellationToken)
        {
            try
            {
                if (_runtimeProvider.Type == Message.RunTimeType.AspNetCoreApi)
                {
                    if (context.Message is INeedUserExist needUserExist)
                    {
                        var users = _memoryCache.Get<List<User>>(CacheService.AllUser);
                        User user;
                        if (users is null)
                        {
                            user = await _userDataProvider.GetByIdAsync(needUserExist.UserId, cancellationToken);
                            if (user is null)
                            {
                                user = await UserExistForDb(needUserExist.UserId, cancellationToken);
                            }
                            _memoryCache.Set(CacheService.AllUser, new List<User> { user });
                        }
                        else
                        {
                            user = users.FirstOrDefault(x => x.Id == needUserExist.UserId);
                            if (user is null)
                            {
                                user = await UserExistForDb(needUserExist.UserId, cancellationToken);
                            }
                            users.Add(user);
                            _memoryCache.Set(CacheService.AllUser, users);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "UserExistSpecification Error{@command}", context.Message);
            }
        }

        private async Task<User> UserExistForDb(string userId, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                user = new User { Id = userId, CreatedBy = userId };
                await _userDataProvider.AddAsync(user);
            }
            return user;
        }

        public Task Execute(TContext context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task OnException(Exception ex, TContext context)
        {
            throw ex;
        }

        public bool ShouldExecute(TContext context, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}

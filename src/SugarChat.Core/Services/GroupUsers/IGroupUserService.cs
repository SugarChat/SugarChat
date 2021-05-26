using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.GroupMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserService
    {
        Task AddGroupMember(AddGroupMemberCommand command, CancellationToken cancellationToken);
    }

    public class GroupMemberService : IGroupUserService
    {
        private readonly IRepository _repository;

        public GroupMemberService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddGroupMember(AddGroupMemberCommand command, CancellationToken cancellationToken)
        {
            if (!await _repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.GroupOwnerId&&x.IsAdmin).ConfigureAwait(false))
            {
                throw new System.Exception("current user is't administrator");
            }
            if (await _repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.MemberId))
            {
                throw new System.Exception("user was group member");
            }
            await _repository.AddAsync(new GroupUser
            {
                UserId = command.MemberId,
                GroupId = command.GroupId
            });
        }
    }
}

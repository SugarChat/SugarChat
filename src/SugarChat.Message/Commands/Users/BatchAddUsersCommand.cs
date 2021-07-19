using Mediator.Net.Contracts;
using SugarChat.Message.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Commands.Users
{
    public class BatchAddUsersCommand : ICommand
    {
        public IEnumerable<UserDto> Users { get; set; }
    }
}

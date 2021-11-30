using Mediator.Net.Contracts;
using SugarChat.Message.Commands;
using SugarChat.Message.Paging;
using System;

namespace SugarChat.Message.Requests
{
    public class GetGroupsOfUserRequest : IRequest, INeedUserExist
    {
        private string _Id;
        [Obsolete("use UserId")]
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = value;
                }
                _Id = value;
            }
        }
        public PageSettings PageSettings { get; set; }
        public string UserId { get; set; }
    }
}
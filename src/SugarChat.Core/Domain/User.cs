using System;
using System.Collections;
using System.Collections.Generic;
using SugarChat.Core.Common;
using SugarChat.Core.Exceptions;

namespace SugarChat.Core.Domain
{
    public class User : Receiver
    {
        public string PublicId { get; private set; }
        public string NickName { get; private set; }
        public string SelfDescription { get; private set; }
        public UserStatus Status { get; private set; }
        public string Forename { get; private set; }
        public string Surname { get; private set; }
        public virtual string FullName => $"{Forename}.{Surname}";
        public DateTime Birthday { get; private set; }
        public virtual ICollection<Guid> FriendIds { get; private set; }
        public virtual ICollection<Guid> GroupIds { get; private set; }

        public User(
            Guid id,
            string avatarUrl,
            RegisterInfo registerInfo,
            string publicId,
            string nickName,
            string selfDescription,
            UserStatus status,
            string forename,
            string surname,
            DateTime birthday)
        {
            Id = CheckId(id);
            AvatarUrl = CheckAvatarUrl(avatarUrl);
            RegisterInfo = CheckRegisterInfo(registerInfo);
            PublicId = CheckPublicId(publicId);
            NickName = CheckNickName(nickName);
            SelfDescription = CheckSelfDescription(selfDescription);
            Status = CheckStatus(status);
            Forename = CheckForename(forename);
            Surname = CheckSurname(surname);
            Birthday = CheckBirthday(birthday);
        }

        private DateTime CheckBirthday(DateTime birthday)
        {
            if (birthday > DateTime.Today || birthday < DateTime.Today.AddYears(-150))
            {
                throw new BusinessException("UserBirthdayError");
            }

            return birthday;
        }

        private string CheckSurname(string surname)
        {
            if (string.IsNullOrWhiteSpace(surname))
            {
                throw new BusinessException("UserSurnameError");
            }

            return surname;
        }

        private string CheckForename(string forename)
        {
            if (string.IsNullOrWhiteSpace(forename))
            {
                throw new BusinessException("UserForenameError");
            }

            return forename;
        }

        private UserStatus CheckStatus(UserStatus status)
        {
            return status;
        }

        private string CheckSelfDescription(string selfDescription)
        {
            return selfDescription;
        }

        private string CheckNickName(string nickName)
        {
            return nickName;
        }

        private string CheckPublicId(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                throw new BusinessException("UserPublicIdError");
            }

            return publicId;
        }

        private RegisterInfo CheckRegisterInfo(RegisterInfo registerInfo)
        {
            if (registerInfo is null)
            {
                throw new BusinessException("UserRegisterInfoError");
            }

            return registerInfo;
        }

        private string CheckAvatarUrl(string avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl))
            {
                throw new BusinessException("UserAvatarUrlError");
            }

            return avatarUrl;
        }

        private Guid CheckId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BusinessException("UserIdError");
            }

            return id;
        }
    }
}
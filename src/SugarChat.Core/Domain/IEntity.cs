﻿using System;

namespace SugarChat.Core.Domain
{
    public interface IEntity
    {
        string Id { get; set; }
        DateTimeOffset LastModifyDate { get; set; }
    }
};
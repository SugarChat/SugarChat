﻿using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SugarChat.IntegrationTest
{
    public class TrueTest
    {
        [Fact]
        public Task Should_Always_Be_Ture()
        {
            var factor = 1;
            factor.ShouldBe(1);
            return Task.CompletedTask;
        }
    }
}
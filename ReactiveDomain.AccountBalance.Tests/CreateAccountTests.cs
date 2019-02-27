using ReactiveDomain.Messaging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ReactiveDomain.AccountBalance.Commands;
using ReactiveDomain.AccountBalance.Events;
using Xunit;

namespace ReactiveDomain.AccountBalance.Tests
{
    [Collection("AggregateTest")]
    public class CreateAccountTests : AccountTestsBase
    {
        [Fact]
        public void CanCreateAccount()
        {
            var cmd = new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            };
            Assert.IsType<Success>(CmdHandler.Handle(cmd));
        }

        [Fact]
        public void CannotCreateAccountWithNullName()
        {
            var cmd = new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = null
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("HolderName can't be empty", msg.Exception.Message);
        }

        [Fact]
        public void CannotCreateAccountWithEmptyName()
        {
            var cmd = new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = string.Empty
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("HolderName can't be empty", msg.Exception.Message);
        }

        [Fact]
        public void CannotCreateAccountWithWhiteSpaceName()
        {
            var cmd = new CreateAccountCommand()
            {
                AccountId = AccountId,
                HolderName = " \t "
            };
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("HolderName can't be empty", msg.Exception.Message);
        }

        [Fact]
        public void CannotCreateAccountWithSameId()
        {
            var cmd = new CreateAccountCommand
            {
                AccountId = AccountId,
                HolderName = "John Doe"
            };
            CmdHandler.Handle(cmd);
            dynamic msg = CmdHandler.Handle(cmd);
            Assert.IsType<Fail>(msg);
            Assert.Equal("An account with this ID already exists", msg.Exception.Message);
        }
    }
}

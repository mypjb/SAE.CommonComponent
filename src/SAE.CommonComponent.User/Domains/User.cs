using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Extension;
using System.ComponentModel.Design;
using SAE.CommonComponent.User.Dtos;

namespace SAE.CommonComponent.User.Domains
{
    public class Account
    {
        public Account()
        {

        }
        public Account(string name, string password)
        {
            Name = name;
            this.SetPassword(password);
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Slat { get; set; }

        /// <summary>
        /// set password
        /// </summary>
        /// <param name="password"></param>
        public UserEvent.ChangePassword SetPassword(string password)
        {
            this.Slat = Guid.NewGuid()
                            .ToString("N")
                            .ToLower();

            this.Password = this.Encrypt(password);

            return new UserEvent.ChangePassword
            {
                Password = this.Password
            };
        }

        /// <summary>
        /// check account password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckPassword(string password)
        {
            return this.Encrypt(password)
                       .Equals(this.Password, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// encrypt password
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Encrypt(string input)
        {
            return $"{input}{this.Slat}".ToLower().ToMd5().ToLower();
        }
    }
    public class User : Document
    {
        public User()
        {

        }
        public User(UserCommand.Create command)
        {
            var @event = new UserEvent.Register
            {
                Id = Utils.GenerateId(),
                Account = new Account(command.Name, command.Password),
                Name = command.Name,
                Status = Status.Enable,
                CreateTime =SAE.CommonLibrary.Constants.DefaultTimeZone
            };
            this.Apply(@event);
        }
        /// <summary>
        /// user id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// user name 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// user account
        /// </summary>
        public Account Account { get; set; }
        /// <summary>
        /// user status 
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// create create time
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// account is exist
        /// </summary>
        /// <param name="userProvider"></param>
        /// <returns></returns>
        public async Task AccountExist(Func<string, Task<bool>> userProvider)
        {
            Assert.Build(await userProvider.Invoke(this.Account.Name))
                  .False($"{this.Account.Name} is exist");
        }

        public bool Authentication(string password)
        {
            return this.Account.CheckPassword(password);
        }

        public void ChangePassword(UserCommand.ChangePassword command)
        {
            Assert.Build(this.Account.CheckPassword(command.OriginalPassword))
                  .True($"password inexactitude!");
            var @event = this.Account.SetPassword(command.Password);

            this.Apply(@event);
        }

        public void ChangeStatus(UserCommand.ChangeStatus command)
        {
            this.Apply<UserEvent.ChangeStatus>(command);
        }

    }
}

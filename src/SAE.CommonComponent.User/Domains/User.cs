using SAE.CommonComponent.User.Commands;
using SAE.CommonComponent.User.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAE.CommonLibrary.Extension;

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
        public void SetPassword(string password)
        {
            this.Slat = Guid.NewGuid()
                            .ToString("N")
                            .ToLower();

            this.Password = this.Encrypt(password);
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
            return $"{input}{this.Slat}".ToLower().ToMd5();
        }
    }
    public class User : Document
    {
        public User()
        {

        }
        public User(UserCommand.Register register)
        {
            var @event = new UserEvent.Register
            {
                Id = Utils.GenerateId(),
                Account = new Account(register.Name, register.Password),
                Name = register.Name,
                Status = Status.Enable,
                CreateTime = Constant.DefaultTimeZone
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
        public DateTime CrateTime { get; set; }

    }
}

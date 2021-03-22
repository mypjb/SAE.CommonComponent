using SAE.CommonComponent.User.Abstract.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.User.Converts
{
    public class UserConvert:TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(UserDto);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null) return null;
            var user = (User.Domains.User)value;
            var result = new UserDto
            {
                Account = new AccountDto
                {
                    Name= user.Account?.Name
                },
                CreateTime = user.CrateTime,
                Id = user.Id,
                Name = user.Name,
                Status = user.Status
            };
            return result;
        }
    }
}

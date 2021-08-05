using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Dtos;
using System;
using System.ComponentModel;
using System.Globalization;

namespace SAE.CommonComponent.Application.Converts
{
    public class AccessCredentialsConvert : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(ClientDto);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null) return null;
            var app = (Client)value;
            var result = new ClientDto
            {
                Id = app.Id,
                Name = app.Name,
                Status = app.Status,
                CreateTime = app.CreateTime,
                Endpoint = new Dtos.EndpointDto
                {
                    PostLogoutRedirectUris = app.Endpoint.PostLogoutRedirectUris,
                    RedirectUris = app.Endpoint.RedirectUris,
                    SignIn = app.Endpoint.SignIn
                },
                Scopes = app.Scopes,
                Secret = app.Secret
            };
            return result;
        }
    }
}

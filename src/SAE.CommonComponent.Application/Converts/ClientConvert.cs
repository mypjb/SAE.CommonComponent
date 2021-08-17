using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Dtos;
using System;
using System.ComponentModel;
using System.Globalization;

namespace SAE.CommonComponent.Application.Converts
{
    public class ClientConvert : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(ClientDto);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null) return null;
            var client = (Client)value;
            var result = new ClientDto
            {
                Id = client.Id,
                AppId = client.AppId,
                Name = client.Name,
                Status = client.Status,
                CreateTime = client.CreateTime,
                Endpoint = new Dtos.EndpointDto
                {
                    PostLogoutRedirectUris = client.Endpoint.PostLogoutRedirectUris,
                    RedirectUris = client.Endpoint.RedirectUris,
                    SignIn = client.Endpoint.SignIn
                },
                Scopes = client.Scopes,
                Secret = client.Secret
            };
            return result;
        }
    }
}

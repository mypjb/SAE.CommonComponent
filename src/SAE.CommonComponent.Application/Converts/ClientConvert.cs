using SAE.CommonComponent.Application.Abstract.Domains;
using SAE.CommonComponent.Application.Dtos;
using System;
using System.ComponentModel;
using System.Globalization;

namespace SAE.CommonComponent.Application.Converts
{
    /// <summary>
    /// <see cref="Client"/> 转换器
    /// </summary>
    /// <inheritdoc/>
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
                Endpoint = new ClientEndpointDto
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

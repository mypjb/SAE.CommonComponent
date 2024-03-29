using SAE.CommonComponent.Application.Dtos;
using System;
using System.Collections.Generic;

namespace SAE.CommonComponent.Application.Dtos
{
    public class ClientDto
    {
        /// <summary>
        /// client id
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// app id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// app name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// endpoint
        /// </summary>
        public EndpointDto Endpoint { get; set; }

        /// <summary>
        /// app secret
        /// </summary>
        /// <value></value>
        public string Secret { get; set; }

        /// <summary>
        /// auth scope
        /// </summary>
        /// <value></value>
        public string[] Scopes { get; set; }

        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// app status
        /// </summary>
        /// <value></value>
        public Status Status { get; set; }
    }
}
﻿using SAE.CommonComponent.ConfigServer.Commands;
using SAE.CommonComponent.ConfigServer.Events;
using SAE.CommonLibrary;
using SAE.CommonLibrary.EventStore.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Models
{
    /// <summary>
    /// 模板
    /// </summary>
    public class Template : Document
    {
        public Template()
        {

        }
        public Template(TemplateCreateCommand command)
        {
            this.Apply<TemplateCreateEvent>(command, e =>
            {
                e.Id = Utils.GenerateId();
                e.CreateTime = DateTime.UtcNow;
            });
        }

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; set; }
        public DateTime CreateTime { get; set; }


        public void Change(TemplateChangeCommand command) => this.Apply<TemplateChangeEvent>(command);
    }
}

using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class TemplateCreateCommand
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; set; }
    }

    public class TemplateChangeCommand: TemplateCreateCommand
    {
        public string Id { get; set; }
    }

    public class TemplateQueryCommand : Paging
    {
    }
}

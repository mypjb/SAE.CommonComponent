using SAE.CommonLibrary.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAE.CommonComponent.ConfigServer.Commands
{
    public class TemplateCommand
    {
        public class Create
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

        public class Change : Create
        {
            public string Id { get; set; }
        }

        public class Query : Paging
        {
            /// <summary>
            /// Template Name
            /// </summary>
            public string Name { get; set; }
        }
    }
}

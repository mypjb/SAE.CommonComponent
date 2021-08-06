using SAE.CommonLibrary.Abstract.Model;

namespace SAE.CommonComponent.Application.Commands
{
    public class AppResourceCommand
    {
        public class Query : Paging
        {
            public string Name { get; set; }
            /// <summary>
            /// resource path
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// resource method (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }
        public class Create
        {
            public string AppId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Index { get; set; }
            /// <summary>
            /// resource name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// resource path
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// resource method (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }
        public class Change
        {
            public string Id { get; set; }
            /// <summary>
            /// resource name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// resource path
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// resource method (get、post、put...)
            /// </summary>
            public string Method { get; set; }
        }

        public class List
        {
            public string AppId { get; set; }
        }
    }
}
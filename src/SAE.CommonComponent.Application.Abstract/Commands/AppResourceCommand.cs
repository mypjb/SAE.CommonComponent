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
            public string Method { get; }
        }
        public class Create
        {
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
            public string Method { get; }
        }
        public class Change: Create
        {
            public string Id { get; set; }
            /// <summary>
            /// resource index relative to the app
            /// </summary>
            public int Index { get; set; }

        }
    }
}
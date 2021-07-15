using System;

namespace SAE.CommonComponent
{
    public enum Status
    {
        Delete = -1,
        Disable = 0,
        Enable = 1
    }

    public enum AccessMethod
    {
        Get = 0,
        Put = 1,
        Delete = 2,
        Post = 3,
        Head = 4,
        Trace = 5,
        Patch = 6,
        Connect = 7,
        Options = 8,
        Custom = 9,
        None = byte.MaxValue
    }
    /// <summary>
    /// dict type
    /// </summary>
    public enum DictType
    {
        Environment = 1,
        Scope = 2
    }
}

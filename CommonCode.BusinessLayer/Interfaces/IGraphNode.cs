using System;

namespace CommonCode.BusinessLayer.Interfaces
{
    public interface IGraphNode
    {
        DateTime CreatedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}

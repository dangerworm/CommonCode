using System;

namespace CommonCode.BusinessLayer.Interfaces
{
    public interface IGraphAuditData
    {
        DateTime CreatedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}

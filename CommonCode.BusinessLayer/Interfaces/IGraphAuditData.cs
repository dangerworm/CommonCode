using System;

namespace CommonCode.BusinessLayer.Interfaces
{
    public interface IGraphAuditData
    {
        Guid OwnerId { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}

using System;

namespace CommonCode.BusinessLayer.Interfaces
{
    public interface IIdentifiableByGuid
    {
        Guid? Id { get; }
    }
}
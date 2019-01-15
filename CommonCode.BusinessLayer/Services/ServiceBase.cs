using CommonCode.BusinessLayer.Helpers;

namespace CommonCode.BusinessLayer.Services
{
    public abstract class ServiceBase
    {
        protected IUnitOfWork UnitOfWork;

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            Verify.NotNull(unitOfWork, nameof(unitOfWork));

            UnitOfWork = unitOfWork;
        }
    }
}
using CommonCode.BusinessLayer.Helpers;

namespace CommonCode.BusinessLayer.Services
{
    public abstract class ServiceBase<TConnection, TTransaction>
    {
        protected IUnitOfWork<TConnection, TTransaction> UnitOfWork;

        protected ServiceBase(IUnitOfWork<TConnection, TTransaction> unitOfWork)
        {
            Verify.NotNull(unitOfWork, nameof(unitOfWork));

            UnitOfWork = unitOfWork;
        }
    }
}
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace CommonCode.Web.Infrastructure
{
    public class WindsorContainerFactory
    {
        private static IWindsorContainer _container;
        private static readonly object SyncObject = new object();

        public static IWindsorContainer Current(IWindsorInstaller windsorInstaller)
        {
            if (_container != null)
            {
                return _container;
            }

            lock (SyncObject)
            {
                _container = new WindsorContainer();
                _container.Install(windsorInstaller);
            }

            return _container;
        }
    }
}
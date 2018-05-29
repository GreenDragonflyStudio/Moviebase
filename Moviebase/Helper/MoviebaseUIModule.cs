using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Interception.Planning.Strategies;
using Ninject.Modules;
using Ninject.Planning.Strategies;

namespace Moviebase.Helper
{
    public class MoviebaseUIModule : NinjectModule
    {
        public override void Load()
        {
            this.KernelInstance.Components.Add<IPlanningStrategy, AutoNotifyInterceptorRegistrationStrategy>();
        }
    }
}

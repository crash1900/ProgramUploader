using Ninject.Modules;
using ProgramUploader.Model;
using ProgramUploader.Model.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IMovementRepository>().To<SqlMovementRepository>();
            Bind<IProgramHeaderMovementRepository>().To<SqlProgramHeaderMovementRepository>();
            Bind<IProgramHeaderRepository>().To<SqlProgramHeaderRepository>();
        }
    }
}

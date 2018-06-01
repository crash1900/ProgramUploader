using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramUploader
{
    public sealed class DbContextInstance
    {
        private static DbContextInstance instance = null;

        private DbContextInstance()
        {

        }

        public static DbContextInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DbContextInstance();
                }
                return instance;
            }
        }
    }
}

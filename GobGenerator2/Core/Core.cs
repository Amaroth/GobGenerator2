using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobGenerator2.Core
{
    class Core
    {
        private static Core instance;

        private Core()
        {

        }

        public static Core Instance
        {
            get
            {
                if (instance == null)
                    instance = new Core();
                return instance;
            }
        }
    }
}

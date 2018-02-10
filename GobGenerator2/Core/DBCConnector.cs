using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDBXLib;
using WDBXLib.Reader;
using WDBXLib.Definitions.WotLK;

namespace GobGenerator2
{
    class DBCConnector
    {
        

        public DBCConnector()
        {

        }

        public void ConnectDBC(string dbcPath)
        {
            var entry = DBReader.Read<GameObjectDisplayInfo>(dbcPath);
        }
    }
}

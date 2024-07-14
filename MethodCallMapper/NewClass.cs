using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodCallMapper
{
    public static class NewClass
    {
        public static void MethodD()
        {
            MethodE();
            MethodF();
        }

        private static void MethodF()
        {
            throw new NotImplementedException();
        }

        private static void MethodE()
        {
            throw new NotImplementedException();
        }
    }
}

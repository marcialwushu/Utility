using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodCallMapper
{
    public class SampleClass
    {
        public void MethodA()
        {
            MethodB();
            MethodC();
        }

        public void MethodC()
        {
            NewClass.MethodD();
        }

        private  void MethodB()
        {
            throw new NotImplementedException();
        }


    }
}

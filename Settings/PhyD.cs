using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAVL.Settings
{
    public class PhyD
    {
        public float stopCondition = 0.005f;
        public float attenuation = 0.1f;
        public float repulsion = 1000.0f;
        public float elastic = 0.005f;

        public PhyD()
        {
            stopCondition = 0.005f;
        }
    }
}

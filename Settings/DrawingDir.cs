using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoAVL.Drawables;

namespace AutoAVL.Settings
{
    public class DrawingDir
    {
        private float nodeRadius;
        private float borderWidth;


        public float TotalRadius()
        {
            return nodeRadius + borderWidth;
        }

        public float GetNodeRadius()
        {
            return nodeRadius;
        }

    }
}

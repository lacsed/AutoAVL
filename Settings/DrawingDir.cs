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
        // Node settings.
        public float nodeRadius;
        public float autoLinkRadius;
        public float borderWidth;
        public float markedRatio = 0.8f;
        public float linkRatio = 0.8f;
        public float arcSize = 10.0f;

        // Colors
        public string strokeColor = "black";
        public string textColor = "black";

        // Strokes
        public string strokeFill = "none";

        // Text
        public float textSize = 25.0f;

        public float TotalRadius()
        {
            return nodeRadius + borderWidth;
        }

    }
}

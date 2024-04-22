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
        public float nodeRadius = 60.0f;
        public float autoLinkRadius = 10.0f;
        public float borderWidth = 2.0f;
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

        public float arrowLength = 5.0f;
        public float overlap = 0.2f;
        public float textDistance = 5.0f;
        public float arrowWidth = 10.0f;
        public string arrowColor = "black";
        public float linkStrokeWidth = 2.0f;

        public float TotalRadius()
        {
            return nodeRadius + borderWidth;
        }

    }
}

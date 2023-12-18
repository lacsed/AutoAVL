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

        private float canvasWidth;
        private float canvasHeight;
        private Vector2D canvasOrigin;


        public float TotalRadius()
        {
            return nodeRadius + borderWidth;
        }

        public void SetCanvasDimensions(Box canvasBox)
        {
            Vector2D topLeft = canvasBox.GetTopLeft();
            Vector2D bottomRight = canvasBox.GetBottomRight();

            canvasOrigin = topLeft;
            canvasWidth = bottomRight.x - topLeft.x;
            canvasHeight = topLeft.y - bottomRight.y;
        }

        public string SvgDimensions()
        {
            return "<svg height=\"" + canvasHeight + "\" width=\"" + canvasWidth + "\">" + Environment.NewLine;
        }

        public string SvgSettings()
        {
            
        }
    }
}

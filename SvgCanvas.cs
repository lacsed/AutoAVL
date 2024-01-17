using AutoAVL.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoAVL
{
    public class SvgCanvas
    {
        private float canvasWidth;
        private float canvasHeight;
        private Vector2D canvasOrigin;

        public void SetUpCanvas(Box canvasBox)
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
            return "<defs><marker id=\"arrowhead\" markerWidth=\"10\" markerHeight=\"7\" refX = \"7\" refY = \"3.5\" orient = \"auto\" ><polygon points=\"0 0, 10 3.5, 0 7\" /></marker></defs>" + Environment.NewLine;
        }

        public Vector2D ToSvgCoordinates(Vector2D v)
        {
            return new Vector2D(v.x - canvasOrigin.x, canvasOrigin.y - v.y);
        }
    }
}

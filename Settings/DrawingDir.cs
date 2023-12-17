using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SetCanvasDimensions((Vector2D, Vector2D) diagonalLimits)
        {
            canvasOrigin = diagonalLimits.Item1;
            canvasWidth = diagonalLimits.Item2.x - diagonalLimits.Item1.x;
            canvasHeight = diagonalLimits.Item1.y - diagonalLimits.Item2.y;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAVL.Drawables
{
    public class Box
    {
        private Vector2D topLeft;
        private Vector2D bottomRight;

        public Box(Vector2D topLeft, Vector2D bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public static Box EncompassingBox(Box a, Box b)
        {
            float minX = (a.topLeft.x < b.topLeft.x) ? a.topLeft.x : b.topLeft.x;
            float maxX = (a.bottomRight.x > b.bottomRight.x) ? a.bottomRight.x : b.bottomRight.x;
            float minY = (a.bottomRight.y < b.bottomRight.y) ? a.bottomRight.y : b.bottomRight.y;
            float maxY = (a.topLeft.y > b.topLeft.y) ? a.topLeft.y : b.topLeft.y;

            Vector2D topLeft = new Vector2D(minX, maxY);
            Vector2D bottomRight = new Vector2D(maxX, minY);

            return new Box(topLeft, bottomRight);
        }
    }
}

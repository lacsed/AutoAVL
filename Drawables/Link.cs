using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoAVL.Settings;

namespace AutoAVL.Drawables
{
    public class Link : Drawable
    {
        public Node start;
        public Node end;

        public string name;
        public Vector2D nameOffset;

        public float radiusPercentage;

        public Guid guid;

        public bool isAutoLink;

        public Link(Node origin, Node destination, string name)
        {
            start = origin;
            end = destination;
            this.name = name;
            guid = Guid.NewGuid();
            isAutoLink = origin == destination ? true : false;
        }

        public static void PullLinks(List<Link> links, PhyD phyD)
        {

        }

        public string ToSvg(DrawingDir drawingDir, SvgCanvas canvas)
        {
            return "";
        }

        public Box GetBox(DrawingDir drawingDir)
        {

        }
    }
}

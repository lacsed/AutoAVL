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
        public bool isInitialLink;


        /// <summary>
        /// Represents the auxiliary point necessary to determine certain types of links.
        /// - In the case of an auto link, auxPoint represents it's center.
        /// - In the case of an initial link, auxPoint represents where the link starts.
        /// - In the case of a standard link, auxPoint is the middle point the links passes through.
        /// </summary>
        public Vector2D auxPoint;

        public Link(Node origin, Node destination, string name)
        {
            start = origin;
            end = destination;
            this.name = name;
            nameOffset = new Vector2D();
            guid = Guid.NewGuid();
            isAutoLink = (origin == destination);
        }

        public static void PullLinks(List<Link> links, PhyD phyD)
        {
            foreach (Link link in links)
            {
                if (!link.isAutoLink)
                    PullLink(link, phyD);
            }
        }

        static void PullLink(Link link, PhyD phyD)
        {
            Vector2D nodeAPos = link.start.position;
            Vector2D nodeBPos = link.end.position;

            float distance = (nodeBPos - nodeAPos).Length();
            float force = (1 - phyD.attenuation) * phyD.elastic * distance;

            Vector2D forceDirection = (nodeBPos - nodeAPos).Normalized();

            link.start.displacement += force * forceDirection;
            link.end.displacement -= force * forceDirection;
        }

        public string ToSvg(DrawingDir drawingDir, SvgCanvas canvas)
        {
            return "";
        }

        public Box GetBox(DrawingDir drawingDir)
        {
            if (isAutoLink)
            {
                float radius = drawingDir.TotalRadius();
                Vector2D topLeft = new Vector2D(auxPoint.x - radius, auxPoint.y + radius);
                Vector2D bottomRight = new Vector2D(auxPoint.x + radius, auxPoint.y - radius);

                return new Box(topLeft, bottomRight);
            }
            else if (isInitialLink)
            {
                return Box.EncompassingBox(auxPoint, end.position);
            }
            else
            {
                Vector2D perpendicular = (end.position - start.position).Perpendicular();
                Vector2D middle = start.position.Middle(end.position) + perpendicular * (drawingDir.arcSize + drawingDir.borderWidth);

                return Box.EncompassingBox(start.position, middle, end.position);
            }
        }
    }
}

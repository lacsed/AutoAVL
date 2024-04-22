using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AutoAVL.Settings;
using static System.TimeZoneInfo;

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

        public static void SetUp(List<Link> links, DrawingDir drawingDir)
        {
            List<Guid> processedOpLinks = new List<Guid>();

            foreach (Link link in links.Where(link => !link.isAutoLink && !link.isInitialLink))
            {
                if (processedOpLinks.Contains(link.guid))
                    continue;

                Link opLink = links.Find(x => x.end == link.start && x.start == link.end);

                if (opLink != null)
                {
                    processedOpLinks.Add(opLink.guid);

                    float transitionDistance = (link.end.position - link.start.position).Length();
                    Vector2D middle = link.start.position.Middle(link.end.position);
                    Vector2D perpendicularVec = (link.end.position - link.start.position).Perpendicular();

                    link.auxPoint = middle + perpendicularVec * transitionDistance * drawingDir.linkRatio;
                    opLink.auxPoint = middle - perpendicularVec * transitionDistance * drawingDir.linkRatio;
                } 
                else
                {
                    Vector2D middle = link.start.position.Middle(link.end.position);

                    link.auxPoint = middle;
                }
            }

            processedOpLinks.Clear();

            foreach (Link link in links.Where(link => link.isAutoLink))
            {
                List<Link> adjacentLinks = links.Where(adjacentLink => (adjacentLink.start == link.start || adjacentLink.end == link.end) && adjacentLink != link).ToList();
                List<Vector2D> adjacentDirections = new List<Vector2D>();

                foreach (Link adjacentLink in adjacentLinks)
                {
                    adjacentDirections.Add((adjacentLink.auxPoint - link.start.position));
                }

                adjacentDirections.Sort((v1, v2) => v1.Angle().CompareTo(v2.Angle()));

                Vector2D sectionStart = new Vector2D(0, 0);
                Vector2D sectionEnd = new Vector2D(0, 0);
                float sectionAngle = float.MinValue;
                Console.WriteLine(adjacentDirections.Count);
                for (int i = 0; i <= adjacentDirections.Count - 1; i++)
                {
                    Console.WriteLine(i.ToString());
                    float angle = Math.Abs(adjacentDirections[(i + 1) % adjacentDirections.Count].Angle() - adjacentDirections[i].Angle());
                    Console.WriteLine("Angle = " + angle);
                    if (angle > sectionAngle)
                    {
                        sectionAngle = angle;
                        sectionStart = adjacentDirections[i];
                        sectionEnd = adjacentDirections[(i + 1) % adjacentDirections.Count];
                    }
                }
                
                Vector2D linkDirection = sectionStart.Rotated(sectionAngle / 2).Normalized();
                link.auxPoint = link.start.position + linkDirection * drawingDir.autoLinkRadius;
            }
        }

        public string ToSvg(DrawingDir drawingDir, SvgCanvas canvas)
        {
            if (isAutoLink)
            {
                return this.AutoLinkSVG(drawingDir, canvas);
            }
            else if (isInitialLink)
            {
                return "";
            }
            else
            {
                if ((auxPoint - start.position).Angle() == (end.position - auxPoint).Angle())
                {
                    return this.GenerateStraightTransitionSVG(canvas, drawingDir);
                } 
                else
                {
                    return this.GenerateCurvedTransitionSVG(canvas, drawingDir); ;
                }
            }
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
                Vector2D middle = auxPoint + perpendicular * drawingDir.linkStrokeWidth;

                return Box.EncompassingBox(start.position, middle, end.position);
            }
        }

        string GenerateStraightTransitionSVG(SvgCanvas canvas, DrawingDir drawingDir)
        {
            Vector2D transitionDirection = (end.position - start.position).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D transitionOrigin = canvas.ToSvgCoordinates(start.position + transitionDirection * drawingDir.nodeRadius);
            Vector2D transitionDestination = canvas.ToSvgCoordinates(end.position - transitionDirection * drawingDir.nodeRadius);

            Vector2D textPosition = auxPoint + perpendicularDirection * drawingDir.textDistance;

            string svgArrowElement = GenerateStraightArrowSVG(transitionOrigin, transitionDestination, drawingDir);
            string svgTextElement = GenerateSvgTextElement(textPosition, perpendicularDirection, canvas, name, drawingDir);

            string svgRepresentation = svgArrowElement + svgTextElement;

            return svgRepresentation;
        }

        string GenerateCurvedTransitionSVG(SvgCanvas canvas, DrawingDir drawingDir)
        {
            Vector2D transitionDirection = (end.position - start.position).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D transitionOrigin = start.position + ((auxPoint - start.position).Normalized() * drawingDir.nodeRadius);
            Vector2D transitionDestination = end.position + ((auxPoint - end.position).Normalized() * drawingDir.nodeRadius);

            Vector2D controlPointSVG = canvas.ToSvgCoordinates(auxPoint);
            Vector2D originSVG = canvas.ToSvgCoordinates(transitionOrigin);
            Vector2D destinationSVG = canvas.ToSvgCoordinates(transitionDestination);

            Vector2D textPosition = auxPoint + perpendicularDirection * (drawingDir.arcSize + drawingDir.textDistance);

            string svgArrowElement = GenerateCurvedArrowSVG(originSVG, destinationSVG, controlPointSVG, drawingDir);
            string svgTextElement = GenerateSvgTextElement(textPosition, perpendicularDirection, canvas, name, drawingDir);

            string svgRepresentation = svgArrowElement + svgTextElement;

            return svgRepresentation;
        }

        static string GenerateCurvedArrowSVG(Vector2D origin, Vector2D destination, Vector2D control, DrawingDir drawingDir)
        {
            Vector2D insideArrow = destination + (control - destination).Normalized() * drawingDir.arrowLength / 2;

            string svgPathElement = GenerateQuadraticBezierPathSVG(origin, insideArrow, control, drawingDir);
            string svgArrowheadElement = GenerateArrowheadPolygonSVG(destination, drawingDir, destination - control);

            string svgRepresentation = svgPathElement + svgArrowheadElement;

            return svgRepresentation;
        }

        static string GenerateQuadraticBezierPathSVG(Vector2D origin, Vector2D destination, Vector2D control, DrawingDir drawingDir)
        {
            // Construct the path attribute.
            string path = $"M {origin.x} {origin.y} Q {control.x} {control.y} {destination.x} {destination.y}";

            // Construct the stroke attribute.
            string stroke = $"stroke=\"{drawingDir.strokeColor}\"";

            // Construct the stroke-width attribute.
            string strokeWidth = $"stroke-width=\"{drawingDir.linkStrokeWidth}\"";

            // Construct the fill attribute.
            string fill = "fill=\"none\"";

            // Construct the SVG path element.
            string svgPathElement = $"<path d=\"{path}\" {stroke} {strokeWidth} {fill} />{Environment.NewLine}";

            return svgPathElement;
        }

        static string GenerateStraightArrowSVG(Vector2D origin, Vector2D destination, DrawingDir drawingDir)
        {
            Vector2D insideArrow = destination + (origin - destination).Normalized() * drawingDir.arrowLength / 2;

            string svgLineElement = GenerateLineElementSVG(origin, insideArrow, drawingDir);
            string svgArrowheadElement = GenerateArrowheadPolygonSVG(destination, drawingDir, destination - origin);

            string svgRepresentation = svgLineElement + svgArrowheadElement;

            return svgRepresentation;
        }

        static string GenerateLineElementSVG(Vector2D origin, Vector2D destination, DrawingDir drawingDir)
        {
            string svgLineElement = $"<line x1=\"{origin.x}\" y1=\"{origin.y}\" x2=\"{destination.x}\" y2=\"{destination.y}\" stroke=\"{drawingDir.strokeColor}\" stroke-width=\"{drawingDir.linkStrokeWidth}\" />{Environment.NewLine}";

            return svgLineElement;
        }

        string AutoLinkSVG(DrawingDir drawingDir, SvgCanvas canvas)
        {
            Vector2D transitionDirection = (auxPoint - start.position).Normalized();

            string output = "";

            float arrowCoverageAngle = Vector2D.AngleBetween(drawingDir.arrowLength, drawingDir.autoLinkRadius, drawingDir.autoLinkRadius);
            float stateToTransitionDistance = drawingDir.autoLinkRadius + drawingDir.nodeRadius - drawingDir.autoLinkRadius * drawingDir.overlap;

            Vector2D transitionCenter = start.position + transitionDirection * stateToTransitionDistance;

            float angleToIntersectPoint = Vector2D.AngleBetween(drawingDir.autoLinkRadius, stateToTransitionDistance, drawingDir.nodeRadius);

            Vector2D initialIntersect = start.position + stateToTransitionDistance * transitionDirection.Rotated(angleToIntersectPoint);
            Vector2D finalIntersect = start.position + stateToTransitionDistance * transitionDirection.Rotated(-angleToIntersectPoint);

            Vector2D transitionCenterToFinalIntersect = (finalIntersect - transitionCenter).Normalized();

            Vector2D transitionEnd = transitionCenter + drawingDir.nodeRadius * transitionCenterToFinalIntersect.Rotated(arrowCoverageAngle);
            Vector2D transitionStart = initialIntersect;

            Vector2D arrowTip = finalIntersect;
            Vector2D arrowDirection = (arrowTip - transitionEnd).Normalized();

            Vector2D transitionEndSVG = canvas.ToSvgCoordinates(transitionEnd);
            Vector2D transitionStartSVG = canvas.ToSvgCoordinates(transitionStart);

            Vector2D arrowTipSVG = canvas.ToSvgCoordinates(arrowTip);
            Vector2D arrowDirectionSVG = canvas.ToSvgCoordinates(arrowDirection);

            Vector2D textPosition = transitionCenter + transitionDirection * (drawingDir.textDistance + drawingDir.autoLinkRadius);

            output += "<path stroke-width=\"1\" stroke=\"black\" fill=\"none\" d=\" M " + transitionStartSVG.x + " " + transitionStartSVG.y + " A " + drawingDir.autoLinkRadius + " " + drawingDir.autoLinkRadius + " 0 1 1 " + transitionEndSVG.x + " " + transitionEndSVG.y + "\" />" + Environment.NewLine;
            output += GenerateArrowheadPolygonSVG(arrowTipSVG, drawingDir, arrowDirectionSVG);
            output += GenerateSvgTextElement(textPosition, transitionDirection, canvas, name, drawingDir);

            return output;
        }

        static string GenerateArrowheadPolygonSVG(Vector2D tip, DrawingDir drawingDir, Vector2D direction)
        {
            // Calculate the base point of the arrowhead by subtracting the normalized direction vector multiplied by the arrow length from the tip.
            Vector2D basePoint = tip - direction.Normalized() * drawingDir.arrowLength;

            // Calculate the side points of the arrowhead by adding/subtracting the perpendicular direction vector multiplied by half of the arrow width to/from the base point.
            Vector2D sidePoint1 = basePoint + direction.Perpendicular() * drawingDir.arrowWidth / 2;
            Vector2D sidePoint2 = basePoint - direction.Perpendicular() * drawingDir.arrowWidth / 2;

            // Construct the SVG polygon element with the calculated points, arrow color, and stroke width.
            string svgPolygonElement = $"<polygon fill=\"{drawingDir.arrowColor}\" stroke-width=\"1\" " +
                $"points=\"{tip.x} {tip.y} {sidePoint1.x} {sidePoint1.y} {sidePoint2.x} {sidePoint2.y}\" />{Environment.NewLine}";

            return svgPolygonElement;
        }

        static string GenerateSvgTextElement(Vector2D position, Vector2D direction, SvgCanvas canvas, string text, DrawingDir drawingDir)
        {
            // Calculate the angle based on the direction.
            float angle = direction.Angle();

            // Determine the text anchor based on the angle.
            string textAnchor;
            if (angle >= 5.4978f || angle <= 0.7854f)
                textAnchor = "start";
            else if (angle > 0.7854f && angle < 2.3562f)
                textAnchor = "middle";
            else if (angle >= 2.3562f && angle <= 3.9269f)
                textAnchor = "end";
            else
                textAnchor = "middle";

            // Convert the position to SVG coordinates.
            Vector2D svgPosition = canvas.ToSvgCoordinates(position);

            // Construct the SVG text element with the specified attributes.
            string svgTextElement = $@"
                <text x=""{svgPosition.x}"" y=""{svgPosition.y}"" 
                      text-anchor=""{textAnchor}"" 
                      font-size=""{drawingDir.textSize}"" fill=""{drawingDir.textColor}"">
                    {text}
                </text>
                {Environment.NewLine}";

            return svgTextElement;
        }
    }
}

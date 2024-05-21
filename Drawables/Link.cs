using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            isInitialLink = false;
            auxPoint = new Vector2D();
        }
        
        public Link(Node initialState)
        {
            start = initialState;
            end = initialState;
            this.name = "";
            nameOffset = new Vector2D();
            guid = Guid.NewGuid();
            isAutoLink = false;
            isInitialLink = true;
            auxPoint = new Vector2D();
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
            if (link.isAutoLink || link.isInitialLink)
                return;
                
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

                Vector2D linkDirection = AcomodateAutoInitialLink(link.start.position, adjacentLinks);
                link.auxPoint = link.start.position + linkDirection * (drawingDir.AutoRadius() + drawingDir.TotalRadius());
            }
            
            Link initialLink = links.Find(x => x.isInitialLink);
            List<Link> iAdjacentLinks = links.Where(adjacentLink => (adjacentLink.start == initialLink.start || adjacentLink.end == initialLink.end) && adjacentLink != initialLink).ToList();
            Vector2D initialLinkDirection = AcomodateAutoInitialLink(initialLink.start.position, iAdjacentLinks);
            initialLink.auxPoint = initialLink.start.position + initialLinkDirection * (drawingDir.initialLinkSize + drawingDir.TotalRadius());
        }
        
        public static Vector2D AcomodateAutoInitialLink(Vector2D nodePos, List<Link> adjacentLinks)
        {
            List<Vector2D> adjacentDirections = new List<Vector2D>();

            foreach(Link adjacentLink in adjacentLinks)
            {
                adjacentDirections.Add(adjacentLink.auxPoint - nodePos);
            }

            adjacentDirections.Sort((v1, v2) => v1.UnsignedAngle().CompareTo(v2.UnsignedAngle()));
            
            Vector2D sectionStart = new Vector2D();
            Vector2D sectionEnd = new Vector2D();
            float sectionAngle = float.MinValue;
            
            for(int i = 0; i <= adjacentDirections.Count - 1; i++)
            {
                Vector2D currentVector = adjacentDirections[i];
                Vector2D nextVector = adjacentDirections[(i + 1) % adjacentDirections.Count];

                float angle = currentVector.UnsignedRotationAngle(nextVector);

                if (angle > sectionAngle)
                {
                    sectionAngle = angle;
                    sectionStart = adjacentDirections[i];
                    sectionEnd = adjacentDirections[(i + 1) % adjacentDirections.Count];
                }
            }
            
            return sectionStart.Rotated(sectionAngle / 2).Normalized();
        }

        public string ToSvg(DrawingDir drawingDir, SvgCanvas canvas)
        {
            if (isAutoLink)
            {
                return this.AutoLinkSVG(drawingDir, canvas);
            }
            else if (isInitialLink)
            {
                return this.GenerateStraightTransitionSVG(canvas, drawingDir);
            }
            else
            {
                if ((auxPoint - start.position.Middle(end.position)).Length() < drawingDir.clipRatio * (start.position - end.position).Length() / 2)
                {
                    return this.GenerateStraightTransitionSVG(canvas, drawingDir);
                } 
                else
                {
                    return this.GenerateArcTransitionSVG(canvas, drawingDir); ;
                }
            }
        }

        public Box GetBox(DrawingDir drawingDir)
        {
            if (isAutoLink)
            {
                float radius = drawingDir.AutoRadius();
                Vector2D topLeft = new Vector2D(auxPoint.x - radius, auxPoint.y + radius);
                Vector2D bottomRight = new Vector2D(auxPoint.x + radius, auxPoint.y - radius);

                return new Box(topLeft, bottomRight);
            }
            else if (isInitialLink)
            {
                return Box.EncompassingBox(auxPoint, start.position);
            }
            else
            {
                Vector2D perpendicular = (end.position - start.position).Perpendicular();
                Vector2D middle = auxPoint + perpendicular * (drawingDir.linkStrokeWidth + 2 * drawingDir.textSize);

                return Box.EncompassingBox(start.position, middle, end.position);
            }
        }
        
        string GenerateArcTransitionSVG(SvgCanvas canvas, DrawingDir drawingDir)
        {
            string svgArc = "";
            
            Vector2D transitionDirection = (end.position - start.position).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();
            Vector2D middlePoint = start.position.Middle(end.position);
            
            Vector2D center = Vector2D.FindCenter(start.position, end.position, auxPoint);
            float arcRadius = (auxPoint - center).Length();

            // Find the arrow's tip and base points by rotating the vector from the arc's center 
            // by the angles formed by the node radius and the arrow length, respectively.
            // Note: Since the rotation is counterclockwise, we must use negative angles.
            float nodeRadiusAngle = Vector2D.AngleBetween(drawingDir.nodeRadius, arcRadius, arcRadius);
            float arrowBaseAngle = Vector2D.AngleBetween(drawingDir.arrowLength, arcRadius, arcRadius);
            Vector2D arrowTip = center + (end.position - center).Rotated(-nodeRadiusAngle);
            Vector2D arrowBase = center + (end.position - center).Rotated(-(nodeRadiusAngle + arrowBaseAngle));
            
            Vector2D arcOrigin = center + (start.position - center).Rotated(nodeRadiusAngle);
            Vector2D arcDestination = arrowBase;
            
            Vector2D textPosition = auxPoint + perpendicularDirection * drawingDir.textDistance;
            
            // Between two points exist four possible arcs in SVG.
            // largeArcFlag determinnes if the arc will be the smallest or biggest between the points.
            // sweepFlag determines if the arc will have a clockwise or counter-clockwise rotation.
            int largeArcFlag = (auxPoint - middlePoint).Length() > arcRadius ? 1 : 0;
            int sweepFlag = (end.position - start.position).Cross(auxPoint - end.position) < 0 ? 0 : 1;
            
            // All points need to be converted to the SVG canva's coordinate system.
            arcOrigin = canvas.ToSvgCoordinates(arcOrigin);
            arcDestination = canvas.ToSvgCoordinates(arcDestination);
            arrowBase = arcDestination;
            arrowTip = canvas.ToSvgCoordinates(arrowTip);
            
            // Construct the arc's path attribute.
            string arcPath = $"M {arcOrigin.x} {arcOrigin.y} A {arcRadius} {arcRadius} 0 {largeArcFlag} {sweepFlag} {arcDestination.x} {arcDestination.y}";

            // Line attributes.
            string stroke = $"stroke=\"{drawingDir.strokeColor}\"";
            string strokeWidth = $"stroke-width=\"{drawingDir.linkStrokeWidth}\"";
            string fill = "fill=\"none\"";

            svgArc += $"<path d=\"{arcPath}\" {stroke} {strokeWidth} {fill} />{Environment.NewLine}";
            svgArc += GenerateArrowheadPolygonSVG(arrowTip, arrowBase, drawingDir);
            svgArc += GenerateSvgTextElement(textPosition, perpendicularDirection, canvas, name, drawingDir);
            
            
            return svgArc;
        }
        
        string GenerateStraightTransitionSVG(SvgCanvas canvas, DrawingDir drawingDir)
        {
            Vector2D transitionDirection = (end.position - start.position).Normalized();
            if (isInitialLink) transitionDirection = (end.position - auxPoint).Normalized();
            
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();
            
            Vector2D arrowTip = end.position - transitionDirection * drawingDir.nodeRadius;
            Vector2D arrowBase = end.position - transitionDirection * (drawingDir.nodeRadius + drawingDir.arrowLength);

            Vector2D lineOrigin = start.position + transitionDirection * drawingDir.nodeRadius;
            if (isInitialLink) lineOrigin = auxPoint;
            
            Vector2D lineDestination = arrowBase;

            Vector2D textPosition = auxPoint + perpendicularDirection * drawingDir.textDistance;
            
            // Convert the points to the SVG canva's coordinate system.
            arrowTip = canvas.ToSvgCoordinates(arrowTip);
            arrowBase = canvas.ToSvgCoordinates(arrowBase);
            lineOrigin = canvas.ToSvgCoordinates(lineOrigin);
            lineDestination = canvas.ToSvgCoordinates(lineDestination);
            
            string svgLine = $"<line x1=\"{lineOrigin.x}\" y1=\"{lineOrigin.y}\" x2=\"{lineDestination.x}\" y2=\"{lineDestination.y}\" stroke=\"{drawingDir.strokeColor}\" stroke-width=\"{drawingDir.linkStrokeWidth}\" />{Environment.NewLine}";
            
            svgLine += GenerateArrowheadPolygonSVG(arrowTip, arrowBase, drawingDir);
            if (isInitialLink) return svgLine;
            
            svgLine += GenerateSvgTextElement(textPosition, perpendicularDirection, canvas, name, drawingDir);

            return svgLine;
        }

        /* string GenerateCurvedTransitionSVG(SvgCanvas canvas, DrawingDir drawingDir)
        {
            Vector2D transitionDirection = (end.position - start.position).Normalized();
            Vector2D perpendicularDirection = transitionDirection.Perpendicular();

            Vector2D transitionOrigin = start.position + ((auxPoint - start.position).Normalized() * drawingDir.nodeRadius);
            Vector2D transitionDestination = end.position + ((auxPoint - end.position).Normalized() * drawingDir.nodeRadius);

            Vector2D controlPointSVG = canvas.ToSvgCoordinates(auxPoint);
            Vector2D originSVG = canvas.ToSvgCoordinates(transitionOrigin);
            Vector2D destinationSVG = canvas.ToSvgCoordinates(transitionDestination);

            Vector2D textPosition = auxPoint;// + perpendicularDirection * drawingDir.textDistance;

            string svgArrowElement = GenerateCurvedArrowSVG(originSVG, destinationSVG, controlPointSVG, drawingDir);
            string svgTextElement = GenerateSvgTextElement(textPosition, perpendicularDirection, canvas, name, drawingDir);

            string svgRepresentation = svgArrowElement + svgTextElement;

            return svgRepresentation;
        } */

        /* static string GenerateCurvedArrowSVG(Vector2D origin, Vector2D destination, Vector2D control, DrawingDir drawingDir)
        {
            Vector2D center = Vector2D.FindCenter(origin, destination, control);
            float arcRadius = (control - center).Length();

            // Find the arrow's tip and base points by rotating the vector from the arc's center 
            // by the angles formed by the node radius and the arrow length, respectively.
            // Note: Since the rotation is counterclockwise, we must use negative angles.
            float arrowPointAngle = Vector2D.AngleBetween(drawingDir.nodeRadius, arcRadius, arcRadius);
            float arrowBaseAngle = Vector2D.AngleBetween(drawingDir.arrowLength, arcRadius, arcRadius);
            Vector2D arrowTip = center + (destination - center).Rotated(-arrowPointAngle);
            Vector2D arrowBase = center + (destination - center).Rotated(-(arrowPointAngle + arrowBaseAngle));


            Vector2D insideArrow = destination + (control - destination).Normalized() * drawingDir.arrowLength / 2;

            string svgPathElement = GenerateQuadraticBezierPathSVG(origin, insideArrow, control, drawingDir);
            string svgArrowheadElement = GenerateArrowheadPolygonSVG(destination, drawingDir, destination - control);

            string svgRepresentation = svgPathElement + svgArrowheadElement;

            return svgRepresentation;
        } */


        /* static string GenerateArcPathSVG(Vector2D origin, Vector2D destination, Vector2D control, DrawingDir drawingDir)
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
        } */

        /* static string GenerateQuadraticBezierPathSVG(Vector2D origin, Vector2D destination, Vector2D control, DrawingDir drawingDir)
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
        } */

        /* static string GenerateStraightArrowSVG(Vector2D origin, Vector2D destination, DrawingDir drawingDir)
        {
            Vector2D insideArrow = destination + (origin - destination).Normalized() * drawingDir.arrowLength / 2;

            string svgLineElement = GenerateLineElementSVG(origin, insideArrow, drawingDir);
            string svgArrowheadElement = GenerateArrowheadPolygonSVG(destination, drawingDir, destination - origin);

            string svgRepresentation = svgLineElement + svgArrowheadElement;

            return svgRepresentation;
        } */

        /* static string GenerateLineElementSVG(Vector2D origin, Vector2D destination, DrawingDir drawingDir)
        {
            string svgLineElement = $"<line x1=\"{origin.x}\" y1=\"{origin.y}\" x2=\"{destination.x}\" y2=\"{destination.y}\" stroke=\"{drawingDir.strokeColor}\" stroke-width=\"{drawingDir.linkStrokeWidth}\" />{Environment.NewLine}";

            return svgLineElement;
        } */

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
            //output += GenerateArrowheadPolygonSVG(arrowTipSVG, drawingDir, arrowDirectionSVG);
            output += GenerateSvgTextElement(textPosition, transitionDirection, canvas, name, drawingDir);

            return output;
        }

        static string GenerateArrowheadPolygonSVG(Vector2D arrowTip, Vector2D arrowBase, DrawingDir drawingDir)
        {
            Vector2D arrowDirection = (arrowTip  - arrowBase).Normalized();
            Vector2D perpendicular = arrowDirection.Perpendicular();
            // Calculate the side points of the arrowhead by adding/subtracting the perpendicular direction vector multiplied by half of the arrow width to/from the base point.
            Vector2D sidePoint1 = arrowBase + perpendicular * drawingDir.arrowWidth / 2;
            Vector2D sidePoint2 = arrowBase - perpendicular * drawingDir.arrowWidth / 2;

            // Construct the SVG polygon element with the calculated points, arrow color, and stroke width.
            string svgPolygonElement = $"<polygon fill=\"{drawingDir.arrowColor}\" stroke-width=\"1\" " +
                $"points=\"{arrowTip.x} {arrowTip.y} {sidePoint1.x} {sidePoint1.y} {sidePoint2.x} {sidePoint2.y}\" />{Environment.NewLine}";

            return svgPolygonElement;
        }

        static string GenerateSvgTextElement(Vector2D position, Vector2D direction, SvgCanvas canvas, string text, DrawingDir drawingDir)
        {
            // Calculate the angle based on the direction.
            float angle = direction.UnsignedAngle();
            
            float anchorAngleDeg = 60.0f;
            float anchorAngleRad = anchorAngleDeg * (float) Math.PI / 180;
            
            // Determine the text anchor based on the angle.
            string textAnchor;
            if (angle >= 2 * (float) Math.PI - anchorAngleRad || angle <= anchorAngleRad)
                textAnchor = "start";
            else if (angle >= (float) Math.PI - anchorAngleRad && angle <= (float) Math.PI + anchorAngleRad)
                textAnchor = "end";
            else
                textAnchor = "middle";
                
            string dominantBaseline;
            if (angle > (float) Math.PI)
                dominantBaseline = "hanging";
            else
                dominantBaseline = "auto";
                
                

            // Convert the position to SVG coordinates.
            Vector2D svgPosition = canvas.ToSvgCoordinates(position);

            // Construct the SVG text element with the specified attributes.
            string svgTextElement = $@"
                <text x=""{svgPosition.x}"" y=""{svgPosition.y}"" 
                      text-anchor=""{textAnchor}"" 
                      dominant-baseline=""{dominantBaseline}"" 
                      font-size=""{drawingDir.textSize}"" fill=""{drawingDir.textColor}"">
                    {text}
                </text>
                {Environment.NewLine}";

            return svgTextElement;
        }
    }
}

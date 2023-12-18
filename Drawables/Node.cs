using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoAVL.Settings;
using UltraDES;

namespace AutoAVL.Drawables
{
    public class Node : Drawable
    {
        public Vector2D position;
        public Vector2D displacement;

        public string name;

        public bool marked;

        public Guid id;

        public Node()
        {
            position = new Vector2D();
            displacement = new Vector2D();
            name = "";
            marked = false;
            id = Guid.NewGuid();
        }

        public Node(string input_name, bool isMarked)
        {
            position = new Vector2D();
            displacement = new Vector2D();
            name = input_name;
            marked = isMarked;
            id = Guid.NewGuid();
        }

        public Node(Vector2D inputPosition, string inputName, bool inputMarked)
        {
            position = inputPosition;
            displacement = new Vector2D();
            name = inputName;
            marked = inputMarked;
            id = Guid.NewGuid();
        }

        public Node(AbstractState state)
        {
            position = new Vector2D();
            displacement = new Vector2D();
            name = state.ToString();
            marked = state.IsMarked;
            id = Guid.NewGuid();
        }

        public static void InitialPositioning(List<Node> nodes)
        {
            float initialRadius = 10.0f;
            float stepAngle = (float)(2 * Math.PI / nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].position = new Vector2D(initialRadius * (float)Math.Cos(i * stepAngle), initialRadius * (float)Math.Sin(i * stepAngle));
            }
        }

        public static void ResetDisplacement(List<Node> nodes)
        {
            foreach (Node node in nodes)
                node.displacement.Reset();
        }

        public static void InteractNodes(List<Node> nodes, PhyD phyD)
        {

        }

        public static float DisplaceNodes(List<Node> nodes)
        {
            return 0;
        }

        public string ToSvg(DrawingDir drawingDir, SvgCanvas canvas)
        {
            string svg = "";

            Vector2D svgPosition = canvas.ConvertVector(position);

            svg += "<circle cx=\"" + svgPosition.x + "\" cy=\"" + svgPosition.y + "\" r=\"" + drawingDir.GetNodeRadius() + "\" stroke=\"" + properties.strokeColor + "\" stroke-width=\"" + properties.strokeWidth + "\" fill=\"" + properties.strokeFill + "\" />" + Environment.NewLine;
            string circle_marked_svg = "";

            if (is_marked)
            {
                circle_marked_svg = "<circle cx=\"" + circle_position.x + "\" cy=\"" + circle_position.y + "\" r=\"" + properties.stateRadius * properties.markedRatio + "\" stroke=\"" + properties.strokeColor + "\" stroke-width=\"" + properties.strokeWidth + "\" fill=\"" + properties.strokeFill + "\" />" + Environment.NewLine;
            }

            string circle_text_svg = "<text x=\"" + circle_position.x + "\" y=\"" + circle_position.y + "\" dominant-baseline=\"central\" font-size=\"" + properties.textSize + "\"em fill=\"" + properties.textColor + "\" text-anchor=\"middle\">" + input_particle.name + "</text>" + Environment.NewLine;
            return circle_svg + circle_marked_svg + circle_text_svg;

            return svg;
        }

        public Box GetBox(DrawingDir drawingDir)
        {

        }
    }
}

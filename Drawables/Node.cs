﻿using System;
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
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    nodes[i].InteractNode(nodes[j], phyD);
                }
            }
        }

        void InteractNode(Node nodeB, PhyD phyD)
        {
            Vector2D normal = (nodeB.position - this.position).Normalized();
            float nodesDistance = (nodeB.position - this.position).Length();

            if (nodesDistance < 1e-6)
            {
                nodesDistance = 1e-6f;
            }

            float force = (1 - phyD.attenuation) * phyD.repulsion / nodesDistance;

            nodeB.displacement += force * normal;
            this.displacement -= force * normal;
        }

        public static float DisplaceNodes(List<Node> nodes)
        {
            float maximumDisplacement = float.MinValue;
            foreach (Node node in nodes)
            {
                float currentDisplacement = node.displacement.Length();

                if (currentDisplacement > maximumDisplacement)
                    maximumDisplacement = currentDisplacement;

                node.Displace();
            }

            return maximumDisplacement;
        }

        void Displace(Vector2D displacement)
        {
            this.position += displacement;
        }

        void Displace()
        {
            this.position += displacement;
        }

        public string ToSvg(DrawingDir drawingDir, SvgCanvas canvas)
        {
            string svg = "";

            Vector2D svgPosition = canvas.ToSvgCoordinates(position);

            svg += "<circle cx=\"" + svgPosition.x + "\" cy=\"" + svgPosition.y + "\" r=\"" + drawingDir.nodeRadius + "\" stroke=\"" + drawingDir.strokeColor + "\" stroke-width=\"" + drawingDir.borderWidth + "\" fill=\"" + drawingDir.strokeFill + "\" />" + Environment.NewLine;

            if (marked)
            {
                svg += "<circle cx=\"" + svgPosition.x + "\" cy=\"" + svgPosition.y + "\" r=\"" + drawingDir.nodeRadius * drawingDir.markedRatio + "\" stroke=\"" + drawingDir.strokeColor + "\" stroke-width=\"" + drawingDir.borderWidth + "\" fill=\"" + drawingDir.strokeFill + "\" />" + Environment.NewLine;
            }

            svg += "<text x=\"" + svgPosition.x + "\" y=\"" + svgPosition.y + "\" dominant-baseline=\"central\" font-size=\"" + drawingDir.textSize + "\"em fill=\"" + drawingDir.textColor + "\" text-anchor=\"middle\">" + name + "</text>" + Environment.NewLine;

            return svg;
        }

        public Box GetBox(DrawingDir drawingDir)
        {
            float radius = drawingDir.TotalRadius();
            Vector2D topLeft = new Vector2D(position.x - radius, position.y + radius);
            Vector2D bottomRight = new Vector2D(position.x + radius, position.y - radius);

            return new Box(topLeft, bottomRight);
        }
    }
}

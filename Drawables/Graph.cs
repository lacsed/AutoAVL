using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AutoAVL.Settings;
using UltraDES;

namespace AutoAVL.Drawables
{
    public class Graph
    {
        public List<Node>? graphNodes;
        public List<Link>? graphLinks;

        public PhyD phyD;
        public DrawingDir drawingDir;

        public bool firstSimulation;

        public Graph(Automaton automaton)
        {
            graphNodes = new List<Node>();
            graphLinks = new List<Link>();

            foreach (AbstractState state in automaton.States())
            {
                graphNodes.Add(new Node(state));
            }

            foreach (Transition transition in automaton.Transitions())
            {
                graphLinks.Add(new Link(graphNodes.Find(x => x.name == transition.Origin.ToString()),
                    graphNodes.Find(x => x.name == transition.Destination.ToString()), transition.ToString()));
            }
        }

        public void Simulate()
        {
            if (firstSimulation)
            {
                Node.InitialPositioning(graphNodes);
                firstSimulation = false;
            }

            float maxDisplacement = float.MaxValue;

            do
            {
                Node.ResetDisplacement(graphNodes);
                Node.InteractNodes(graphNodes, phyD);
                Link.PullLinks(graphLinks, phyD);

                maxDisplacement = Node.DisplaceNodes(graphNodes);

            } while (maxDisplacement > phyD.stopCondition);
        }

        public string ToSvg()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string svgImage = "";

            drawingDir.SetCanvasDimensions(this.GetCanvasLimits());

            foreach (Node node in graphNodes)
                svgImage += node.ToSvg(drawingDir);

            return "";
        }

        public (Vector2D topLeft, Vector2D bottomRight) GetCanvasLimits()
        {
            Vector2D topLeft = new Vector2D();
            Vector2D bottomRight = new Vector2D();


            foreach (Drawable drawable in graphNodes)

            foreach (Node node in graphNodes)
            {
                topLeft = Vector2D.GetMinValues(topLeft, node.position - new Vector2D(drawingDir.TotalRadius(), drawingDir.TotalRadius()));
                bottomRight = Vector2D.GetMaxValues(topLeft, node.position + new Vector2D(drawingDir.TotalRadius(), drawingDir.TotalRadius()));
            }

            foreach (Link link in graphLinks)
            {
                topLeft = Vector2D.GetMinValues(topLeft, link.GetExtremes() - new Vector2D(drawingDir.TotalRadius(), drawingDir.TotalRadius()));
                bottomRight = Vector2D.GetMaxValues(topLeft, link.GetExtremes() + new Vector2D(drawingDir.TotalRadius(), drawingDir.TotalRadius()));
            }

            return (topLeft, bottomRight);
        }
    }
}


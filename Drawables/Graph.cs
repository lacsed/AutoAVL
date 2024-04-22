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

            phyD = new PhyD();
            drawingDir = new DrawingDir();

            firstSimulation = true;

            foreach (AbstractState state in automaton.States())
            {
                graphNodes.Add(new Node(state));
            }

            foreach (Transition transition in automaton.Transitions())
            {
                graphLinks.Add(new Link(graphNodes.Find(x => x.name == transition.Origin.ToString()),
                    graphNodes.Find(x => x.name == transition.Destination.ToString()), transition.Trigger.ToString()));
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

            Link.SetUp(graphLinks, drawingDir);
        }

        public string ToSvg()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string svgImage = "";
            SvgCanvas canvas = new SvgCanvas();

            canvas.SetUpCanvas(this.GetCanvasLimits());

            foreach (Drawable drawable in graphNodes.Concat<Drawable>(graphLinks).ToList())
                svgImage += drawable.ToSvg(drawingDir, canvas);

            string svgDimensions = canvas.SvgDimensions();
            string svgSettings = canvas.SvgSettings();

            return svgDimensions + svgSettings + svgImage;
        }

        public Box GetCanvasLimits()
        {
            Box canvasBox = new Box();

            foreach (Drawable drawable in graphNodes.Concat<Drawable>(graphLinks).ToList())
            {
                canvasBox = Box.EncompassingBox(canvasBox, drawable.GetBox(drawingDir));
            }

            return canvasBox;
        }

        public void SaveSVG(string path, string fileName)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            StreamWriter sw = File.CreateText(path + "\\" + fileName + ".html");
            sw.Write(this.ToSvg());
            sw.Close();
        }
    }
}


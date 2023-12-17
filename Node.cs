using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraDES;

namespace AutoAVL
{
    public class Node
    {
        public Vector2D position;
        public Vector2D displacement;

        public string name;
        public Vector2D nameOffset;

        public bool marked;

        public Guid id;

        public Node()
        {
            position = new Vector2D();
            displacement = new Vector2D();
            name = "";
            nameOffset = new Vector2D();
            marked = false;
            id = Guid.NewGuid();
        }

        public Node(string input_name, bool isMarked)
        {
            position = new Vector2D();
            displacement = new Vector2D();
            name = input_name;
            nameOffset = new Vector2D();
            marked = isMarked;
            id = Guid.NewGuid();
        }

        public Node(Vector2D inputPosition, string inputName, bool inputMarked)
        {
            position = inputPosition;
            displacement = new Vector2D();
            name = inputName;
            nameOffset = new Vector2D();
            marked = inputMarked;
            id = Guid.NewGuid();
        }

        public Node(AbstractState state)
        {
            position = new Vector2D();
            displacement = new Vector2D();
            name = state.ToString();
            nameOffset = new Vector2D();
            marked = state.IsMarked;
            id = Guid.NewGuid();
        }

        public static void InitialPositioning(List<Node> nodes)
        {
            float initialRadius = 10.0f;
            float stepAngle = (float) (2 * Math.PI / nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].position = new Vector2D(initialRadius * (float) Math.Cos(i * stepAngle), initialRadius * (float) Math.Sin(i * stepAngle));
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
    }
}

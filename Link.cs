using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAVL
{
    public class Link
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
            this.start = origin;
            this.end = destination;
            this.name = name;
            this.guid = Guid.NewGuid();
            this.isAutoLink = (origin == destination) ? true : false;
        }

        public static void PullLinks(List<Link> links, PhyD phyD) 
        {
            
        }

        public Vector2D GetExtremes()
        {
            
        }
    }
}

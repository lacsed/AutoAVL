using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoVL
{
    public class Node
    {
        public Vector2D position;

        public string name;
        public Vector2D nameOffset;

        public bool marked;

        public Guid id;

        public Node()
        {
            position = new Vector2D();
            name = "";
            nameOffset = new Vector2D();
            marked = false;
            id = Guid.NewGuid();
        }

        public Node(string input_name, bool isMarked)
        {
            position = new Vector2D();
            name = input_name;
            nameOffset = new Vector2D();
            marked = isMarked;
            id = Guid.NewGuid();
        }

        public Node(Vector2D inputPosition, string inputName, bool inputMarked)
        {
            position = inputPosition;
            name = inputName;
            nameOffset = new Vector2D();
            marked = inputMarked;
            id = Guid.NewGuid();
        }
    }
}

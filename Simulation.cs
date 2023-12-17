using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAVL
{
    public class Simulation
    {

        public static Graph Simulate(Automaton automaton)
        {
            Graph graph = new Graph(automaton);
            Simulation.Simulate(graph);
            return graph;
        }

        public static void Simulate(Graph graph)
        {

        }
    }
}

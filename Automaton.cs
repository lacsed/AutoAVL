using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraDES;

namespace AutoAVL
{
    public class Automaton
    {
        private DeterministicFiniteAutomaton? dfa;
        private NondeterministicFiniteAutomaton? ndfa;

        public Automaton(DeterministicFiniteAutomaton automaton)
        {
            this.dfa = automaton;
        }

        public Automaton(NondeterministicFiniteAutomaton automaton)
        {
            this.ndfa = automaton;
        }

        public List<AbstractState> States()
        {
            return (dfa == null) ? ndfa.States.ToList() : dfa.States.ToList();
        }

        public List<Transition> Transitions()
        {
            return (dfa == null) ? ndfa.Transitions.ToList() : dfa.Transitions.ToList();
        }

        public string InitialState()
        {
            return (dfa == null) ? ndfa.InitialState.ToString() : dfa.InitialState.ToString();
        }
    }
}

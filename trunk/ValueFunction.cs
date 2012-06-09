using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MarkovDecisionProcess
{
    class ValueFunction : Policy
    {

        //more data structures here
        private Domain m_dDomain;
        private Dictionary<State, Action> bestActions;
        private Dictionary<State, Double> V;
        public double MaxValue { get; private set; }
        public double MinValue { get; private set; }

        public ValueFunction(Domain d)
        {            
            m_dDomain = d;
            MaxValue = 0.0;
            MinValue = 0.0;
            bestActions = new Dictionary<State, Action>();
            V = new Dictionary<State, Double>();
        }

        public double ValueAt(State s)
        {            
            return V[s];
        }


        public override Action GetAction(State s)
        {
            return bestActions[s];
        }

        public void ValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {
            double gamma = 0.1;
            Debug.WriteLine("Starting value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;            
                                    
            foreach (State s in m_dDomain.States)             
                V[s] = 0;           

            do
            {
                foreach (State s in m_dDomain.States)
                {
                    cUpdates++;
                    Action maxAction=null;
                    double maxQsa = Double.MinValue;
                    foreach (Action a in m_dDomain.Actions)
                    {
                        double qsa = s.Reward(a);
                        double sig = 0.0;
                        foreach (State sTag in s.Successors(a))
                        {
                            sig += s.TransitionProbability(a, sTag) * V[sTag];
                        }
                        qsa += gamma * sig;
                        if(Math.Abs(qsa-V[s]) > maxQsa)
                        {
                            maxQsa = Math.Abs(qsa - V[s]);
                            maxAction = a;
                        }
                    }
                    V[s] = maxQsa;
                    if (maxQsa > MaxValue)
                        MaxValue = maxQsa;
                    bestActions[s]=maxAction;
                }
            }
            while (MaxValue < dEpsilon);


            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished value iteration");
        }

        public void RealTimeDynamicProgramming(int cTrials, out int cUpdates, out TimeSpan tsExecutionTime)
        {
            Debug.WriteLine("Starting RTDP");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;

            //your code here
            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished RTDP");
        }

        public void PrioritizedValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {
            Debug.WriteLine("Starting prioritized value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;
            //your code here
            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished prioritized value iteration");
        }

    }
}

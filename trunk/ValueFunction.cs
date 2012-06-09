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
        private Dictionary<State, List<State>> preds;
        public double MaxValue { get; private set; }
        public double MinValue { get; private set; }
        public double gamma { get; private set; }

        public ValueFunction(Domain d)
        {            
            m_dDomain = d;
            gamma = 0.1;
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

        public double update(State s)
        {
            Action maxAction = null;
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
                if (Math.Abs(qsa - V[s]) > maxQsa)
                {
                    maxQsa = Math.Abs(qsa - V[s]);
                    maxAction = a;
                }
            }
            V[s] = maxQsa;
            if (maxQsa > MaxValue)
                MaxValue = maxQsa;
            bestActions[s] = maxAction;
            return MaxValue;
        }

        public void ValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {            
            Debug.WriteLine("Starting value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;

            MaxValue = 0.0;
            foreach (State s in m_dDomain.States)
            {
                V[s] = 0;
                bestActions[s] = null;
            }
            do
            {
                foreach (State s in m_dDomain.States)
                {
                    cUpdates++;
                    MaxValue = update(s);
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

        public Dictionary<State, List<State>> initPreds()
        {
            Dictionary<State, List<State>> ans = new Dictionary<State, List<State>>();
            foreach (State s in m_dDomain.States)
            {
                ans[s] = new List<State>();
            }

            foreach(State s in m_dDomain.States)
            {
                foreach (Action a in m_dDomain.Actions)
                {
                    foreach (State sTag in s.Successors(a))
                    {
                        ans[sTag].Add(s);                        
                    }
                }
            }

            return ans;
        }

        public void PrioritizedValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {
            Debug.WriteLine("Starting prioritized value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;

            MaxValue = 0.0;
            preds = initPreds();
            PriorityQueue<State, double> pq = new PriorityQueue<State, double>();
            foreach(State s in m_dDomain.States)
            {
                double maxReward=0.0;
                foreach(Action a in m_dDomain.Actions)
                {
                    if(maxReward<s.Reward(a))
                    {
                        maxReward=s.Reward(a);
                    }
                }
                pq.Enqueue(s,maxReward);
            }

            while (pq.topPriority()>dEpsilon)
            {
                State s = pq.Dequeue();
                MaxValue = update(s);
                foreach (State sTag in preds[s])
                { 

                }
            }


            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished prioritized value iteration");
        }

    }
}

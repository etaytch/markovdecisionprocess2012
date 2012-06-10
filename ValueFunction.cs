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

        public double update(State s)
        {            
            Action maxAction = null;
            double ans=0.0;
            double maxQsa = Double.MinValue;
            foreach (Action a in m_dDomain.Actions)
            {
                double qsa = s.Reward(a);                    
                double sig = 0.0;
                foreach (State sTag in s.Successors(a))
                {
                    sig += s.TransitionProbability(a, sTag) * V[sTag];
                }
                qsa += m_dDomain.gamma * sig;
                if (qsa > maxQsa)
                {
                    maxQsa = qsa;
                    maxAction = a;
                }
            }            
            bestActions[s] = maxAction;
            ans = Math.Abs(maxQsa - V[s]);
            V[s] = maxQsa;              
            return ans;
        }

        public void ValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {            
            Debug.WriteLine("Starting value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;
            
            bestActions = new Dictionary<State, Action>();
            V = new Dictionary<State, Double>();
            
            foreach (State s in m_dDomain.States)
            {
                V[s] = 0.0;
                bestActions[s] = null;
            }
            do
            {
                MaxValue = Double.MinValue;
                foreach (State s in m_dDomain.States)
                {
                    cUpdates++;
                    double deltaS = update(s);
                    if (deltaS > MaxValue)
                    {
                        MaxValue = deltaS;
                    }
                }
            }
            while (MaxValue > dEpsilon);


            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished value iteration");
        }

        public double getMaxGlobalReward()
        {
            double reward = double.MinValue;
            foreach (State s in m_dDomain.States)
            {
                foreach (Action a in m_dDomain.Actions)
                {
                    double currentReward = s.Reward(a);
                    if (reward < currentReward)
                        reward = currentReward;
                }
            }
            return reward;
        }

        public void RealTimeDynamicProgramming(int cTrials, out int cUpdates, out TimeSpan tsExecutionTime)
        {
            Debug.WriteLine("Starting RTDP");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;

            bestActions = new Dictionary<State, Action>();
            V = new Dictionary<State, Double>();
            MaxValue = 0.0;
            double globalReward = getMaxGlobalReward();
            foreach (State st in m_dDomain.States)
            {
                V[st] = globalReward;// 0.0;
                bestActions[st] = null;
            }

            for (int i = 0; i < cTrials ;i++ )
            {
                State s = m_dDomain.StartState;
                Stack<State> stack = new Stack<State>();
                while (!m_dDomain.IsGoalState(s))
                {
                    cUpdates++;
                    update(s);                    
                    Action optimalAction = bestActions[s];
                    stack.Push(s);                    
                    s = s.Apply(optimalAction);
                }
                while (stack.Count > 0)
                {
                    cUpdates++;
                    s = stack.Pop();
                    update(s);
                }
            }            
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
                        if(!ans[sTag].Contains(s))
                            ans[sTag].Add(s);                        
                    }
                }
            }

            return ans;
        }

        public void PrioeritizedValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {
            Debug.WriteLine("Starting prioritized value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;

            MaxValue = 0.0;                        
            bestActions = new Dictionary<State, Action>();
            V = new Dictionary<State, Double>();
            foreach (State s in m_dDomain.States)
            {
                V[s] = 0.0;
                bestActions[s] = null;
            }

            preds = initPreds();            
            Heap<State> pq = new Heap<State>();
            foreach(State s in m_dDomain.States)
            {
                double maxReward=Double.MinValue;
                foreach(Action a in m_dDomain.Actions)
                {
                    double immediateReward = s.Reward(a);                    
                    if (maxReward < immediateReward)
                    {
                        maxReward = immediateReward;
                    }
                }                
                pq.Insert(s, maxReward);
            }
            double maxPriority = pq.GetMaxPriority();
            while (maxPriority > dEpsilon)
            {
                State s = pq.ExtractMax();
                cUpdates++;
                MaxValue = update(s);
                foreach (State sTag in preds[s])
                {
                    pq.IncreasePriority(sTag, m_dDomain.gamma * MaxValue);
                }
                maxPriority = pq.GetMaxPriority();
            }
            
            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished prioritized value iteration");
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MarkovDecisionProcess
{
    public class QSAPair<T, U> {
    public QSAPair() {
    }

    public QSAPair(T first, U second) {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
    };

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
            gamma = 0.5;
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

        public QSAPair<double,Action> update(State s, bool useGlobal = false, double globalReward = 0)
        {
            QSAPair<double, Action> ans = new QSAPair<double, Action>();
            Action maxAction = null;
            double maxQsa = Double.MinValue;
            foreach (Action a in m_dDomain.Actions)
            {

                double qsa;// = s.Reward(a);
                if (useGlobal)
                    qsa = globalReward;
                else
                    qsa = s.Reward(a);
                double sig = 0.0;
                foreach (State sTag in s.Successors(a))
                {
                    sig += s.TransitionProbability(a, sTag) * V[sTag];
                }
                qsa += gamma * sig;
                if (Math.Abs(qsa - V[s]) >= maxQsa)
                {
                    maxQsa = Math.Abs(qsa - V[s]);
                    maxAction = a;
                }
            }
            V[s] = maxQsa;
            //if (maxQsa > MaxValue)
            //    MaxValue = maxQsa;
            bestActions[s] = maxAction;
            //return MaxValue;
            ans.First = maxQsa;
            ans.Second = maxAction;
            return ans;
        }

        public void ValueIteration(double dEpsilon, out int cUpdates, out TimeSpan tsExecutionTime)
        {            
            Debug.WriteLine("Starting value iteration");
            DateTime dtBefore = DateTime.Now;
            cUpdates = 0;
            
            bestActions = new Dictionary<State, Action>();
            V = new Dictionary<State, Double>();
            MaxValue = 0.0;
            foreach (State s in m_dDomain.States)
            {
                V[s] = 0.0;
                bestActions[s] = null;
            }
            do
            {
                foreach (State s in m_dDomain.States)
                {
                    cUpdates++;
                    double deltaS = update(s).First;
                    if (MaxValue < deltaS)
                    {
                        MaxValue = deltaS;
                    }
                }
            }
            while (MaxValue < dEpsilon);


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
            foreach (State st in m_dDomain.States)
            {
                V[st] = 0.0;
                bestActions[st] = null;
            }

            double globalReward = getMaxGlobalReward();
            State s = m_dDomain.StartState;
            Stack<State> stack = new Stack<State>();
            while(!m_dDomain.IsGoalState(s))
            {
                Action optimalAction = update(s/*, true, globalReward*/).Second;
                stack.Push(s);
                s = s.Apply(optimalAction);
            }
            while(stack.Count>0)
            {
                s = stack.Pop();
                update(s);
            }

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
            //PriorityQueue<State, double> pq = new PriorityQueue<State, double>();
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
                //pq.Enqueue(s,maxReward);
                pq.Insert(s, maxReward);
            }
            double maxPriority = pq.GetMaxPriority();
            while (maxPriority > dEpsilon)
            {
                State s = pq.ExtractMax();
                MaxValue = update(s).First;
                foreach (State sTag in preds[s])
                {
                    pq.IncreasePriority(sTag, gamma*MaxValue);
                }
                maxPriority = pq.GetMaxPriority();
            }

            Dictionary<State, Double> vals = getVValues();
            tsExecutionTime = DateTime.Now - dtBefore;
            Debug.WriteLine("\nFinished prioritized value iteration");
        }

        public Dictionary<State, Double> getVValues()
        {        
            Dictionary<State, Double> ans = new Dictionary<State, Double>();
            foreach (State s in V.Keys)
            {
                if (V[s] != 0.0)
                    ans.Add(s, V[s]);
            }
            return ans;
        }

    }
}

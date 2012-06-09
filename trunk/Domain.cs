using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MarkovDecisionProcess
{
    abstract class Domain
    {
        public abstract IEnumerable<State> States { get; }
        public abstract IEnumerable<Action> Actions { get; }
        public abstract double MaxReward { get; }
        public abstract State StartState{ get; }
        public abstract bool IsGoalState(State s);
        public double DiscountFactor { get; protected set; }
        public double ComputeAverageDiscountedReward(Policy p, int cTrials, int cStepsPerTrial)
        {
            Debug.WriteLine("Started computing ADR");
            double dSumRewards = 0.0;
            double gamma = 0.1;
            double ARD = 0.0;
            for (int j = 0; j < cTrials; j++ )
            {
                State s = StartState;
                double r = 0;
                int i = 0;
                int stepCounter=0;
                while(!IsGoalState(s) && (stepCounter<cStepsPerTrial))
                {
                    Action a = p.GetAction(s);
                    r += Math.Pow(gamma, i) * s.Reward(a);
                    i++;
                    /*
                    Random rand = new Random();
                    int count=0;
                    foreach(State succ in s.Successors(a))
                        count++;
                    int position = rand.Next(0, count);
                    s = s.Successors(a).ElementAt(position);
                     */
                    s=s.Apply(a);
                }
                for (int k = 0; k < i;k++ )
                {
                    ARD+=(Math.Pow(gamma,i)*r);
                }
            }
            ARD = (1/cTrials)*ARD;

            Debug.WriteLine("\nDone computing ADR");
            return dSumRewards;
        }
    }
}

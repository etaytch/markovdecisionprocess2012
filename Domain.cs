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
        public double gamma = 0.99;

        public double ComputeAverageDiscountedReward(Policy p, int cTrials, int cStepsPerTrial)

        {
            Debug.WriteLine("Started computing ADR");
            double dSumRewards = 0.0;            
            double ARD = 0.0;
            for (int j = 0; j < cTrials; j++ )
            {
                State s = StartState;
                double r = 0;
                int i = 0;                
                while(!IsGoalState(s) && (i<cStepsPerTrial))
                {
                    Action a = p.GetAction(s);
                    if (a == null)
                        break;
                    r += Math.Pow(gamma, i) * s.Reward(a);
                    i++;
                    
                    s=s.Apply(a);
                    if (s == null)
                        break;
                }                
                ARD+=r;                
            }
            dSumRewards = (ARD / cTrials);

            Debug.WriteLine("\nDone computing ADR");
            return dSumRewards;
        }            
    }


}

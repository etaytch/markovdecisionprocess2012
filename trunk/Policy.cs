using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkovDecisionProcess
{
    abstract class Policy
    {
        public abstract Action GetAction(State s);
    }
}

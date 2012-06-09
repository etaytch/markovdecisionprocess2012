using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkovDecisionProcess
{
    class VelocityAction : Action
    {
        public int XIncrease;
        public int YIncrease;
        public VelocityAction(int iXIncrease, int iYIncrease)
        {
            XIncrease = iXIncrease;
            YIncrease = iYIncrease;
        }
        public override bool Equals(object obj)
        {
            if (obj is VelocityAction)
            {
                VelocityAction va = (VelocityAction)obj;
                return va.XIncrease == XIncrease && va.YIncrease == YIncrease;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (XIncrease + 1) * 3 + YIncrease + 1;
        }
        public override string ToString()
        {
            return "<" + XIncrease + "," + YIncrease + ">";
        }
    }
}

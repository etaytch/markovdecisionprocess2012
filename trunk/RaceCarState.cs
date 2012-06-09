using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkovDecisionProcess
{
    class RaceCarState : State
    {
        public int X{get; private set;}
        public int Y{get; private set;}
        public double Velocity { get { return Math.Sqrt(m_iXVelocity * m_iXVelocity + m_iYVelocity * m_iYVelocity); } }
        private int m_iXVelocity, m_iYVelocity;
        private RaceTrack m_rtTrack;

        public RaceCarState(int iX, int iY, int iXVelocity, int iYVelocity, RaceTrack rt)
        {
            X = iX;
            Y = iY;
            m_iXVelocity = iXVelocity;
            m_iYVelocity = iYVelocity;
            m_rtTrack = rt;
        }

        public RaceCarState(int iX, int iY, RaceTrack rt)
            : this(iX, iY, 0, 0, rt)
        {
        }

        public RaceCarState(RaceCarState s)
            : this(s.X, s.Y, s.m_iXVelocity, s.m_iYVelocity, s.m_rtTrack)
        {
        }

        public override IEnumerable<State> Successors(Action a)
        {
            RaceCarState sTagNoApply = new RaceCarState(this);
            RaceCarState sTagApply = new RaceCarState(this);
            sTagApply.Apply((VelocityAction)a, true);
            sTagNoApply.Apply((VelocityAction)a, false);
            yield return sTagApply;
            yield return sTagNoApply;
        }

        private bool Apply(VelocityAction a, bool bApply)
        {
            if (bApply)
            {
                m_iXVelocity += a.XIncrease;
                if (m_iXVelocity > RaceTrack.MAX_VELOCITY)
                    m_iXVelocity = RaceTrack.MAX_VELOCITY;
                if (-m_iXVelocity > RaceTrack.MAX_VELOCITY)
                    m_iXVelocity = -RaceTrack.MAX_VELOCITY;
                m_iYVelocity += a.YIncrease;
                if (m_iYVelocity > RaceTrack.MAX_VELOCITY)
                    m_iYVelocity = RaceTrack.MAX_VELOCITY;
                if (-m_iYVelocity > RaceTrack.MAX_VELOCITY)
                    m_iYVelocity = -RaceTrack.MAX_VELOCITY;
            }
            int iFinalX, iFinalY;
            bool bNoCollision = m_rtTrack.Move(X, Y, m_iXVelocity, m_iYVelocity, out iFinalX, out iFinalY);
            X = iFinalX;
            Y = iFinalY;
            return bNoCollision;
        }

        public override State Apply(Action a)
        {
            RaceCarState sTag = new RaceCarState(this);
            sTag.Apply((VelocityAction)a, RandomGenerator.NextDouble() < RaceTrack.ACTION_SUCCESS_PROBABILITY);
            return sTag;
        }
        public override bool Equals(object obj)
        {
            if (obj is RaceCarState)
                return Equals((RaceCarState)obj);
            return false;
        }
        public bool Equals(RaceCarState s)
        {
            if (s.X != X)
                return false;
            if (s.Y != Y)
                return false;
            if (s.m_iXVelocity != m_iXVelocity)
                return false;
            if (s.m_iYVelocity != m_iYVelocity)
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            int iCode = 0;
            iCode += X;
            iCode *= m_rtTrack.Height;
            iCode += Y;
            iCode *= RaceTrack.MAX_VELOCITY * 2 + 1;
            iCode += m_iXVelocity;
            iCode *= RaceTrack.MAX_VELOCITY * 2 + 1;
            iCode += m_iYVelocity;
            return iCode;
        }

        public override double TransitionProbability(Action a, State sTag)
        {
            RaceCarState sTagApply = new RaceCarState(this);
            sTagApply.Apply((VelocityAction)a, true);
            if (sTag.Equals(sTagApply))
                return RaceTrack.ACTION_SUCCESS_PROBABILITY;
            RaceCarState sTagNoApply = new RaceCarState(this);
            sTagNoApply.Apply((VelocityAction)a, false);
            if (sTag.Equals(sTagNoApply))
                return 1 - RaceTrack.ACTION_SUCCESS_PROBABILITY;
            return 0.0;
        }



        public override double Reward(Action a)
        {
            if (m_rtTrack.IsRaceEnd(this))
                return m_rtTrack.MaxReward;
            if (m_rtTrack.IsGoalState(this))
                return 0.0;
            RaceCarState sTagApply = new RaceCarState(this);
            bool bNoCollision = sTagApply.Apply((VelocityAction)a, true);
            if (bNoCollision)
                return -0.01;
            else
                return -sTagApply.Velocity;
        }

        public override string ToString()
        {
            return "[" + X + "," + Y + "] <" + m_iXVelocity + "," + m_iYVelocity + ">";
        }
    }
}

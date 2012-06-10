using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace MarkovDecisionProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fs = new FileStream("Debug.txt", FileMode.Create);
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.Listeners.Add(new TextWriterTraceListener(fs));

            RaceTrack rc = new RaceTrack("RaceTrack1.bmp");//change here to a different race BMP: 2 is very small, 3 is very large
            RandomPolicy p = new RandomPolicy(rc);
            RaceViewer form = new RaceViewer(rc);
            form.Start();
            //rc.DrawRace(p, form);

            ValueFunction vi1 = new ValueFunction(rc);
            ValueFunction vi2 = new ValueFunction(rc);
            ValueFunction vi3 = new ValueFunction(rc);
            int cUpdates1 = 0, cUpdates2 = 0, cUpdates3 = 0;
            TimeSpan ts1, ts2, ts3;

            form.StateValues = vi3;
            vi3.RealTimeDynamicProgramming(100, out cUpdates3, out ts3);
            
            //form.StateValues = vi1;
            //vi1.ValueIteration(0.5, out cUpdates1, out ts1);

            //form.StateValues = vi2;
            //vi2.PrioeritizedValueIteration(0.5, out cUpdates2, out ts2);            
            rc.DrawRace(vi3, form);

           // double dADR1 = rc.ComputeAverageDiscountedReward(vi1, 1000, 100);
            //double dADR2 = rc.ComputeAverageDiscountedReward(vi2, 1000, 100);
           // double dADR3 = rc.ComputeAverageDiscountedReward(vi3, 1000, 100);

            Debug.Close();
        }
    }
}

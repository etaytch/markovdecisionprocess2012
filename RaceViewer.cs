using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MarkovDecisionProcess
{
    public partial class RaceViewer : Form
    {
        private RaceTrack m_rtTrack;
        internal RaceCarState CarState;
        internal RaceCarState CurretnUpdateState;
        private int SCALE = 10;
        internal delegate void RefreshDelegate();
        internal RefreshDelegate RefreshForm;
        internal delegate void HideDelegate();
        internal HideDelegate HideForm;
        internal ValueFunction StateValues;
        private bool m_bThreadRunning;
        public bool Active { get; private set; }

        internal RaceViewer(RaceTrack rt)
        {
            InitializeComponent();
            m_rtTrack = rt;
            Size = new Size(m_rtTrack.Width * SCALE + 50, 100 + m_rtTrack.Height * SCALE);
            TrackPictureBox.Size = new Size(m_rtTrack.Width * SCALE, m_rtTrack.Height * SCALE);
            RefreshForm = new RefreshDelegate(RefreshFormMethod);
            HideForm = new HideDelegate(HideFormMethod);
            CarState = null;
            StateValues = null;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
            Active = false;
            m_bThreadRunning = false;
            CurretnUpdateState = null;
        }

        private void RaceViewer_Paint(object sender, PaintEventArgs e)
        {
        }

        private void DrawValueFunction(Graphics g)
        {
            Pen p = new Pen(Color.Black, 1);

            double[,] aSquareValues = new double[m_rtTrack.Width, m_rtTrack.Height];
            for (int x = 0; x < m_rtTrack.Width; x++)
                for (int y = 0; y < m_rtTrack.Height; y++)
                    aSquareValues[x, y] = double.NegativeInfinity;
            foreach (RaceCarState s in m_rtTrack.States)
            {
                Color c = Color.White;
                double dStateValue = StateValues.ValueAt(s);
                if (s.X < 0 || s.Y < 0) // the goal state
                    continue;
                if (dStateValue >= aSquareValues[s.X, s.Y])
                {
                    aSquareValues[s.X, s.Y] = dStateValue;
                    int iIntensity = 0;
                    if (dStateValue < 0)
                    {
                        iIntensity = (int)(255 * dStateValue / StateValues.MinValue);
                        if (iIntensity > 255)
                            iIntensity = 255;
                        c = Color.FromArgb(iIntensity, 0, 0);
                    }
                    else
                    {
                        iIntensity = (int)(255 * dStateValue / StateValues.MaxValue);
                        if (iIntensity > 255)
                            iIntensity = 255;
                        if (iIntensity < 0)
                            iIntensity = 0;
                        c = Color.FromArgb(0, iIntensity, 0);
                    }
                    Brush b = new SolidBrush(c);
                    g.FillRectangle(b, s.X * SCALE, s.Y * SCALE, SCALE, SCALE);
                }
            }
            if(CurretnUpdateState != null)
                g.FillRectangle(Brushes.Red, CurretnUpdateState.X * SCALE, CurretnUpdateState.Y * SCALE, SCALE, SCALE);
        }

        private void DrawRace(Graphics g)
        {
            Pen p = new Pen(Color.Black, 1);
            int iX = 0, iY = 0;
            for (iX = 0; iX < Width; iX++)
            {
                for (iY = 0; iY < Height; iY++)
                {
                    if (CarState != null && iX == CarState.X && iY == CarState.Y)
                        g.FillRectangle(Brushes.Yellow, iX * SCALE, iY * SCALE, SCALE, SCALE);
                    else if (m_rtTrack.IsRaceStart(iX, iY))
                        g.FillRectangle(Brushes.Red, iX * SCALE, iY * SCALE, SCALE, SCALE);
                    else if (m_rtTrack.IsRaceEnd(iX, iY))
                        g.FillRectangle(Brushes.Green, iX * SCALE, iY * SCALE, SCALE, SCALE);
                    else if (m_rtTrack.ValidPoint(iX, iY))
                    {
                        g.FillRectangle(Brushes.Black, iX * SCALE, iY * SCALE, SCALE, SCALE);
                    }
                }
            }
            Text = CarState.ToString(); 
        }

        internal void RefreshFormMethod()
        {
            //Update();
            Refresh();
            Application.DoEvents();

        }

        public bool Start()
        {
            if (m_bThreadRunning)
                return false;
            m_bThreadRunning = true;
            Active = true;
            Thread t = new Thread(Run);
            t.Start();
            return true;
        }

        internal void Run()
        {
            //ShowDialog();
            Show();
            while (Active)
            {
                TrackPictureBox.Invalidate();
                Application.DoEvents();
                Thread.Sleep(10);
            }
        }
        internal void HideFormMethod()
        {
            Visible = false;
            Hide();
        }

        private void TrackPictureBox_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                if (StateValues == null)
                    DrawRace(g);
                else
                    DrawValueFunction(g);
            }
            catch (Exception ex)
            {
            }
        }

        private void RaceViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Active = false;
        }

    }
}

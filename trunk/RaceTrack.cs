using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.ComponentModel;

namespace MarkovDecisionProcess
{
    class RaceTrack : Domain
    {
        private bool[,] m_aMap;
        public int Width{get; private set;}
        public int Height { get; private set; }
        private int m_iStartX, m_iStartY, m_iEndX, m_iEndY;
        public override double MaxReward
        {
            get { return 10; }
        }
        public static int MAX_VELOCITY = 5;
        public static double ACTION_SUCCESS_PROBABILITY = 0.5;
        public override IEnumerable<State> States { get { return GetStates(); } }
        public override IEnumerable<Action> Actions { get { return GetActions(); } }

        public RaceTrack(string sBMPFileName)
        {
            LoadMap(sBMPFileName);
            DiscountFactor = 0.99;
        }

        public void PrintTrack()
        {
            Debug.WriteLine("");
            string sLine = "";
            int x = 0, y = 0;
            for (y = 0; y < Height; y++)
            {
                sLine = "";
                for (x = 0; x < Width; x++)
                {
                    if (x == m_iStartX && y == m_iStartY)
                        sLine += "S";
                    else if (x == m_iEndX && y == m_iEndY)
                        sLine += "G";
                    else if (m_aMap[x, y])
                        sLine += " ";
                    else
                        sLine += "#";
                }
                Debug.WriteLine(sLine);
            }
        }

        private void LoadMap(string sBMPFileName)
        {
            FileStream fs = new FileStream(sBMPFileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            if (br.ReadChar().ToString() + br.ReadChar().ToString() == "BM")
            {
                int bfSize = br.ReadInt32();
                int bfReserved1 = br.ReadInt16();
                int bfReserved2 = br.ReadInt16();
                int bfoffbits = br.ReadInt32();
                int biSize = br.ReadInt32();
                int biWidth = br.ReadInt32();
                int biHeight = br.ReadInt32();
                int biPlanes = br.ReadInt16();
                int biBitCount = br.ReadInt16();
                int biCompression = br.ReadInt32();
                int biSizeImage = br.ReadInt32();
                int biXPelsPerMeter = br.ReadInt32();
                int biYPelsPerMeter = br.ReadInt32();
                int biClrUsed = br.ReadInt32();
                int biClrImportant = br.ReadInt32();
                int iOffset = bfoffbits;
                int cPadding = 4 - ( biWidth * 3 ) % 4;
                if (cPadding == 4)
                    cPadding = 0;
                int rgbRed = 0;
                int rgbGreen = 0;
                int rgbBlue = 0;
                m_aMap = new bool[biWidth,biHeight];
                Height = biHeight;
                Width = biWidth;
                try
                {
                    for (int y = biHeight - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < biWidth; x++)
                        {
                            rgbBlue = br.ReadByte();
                            rgbGreen = br.ReadByte();
                            rgbRed = br.ReadByte();
                            if (rgbBlue + rgbGreen + rgbRed > 0)
                                m_aMap[x, y] = false;
                            else
                                m_aMap[x, y] = true;
                            if (rgbRed > rgbGreen)
                            {
                                m_aMap[x, y] = true;
                                m_iStartX = x;
                                m_iStartY = y;
                            }
                            if (rgbGreen > rgbRed)
                            {
                                m_aMap[x, y] = true;
                                m_iEndX = x;
                                m_iEndY = y;
                            }
                        }
                        for (int i = 0; i < cPadding; i++)
                            br.ReadByte();
                    }
                }
                catch
                {
                    Debug.WriteLine("Error in bitmap");
                }
            }
            else
                Debug.WriteLine("this is not a bitmap file");
            fs.Close();
        }

        private IEnumerable<State> GetStates()
        {
            int iX = 0, iY = 0, iXVelocity = 0, iYVelocity = 0;
            for (iX = 0; iX < Width; iX++)
            {
                for (iY = 0; iY < Height; iY++)
                {
                    if (m_aMap[iX, iY])
                    {
                        for (iXVelocity = -MAX_VELOCITY; iXVelocity <= MAX_VELOCITY; iXVelocity++)
                        {
                            for (iYVelocity = -MAX_VELOCITY; iYVelocity <= MAX_VELOCITY; iYVelocity++)
                            {
                                yield return new RaceCarState(iX, iY, iXVelocity, iYVelocity, this);
                            }
                        }
                    }
                }
            }
            for (iXVelocity = -MAX_VELOCITY; iXVelocity <= MAX_VELOCITY; iXVelocity++)
            {
                for (iYVelocity = -MAX_VELOCITY; iYVelocity <= MAX_VELOCITY; iYVelocity++)
                {
                    yield return new RaceCarState(-1, -1, iXVelocity, iYVelocity, this);
                }
            }

        }

        private  IEnumerable<Action> GetActions()
        {
            int iXVelocity = 0, iYVelocity = 0;
            for (iXVelocity = -1; iXVelocity <= 1; iXVelocity++)
            {
                for (iYVelocity = -1; iYVelocity <= 1; iYVelocity++)
                {
                    yield return new VelocityAction(iXVelocity, iYVelocity);
                }
            }
        }

        public override State StartState { get { return new RaceCarState(m_iStartX, m_iStartY, this); } }

        public bool IsGoalState(RaceCarState s)
        {
            return s.X == -1 && s.Y == -1;
        }
        public bool IsRaceEnd(RaceCarState s)
        {
            return s.X == m_iEndX && s.Y == m_iEndY;
        }
        public bool IsRaceStart(RaceCarState s)
        {
            return s.X == m_iStartX && s.Y == m_iStartY;
        }

        public bool IsRaceEnd(int iX, int iY)
        {
            return iX == m_iEndX && iY == m_iEndY;
        }
        public bool IsRaceStart(int iX, int iY)
        {
            return iX == m_iStartX && iY == m_iStartY;
        }

        internal bool ValidPoint(int iX, int iY)
        {
            if (iX < 0 || iX >= Width)
                return false;
            if (iY < 0 || iY >= Height)
                return false;
            return m_aMap[iX, iY];
        }

        public bool Move(int iStartX, int iStartY, int iVelocityX, int iVelocityY, out int iFinalX, out int iFinalY)
        {
            iFinalX = -1;
            iFinalY = -1;
            if (iStartX == m_iEndX && iStartY == m_iEndY || iStartX == -1 && iStartY == -1)
            {
                return true;
            }
            else if (iVelocityX == 0 && iVelocityY == 0)
            {
                iFinalX = iStartX;
                iFinalY = iStartY;
                return true;
            }
            else
            {
                double dCurrentX = iStartX;
                double dCurrentY = iStartY;
                int iEndX = iStartX + iVelocityX;
                int iEndY = iStartY + iVelocityY;
                while ((Math.Abs(dCurrentX - iEndX) > 0.01 || Math.Abs(dCurrentY - iEndY) > 0.01) && ValidPoint((int)Math.Round(dCurrentX), (int)Math.Round(dCurrentY)))
                {
                    iFinalX = (int)Math.Round(dCurrentX);
                    iFinalY = (int)Math.Round(dCurrentY);
                    dCurrentX += (1.0 * iVelocityX) / MAX_VELOCITY;
                    dCurrentY += (1.0 * iVelocityY) / MAX_VELOCITY;
                }
                if (iFinalX == iEndX && iFinalY == iEndY)
                    return true;
            }
            return false;
        }

        public override bool IsGoalState(State s)
        {
            return IsGoalState((RaceCarState)s);
        }

        public void DrawRace(Policy p, RaceViewer form)
        {
            form.StateValues = null;
            form.Start();
            //form.ShowDialog();
            while (form.Active)
            {
                Thread.Sleep(100);
                RaceCarState s = (RaceCarState)StartState;
                VelocityAction a = null;
                while (form.Active && !IsGoalState(s))
                {
                    a = (VelocityAction)p.GetAction(s);
                    if (a == null)
                        break;
                    form.CarState = s;
                    //form.Invoke(form.RefreshForm);
                    //form.SetCarState(s);
                    Thread.Sleep(100);
                    RaceCarState sTag = (RaceCarState)s.Apply(a);
                    s = sTag;
                }
            }
        }
    }
}

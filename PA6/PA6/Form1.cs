﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PA6
{
    public partial class Form1 : Form
    {
        int BMIN = 3; //Birth Law min
        int BMAX = 3; //Birth Law max
        int SMIN = 2; //Survival Law min
        int SMAX = 3; //Survival Law max
        int Generations = 10; //Generations to iterate when "Start" is pushed
        int index, generationCounter;
        List<Cell> cellsToActivate = new List<Cell>();
        List<Cell> cellsToKill = new List<Cell>();
        public Cell[,] cellArray;
        public int row;
        public int col;
        public static float cellWidth, cellHeight;
        Pen pen = new Pen(Color.Black, 1);
        SolidBrush sbAlive = new SolidBrush(Color.Green);
        SolidBrush sbDead = new SolidBrush(Color.DarkGray);
        Grid grid;

        public Form1()
        {
            bool startGrid = ShowStartupForm();
            if (startGrid)
            {
                InitializeComponent();
                SetCell();
            }
            else
                Application.Exit();
        }

        public void SetCell()
        {
            cellHeight = (ClientSize.Height - menuStrip1.Height)/(float) row;
            cellWidth = ClientSize.Width/(float) col;
        }

        public void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (toggleGridToolStripMenuItem.Checked)
                LoadGrid(e);
            grid = new Grid(cellArray, cellHeight, cellWidth, menuStrip1.Height, e, sbAlive, sbDead);
        }

        public void LoadGrid(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            BackColor = pen.Color;
            float x = 0;
            float y = menuStrip1.Height;
            for (int i = 1; i < col + 1; i++)
            {
                //Offset Drawing row lines to start at the base of the menuStrip
                g.DrawLine(pen, x, y, x, menuStrip1.Height + ClientSize.Height);
                x += cellWidth;
            }
            x = 0;
            for (int i = 1; i < row + 1; i++)
            {
                //Offset Drawing row lines to start at the base of the menuStrip
                g.DrawLine(pen, x, y, ClientSize.Width, y);
                y += cellHeight;
            }
        }

        /// <summary>
        /// Mouse event that gets the coordinate of the mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Graphics g = CreateGraphics();
            try
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        int x = e.X;
                        int y = e.Y - menuStrip1.Height;
                        int c = (int) Math.Floor((double) y/cellHeight);
                        int r = (int) Math.Floor((double) x/cellWidth);
                        cellArray = grid.GetCellArray;
                        cellArray[r, c].ToggleAlive(true, g, sbAlive);
                        break;
                    case MouseButtons.Right:
                        int x1 = e.X;
                        int y1 = e.Y - menuStrip1.Height;
                        int c1 = (int) Math.Floor((double) y1/cellHeight);
                        int r1 = (int) Math.Floor((double) x1/cellWidth);
                        cellArray = grid.GetCellArray;
                        cellArray[r1, c1].ToggleAlive(false, g, sbDead);
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"Something went wrong. Please enter valid values.");
                Dispose();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetCell();
            Invalidate();
        }

        private void evolutionParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvolutionParameters ep = new EvolutionParameters();
            ep.SMIN = SMIN;
            ep.SMAX = SMAX;
            ep.BMAX = BMAX;
            ep.BMIN = BMIN;
            ep.Generations = Generations;
            DialogResult dr = ep.ShowDialog();

            //retrieve the user set evolution paramters
            if (dr == DialogResult.OK)
            {
                BMIN = ep.BMIN;
                BMAX = ep.BMAX;
                SMIN = ep.SMIN;
                SMAX = ep.SMAX;
                Generations = ep.Generations;
            }
            //do nothing. dont really need this
            else if (dr == DialogResult.Cancel){}
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            Graphics g = CreateGraphics();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                sbDead = new SolidBrush(cd.Color);
                for (int i = 0; i < cellArray.GetLength(0); i++)
                    for (int j = 0; j < cellArray.GetLength(1); j++)
                        if (cellArray[i, j].IsAlive == false)
                            cellArray[i, j].ToggleAlive(false, g, sbDead);
            }
        }

        private void creatureColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                sbAlive = new SolidBrush(cd.Color);
                for (int i = 0; i < cellArray.GetLength(0); i++)
                    for (int j = 0; j < cellArray.GetLength(1); j++)
                        if (cellArray[i, j].IsAlive)
                            cellArray[i, j].ToggleAlive(true, g, sbAlive);
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                pen = new Pen(cd.Color);
                Invalidate();
            }
        }

        public bool ShowStartupForm()
        {
            StartupForm StartupDialog = new StartupForm();
            var result = StartupDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                col = StartupDialog.StartupColumns;
                row = StartupDialog.StartupRows;
                cellArray = new Cell[col, row];
                return true;
            }
            return false;
        }
        private void CheckNeighbors(Cell[,] cellArray, int c, int r)
        {
            int activeNeighbors = 0;
            int[] temp = new int[12];
            //neighbor 1
            temp[0] = Mod((c - 1), row);
            //Console.WriteLine("Checking element {0}, {1}", r, temp[0]);
            if (cellArray[r, Mod((c - 1), row)].IsAlive)
                activeNeighbors++;
            //n2
            temp[1] = Mod((r - 1), row);
            temp[2] = Mod((c - 1), row);
            //Console.WriteLine("Checking element {0}, {1}", temp[1], temp[2]);
            if (cellArray[Mod((r - 1), col), Mod((c - 1), row)].IsAlive)
                activeNeighbors++;
            //n3
            temp[3] = Mod((r - 1), col);
            // Console.WriteLine("Checking element {0}, {1}", temp[3], c);
            if (cellArray[Mod((r - 1), col), c].IsAlive)
                activeNeighbors++;
           //n4
            temp[4] = Mod((r - 1), col);
            temp[5] = Mod((c + 1), row);
            // Console.WriteLine("Checking element {0}, {1}", temp[4], temp[5]);
            if (cellArray[Mod((r - 1), col), Mod((c + 1), row)].IsAlive)
                activeNeighbors++;
            //n5
            temp[6] = Mod((c + 1), row);
            // Console.WriteLine("Checking element {0}, {1}", r, temp[6]);
            if (cellArray[r, Mod((c + 1), row)].IsAlive)
                activeNeighbors++;
            //n6
            temp[7] = Mod((r + 1), col);
            temp[8] = Mod((c + 1), row);
            //Console.WriteLine("Checking element {0}, {1}", temp[7], temp[8]);
            if (cellArray[Mod((r + 1), col), Mod((c + 1), row)].IsAlive)
                activeNeighbors++;
            //n7
            temp[9] = Mod((r + 1), col);
            //Console.WriteLine("Checking element {0}, {1}", temp[9], c);
            if (cellArray[Mod((r + 1), col), c].IsAlive)
                activeNeighbors++;
            //n8
            temp[10] = Mod((r + 1), col);
            temp[11] = Mod((c - 1), row);
            //Console.WriteLine("Checking element {0}, {1}", temp[10], temp[11]);
            if (cellArray[Mod((r + 1), col), Mod((c - 1), row)].IsAlive)
                activeNeighbors++;

            //this checks alive cell with neighbors
            //Survival
            if (cellArray[r, c].IsAlive && activeNeighbors >= SMIN && activeNeighbors <= SMAX)
                //cellArray[r, c].ToggleAlive(true, g);
                cellsToActivate.Add(cellArray[r, c]);
            //Birth
            else if (!cellArray[r, c].IsAlive && (activeNeighbors >= BMIN && activeNeighbors <= BMAX))
                //cellArray[r, c].ToggleAlive(true, g);
                cellsToActivate.Add(cellArray[r, c]);
            //Death
            else if (cellArray[r,c].IsAlive)
                cellsToKill.Add(cellArray[r, c]);
                //cellArray[r, c].ToggleAlive(false, g);
        }
        
        private void OneGeneration()
        {
            for (int i = 0; i < cellArray.GetLength(0); i++)
                for (int j = 0; j < cellArray.GetLength(1); j++)
                    CheckNeighbors(cellArray, j, i);
            CellList(cellsToActivate, cellsToKill);
            cellsToActivate.Clear();
            cellsToKill.Clear();
            generationLabel.Text = String.Format("Generation Count: {0}", ++generationCounter);;
        }

        public int Mod(int a, int n)
        {
            return ((a % n) + n) % n;
        }


        private void CellList(List<Cell> aliveCells, List<Cell> deadCells )
        {
            Graphics g = CreateGraphics();
            foreach (Cell c in aliveCells)
                c.ToggleAlive(true, g, sbAlive);
            foreach (Cell c in deadCells)
                c.ToggleAlive(false, g, sbDead);
        }

        /// <summary>
        /// Timer event that handles the the generations from the Play button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generationTimer_Tick(object sender, EventArgs e)
        {
            generationTimer.Interval = (int)(1000 / numEvoRate.Value);
            //index used to handle how many iterations of OneGeneration has been counted.
            if (index < Generations)
            {
                index++;
                OneGeneration();
            }
        }

        /// <summary>
        /// Play button event that triggers the Timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (index == Generations)   //if game has already started, continue with the last gen by setting index to counter
                index = 0;
            generationTimer.Start();
        }

        /// <summary>
        /// Button event that handles one generation of evolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void singleStepEvolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OneGeneration();
            index = Generations;  //Set index to Generations to let Timer be unaffected.
        }

        /// <summary>
        /// Pause button to handle pausing the evolution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pauseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            generationTimer.Stop();
        }

        /// <summary>
        /// Clears everything off the grid by setting the Cell to be dead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear_Grid(object sender, EventArgs e)
        {
            for (int i = 0; i < cellArray.GetLength(0); i++)
                for (int j = 0; j < cellArray.GetLength(1); j++)
                    cellArray[i, j].IsAlive = false; 
            Invalidate();
        }

        /// <summary>
        /// Button event that generates random cells dead or alive on the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateRandomStateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            grid = grid.RandomGrid(grid, sbAlive, sbDead);
            Invalidate();
        }

        /// <summary>
        /// Toggles the grid on or off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toggleGridToolStripMenuItem.Checked)
            {
                toggleGridToolStripMenuItem.Text = "Grid Toggle: OFF";
                toggleGridToolStripMenuItem.Checked = false;
                BackColor = sbDead.Color;
                Invalidate();
            }
            else if (!toggleGridToolStripMenuItem.Checked)
            {
                toggleGridToolStripMenuItem.Text = "Grid Toggle: ON";
                toggleGridToolStripMenuItem.Checked = true;
                Invalidate();
            }
        }
    }
}
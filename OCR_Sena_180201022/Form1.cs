using System;
using System.Drawing;
using System.Windows.Forms;

namespace OCR_Sena_180201022
{
    public partial class Form1 : Form
    {
        Bitmap Learn_IMG, Compare_IMG;
        int[] H, V, F_H, F_V; //from A to G
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string F_Name;
            openFileDialog1.ShowDialog();
            F_Name = openFileDialog1.FileName;
            Learn_IMG = new Bitmap(F_Name);
            pictureBox1.Image = Learn_IMG;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            F_H = new int[Learn_IMG.Height];
            F_V = new int[Learn_IMG.Width];
            
            for (int y = 0; y < Learn_IMG.Height; y++)
                F_H[y] = 0;
            for (int x = 0; x < Learn_IMG.Width; x++)
                F_V[x] = 0;

            for (int y = 0; y < Learn_IMG.Height; y++) //vertical
                for (int x = 0; x < Learn_IMG.Width; x++)
                    if(Learn_IMG.GetPixel(x,y).G < 10)
                        F_H[y]++;

            for (int x = 0; x < Learn_IMG.Width; x++) //horizontal projection
                for (int y = 0; y < Learn_IMG.Height; y++)
                    if (Learn_IMG.GetPixel(x, y).G < 10)
                        F_V[x]++;

            int Pivot;
            for (Pivot = 0; Pivot < Learn_IMG.Height; Pivot++) //shifting horizontal
                if (F_H[Pivot] > 0) break;

            for (int x = 0; x < Learn_IMG.Height - Pivot; x++)
            {
                F_H[x] = F_H[x + Pivot];
                F_H[x + Pivot] = 0;
            }


            for (Pivot = 0; Pivot < Learn_IMG.Width; Pivot++) //shifting Vertical
                if (F_V[Pivot] > 0) break;
            
            for (int x = 0; x < Learn_IMG.Width - Pivot; x++) {
                F_V[x] = F_V[x + Pivot];
                F_V[x + Pivot] = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string F_Name;
            openFileDialog1.ShowDialog();
            F_Name = openFileDialog1.FileName;
            Compare_IMG = new Bitmap(F_Name);
            pictureBox2.Image = Compare_IMG;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < Compare_IMG.Width; x++)
                for (int y = 0; y < Compare_IMG.Height; y++)
                {
                    int RR = Compare_IMG.GetPixel(x, y).R;
                    int GG = Compare_IMG.GetPixel(x, y).G;
                    int BB = Compare_IMG.GetPixel(x, y).B;

                    int GRAY = (int)(0.3*RR + 0.58*GG + 0.12*BB);
                    Compare_IMG.SetPixel(x, y, Color.FromArgb(GRAY, GRAY, GRAY));
                }

            //histogram
            int[] Hist = new int[256];
            for (int i = 0; i < 256; i++)
                Hist[i] = 0;

            for (int x = 0; x < Compare_IMG.Width; x++)
                for (int y = 0; y < Compare_IMG.Height; y++)
                    Hist[Compare_IMG.GetPixel(x, y).G]++;

            int Peak1_lvl = 64, Peak1_val = 0;
            for (int i = 64; i < 128; i++)
                if (Hist[i] > Peak1_val)
                {
                    Peak1_lvl = i;
                    Peak1_val = Hist[i];
                }

            int Peak2_lvl = 128, Peak2_val = 0;
            for (int i = 128; i < 192; i++)
                if (Hist[i] > Peak2_val)
                {
                    Peak2_lvl = i;
                    Peak2_val = Hist[i];
                }

            int Th_lvl = (Peak1_lvl + Peak2_lvl) / 2;

            H = new int[Compare_IMG.Height];
            V = new int[Compare_IMG.Width];
            
            for (int y = 0; y < Compare_IMG.Height; y++)
                H[y] = 0;
            for (int x = 0; x < Compare_IMG.Width; x++)
                V[x] = 0;

            for (int y = 0; y < Compare_IMG.Height; y++)
                for (int x = 0; x < Compare_IMG.Width; x++)     //vertical
                    if (Compare_IMG.GetPixel(x, y).G < Th_lvl)
                        H[y]++;

            for (int x = 0; x < Compare_IMG.Width; x++)         //horizontal
                for (int y = 0; y < Compare_IMG.Height; y++)
                    if (Compare_IMG.GetPixel(x, y).G < Th_lvl)
                        V[x]++;

            //SHIFTING H
            int Pivot;
            for (Pivot = 0; Pivot < Compare_IMG.Height; Pivot++)
                if (H[Pivot] > 0) break;

            for (int y = 0; y < Compare_IMG.Height - Pivot; y++)
            {
                H[y] = H[y + Pivot];
                H[y + Pivot] = 0;
            }

            //SHIFTING V
            for (Pivot = 0; Pivot < Compare_IMG.Width; Pivot++)
                if (V[Pivot] > 0) break;

            for (int x = 0; x < Compare_IMG.Width - Pivot; x++)
            {
                V[x] = V[x + Pivot];
                V[x + Pivot] = 0;
            }

            //difference
            int v_diff = 0, h_diff = 0;
            for (int x = 0; x < Compare_IMG.Width; x++)
                v_diff += Math.Abs(F_V[x] - V[x]);
            for (int y = 0; y < Compare_IMG.Height; y++)
                h_diff += Math.Abs(F_H[y] - H[y]);

            textBox1.Text = v_diff.ToString();
            textBox2.Text = h_diff.ToString();
        }
    }
}
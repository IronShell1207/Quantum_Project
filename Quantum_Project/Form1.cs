using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lachesis.QuantumComputing;

namespace Quantum_Project
{
    public partial class Form1 : Form
    {
        bool down_mouse = false;
        public List<PictureBox> pictureBoxes = new List<PictureBox>();
        Qubit st1 = Qubit.Zero, st2 = Qubit.Zero, st3 = Qubit.Zero, st4 = Qubit.Zero, st5 = Qubit.Zero, st6 = Qubit.Zero;
        PictureBox p;
        List<string> gatsST = new List<string> { "Z", "X", "Y", "H", "id", "sqrtnot", "swap", "Sz", "Ts", "Toffoli", "CNOT","oracle", "gate1" };
        List<QuantumGate> gatsQG = new List<QuantumGate> { QuantumGate.PauliZGate, QuantumGate.NotGate, QuantumGate.PauliYGate, QuantumGate.HadamardGate,
            QuantumGate.IdentityGate, QuantumGate.SquareRootNotGate, QuantumGate.SwapGate, QuantumGate.PhaseGate, QuantumGate.PhaseShiftGate(Math.PI/4),
            QuantumGate.ToffoliGate,QuantumGate.ControlledNotGate,QuantumGate.OracleGate,QuantumGate.gate1};
        List<int> gatsWeight = new List<int> { 0, 0, 0, 0, 0, 0, 1, 0, 0, 2, 1, 1, 0 };
        int cR1 = 0, cR2 = 0, cR3 = 0, cR4 = 0, cR5 = 0, cR6 = 0, cR7 = 0, cR8 = 0, cR9 = 0, cR10 = 0, cR11 = 0, mY = 0, mX = 0;
        List<QuantumGate> calculations = new List<QuantumGate>();
        Point lastpoint, max, max2;
        int Xi = 0, mouseX, mouseY, deltax = 0, deltay = 0;
        List<int> xpos = new List<int> { 40, 100, 160, 220, 280, 340, 400, 460, 520, 580, 640 };
        List<int> ypos = new List<int> { 80, 140, 200, 260, 320, 380 };
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {  

        }
        #region picboxses_controls
        void Gate_creator(Image image, string name)
        {
            PictureBox picturebod = new PictureBox()
            {
                Name = "empty",
                Image = Properties.Resources.empty,
                Visible = false,
                Size = Properties.Resources.empty.Size,
                Location = get_point(60, "sw")
            };
            PictureBox picturebx = new PictureBox()
            {
                Name = "empty",
                Image = Properties.Resources.empty,
                Visible = false,
                Size = Properties.Resources.empty.Size,
                Location = get_point(120,"tof")
            };
            p = new PictureBox()
            {
                Image = image,
                Location = get_point(0,""),
                Size = image.Size,
                BackColor = Color.Transparent,
                Name = name + Xi.ToString()
            };
            switch (name)
            {
                case "Toffoli":
                    pictureBoxes.Add(picturebod);
                    panel1.Controls.Add(picturebod); pictureBoxes.Add(picturebx);
                    panel1.Controls.Add(picturebx);
                    break;
                case "swap":
                    pictureBoxes.Add(picturebod);
                    panel1.Controls.Add(picturebod);
                    break;
                case "oracle":
                    pictureBoxes.Add(picturebod);
                    panel1.Controls.Add(picturebod);
                    break;
                case "CNOT":
                    pictureBoxes.Add(picturebod);
                    panel1.Controls.Add(picturebod);
                    break;

            }
            pictureBoxes.Add(p);
            panel1.Controls.Add(p);
            p.MouseDown += new MouseEventHandler(PicBox_MouseDown);
            p.MouseMove += new MouseEventHandler(PicBox_MouseMove);
            p.MouseUp += new MouseEventHandler(PicBox_MouseUp);
            Xi++;
        }
        private void Panel1_Paint_1(object sender, PaintEventArgs e)
        {
            //построение линий на панели (кубитоВВВВ)
            List<int> xcord = new List<int> { 100, 160, 220, 280, 340, 400 };
            for (int i = 0; i < xcord.Count; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 2), 30, xcord[i], 750, xcord[i]);
            }
        }
        private Point get_point(int correct,string gate)
        {
            bool lic = false;
            foreach (int yps in ypos)
                foreach (int xps in xpos)
                {
                    if (pictureBoxes.Count > 0)
                        foreach (PictureBox l in pictureBoxes)
                            if (l.Location.X == xps && l.Location.Y == yps)
                            {
                                lic = true;
                                break;
                            }
                    if (lic == false)
                    {
                        return new Point(xps, yps + correct);
                    }
                        
                    else lic = false;
                }
            return new Point(0, 0);
        }
        void PBremovEmpty(PictureBox p)
        {
            foreach (PictureBox s in pictureBoxes)
            {
                if (p.Location.Y + 60 == s.Location.Y && p.Location.X == s.Location.X)
                {
                    pictureBoxes.Remove(s);
                    panel1.Controls.Remove(s);
                    break;
                }
            }
        }
        private void PicBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { }
            else
                if ((e.X < p.Width) && (e.X > 0))
                    if ((e.Y < p.Height) && (e.Y > 0))
                    {
                        down_mouse = true;
                        p = (PictureBox)sender;
                        if (p.Name.IndexOf("swap") == 0)
                            PBremovEmpty(p);
                        if (p.Name.IndexOf("oracle") == 0)
                            PBremovEmpty(p);
                        else if (p.Name.IndexOf("CNOT") == 0)
                            PBremovEmpty(p);
                        else if (p.Name.IndexOf("Toffoli") == 0)
                        {
                            List<Control> ps = new List<Control> { };
                            List<PictureBox> pb = new List<PictureBox> { };
                            foreach (PictureBox s in pictureBoxes)
                            {
                                if (p.Location.Y + 60 == s.Location.Y && p.Location.X == s.Location.X)
                                {
                                    ps.Add(s);
                                    pb.Add(s);
                                }
                                if (p.Location.Y + 120 == s.Location.Y && p.Location.X == s.Location.X)
                                {
                                    ps.Add(s);
                                    pb.Add(s);
                                }
                            }
                            for (int i = 0; i < ps.Count; i++)
                            {
                                pictureBoxes.Remove(pb[i]);
                                panel1.Controls.Remove(ps[i]);
                            }

                        }
                    deltax = e.X - p.Location.X;
                    deltay = e.Y - p.Location.Y;
                    lastpoint = new Point(p.Location.X, p.Location.Y);
                    toolStripLabel2.Text = p.Location.X + "  " + p.Location.Y;
                    }
        }
        private void PicBox_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (down_mouse)
            {
                int s1 = MousePosition.X - this.Location.X - 20;
                int s2 = MousePosition.Y - this.Location.Y - 47;
                p.Location = new Point(MousePosition.X - this.Location.X - 20, MousePosition.Y - this.Location.Y - 47);
                //p.Location = new Point(e.X, e.Y);
                toolStripLabel1.Text = s1 + "  " + s2;
            }
        }
        private bool checkpos(Point point, PictureBox picture)
        {
            Point last1 = minvaluepoint(point);
            foreach (PictureBox box in pictureBoxes)
                if (picture != box)
                    if (box.Location == last1) return false;
            return true;
        }
        void PBSpawnEmpty(int Xx, int Yy)
        {
            PictureBox picturebod = new PictureBox()
            {
                Name = "empty",
                Image = Properties.Resources.empty,
                Visible = false,
                Size = Properties.Resources.empty.Size,
                Location = new Point(Xx, Yy + 60)
            };
            pictureBoxes.Add(picturebod);
            panel1.Controls.Add(picturebod);
        }
        private Point minvaluepoint(Point point)
        {
            double min = double.MaxValue; Point minpoint = new Point();
            foreach (int yps in ypos)
            {
                foreach (int xps in xpos)
                {
                    Point p2 = new Point(xps, yps);
                    double value = Math.Sqrt(Math.Pow(p2.X - point.X, 2) + Math.Pow(p2.Y - point.Y, 2));
                    if (value < min) {
                        min = value;
                        minpoint = new Point(p2.X, p2.Y);
                    }
                }
            }
            return minpoint;
        }
        private void PicBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                p = (PictureBox)sender;
                if (MessageBox.Show(p.Location.ToString() + "\r\nВентиль " + p.Name, "", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                    down_mouse = false;
            }
            if (e.Button == MouseButtons.Left)
            {
                down_mouse = false;
                // 40, 100, 160, 220, 280, 340, 400, 460, 520
                int xloc = p.Location.X, yloc = p.Location.Y;
                Point point = new Point(MousePosition.X - this.Location.X - 20, MousePosition.Y - this.Location.Y - 47);
                Point result = minvaluepoint(point);
                if (checkpos(p.Location, p) == false)
                    p.Location = new Point(lastpoint.X, lastpoint.Y);
                else if ((p.Location.X > TRASHBOX.Location.X) && (p.Location.Y > TRASHBOX.Location.Y))
                    panel1.Controls.Remove(p);
                else
                {
                    if (p.Name.IndexOf("swap") == 0)
                        PBSpawnEmpty(result.X, result.Y);
                    else if (p.Name.IndexOf("oracle") == 0)
                        PBSpawnEmpty(result.X, result.Y);
                    else if (p.Name.IndexOf("CNOT") == 0)
                        PBSpawnEmpty(result.X, result.Y);
                    else if (p.Name.IndexOf("Toffoli") == 0)
                    {
                        PBSpawnEmpty(result.X, result.Y);
                        PBSpawnEmpty(result.X, result.Y+60);
                    }
                    p.Location = new Point(result.X, result.Y);
                }
            }
        }
        #endregion
        static int Max(int[] array)
        {
            int max;
            return max = array.Max();
        }
        private Point lastelementY()
        {
            Point max = new Point(0, 0);
            for (int ix = xpos.Count - 1; ix >= 0; ix--)
                for (int iy = ypos.Count - 1; iy >= 0; iy--)
                    foreach (PictureBox l in pictureBoxes)
                        if (l.Location.X == xpos[ix] && l.Location.Y == ypos[iy])
                        {
                            max = new Point(l.Location.X, l.Location.Y);
                            return new Point(max.X, max.Y);
                        }
            return new Point(max.X, max.Y);
        }
        private Point lastelementX()
        {
            Point max = new Point(0, 0);
            for (int iy = ypos.Count - 1; iy >= 0; iy--)
                for (int ix = xpos.Count - 1; ix >= 0; ix--)
                    foreach (PictureBox l in pictureBoxes)
                        if (l.Location.X == xpos[ix] && l.Location.Y == ypos[iy])
                        {
                            max = new Point(l.Location.X, l.Location.Y);
                            return new Point(max.X, max.Y);
                        }
            return new Point(max.X, max.Y);
        }
        void addtocalc(int activeregisters, int maximumWEigh)
        {
            List<QuantumGate> allregs = new List<QuantumGate> { };
            int[] weightReG = new int[] { cR1, cR2, cR3, cR4, cR5, cR6, cR7, cR8, cR9, cR10, cR11 };//чтобы потом сравнить и добавить недостающие
            int[] weighFinal = new int[] { };
            for (int ia = 0; ia < maximumWEigh; ia++)
            {
                Array.Resize(ref weighFinal, weighFinal.Length + 1);
                weighFinal[weighFinal.Length - 1] = weightReG[ia];
            }
            for (int ia = 0; ia < weighFinal.Length; ia++)
                weighFinal[ia] = activeregisters;
            foreach (int xps in xpos)
                foreach (int yps in ypos) // по горизонтали
                    if (pictureBoxes.Count > 0)
                        foreach (PictureBox l in pictureBoxes)
                            if (l.Location.Y == yps && l.Location.X == xps) //если совпадает с координатами по вертикали и горизонтали с изначальным
                                for (int i = 0; i < xpos.Count; i++)  //по количеству точек X массива
                                    if (xps == xpos[i])//по вертикали
                                        for (int ix = 0; ix < gatsST.Count; ix++)//сверка вентилей ix - номер вентиля в списке
                                            if (l.Name.IndexOf(gatsST[ix]) == 0)
                                            {
                                                weighFinal[i] -= gatsWeight[ix];
                                                allregs.Add(gatsQG[ix]);
                                            }
            List<Qubit> flist = new List<Qubit> { st1, st2, st3, st4, st5, st6 };
            List<Qubit> slist = new List<Qubit> { };
            for (int i = 0; i < activeregisters; i++)
                slist.Add(flist[i]);
            QuantumRegister reg = new QuantumRegister(slist);
            int ireg = 0;
            foreach (int weight in weighFinal)
            {
                int igs = 0;
                List<QuantumGate> fqg = new List<QuantumGate> { };
                for (int ih = ireg; ih < weight + ireg; ih++)
                {
                    fqg.Add(allregs[ih]);
                    igs++;
                }
                ireg += igs;
                QuantumGate gate = new QuantumGate(fqg);
                reg = gate * reg;
            }
            textBoxResult.Text = reg.ToString();
        }
        void createvoid(Point loc)
        {
            p = new PictureBox()
            {
                Image = Properties.Resources.gate1,
                Location = loc,
                Size = Properties.Resources.gate1.Size,
                BackColor = Color.Transparent,
                Visible = false,
                Name = "gate1"
            };
            pictureBoxes.Add(p);
            panel1.Controls.Add(p);
        }
        void takefreespaceforcalc()
        {
            mY = 0; mX = 0;
            max = lastelementY();
            Point max2 = lastelementX();
            int[] maxX = new int[] { max.X,max2.X };
            int[] maxY = new int[] { max.Y, max2.Y };
            int maxXX = Max(maxX); int maxYY = Max(maxY);
            int voidcount = 0;
            List<Point> ppoints = new List<Point> { };
            foreach (PictureBox l in pictureBoxes)
            {
                ppoints.Add(new Point(l.Location.X, l.Location.Y));
            }
            bool lic = false;
            foreach (int yposition in ypos)
                if (yposition <= maxYY)
                for (int i = 0; i < xpos.Count; i++)
                    if (xpos[i] <= maxXX)
                    {
                        mY = yposition; mX = xpos[i];
                        Point newp = new Point(xpos[i], yposition);
                        for (int l = 0; l < ppoints.Count; l++)
                        {
                            if (ppoints[l] == newp)
                            {
                                lic = true;
                                break;
                            }
                            else lic = false;
                        }
                        if (lic == false)
                        {
                            createvoid(newp);
                            voidcount++;
                        }
                    }
            for (int x = 0; x<ypos.Count;x++)
                if (mY==ypos[x])
                    mY = x;
            for (int x = 0; x < xpos.Count; x++)
                if (mX == xpos[x])
                    mX = x;
        }
        void clearvoid()
        {
            List<PictureBox> pss = new List<PictureBox> { };
            List<Control> css = new List<Control> { };
            foreach (PictureBox o in pictureBoxes)
                if (o.Name == "gate1")
                {
                    pss.Add(o);
                    css.Add(o);
                }
            for (int i = 0; i < pss.Count; i++)
            {
                pictureBoxes.Remove(pss[i]);
                panel1.Controls.Remove(css[i]);
            }
        }
        private void CalcBTNClick(object sender, EventArgs e)
        {
            takefreespaceforcalc();
            calculations.Clear();
            addtocalc(mY+1,mX+1);
            clearvoid();
        }
        #region ToolStripGates
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.oracle, "oracle");
        }
        private void ToolStripY_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.YGate, "Y");
        }
        private void ToolStripZ_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.ZGate, "Z");
        }
        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.id, "id");
        }
        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.scrnot, "sqrtnot");
        }
        private void ToolStripButton1_Click_1(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources._100X405, "swap");
        }
        private void ToolStripH_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.Hgate, "H");
        }
        private void ToolStripX_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.XGate, "X");
        }
        private void ToolSGate_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.sgate, "Sz");
        }
        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.TGate, "Ts");
        }
        private void ToolStripButton4_Click_1(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.toffili, "Toffoli");
        }
        private void ToolStripButton5_Click_1(object sender, EventArgs e)
        {
            Gate_creator(Properties.Resources.CNOT, "CNOT");
        }
        #endregion
        #region labelclick
        void labelclc(Label label,ref Qubit qubit)
        {
            switch(label.Text)
            {
                case "|0>":
                    qubit = Qubit.One;
                    label.Text = "|1>";
                    break;
                case "|1>":
                    qubit = Qubit.Zero;
                    label.Text = "|0>";
                    break;
            }
        }
        private void Label1_Click(object sender, EventArgs e)
        {
            labelclc(lqubit1, ref st1);
        }
        private void Label2_Click(object sender, EventArgs e)
        {
            labelclc(lqubut2, ref st2);
        }

        private void ToolStripButton6_Click(object sender, EventArgs e)
        {
            pictureBoxes.Clear();
            for (int i=0; i<7; i++)
            foreach (Control item in panel1.Controls.OfType<PictureBox>())
            {
                if (item.Name == "TRASHBOX") { }
                else panel1.Controls.Remove(item);
            }
        }
        private void Label3_Click(object sender, EventArgs e)
        {
            labelclc(lqubit3,ref st3);
        }
        private void Label4_Click(object sender, EventArgs e)
        {
            labelclc(lqubit4,ref st4);
        }
        private void Label18_Click(object sender, EventArgs e)
        {
            labelclc(lqubit5,ref st5);
        }
        private void Label17_Click(object sender, EventArgs e)
        {
            labelclc(lqubit6,ref st6);
        }
        #endregion
    }
}

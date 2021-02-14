using System;
using System.IO ;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace WindowsApplication3
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox tb1;
		private System.Windows.Forms.TextBox path1;
		private System.Windows.Forms.TextBox origin;
		private System.Windows.Forms.TextBox destin;
		private System.Windows.Forms.PictureBox picBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		public cfeature_vector[] featurein;
		public cfeature_vector current_features;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		public double[] distence;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button2;
		public int view_flag = 0;



		public void vector_dump(cfeature_vector fetu)
		{
			FileStream filedump = new FileStream(path1.Text, FileMode.OpenOrCreate, FileAccess.Write);

			StreamWriter str = new StreamWriter(filedump);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 8; k++)
					{
						str.Write(fetu.tracks[i].sectors[j].relations[k]);
						str.Write('v');
					}
				}
			}
			str.Close();
			filedump.Close();

		}
		public void fix_scale()
		{
			double[] sum;
			int h = 0;
			sum = new double[28];

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					sum[h] = new double();
					sum[h] = 0;
					for (int k = 0; k < 8; k++)
					{
						sum[h] += current_features.tracks[i].sectors[j].relations[k];
					}
					h++;

				}
			}
			h = 0;
			for (int i = 0; i < 28; i++)
			{
				if (sum[i] == 0)
					sum[i] = 1;
			}


			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{

					for (int k = 0; k < 8; k++)
					{
						current_features.tracks[i].sectors[j].relations[k] /= sum[h];

					}
					h++;
				}

			}


		}


		public void get_stuff(Bitmap pict, int swtch)
		{

			int counter = 0;
			double Xctot = 0;
			double Yctot = 0;
			int Xc;
			int Yc;
			double Rmax = 0;
			double track_step = 0;
			cpixel_node[] pixels;
			//cfeature_vector fetu1 ;
			//get black pixel count
			for (int i = 0; i < pict.Height; i++)
			{
				for (int j = 0; j < pict.Width; j++)
				{
					if (pict.GetPixel(j, i) == Color.FromArgb(0, 0, 0))
					{
						counter++;
						Xctot += j;
						Yctot += i;
					}
				}
			}
			Xc = (int)Xctot / counter;
			Yc = (int)Yctot / counter;

			pixels = new cpixel_node[counter];
			int k = 0;



			// get pixels
			for (int i = 0; i < pict.Height; i++)
			{
				for (int j = 0; j < pict.Width; j++)
				{
					if (pict.GetPixel(j, i) == Color.FromArgb(0, 0, 0))
					{
						pixels[k] = new cpixel_node();
						pixels[k].x = j;
						pixels[k].y = i;
						k++;
					}
				}
			}
			//get rmx
			double xtot = 0;
			double ytot = 0;
			double rmaxtemp;
			int xrmx = 0;
			int yrmx = 0;

			for (int i = 0; i < counter; i++)
			{
				xtot = Xc - pixels[i].x;
				xtot *= xtot;//squareing
				ytot = Yc - pixels[i].y;
				ytot *= ytot;//squareing

				rmaxtemp = xtot + ytot;
				rmaxtemp = Math.Sqrt(rmaxtemp);

				if (rmaxtemp > Rmax)
				{
					Rmax = rmaxtemp;
					xrmx = pixels[i].x;
					yrmx = pixels[i].y;
				}
			}
			track_step = Rmax / 5;


			//get secotrs track elakh
			for (int i = 0; i < counter; i++)
			{
				if (pixels[i].x != Xc && pixels[i].y != Yc)
				{
					pixels[i].get_relation(pict);
					pixels[i].get_sector(Xc, Yc);
					pixels[i].get_track(Xc, Yc, track_step);
				}
			}
			//stuff for documentsition
			for (int i = 0; i < counter; i++)
			{
				pict.SetPixel(pixels[i].x, pixels[i].y, Color.Black);
			}
			//pict.SetPixel(Xc,Yc,Color.Black) ;
			//pict.SetPixel(xrmx,yrmx,Color.Black) ;
			pict.Save(destin.Text);

			//end doc stuf


			current_features = new cfeature_vector();

			current_features.feature_extractor(pixels, counter);
			fix_scale();
			if (swtch == 1)
				vector_dump(current_features);





		}
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			picBox1.Image = new Bitmap(picBox1.Width + 2, picBox1.Height + 2);
			picBox1.MouseMove += new MouseEventHandler(picBox1_MouseMove);
			picBox1.MouseUp += new MouseEventHandler(picBox1_MouseUp);
			picBox1.Paint += new PaintEventHandler(picBox1_Paint);
			this.Paint += new PaintEventHandler(Form1_Paint);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.picBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tb1 = new System.Windows.Forms.TextBox();
            this.path1 = new System.Windows.Forms.TextBox();
            this.origin = new System.Windows.Forms.TextBox();
            this.destin = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // picBox1
            // 
            this.picBox1.Location = new System.Drawing.Point(12, 62);
            this.picBox1.Name = "picBox1";
            this.picBox1.Size = new System.Drawing.Size(471, 357);
            this.picBox1.TabIndex = 0;
            this.picBox1.TabStop = false;
            this.picBox1.Click += new System.EventHandler(this.picBox1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(526, 257);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(214, 37);
            this.button1.TabIndex = 1;
            this.button1.Text = "Extract Shape Map";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb1
            // 
            this.tb1.Enabled = false;
            this.tb1.Location = new System.Drawing.Point(782, 300);
            this.tb1.Name = "tb1";
            this.tb1.Size = new System.Drawing.Size(102, 27);
            this.tb1.TabIndex = 2;
            this.tb1.Text = "textBox1";
            this.tb1.TextChanged += new System.EventHandler(this.tb1_TextChanged);
            // 
            // path1
            // 
            this.path1.Location = new System.Drawing.Point(750, 123);
            this.path1.Name = "path1";
            this.path1.Size = new System.Drawing.Size(168, 27);
            this.path1.TabIndex = 4;
            this.path1.Text = "Path for texture image";
            // 
            // origin
            // 
            this.origin.Location = new System.Drawing.Point(750, 163);
            this.origin.Name = "origin";
            this.origin.Size = new System.Drawing.Size(168, 27);
            this.origin.TabIndex = 5;
            this.origin.Text = "Image to regnize";
            // 
            // destin
            // 
            this.destin.Location = new System.Drawing.Point(750, 207);
            this.destin.Name = "destin";
            this.destin.Size = new System.Drawing.Size(134, 27);
            this.destin.TabIndex = 6;
            this.destin.Text = "Save As";
            this.destin.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(918, 123);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(33, 27);
            this.button3.TabIndex = 8;
            this.button3.Text = "...";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(918, 163);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(33, 27);
            this.button4.TabIndex = 8;
            this.button4.Text = "...";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(918, 202);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(33, 32);
            this.button5.TabIndex = 8;
            this.button5.Text = "...";
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(526, 300);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(214, 37);
            this.button6.TabIndex = 7;
            this.button6.Text = "Apply Map to Target";
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(772, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "Pixel Matches";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(526, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(224, 37);
            this.label3.TabIndex = 11;
            this.label3.Text = "Texture to recognize";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(526, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(190, 37);
            this.label4.TabIndex = 12;
            this.label4.Text = "Image to process";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(526, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(174, 34);
            this.label5.TabIndex = 13;
            this.label5.Text = "Destination File";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("MS Reference Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(80, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(255, 35);
            this.label7.TabIndex = 14;
            this.label7.Text = "Ingredient Identifier";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(526, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(258, 25);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "Read From Disk";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(830, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 37);
            this.button2.TabIndex = 17;
            this.button2.Text = "Clear";
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 20);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(976, 481);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.destin);
            this.Controls.Add(this.origin);
            this.Controls.Add(this.path1);
            this.Controls.Add(this.tb1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.picBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Name = "Form1";
            this.Text = "Ingredient Texture Mapping";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			Application.Run(new Form1());

		}


		private void Contour(Bitmap x, Bitmap x2)
		{
			Color y;
			String nm = "bb";
			y = new Color();
			for (int i = 0; i < x.Width; i++)
			{
				for (int j = 0; j < x.Height; j++)
				{
					nm = j.ToString();
					tb1.Text = nm;

					y = x.GetPixel(i, j);
					y.ToKnownColor();
					//y.Equals(Color.Black) ;
					if (y == Color.FromArgb(0, 0, 0))
					{
						//x.SetPixel(i,j,Color.Tomato) ;
						if (x.GetPixel(i - 1, j - 1) == Color.FromArgb(0, 0, 0) && x.GetPixel(i - 1, j) == Color.FromArgb(0, 0, 0) && x.GetPixel(i - 1, j + 1) == Color.FromArgb(0, 0, 0) && x.GetPixel(i, j - 1) == Color.FromArgb(0, 0, 0) && x.GetPixel(i, j + 1) == Color.FromArgb(0, 0, 0) && x.GetPixel(i + 1, j - 1) == Color.FromArgb(0, 0, 0) && x.GetPixel(i + 1, j) == Color.FromArgb(0, 0, 0) && x.GetPixel(i + 1, j + 1) == Color.FromArgb(0, 0, 0))
						{
							x2.SetPixel(i, j, Color.White);

						}
					}
				}
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			button1.Enabled = false;
			Bitmap x, x2;

			if (checkBox1.Checked == true)
			{
				x = new Bitmap(origin.Text);
				x2 = new Bitmap(origin.Text);
				//picBox1.Image=x2 ;
			}
			else
			{
				x = new Bitmap(picBox1.Image);
				x2 = new Bitmap(picBox1.Image);
			}


			Contour(x, x2);


			x2.Save(destin.Text);


			get_stuff(x2, 1);
			picBox1.Image = x2;

			button1.Enabled = true;
		}

		public void feature_read(int pos, FileStream fileread)
		{
			//FileStream fileread=new FileStream(path1.Text,FileMode.OpenOrCreate,FileAccess.Read);
			StreamReader rite = new StreamReader(fileread);
			string st1;


			st1 = rite.ReadToEnd();

			int i = 0, f = 0;

			string[] strVals = st1.Split('v');

			for (i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 8; k++)
					{
						double currVal = double.Parse(strVals[f]);
						f++;
						featurein[pos].tracks[i].sectors[j].relations[k] = currVal;
					}
				}
			}



			rite.Close();
		}

		public void get_distence(cfeature_vector vec2, int num)
		{
			double x = 0, y = 0, z = 0, zf = 0;

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 8; k++)
					{
						x = current_features.tracks[i].sectors[j].relations[k];
						y = vec2.tracks[i].sectors[j].relations[k];
						z = x - y;
						z *= z;
						zf += z;


					}
				}
			}
			distence[num] = new double();
			distence[num] = zf;

		}




		public void Form1_Load(object sender, System.EventArgs e)
		{
			/*if (view_flag==0)
			{
				form1
			
				Form2 f2=new Form2();
				f2.Visible=true ;
				
				Form1 &f1=new Form1();
				f1.Visible=false;
				view_flag=1;
			}*/




		}

		public void tb1_TextChanged(object sender, System.EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{

		}


		private void file_read_Click(object sender, System.EventArgs e)
		{



			#region Delay it Now
			/*for(int h=0;h<st1.Length;h++)
			{
				if (st1[i]!='v')
				{
					st2[j]=st1[i] ;
					j++ ;
				}
				else
				{
					val=int.Parse(st2);
					j=0 ;
					featurein[k].tracks[l].sectors[m].relations[n]=val  ;
					if (n==7)
					{
						n=0;
						if (m==3)
						{
							m=0;
							if (l==5)
							{
								l=0 ;

							}
							else
							{
								l++ ;
								if (k!=maxl-1)
								{
									k++ ;
								}
							}
						}
						else
						{
							m++;
						}
					}
					else
					{
						n++;

					}
				}

				

				

				}*/
			#endregion
		}

		private void button2_Click(object sender, System.EventArgs e)
		{

		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.ShowDialog();
			this.origin.Text = dlg.FileName;
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.ShowDialog();
			this.destin.Text = dlg.FileName;
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.ShowDialog();
			this.path1.Text = dlg.FileName;
		}


		public void button6_Click(object sender, System.EventArgs e)
		{

			button6.Enabled = false;
			featurein = new cfeature_vector[21];
			distence = new double[21];
			for (int i = 0; i < 21; i++)
			{
				featurein[i] = new cfeature_vector();
				string str = "c:/ocr/" + i + ".txt";
				FileStream fileread = new FileStream(str, FileMode.OpenOrCreate, FileAccess.Read);
				feature_read(i, fileread);

				get_distence(featurein[i], i);

				//StreamReader sr = new StreamReader(str);
				fileread.Close();


			}

			// find max
			double max = distence[0];
			int maxi = 0;
			char letter = 'u';
			for (int i = 0; i < 21; i++)
				if (distence[i] < max)
				{
					maxi = i;
					max = distence[i];
				}
			switch (maxi)
			{
				case 0:
					letter = 'A';
					break;
				case 1:
					letter = 'B';
					break;
				case 2:
					letter = 'C';
					break;
				case 3:
					letter = 'D';
					break;
				case 4:
					letter = 'E';
					break;
				case 5:
					letter = 'F';
					break;
				case 6:
					letter = 'G';
					break;
				case 7:
					letter = 'H';
					break;
				case 8:
					letter = 'I';
					break;
				case 9:
					letter = 'J';
					break;
				case 10:
					letter = 'K';
					break;
				case 11:
					letter = 'L';
					break;
				case 12:
					letter = 'M';
					break;
				case 13:
					letter = 'N';
					break;
				case 14:
					letter = 'O';
					break;
				case 15:
					letter = 'P';
					break;
				case 16:
					letter = 'Q';
					break;
				case 17:
					letter = 'R';
					break;
				case 18:
					letter = 'S';
					break;
				case 19:
					letter = 'T';
					break;
				case 20:
					letter = 'W';
					break;

			}



			button6.Enabled = true;
		}

		private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{

		}

		private void about_Click(object sender, System.EventArgs e)
		{

		}


		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{

		}

		private void label3_Click(object sender, System.EventArgs e)
		{

		}

		private void picBox1_Click(object sender, System.EventArgs e)
		{

		}
		Point pt;
		int count = 0;
		private void picBox1_MouseMove(object sender, MouseEventArgs e)
		{
			Graphics g = Graphics.FromImage(picBox1.Image);
			if (e.Button == MouseButtons.Left && count != 0)
			{
				Pen p = new Pen(Color.Black, 10);
				g.FillEllipse(new SolidBrush(Color.Black), pt.X, pt.Y, 10, 10);
				g.FillEllipse(new SolidBrush(Color.Black), e.X, e.Y, 10, 10);

				g.DrawLine(p, pt.X + 5, pt.Y + 5, e.X + 5, e.Y + 5);
			}
			pt = new Point(e.X, e.Y);
			count = 1;
			picBox1.Invalidate();
		}

		private void picBox1_MouseUp(object sender, MouseEventArgs e)
		{
			count = 0;
		}

		private void picBox1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(picBox1.Image, e.ClipRectangle);

		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(new Pen(Color.Black), picBox1.Location.X - 1, picBox1.Location.Y - 1, picBox1.Width + 1, picBox1.Height + 1);
		}

		private void button2_Click_1(object sender, System.EventArgs e)
		{
			picBox1.Image = new Bitmap(picBox1.Width + 2, picBox1.Height + 2);

		}

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

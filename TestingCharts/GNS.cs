using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;

namespace TestingCharts
{
    public partial class Form1 : Form
    {
        private Image rocketImage; // tutaj będziemy przechowywać obraz rakiety
        private float pitch = -3;   // początkowa wartość pitch
        private float roll = 0;    // początkowa wartość roll
        private float heading = 0; // początkowa wartość heading

        // Dodaj panel do rysowania
        private Panel drawingPanel;

        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.DoubleBuffered = true;
            this.Size = new Size(1920, 1080);
            this.BackColor = Color.FromArgb(255, 20, 33, 61);
            rocketImage = Image.FromFile("C:\\Users\\adria\\source\\repos\\GNS-Lik\\RocketPhoto\\Rakieta.png");
            

            
            Panel panel = new Panel
            {
                Size = new Size(640, 980),
                Location = new Point(10, 10),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle,
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel.Controls.Add(cartesianChart1);
            panel.Controls.Add(cartesianChart2);
            panel.Controls.Add(cartesianChart3);
            panel.Controls.Add(label1);
            panel.Controls.Add(Label2);
            panel.Controls.Add(label3);
            panel.Controls.Add(label10);
            panel.Controls.Add(label11);
            panel.Controls.Add(label12);

            this.Controls.Add(panel);
            

            Panel panel2 = new Panel
            {
                Size = new Size(620, 1080),
                Location = new Point(640, 0),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            this.Controls.Add(panel2);

            Panel panelLIKWIDATOR = new Panel
            {
                Size = new Size(600, 160),
                Location = new Point(20, 10),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None
            };
            //2, 62, 138
            panel2.Controls.Add(panelLIKWIDATOR);

            Panel panelRakieta = new Panel
            {
                Size = new Size(600, 700),
                Location = new Point(20, 180),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None
            };

            panel2.Controls.Add(panelRakieta);

            panelLIKWIDATOR.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panelLIKWIDATOR.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panelLIKWIDATOR.ClientRectangle,
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid); // Dolna strona
            };


            panelRakieta.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panelRakieta.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panelRakieta.ClientRectangle,
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid); // Dolna strona

                // Dodanie rysowania rakiety
                if (rocketImage != null)
                {
                    // Środek panelu
                    int centerX = panelRakieta.Width / 2;
                    int centerY = panelRakieta.Height / 2;

                    // Translacja i obrót na środku
                    e.Graphics.TranslateTransform(centerX, centerY);
                    e.Graphics.RotateTransform(pitch);   // rotacja w osi X
                    e.Graphics.RotateTransform(roll);    // rotacja w osi Z (roll)
                    e.Graphics.RotateTransform(heading); // rotacja w osi Y

                    // Rysowanie obrazu rakiety
                    e.Graphics.DrawImage(rocketImage, -rocketImage.Width / 2, -rocketImage.Height / 2);

                    // Reset transformacji
                    e.Graphics.ResetTransform();
                }
            };

            label15.Text = "Pitch:";
            label15.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label15.Location = new Point(20, 620);
            label15.TextAlign = ContentAlignment.MiddleCenter;
            label15.ForeColor = Color.White; // Kolor czcionki na biały
            label15.BackColor = Color.Transparent; // Przezroczyste tło

            label16.Text = "Roll:";
            label16.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label16.Location = new Point(220, 620);
            label16.TextAlign = ContentAlignment.MiddleCenter;
            label16.ForeColor = Color.White; // Kolor czcionki na biały
            label16.BackColor = Color.Transparent; // Przezroczyste tło

            label17.Text = "Heading:";
            label17.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label17.Location = new Point(380, 620);
            label17.TextAlign = ContentAlignment.MiddleCenter;
            label17.ForeColor = Color.White; // Kolor czcionki na biały
            label17.BackColor = Color.Transparent; // Przezroczyste tło

            label18.Text = $"{pitch}°";
            label18.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label18.Location = new Point(label15.Width + 20, 620);
            label18.TextAlign = ContentAlignment.MiddleCenter;
            label18.ForeColor = Color.White; // Kolor czcionki na biały
            label18.BackColor = Color.Transparent; // Przezroczyste tło

            label19.Text = $"{roll}°";
            label19.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label19.Location = new Point(label16.Width + 220, 620);
            label19.TextAlign = ContentAlignment.MiddleCenter;
            label19.ForeColor = Color.White; // Kolor czcionki na biały
            label19.BackColor = Color.Transparent; // Przezroczyste tło

            label20.Text = $"{heading}°";
            label20.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label20.Location = new Point(label17.Width + 380, 620);
            label20.TextAlign = ContentAlignment.MiddleCenter;
            label20.ForeColor = Color.White; // Kolor czcionki na biały
            label20.BackColor = Color.Transparent; // Przezroczyste tło

            panelRakieta.Controls.Add(label15);
            panelRakieta.Controls.Add(label16);
            panelRakieta.Controls.Add(label17);
            panelRakieta.Controls.Add(label18);
            panelRakieta.Controls.Add(label19);
            panelRakieta.Controls.Add(label20);


            button1.Text = "START!";
            button1.Size = new Size(200, 100); // Rozmiar przycisku
            button1.Location = new Point(20, 890); // Lokalizacja przycisku w panelu2
            button1.BackColor = Color.Transparent; // Kolor tła
            button1.ForeColor = Color.White; // Kolor tekstu
            button1.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            button1.FlatStyle = FlatStyle.Flat; // Zmieniamy styl na płaski
            button1.FlatAppearance.BorderSize = 3; // Ustawiamy rozmiar obramowania na 2
            button1.FlatAppearance.BorderColor = Color.FromArgb(255, 0, 255, 0);

            button1.MouseEnter += (sender, e) =>
            {
                button1.BackColor = Color.Green; // Podświetlenie na zielono po najechaniu
            };

            button1.MouseLeave += (sender, e) =>
            {
                button1.BackColor = Color.Transparent; // Powrót do przezroczystości po opuszczeniu
            };

            // Dodanie przycisku do panel2
            panel2.Controls.Add(button1);

            button2.Text = "FTS";
            button2.Size = new Size(380, 100); // Rozmiar przycisku
            button2.Location = new Point(240, 890); // Lokalizacja przycisku w panelu2
            button2.BackColor = Color.Transparent; // Kolor tła
            button2.ForeColor = Color.White; // Kolor tekstu
            button2.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            button2.FlatStyle = FlatStyle.Flat; // Zmieniamy styl na płaski
            button2.FlatAppearance.BorderSize = 3; // Ustawiamy rozmiar obramowania na 2
            button2.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 0, 0);

            button2.MouseEnter += (sender, e) =>
            {
                button2.BackColor = Color.Red; // Podświetlenie na zielono po najechaniu
            };

            button2.MouseLeave += (sender, e) =>
            {
                button2.BackColor = Color.Transparent; // Powrót do przezroczystości po opuszczeniu
            };

            // Dodanie przycisku do panel2
            panel2.Controls.Add(button2);

            Label labelLikwidator = new Label
            {
                Text = "LIKWIDATOR",
                AutoSize = true,
                Font = new Font("Times New Roman", 24, FontStyle.Bold), // Ustaw czcionkę
                ForeColor = Color.White, // Ustaw kolor tekstu
                TextAlign = ContentAlignment.MiddleCenter // Wyśrodkowanie tekstu
            };

            // Dodaj Label do paneluLIKWIDATOR
            panelLIKWIDATOR.Controls.Add(labelLikwidator);

            labelLikwidator.Location = new Point((panelLIKWIDATOR.Width - labelLikwidator.Width) / 2, 10);

            Label labelGSA = new Label
            {
                Text = "Ground Station Application",
                AutoSize = true,
                Font = new Font("Times New Roman", 24, FontStyle.Bold), // Ustaw czcionkę
                ForeColor = Color.White, // Ustaw kolor tekstu
                TextAlign = ContentAlignment.MiddleCenter // Wyśrodkowanie tekstu
            };

            panelLIKWIDATOR.Controls.Add(labelGSA);

            labelGSA.Location = new Point((panelLIKWIDATOR.Width - labelGSA.Width) / 2, 50);

            Label labelPoweredBy = new Label
            {
                Text = "Powered by",
                AutoSize = true,
                Font = new Font("Times New Roman", 14, FontStyle.Bold), // Ustaw czcionkę
                ForeColor = Color.White, // Ustaw kolor tekstu
                TextAlign = ContentAlignment.MiddleCenter // Wyśrodkowanie tekstu
            };

            panelLIKWIDATOR.Controls.Add(labelPoweredBy);

            labelPoweredBy.Location = new Point((panelLIKWIDATOR.Width - labelPoweredBy.Width) / 2, 100);

            Label labelKoloNaukowe = new Label
            {
                Text = "Koło Naukowe Studentów Lotnictwa i Kosmonautyki",
                AutoSize = true,
                Font = new Font("Times New Roman", 14, FontStyle.Bold), // Ustaw czcionkę
                ForeColor = Color.White, // Ustaw kolor tekstu
                TextAlign = ContentAlignment.MiddleCenter // Wyśrodkowanie tekstu
            };

            panelLIKWIDATOR.Controls.Add(labelKoloNaukowe);

            labelKoloNaukowe.Location = new Point((panelLIKWIDATOR.Width - labelKoloNaukowe.Width) / 2, 125);

            Panel panel3 = new Panel
            {
                Size = new Size(640, 670),
                Location = new Point(1270, 10),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel3.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel3.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel3.ClientRectangle,
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel3.Controls.Add(cartesianChart4);
            panel3.Controls.Add(cartesianChart5);
            panel3.Controls.Add(label4);
            panel3.Controls.Add(label5);
            panel3.Controls.Add(label13);
            panel3.Controls.Add(label14);
            this.Controls.Add(panel3);

            Panel panel4 = new Panel
            {
                Size = new Size(640, 130),
                Location = new Point(1270, 690),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel4.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel4.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel4.ClientRectangle,
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 2, 62, 138), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel4.Controls.Add(label6);
            panel4.Controls.Add(label7);
            panel4.Controls.Add(label8);
            panel4.Controls.Add(label9);
            this.Controls.Add(panel4);

            label1.Text = "Altitude";
            label1.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            //label1.Location = new Point(262, 20);
            label1.Location = new Point((640 - label1.Width) / 2, 10);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.ForeColor = Color.White; // Kolor czcionki na biały
            label1.BackColor = Color.Transparent; // Przezroczyste tło

            Label2.Text = "Vertical velocity";
            Label2.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            //Label2.Location = new Point(262, 330);
            Label2.Location = new Point((640 - Label2.Width) / 2, 340);
            Label2.TextAlign = ContentAlignment.MiddleCenter;
            Label2.ForeColor = Color.White; // Kolor czcionki na biały
            Label2.BackColor = Color.Transparent; // Przezroczyste tło

            label3.Text = "Vertical acceleration";
            label3.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            //label3.Location = new Point(262, 670);
            label3.Location = new Point((640 - label3.Width) / 2, 660);
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.ForeColor = Color.White; // Kolor czcionki na biały
            label3.BackColor = Color.Transparent; // Przezroczyste tło

            label4.Text = "Pressure";
            label4.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            //label4.Location = new Point(262, 20);
            label4.Location = new Point(((640 - label4.Width) / 2), 10);
            label4.TextAlign = ContentAlignment.MiddleCenter;
            label4.ForeColor = Color.White; // Kolor czcionki na biały
            label4.BackColor = Color.Transparent; // Przezroczyste tło

            label5.Text = "Satellites over ground";
            label5.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            //label5.Location = new Point(262, 330);
            label5.Location = new Point(((640 - label5.Width) / 2), 340);
            label5.TextAlign = ContentAlignment.MiddleCenter;
            label5.ForeColor = Color.White; // Kolor czcionki na biały
            label5.BackColor = Color.Transparent; // Przezroczyste tło

            label6.Text = "Latitude";
            label6.Font = new Font("Times New Roman", 28, FontStyle.Bold);
            //label3.Location = new Point(262, 670);
            label6.Location = new Point(80, 20);
            label6.TextAlign = ContentAlignment.MiddleCenter;
            label6.ForeColor = Color.White; // Kolor czcionki na biały
            label6.BackColor = Color.Transparent; // Przezroczyste tło

            label7.Text = "Longitude";
            label7.Font = new Font("Times New Roman", 28, FontStyle.Bold);
            //label3.Location = new Point(262, 670);
            label7.Location = new Point(380, 20);
            label7.TextAlign = ContentAlignment.MiddleCenter;
            label7.ForeColor = Color.White; // Kolor czcionki na biały
            label7.BackColor = Color.Transparent; // Przezroczyste tło

            label8.Text = "52°13'47.17\"N";
            label8.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //label3.Location = new Point(262, 670);
            label8.Location = new Point(80, 80);
            label8.TextAlign = ContentAlignment.MiddleCenter;
            label8.ForeColor = Color.White; // Kolor czcionki na biały
            label8.BackColor = Color.Transparent; // Przezroczyste tło

            label9.Text = "21°0'42.41\"E";
            label9.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //label3.Location = new Point(262, 670);
            label9.Location = new Point(400, 80);
            label9.TextAlign = ContentAlignment.MiddleCenter;
            label9.ForeColor = Color.White; // Kolor czcionki na biały
            label9.BackColor = Color.Transparent; // Przezroczyste tło

            label10.Text = "0 m";
            label10.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //label1.Location = new Point(262, 20);
            label10.Location = new Point((640 - label10.Width) / 2, 50);
            label10.TextAlign = ContentAlignment.MiddleCenter;
            label10.ForeColor = Color.White; // Kolor czcionki na biały
            label10.BackColor = Color.Transparent; // Przezroczyste tło

            label11.Text = "0 m/s";
            label11.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //Label2.Location = new Point(262, 330);
            label11.Location = new Point((640 - label11.Width) / 2, 380);
            label11.TextAlign = ContentAlignment.MiddleCenter;
            label11.ForeColor = Color.White; // Kolor czcionki na biały
            label11.BackColor = Color.Transparent; // Przezroczyste tło

            label12.Text = "0 m/s^2";
            label12.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //label3.Location = new Point(262, 670);
            label12.Location = new Point((640 - label12.Width) / 2, 700);
            label12.TextAlign = ContentAlignment.MiddleCenter;
            label12.ForeColor = Color.White; // Kolor czcionki na biały
            label12.BackColor = Color.Transparent; // Przezroczyste tłos

            label13.Text = "0 Pa";
            label13.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //label4.Location = new Point(262, 20);
            label13.Location = new Point(((640 - label13.Width) / 2), 50);
            label13.TextAlign = ContentAlignment.MiddleCenter;
            label13.ForeColor = Color.White; // Kolor czcionki na biały
            label13.BackColor = Color.Transparent; // Przezroczyste tło

            label14.Text = "0 m/s";
            label14.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            //label5.Location = new Point(262, 330);
            label14.Location = new Point(((640 - label14.Width) / 2), 380);
            label14.TextAlign = ContentAlignment.MiddleCenter;
            label14.ForeColor = Color.White; // Kolor czcionki na biały
            label14.BackColor = Color.Transparent; // Przezroczyste tło

            cartesianChart1.Size = new Size(600, 250);
            cartesianChart1.Location = new Point(12, 90);
            cartesianChart1.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart1.BackColor = Color.Transparent;

            cartesianChart2.Size = new Size(600, 250);
            cartesianChart2.Location = new Point(12, 410);
            cartesianChart2.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart2.BackColor = Color.Transparent;

            cartesianChart3.Size = new Size(600, 250);
            cartesianChart3.Location = new Point(12, 730);
            cartesianChart3.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart3.BackColor = Color.Transparent;

            cartesianChart4.Size = new Size(600, 250);
            cartesianChart4.Location = new Point(20, 90);
            cartesianChart4.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart4.BackColor = Color.Transparent;

            cartesianChart5.Size = new Size(600, 250);
            cartesianChart5.Location = new Point(20, 410);
            cartesianChart5.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart5.BackColor = Color.Transparent;

            /*
            cartesianChart6.Size = new Size(600, 250);
            cartesianChart6.Location = new Point(1300, 750);
            cartesianChart6.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart6.BackColor = Color.Transparent;
            */

            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18,

            });

            cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Y Axis",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });

            cartesianChart2.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18

            });

            cartesianChart2.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Y Axis",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });

            cartesianChart3.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18

            });

            cartesianChart3.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Y Axis",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });

            cartesianChart4.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18

            });

            cartesianChart4.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Y Axis",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });

            cartesianChart5.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18

            });

            cartesianChart5.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Y Axis",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });

            /*
            cartesianChart6.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18

            });

            cartesianChart6.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Y Axis",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });
            */

            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,0),
                        new ObservablePoint(1,1),
                        new ObservablePoint(2,2),
                        new ObservablePoint(3,3),
                        new ObservablePoint(4,4),
                        new ObservablePoint(5,5),
                        new ObservablePoint(6,6),
                        new ObservablePoint(7,7),
                        new ObservablePoint(8,8),
                        new ObservablePoint(9,9),
                        new ObservablePoint(10,20)
                    },
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 220, 20, 60))
                }
            };
            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,10),
                        new ObservablePoint(4,7),
                        new ObservablePoint(5,3),
                        new ObservablePoint(7,6),
                        new ObservablePoint(10,8)
                    },
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 30, 144, 255))
                }
            };
            cartesianChart3.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,10),
                        new ObservablePoint(3,7),
                        new ObservablePoint(5,2),
                        new ObservablePoint(7,6),
                        new ObservablePoint(15,9)
                    },
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 124, 252, 0))
                }
            };
            cartesianChart4.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,0),
                        new ObservablePoint(1,2),
                        new ObservablePoint(2,4),
                        new ObservablePoint(3,8),
                        new ObservablePoint(4,16),
                        new ObservablePoint(5,32),
                        new ObservablePoint(6,64),
                        new ObservablePoint(7,128),
                        new ObservablePoint(8,256),
                        new ObservablePoint(9,512),
                        new ObservablePoint(10,1028)
                    },
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 255, 255, 0))
                }
            };
            cartesianChart5.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,0),
                        new ObservablePoint(1,2),
                        new ObservablePoint(2,6),
                        new ObservablePoint(3,10),
                        new ObservablePoint(4,30),
                        new ObservablePoint(5,70),
                        new ObservablePoint(6,90),
                        new ObservablePoint(7,110),
                        new ObservablePoint(8,180),
                        new ObservablePoint(9,200),
                        new ObservablePoint(10,180)
                    },
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 255, 165, 0))
                }
            };
            /*
            cartesianChart6.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,0),
                        new ObservablePoint(1,1),
                        new ObservablePoint(2,2),
                        new ObservablePoint(3,3),
                        new ObservablePoint(4,4),
                        new ObservablePoint(5,5),
                        new ObservablePoint(6,6),
                        new ObservablePoint(7,7),
                        new ObservablePoint(8,8),
                        new ObservablePoint(9,9),
                        new ObservablePoint(10,20)
                    },
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 220, 20, 60))
                }
            };
            */
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cartesianChart2_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (rocketImage != null)
            {
                Graphics g = e.Graphics;

                // Wyliczasz połączoną rotację

                // Ustawienie środka obrazu do obrotu
                g.TranslateTransform(this.ClientSize.Width / 2, this.ClientSize.Height / 2);

                // Zastosowanie obrotów
                g.RotateTransform(pitch);
                g.RotateTransform(roll);
                g.RotateTransform(heading);

                // Rysowanie rakiety
                //g.DrawImage(rocketImage, -rocketImage.Width / 2, -rocketImage.Height / 2);

                if (rocketImage != null)
                {   
                    //g.DrawImage(rocketImage, -rocketImage.Width / 2, -rocketImage.Height / 2);
                }
                else
                {
                    g.DrawString("Obraz rakiety nie został załadowany.", this.Font, Brushes.White, new PointF(10, 10));
                }

                // Resetowanie transformacji
                g.ResetTransform();
            }
            else
            {
                MessageBox.Show("Obraz rakiety nie został wczytany!");
            }
        }
    }
}



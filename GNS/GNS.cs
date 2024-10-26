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

using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Windows.Forms.Integration;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using OpenTK.Graphics;
using OpenTK.Input;
using System.IO;
//using System.Windows.Media;


namespace GNS
{
    public partial class GNS : Form
    {
        private float pitch = 0;   // początkowa wartość pitch
        private float roll = 0;    // początkowa wartość roll
        private float heading = 0; // początkowa wartość heading

        private GameWindow gameWindow;

        private GMapControl gMapControl;

        private ElementHost host;
        private HelixViewport3D viewport;

        public GNS()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(1920, 1080);
            this.BackColor = Color.FromArgb(255, 20, 33, 61);
            this.Load += new EventHandler(GNS_Load); // Dodanie zdarzenia Load
            
            host = new ElementHost();
            host.Size = new Size(622, 542);
            host.Location = new Point(4, 4);

            viewport = new HelixViewport3D();
            host.Child = viewport;
            //LoadRocketModel("C:\\Users\\fs24f\\source\\repos\\GNS\\RocketPhoto\\12217_rocket_v1_l1.obj");

            Panel panel = new Panel
            {
                Size = new Size(630, 350),
                Location = new Point(10, 10),
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
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel.Controls.Add(cartesianChart1);
            panel.Controls.Add(label1);
            panel.Controls.Add(label10);

            this.Controls.Add(panel);

            Panel panel2 = new Panel
            {
                Size = new Size(305, 130),
                Location = new Point(1605, 860),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel2.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel2.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel2.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel2.Controls.Add(label7);
            panel2.Controls.Add(label9);
            this.Controls.Add(panel2);

            Panel panelRakieta = new Panel
            {
                Size = new Size(630, 550),
                Location = new Point(650, 370),
                //BackColor = Color.FromArgb(255, 237, 237, 237),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None
            };


            this.Controls.Add(panelRakieta);

            panelRakieta.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panelRakieta.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panelRakieta.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panelRakieta.Controls.Add(host);

            label15.Text = "Pitch:";
            label15.Font = new Font("Times New Roman", 20, FontStyle.Bold);
            label15.Location = new Point(30, 15);
            label15.TextAlign = ContentAlignment.MiddleCenter;
            label15.ForeColor = Color.White; // Kolor czcionki na biały
            label15.BackColor = Color.Transparent; // Przezroczyste tło

            label16.Text = "Roll:";
            label16.Font = new Font("Times New Roman", 20, FontStyle.Bold);
            label16.Location = new Point(40, 15);
            label16.TextAlign = ContentAlignment.MiddleCenter;
            label16.ForeColor = Color.White; // Kolor czcionki na biały
            label16.BackColor = Color.Transparent; // Przezroczyste tło

            label17.Text = "Heading:";
            label17.Font = new Font("Times New Roman", 20, FontStyle.Bold);
            label17.Location = new Point(20, 15);
            label17.TextAlign = ContentAlignment.MiddleCenter;
            label17.ForeColor = Color.White; // Kolor czcionki na biały
            label17.BackColor = Color.Transparent; // Przezroczyste tło

            label18.Text = $"{pitch}°";
            label18.Font = new Font("Times New Roman", 20, FontStyle.Bold);
            label18.Location = new Point(label15.Width + 30, 15);
            label18.TextAlign = ContentAlignment.MiddleCenter;
            label18.ForeColor = Color.White; // Kolor czcionki na biały
            label18.BackColor = Color.Transparent; // Przezroczyste tło

            label19.Text = $"{roll}°";
            label19.Font = new Font("Times New Roman", 20, FontStyle.Bold);
            label19.Location = new Point(label16.Width + 40, 15);
            label19.TextAlign = ContentAlignment.MiddleCenter;
            label19.ForeColor = Color.White; // Kolor czcionki na biały
            label19.BackColor = Color.Transparent; // Przezroczyste tło

            label20.Text = $"{heading}°";
            label20.Font = new Font("Times New Roman", 20, FontStyle.Bold);
            label20.Location = new Point(label17.Width + 20, 15);
            label20.TextAlign = ContentAlignment.MiddleCenter;
            label20.ForeColor = Color.White; // Kolor czcionki na biały
            label20.BackColor = Color.Transparent; // Przezroczyste tło

            Panel panel3 = new Panel
            {
                Size = new Size(620, 480),
                Location = new Point(1290, 370),
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

                // 70, 103, 195
                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel3.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            gMapControl = new GMapControl
            {
                Size = new Size(612, 472),
                Location = new Point(4, 4),
                //Dock = DockStyle.Fill // Wypełni cały formularz
            };

            // Ustawienia GMapControl
            gMapControl.MapProvider = GMapProviders.GoogleMap; // Możesz zmienić na inny dostawca
            GMaps.Instance.Mode = AccessMode.ServerAndCache; // Używamy trybu serwerowego i pamięci podręcznej
            double lat = 52.2297;
            double lng = 21.0122;
            gMapControl.Position = new PointLatLng(lat, lng); // Ustawienie pozycji (Warszawa)
            gMapControl.MinZoom = 5;
            gMapControl.MaxZoom = 100;
            gMapControl.Zoom = 15;

            PointLatLng point = new PointLatLng(lat, lng);

            // Tworzymy znacznik (pineskę) na mapie
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);

            // Tworzymy nakładkę (overlay) na mapie, w której będą przechowywane wszystkie znaczniki
            GMapOverlay markersOverlay = new GMapOverlay("markers");

            // Dodajemy znacznik do nakładki
            markersOverlay.Markers.Add(marker);

            // Dodajemy nakładkę do kontrolki mapy
            gMapControl.Overlays.Add(markersOverlay);

            // Dodanie kontrolki do formularza
            panel3.Controls.Add(gMapControl);

            this.Controls.Add(panel3);

            Panel panel4 = new Panel
            {
                Size = new Size(305, 130),
                Location = new Point(1290, 860),
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel4.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel4.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);
                // 2, 62, 138
                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel4.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel4.Controls.Add(label6);
            panel4.Controls.Add(label8);
            this.Controls.Add(panel4);

            Panel panel5 = new Panel
            {
                Size = new Size(630, 620),
                Location = new Point(10, 370),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel5.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel5.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel5.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            this.Controls.Add(panel5);

            Panel panel6 = new Panel
            {
                Size = new Size(630, 350),
                Location = new Point(650, 10),
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel6.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel6.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel6.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel6.Controls.Add(cartesianChart2);
            panel6.Controls.Add(label2);
            panel6.Controls.Add(label11);
            this.Controls.Add(panel6);

            Panel panel7 = new Panel
            {
                Size = new Size(620, 350),
                Location = new Point(1290, 10),
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel7.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel7.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel7.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel7.Controls.Add(cartesianChart3);
            panel7.Controls.Add(label3);
            panel7.Controls.Add(label12);
            this.Controls.Add(panel7);

            Panel panel8 = new Panel
            {
                Size = new Size(200, 60),
                Location = new Point(650, 930),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel8.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel8.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel8.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel8.Controls.Add(label15);
            panel8.Controls.Add(label18);
            this.Controls.Add(panel8);

            Panel panel9 = new Panel
            {
                Size = new Size(200, 60),
                Location = new Point(860, 930),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel9.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel9.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel9.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel9.Controls.Add(label16);
            panel9.Controls.Add(label19);

            this.Controls.Add(panel9);

            Panel panel10 = new Panel
            {
                Size = new Size(210, 60),
                Location = new Point(1070, 930),
                AutoScroll = true,
                BackColor = Color.Transparent,
            };

            panel10.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel10.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel10.ClientRectangle,
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel10.Controls.Add(label17);
            panel10.Controls.Add(label20);
            this.Controls.Add(panel10);

            label1.Text = "Vertical velocity";
            label1.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label1.Location = new Point((panel.Width - label1.Width) / 2, 10);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.ForeColor = Color.White; // Kolor czcionki na biały
            label1.BackColor = Color.Transparent; // Przezroczyste tło

            label2.Text = "Vertical acceleration";
            label2.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label2.Location = new Point(((panel6.Width - label2.Width) / 2), 10);
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.ForeColor = Color.White; // Kolor czcionki na biały
            label2.BackColor = Color.Transparent; // Przezroczyste tło

            label3.Text = "Satellites over ground";
            label3.Font = new Font("Times New Roman", 24, FontStyle.Bold);
            label3.Location = new Point(((panel7.Width - label3.Width) / 2), 10);
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.ForeColor = Color.White; // Kolor czcionki na biały
            label3.BackColor = Color.Transparent; // Przezroczyste tło

            label6.Text = "Latitude";
            label6.Font = new Font("Times New Roman", 28, FontStyle.Bold);
            label6.Location = new Point((panel4.Width - label6.Width) / 2, 20);
            label6.TextAlign = ContentAlignment.MiddleCenter;
            label6.ForeColor = Color.White; // Kolor czcionki na biały
            label6.BackColor = Color.Transparent; // Przezroczyste tło

            label7.Text = "Longitude";
            label7.Font = new Font("Times New Roman", 28, FontStyle.Bold);
            label7.Location = new Point((panel2.Width - label7.Width) / 2, 20);
            label7.TextAlign = ContentAlignment.MiddleCenter;
            label7.ForeColor = Color.White; // Kolor czcionki na biały
            label7.BackColor = Color.Transparent; // Przezroczyste tło

            label8.Text = "52°13'47.17\"N";
            label8.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            label8.Location = new Point((panel4.Width - label8.Width) / 2, 80);
            label8.TextAlign = ContentAlignment.MiddleCenter;
            label8.ForeColor = Color.White; // Kolor czcionki na biały
            label8.BackColor = Color.Transparent; // Przezroczyste tło

            label9.Text = "21°0'42.41\"E";
            label9.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            label9.Location = new Point((panel2.Width - label9.Width) / 2, 80);
            label9.TextAlign = ContentAlignment.MiddleCenter;
            label9.ForeColor = Color.White; // Kolor czcionki na biały
            label9.BackColor = Color.Transparent; // Przezroczyste tło

            label10.Text = "0 m/s";
            label10.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            label10.Location = new Point(((panel.Width - label10.Width) / 2), 50);
            label10.TextAlign = ContentAlignment.MiddleCenter;
            label10.ForeColor = Color.White; // Kolor czcionki na biały
            label10.BackColor = Color.Transparent; // Przezroczyste tło

            label11.Text = "0 m/s^2";
            label11.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            label11.Location = new Point(((panel6.Width - label11.Width) / 2), 50);
            label11.TextAlign = ContentAlignment.MiddleCenter;
            label11.ForeColor = Color.White; // Kolor czcionki na biały
            label11.BackColor = Color.Transparent; // Przezroczyste tło

            label12.Text = "0";
            label12.Font = new Font("Times New Roman", 18, FontStyle.Bold);
            label12.Location = new Point(((panel7.Width - label12.Width) / 2), 50);
            label12.TextAlign = ContentAlignment.MiddleCenter;
            label12.ForeColor = Color.White; // Kolor czcionki na biały
            label12.BackColor = Color.Transparent; // Przezroczyste tłos

            cartesianChart1.Size = new Size(600, 250);
            cartesianChart1.Location = new Point(((panel.Width - cartesianChart1.Width) / 2), 90);
            cartesianChart1.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart1.BackColor = Color.Transparent;

            cartesianChart2.Size = new Size(600, 250);
            cartesianChart2.Location = new Point(((panel6.Width - cartesianChart2.Width) / 2), 90);
            cartesianChart2.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart2.BackColor = Color.Transparent;

            cartesianChart3.Size = new Size(600, 250);
            cartesianChart3.Location = new Point(((panel7.Width - cartesianChart3.Width) / 2), 90);
            cartesianChart3.Font = new Font("Times New Roman", 14, FontStyle.Regular);
            cartesianChart3.BackColor = Color.Transparent;

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

            ElementHost elementHost = new ElementHost
            {
                Size = new Size(622, 612),
                Location = new Point(4, 4),
            };

            // Tworzymy widok 3D
            HelixViewport3D helixViewport = new HelixViewport3D();

            // Ustawiamy kamerę
            helixViewport.Camera = new PerspectiveCamera
            {
                Position = new Point3D(10, 10, 10),
                LookDirection = new Vector3D(-10, -10, -10),
                UpDirection = new Vector3D(0, 0, 1),
                FieldOfView = 45
            };

            helixViewport.Children.Add(new GridLinesVisual3D
            {
                Width = 40,
                Length = 40,
                MinorDistance = 1,
                MajorDistance = 5
            });


            var pointsCollection = new Point3DCollection();

            // Tworzymy 100 punktów od (0, 0, 0) do (99, 99, 99)
            for (double i = 0; i < 100; i += 0.1)
            {
                pointsCollection.Add(new Point3D(0, 0, i));
            }

            // Ustawiamy wizualizację punktów
            var pointsVisual = new PointsVisual3D
            {
                Color = System.Windows.Media.Colors.Red,
                Size = 5,
                Points = new Point3DCollection() // Pusta kolekcja, aby dodawać punkty jeden po drugim
            };

            helixViewport.Children.Add(pointsVisual);

            // Przypisujemy HelixViewport do elementu hostującego
            elementHost.Child = helixViewport;

            // Dodajemy element hostujący do formularza
            panel5.Controls.Add(elementHost);

            var timer = new Timer { Interval = 1000 }; // 1000ms = 1 sekunda
            int currentPointIndex = 0;

            // Timer tick event
            timer.Tick += (sender, e) =>
            {
                if (currentPointIndex < pointsCollection.Count)
                {
                    // Dodajemy kolejny punkt do wizualizacji
                    pointsVisual.Points.Add(pointsCollection[currentPointIndex]);
                    currentPointIndex++; // Przesuwamy do następnego punktu
                }
                else
                {
                    // Zatrzymujemy timer, jeśli wszystkie punkty zostały wyświetlone
                    timer.Stop();
                }
            };

            timer.Start(); // Rozpoczynamy timer

        }

        private void GNS_Load(object sender, EventArgs e)
        {
            LoadRocketModel("Resources\\RocketPhoto\\12217_rocket_v1_l1.obj");
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


        private void LoadRocketModel(string path)
        {

            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RocketPhoto", "12217_rocket_v1_l1.obj");

            if (!File.Exists(modelPath))
            {
                MessageBox.Show("Nie znaleziono pliku: " + modelPath);
                return;
            }

            var importer = new ModelImporter();
            Model3D model = importer.Load(modelPath);

            var whiteMaterial = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255)));

            if (model is Model3DGroup modelGroup)
            {
                foreach (var child in modelGroup.Children)
                {
                    if (child is GeometryModel3D geometryModel)
                    {
                        geometryModel.Material = null;
                        geometryModel.Material = whiteMaterial;
                    }
                }
            }
            else if (model is GeometryModel3D geometryModel)
            {
                geometryModel.Material = null;
                geometryModel.Material = whiteMaterial;
            }

            // Dodaj model do widoku
            var modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = model;

            // Skonfiguruj grupę transformacji
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new ScaleTransform3D(0.0012, 0.0012, 0.0012)); // Skalowanie
            modelVisual3D.Transform = transformGroup;

            viewport.Children.Add(modelVisual3D);

            // Ustaw kamerę
            viewport.Camera.Position = new Point3D(2, 2, 2.5);
            viewport.Camera.LookDirection = new Vector3D(-1, -1, -1);
            viewport.Camera.UpDirection = new Vector3D(0, 0, 1);

            UpdateRocketOrientation(modelVisual3D); // Aktualizacja orientacji
        }

        private void UpdateRocketOrientation(ModelVisual3D modelVisual3D)
        {
            double pitch = GetPitchValue();
            double roll = GetRollValue();
            double heading = GetHeadingValue();

            var pitchRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), pitch);
            var rollRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), roll);
            var headingRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), heading);

            var rotationGroup = new Transform3DGroup();
            rotationGroup.Children.Add(new RotateTransform3D(pitchRotation));
            rotationGroup.Children.Add(new RotateTransform3D(headingRotation));
            rotationGroup.Children.Add(new RotateTransform3D(rollRotation));

            // Wyłącza działanie myszki
            viewport.IsEnabled = false;

            // Dodaj rotację do istniejącej grupy transformacji
            var transformGroup = (Transform3DGroup)modelVisual3D.Transform;
            if (transformGroup.Children.Count > 1)
            {
                transformGroup.Children[1] = rotationGroup;
            }
            else
            {
                transformGroup.Children.Add(rotationGroup);
            }
        }

        // Przykładowe funkcje pobierające wartości pitch, roll, heading
        private double GetPitchValue() { return pitch; }
        private double GetRollValue() { return roll; }
        private double GetHeadingValue() { return heading; }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

    }
}
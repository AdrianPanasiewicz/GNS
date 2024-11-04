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
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using System.Threading;
using static GMap.NET.Entity.OpenStreetMapRouteEntity;
using System.Windows.Media;
using SerialCom;

namespace GNS
{
    public partial class GNS : Form
    {
        private double pitch = 0;
        private double roll = 0;
        private double heading = 0;

        private double lat = 0;
        private double lng = 0;

        private GameWindow gameWindow;

        private GMapControl gMapControl;

        private ElementHost host;
        private HelixViewport3D viewport;

        private ConcurrentQueue<TelemetryData> telemetryDataQueue;
        private SeriesCollection seriesCollection1, seriesCollection2, seriesCollection3;

        private ModelVisual3D _RocketModel;

        private string _RocketFilePath;

        public GNS()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(1920, 1080);
            this.BackColor = System.Drawing.Color.FromArgb(255, 20, 33, 61);
            //this.Load += new EventHandler(GNS_Load); // Dodanie zdarzenia Load

            telemetryDataQueue = new ConcurrentQueue<TelemetryData>();

            host = new ElementHost();
            host.Size = new Size(622, 542);
            host.Location = new Point(4, 4);

            viewport = new HelixViewport3D();
            host.Child = viewport;

            // Znajdz sciezke do przestrzeni roboczej
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            this._RocketFilePath = projectDirectory + "\\GNS\\Resources\\RocketPhoto\\LIKWIDATOR_Assembly.obj";

            LoadRocketModel();

            // Create and define the axis lines
            var xAxis = new LinesVisual3D
            {
                Color = Colors.Red,
                Thickness = 2,
                Points = new Point3DCollection
                {
                    new Point3D(-0.5, -0.5, -2),
                    new Point3D(-0.2, -0.5, -2) 
                }
            };

            var yAxis = new LinesVisual3D
            {
                Color = Colors.Green,
                Thickness = 2,
                Points = new Point3DCollection
        {
            new Point3D(-0.5, -0.5, -2),
            new Point3D(-0.5, -0.2, -2)
        }
            };

            var zAxis = new LinesVisual3D
            {
                Color = Colors.Blue,
                Thickness = 2,
                Points = new Point3DCollection
        {
            new Point3D(-0.5, -0.5, -2),
            new Point3D(-0.5, -0.5, -1.7) 
        }
            };

            // Add the axes to the viewport
            viewport.Children.Add(xAxis);
            viewport.Children.Add(yAxis);
            viewport.Children.Add(zAxis);

            Panel panel = new Panel
            {
                Size = new Size(630, 350),
                Location = new Point(10, 10),
                AutoScroll = true,
                BackColor = System.Drawing.Color.Transparent,
            };

            panel.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
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
                BackColor = System.Drawing.Color.Transparent,
            };

            panel2.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel2.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel2.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel2.Controls.Add(label7);
            panel2.Controls.Add(label9);
            this.Controls.Add(panel2);

            Panel panelRakieta = new Panel
            {
                Size = new Size(630, 550),
                Location = new Point(650, 370),
                //BackColor = Color.FromArgb(255, 237, 237, 237),
                BackColor = System.Drawing.Color.Transparent,
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
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panelRakieta.Controls.Add(host);

            label15.Text = "Pitch:";
            label15.Font = new Font("Aptos", 20, FontStyle.Bold);
            label15.Location = new Point(30, 15);
            label15.TextAlign = ContentAlignment.MiddleCenter;
            label15.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label15.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label16.Text = "Roll:";
            label16.Font = new Font("Aptos", 20, FontStyle.Bold);
            label16.Location = new Point(40, 15);
            label16.TextAlign = ContentAlignment.MiddleCenter;
            label16.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label16.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label17.Text = "Heading:";
            label17.Font = new Font("Aptos", 20, FontStyle.Bold);
            label17.Location = new Point(20, 15);
            label17.TextAlign = ContentAlignment.MiddleCenter;
            label17.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label17.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label18.Text = $"{pitch}°";
            label18.Font = new Font("Aptos", 20, FontStyle.Bold);
            label18.Location = new Point(label15.Width + 30, 15);
            label18.TextAlign = ContentAlignment.MiddleCenter;
            label18.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label18.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label19.Text = $"{roll}°";
            label19.Font = new Font("Aptos", 20, FontStyle.Bold);
            label19.Location = new Point(label16.Width + 40, 15);
            label19.TextAlign = ContentAlignment.MiddleCenter;
            label19.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label19.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label20.Text = $"{heading}°";
            label20.Font = new Font("Aptos", 20, FontStyle.Bold);
            label20.Location = new Point(label17.Width + 20, 15);
            label20.TextAlign = ContentAlignment.MiddleCenter;
            label20.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label20.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            Panel panel3 = new Panel
            {
                Size = new Size(620, 480),
                Location = new Point(1290, 370),
                //Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.Transparent,
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
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
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
                BackColor = System.Drawing.Color.Transparent,
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
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
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
                BackColor = System.Drawing.Color.Transparent,
            };

            panel5.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel5.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel5.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            this.Controls.Add(panel5);

            Panel panel6 = new Panel
            {
                Size = new Size(630, 350),
                Location = new Point(650, 10),
                AutoScroll = true,
                BackColor = System.Drawing.Color.Transparent,
            };

            panel6.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel6.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel6.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
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
                BackColor = System.Drawing.Color.Transparent,
            };

            panel7.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel7.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel7.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
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
                BackColor = System.Drawing.Color.Transparent,
            };

            panel8.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel8.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel8.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
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
                BackColor = System.Drawing.Color.Transparent,
            };

            panel9.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel9.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel9.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel9.Controls.Add(label16);
            panel9.Controls.Add(label19);

            this.Controls.Add(panel9);

            Panel panel10 = new Panel
            {
                Size = new Size(210, 60),
                Location = new Point(1070, 930),
                AutoScroll = true,
                BackColor = System.Drawing.Color.Transparent,
            };

            panel10.Paint += (sender, e) =>
            {
                // Prostokąt panelu
                Rectangle rect = panel10.ClientRectangle;

                // Zmniejsz rozmiar prostokąta o 1 piksel, aby upewnić się, że obramowanie będzie rysowane wewnątrz krawędzi
                rect.Inflate(-1, -1);

                // Rysowanie obramowania o grubości 2 pikseli wokół całego panelu
                ControlPaint.DrawBorder(e.Graphics, panel10.ClientRectangle,
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Lewa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Górna strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid,  // Prawa strona
                    System.Drawing.Color.FromArgb(255, 70, 103, 195), 4, ButtonBorderStyle.Solid); // Dolna strona
            };

            panel10.Controls.Add(label17);
            panel10.Controls.Add(label20);
            this.Controls.Add(panel10);

            label1.Text = "Vertical velocity";
            label1.Font = new Font("Aptos", 24, FontStyle.Bold);
            label1.Location = new Point((panel.Width - label1.Width) / 2, 10);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label1.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label2.Text = "Vertical acceleration";
            label2.Font = new Font("Aptos", 24, FontStyle.Bold);
            label2.Location = new Point(((panel6.Width - label2.Width) / 2), 10);
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label2.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label3.Text = "GPS Altitude";
            label3.Font = new Font("Aptos", 24, FontStyle.Bold);
            label3.Location = new Point(((panel7.Width - label3.Width) / 2), 10);
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label3.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label6.Text = "Latitude";
            label6.Font = new Font("Aptos", 28, FontStyle.Bold);
            label6.Location = new Point((panel4.Width - label6.Width) / 2, 20);
            label6.TextAlign = ContentAlignment.MiddleCenter;
            label6.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label6.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label7.Text = "Longitude";
            label7.Font = new Font("Aptos", 28, FontStyle.Bold);
            label7.Location = new Point((panel2.Width - label7.Width) / 2, 20);
            label7.TextAlign = ContentAlignment.MiddleCenter;
            label7.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label7.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label8.Text = "52°13'47.17\"N";
            label8.Font = new Font("Aptos", 18, FontStyle.Bold);
            label8.Location = new Point((panel4.Width - label8.Width) / 2, 80);
            label8.TextAlign = ContentAlignment.MiddleCenter;
            label8.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label8.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label9.Text = "21°0'42.41\"E";
            label9.Font = new Font("Aptos", 18, FontStyle.Bold);
            label9.Location = new Point((panel2.Width - label9.Width) / 2, 80);
            label9.TextAlign = ContentAlignment.MiddleCenter;
            label9.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label9.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label10.Text = "0 m/s";
            label10.Font = new Font("Aptos", 18, FontStyle.Bold);
            label10.Location = new Point(((panel.Width - label10.Width - 45) / 2), 50);
            label10.TextAlign = ContentAlignment.MiddleCenter;
            label10.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label10.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label11.Text = "0 m/s^2";
            label11.Font = new Font("Aptos", 18, FontStyle.Bold);
            label11.Location = new Point(((panel6.Width - label11.Width - 45) / 2), 50);
            label11.TextAlign = ContentAlignment.MiddleCenter;
            label11.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label11.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tło

            label12.Text = "0 m";
            label12.Font = new Font("Aptos", 18, FontStyle.Bold);
            label12.Location = new Point(((panel7.Width - label12.Width - 45) / 2), 50);
            label12.TextAlign = ContentAlignment.MiddleCenter;
            label12.ForeColor = System.Drawing.Color.White; // Kolor czcionki na biały
            label12.BackColor = System.Drawing.Color.Transparent; // Przezroczyste tłos

            cartesianChart1.Size = new Size(600, 250);
            cartesianChart1.Location = new Point(((panel.Width - cartesianChart1.Width) / 2), 90);
            cartesianChart1.Font = new Font("Aptos", 14, FontStyle.Regular);
            cartesianChart1.BackColor = System.Drawing.Color.Transparent;

            cartesianChart2.Size = new Size(600, 250);
            cartesianChart2.Location = new Point(((panel6.Width - cartesianChart2.Width) / 2), 90);
            cartesianChart2.Font = new Font("Aptos", 14, FontStyle.Regular);
            cartesianChart2.BackColor = System.Drawing.Color.Transparent;

            cartesianChart3.Size = new Size(600, 250);
            cartesianChart3.Location = new Point(((panel7.Width - cartesianChart3.Width) / 2), 90);
            cartesianChart3.Font = new Font("Aptos", 14, FontStyle.Regular);
            cartesianChart3.BackColor = System.Drawing.Color.Transparent;

            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "t[s]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi X
                FontSize = 18,

            });

            cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "v[m/s]",
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
                Title = "a[m/s^2]",
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
                Title = "h[m]",
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White), // Kolor osi Y
                FontSize = 18
            });


            cartesianChart1.Series = seriesCollection1 = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 220, 20, 60)),
                    LineSmoothness = 0

                }
            };
            cartesianChart2.Series = seriesCollection2 = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 30, 144, 255)),
                    LineSmoothness = 0
                }
            };
            cartesianChart3.Series = seriesCollection3 = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometrySize = 12,
                    Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 124, 252, 0)),
                    LineSmoothness = 0
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



            // Uruchomienie wątku do aktualizacji wykresu
            Thread guiUpdateThread = new Thread(UpdateChartLoop);
            guiUpdateThread.IsBackground = true;
            guiUpdateThread.Start();


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

            var timer = new System.Windows.Forms.Timer { Interval = 1000 }; // 1000ms = 1 sekunda
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
            // LoadRocketModel("RocketPhoto/12217_rocket_v1_l1.obj");
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

        private void LoadRocketModel()
        {

            if (!File.Exists(_RocketFilePath))
            {
                MessageBox.Show("Nie znaleziono pliku: " + _RocketFilePath);
                return;
            }

            var importer = new ModelImporter();
            Model3D model = importer.Load(_RocketFilePath); // ModelImporter automatycznie załaduje plik .mtl, jeśli jest w tej samej lokalizacji i .obj na niego wskazuje

            // Utwórz model 3D do wyświetlenia
            _RocketModel = new ModelVisual3D { Content = model };

            // Skonfiguruj grupę transformacji
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new ScaleTransform3D(0.0012, 0.0012, 0.0012)); // Skalowanie modelu
            _RocketModel.Transform = transformGroup;

            // Dodaj model do widoku
            viewport.Children.Add(_RocketModel);

            // Ustaw kamerę
            viewport.Camera.Position = new Point3D(2, 2, 2.5);
            viewport.Camera.LookDirection = new Vector3D(-1, -1, -1);
            viewport.Camera.UpDirection = new Vector3D(0, 0, 1);

            UpdateRocketOrientation(); // Aktualizacja orientacji
        }

        private void UpdateRocketOrientation()
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
            var transformGroup = (Transform3DGroup)_RocketModel.Transform;
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

        /// <summary>
        /// Funkcja do regularnej aktualizacji wykresu
        /// </summary>
        private void UpdateChartLoop()
        {
            DateTime _startDateTime = DateTime.Now;
            DateTime _nowDateTime = DateTime.Now;
            double _timestamp = 0;

            while (true)
            {
                if (telemetryDataQueue.TryDequeue(out TelemetryData telemetryPacket))
                {
                    // Zaktualizuj wykres w bezpieczny dla wątków sposób
                    this.Invoke(new Action(() =>
                    {
                        // Oblicz timestamp od uruchomienie programu
                        _nowDateTime = DateTime.Now;
                        _timestamp = (_nowDateTime - _startDateTime).TotalSeconds;

                        // Wyslij punkt do wykresow
                        seriesCollection1[0].Values.Add(new ObservablePoint(_timestamp, telemetryPacket.Baro.VerticalVelocity));
                        seriesCollection2[0].Values.Add(new ObservablePoint(_timestamp, telemetryPacket.Baro.AccZInertial));  // Dopytac sie o ktora predkosc chodzi
                        seriesCollection3[0].Values.Add(new ObservablePoint(_timestamp, telemetryPacket.Baro.Altitude));

                        // Trzymaj tylko 100 najnowszych punktow na wykresie
                        if (seriesCollection1[0].Values.Count > 100) seriesCollection1[0].Values.RemoveAt(0);
                        if (seriesCollection2[0].Values.Count > 100) seriesCollection2[0].Values.RemoveAt(0);
                        if (seriesCollection3[0].Values.Count > 100) seriesCollection3[0].Values.RemoveAt(0);


                        label10.Text = (telemetryPacket.Baro.VerticalVelocity).ToString() + " m/s";
                        label11.Text = (telemetryPacket.Baro.AccZInertial).ToString() + " m/s^2";
                        label12.Text = (telemetryPacket.Baro.Altitude).ToString() + " m";



                        // Zakutalizuj wartosci obrotu rakiety
                        pitch = telemetryPacket.IMU.Pitch;
                        roll = telemetryPacket.IMU.Roll;
                        heading = telemetryPacket.IMU.Heading;

                        // Change
                        // Wyswietl wartosci obrotu rakiety na GUI
                        label18.Text = $"{(int)pitch}°";
                        label19.Text = $"{(int)roll}°";
                        label20.Text = $"{(int)heading}°";

                        lat = telemetryPacket.GPS.Latitude;
                        lng = telemetryPacket.GPS.Longitude;
                        gMapControl.Position = new PointLatLng(lat, lng); // Ustawienie pozycji (Warszawa)

                        // Przeslij nowe wspolrzedne #TODO 
                        label8.Text = telemetryPacket.GPS.Latitude + "° N";
                        label9.Text = telemetryPacket.GPS.Longitude + "° E";

                        UpdateRocketOrientation();

                    }));
                }

                // Odśwież wykres co 100 ms
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Funkcja do dodawania nowego punktu telemetrycznego do kolejki
        /// </summary>
        /// <param name="time"></param>
        /// <param name="telemetryPacket"></param>
        public void AddTelemetryDataPoint(TelemetryData telemetryPacket)
        {
            telemetryDataQueue.Enqueue(telemetryPacket);
        }

    }
}
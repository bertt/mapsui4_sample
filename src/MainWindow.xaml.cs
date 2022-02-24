using Dapper;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Windows;

namespace mapsui4
{
    public partial class MainWindow : Window
    {
        private string db = @"ukraine.sqlite";
        public MainWindow()
        {
            InitializeComponent();
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            SqlMapper.AddTypeHandler(new GeometryTypeHandler());
            string sql = "SELECT name, st_asbinary(ST_Transform(geometry,3857)) as geometry from ukraine";
            string connectString = "Data Source=" + db;
            var connection = new SqliteConnection(connectString);
            connection.Open();
            SpatialiteLoader.Load(connection);

            var countries = connection.Query<District>(sql);
            connection.Close();
            MapControl.Map.Layers.Add(CreatePointLayer(countries));
        }

        private static MemoryLayer CreatePointLayer(IEnumerable<District> countries)
        {
            return new MemoryLayer
            {
                Name = "Points",
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider<IFeature>(GetDistricts(countries)),
                Style = CreatePointStyle()
            };
        }

        private static IEnumerable<IFeature> GetDistricts(IEnumerable<District> countries)
        {
            var res = new List<IFeature>();
            foreach(var country in countries)
            {
                if (country.Geometry != null)
                {
                     res.Add(new GeometryFeature { Geometry = country.Geometry });
                }
            }
            return res;
        }

        private static IStyle CreatePointStyle()
        {
            var pointLayerColor = new Color(240, 240, 240, 240);
            return new VectorStyle
            {
                Fill = new Brush(pointLayerColor),
                Line = new Pen(pointLayerColor, 3),
                Outline = new Pen(Color.Gray, 2)
            };
        }
    }
}

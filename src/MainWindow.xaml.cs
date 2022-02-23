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
            connection.EnableExtensions(true);

            SpatialLoader(connection);
            var countries = connection.Query<Country>(sql);
            connection.Close();
            MapControl.Map.Layers.Add(CreatePointLayer(countries));
        }

        private void SpatialLoader(SqliteConnection connection)
        {
            SpatialiteLoader.Load(connection);
        }

        private static MemoryLayer CreatePointLayer(IEnumerable<Country> countries)
        {
            return new MemoryLayer
            {
                Name = "Points",
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider<IFeature>(GetCities2(countries)),
                Style = CreatePointStyle()
            };
        }

        private static IEnumerable<IFeature> GetCities2(IEnumerable<Country> countries)
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

        private static readonly Color PointLayerColor = new Color(240, 240, 240, 240);

        private static IStyle CreatePointStyle()
        {
            return new VectorStyle
            {
                Fill = new Brush(PointLayerColor),
                Line = new Pen(PointLayerColor, 3),
                Outline = new Pen(Color.Gray, 2)
            };
        }
    }
}

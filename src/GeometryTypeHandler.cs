using Dapper;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Data;

namespace mapsui4
{
    public class GeometryTypeHandler : SqlMapper.TypeHandler<Geometry>
    {
        public override Geometry Parse(object value)
        {
            var stream = (byte[])value;
            var g = new WKBReader().Read(stream);
            return g;
        }

        public override void SetValue(IDbDataParameter parameter, Geometry value)
        {
            parameter.Value = value;
        }
    }
}

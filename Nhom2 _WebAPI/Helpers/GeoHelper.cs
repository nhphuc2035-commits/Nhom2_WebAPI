using System;

namespace Nhom2__WebAPI.Helpers
{
    public static class GeoHelper
    {
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// Tính khoảng cách giữa hai tọa độ GPS theo công thức Haversine (trả về mét)
        /// </summary>
        public static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distanceKm = EarthRadiusKm * c;

            return Math.Round(distanceKm * 1000, 2); // Chuyển sang mét
        }

        private static double DegreesToRadians(double deg)
        {
            return deg * (Math.PI / 180.0);
        }
    }
}

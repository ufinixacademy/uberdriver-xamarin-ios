using System;
using System.Net.Http;
using System.Threading.Tasks;
using Acxi.Helpers;
using CoreLocation;
using Google.Maps;
using Newtonsoft.Json;
using UIKit;

namespace uberdriver.Helpers
{
    public class MapFunctionHelper
    {
        string mapKey;
        MapView googleMap;
        public double duration;
        public double distance;
        public string durationString;
        public string distanceString;

        Marker positionMarker;
        private bool isRequestingDirection;

        public MapFunctionHelper(string _mapKey, MapView map)
        {
            mapKey = _mapKey;
            googleMap = map;
        }


        public async Task<string> GetDirectionJsonAsync(CLLocationCoordinate2D location, CLLocationCoordinate2D destination)
        {
            string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={location.Latitude},{location.Longitude}&destination={destination.Latitude},{destination.Longitude}&mode=driving&key={mapKey}";

            string JsonResponse;

            var handle = new HttpClientHandler();
            HttpClient client = new HttpClient(handle);
            JsonResponse = await client.GetStringAsync(url);

            return JsonResponse;
        }

        public void DrawTripOnMap(string json)
        {
            var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);

            var points = directionData.routes[0].overview_polyline.points;

            Google.Maps.Path gmspath = Google.Maps.Path.FromEncodedPath(points);
            Google.Maps.Polyline gmspolyline = Google.Maps.Polyline.FromPath(gmspath);
            gmspolyline.StrokeWidth = 4;
            gmspolyline.StrokeColor = UIColor.FromRGB(6, 144, 193);
            gmspolyline.Geodesic = true;
            gmspolyline.Map = googleMap;


            int pathCount = (int)gmspath.Count;
            for(int i = 0; i<pathCount; i++)
            {
                CLLocationCoordinate2D cPoint = gmspath.CoordinateAtIndex((nuint)i);
                Console.WriteLine("Position " + i.ToString() + " = " + cPoint.Latitude.ToString() + " , " + cPoint.Longitude.ToString());
            }

            double startlat = directionData.routes[0].legs[0].start_location.lat;
            double startlng = directionData.routes[0].legs[0].start_location.lng;
            double endlat = directionData.routes[0].legs[0].end_location.lat;
            double endlng = directionData.routes[0].legs[0].end_location.lng;

            Circle circleLocation = new Circle();
            circleLocation.Position = new CLLocationCoordinate2D(startlat, startlng);
            circleLocation.Radius = 8;
            circleLocation.StrokeColor = UIColor.FromRGB(6, 144, 193);
            circleLocation.FillColor = UIColor.FromRGB(6, 144, 193);
            circleLocation.Map = googleMap;

            Circle circleDestination = new Circle();
            circleDestination.Position = new CLLocationCoordinate2D(endlat, endlng);
            circleDestination.Radius = 8;
            circleDestination.StrokeColor = UIColor.FromRGB(6, 144, 193);
            circleDestination.FillColor = UIColor.FromRGB(6, 144, 193);
            circleDestination.Map = googleMap;

            //My take off position
            Marker currentLocationMarker = new Marker();
            currentLocationMarker.Icon = Marker.MarkerImage(UIColor.Red);
            currentLocationMarker.Title = "Current Location";
            currentLocationMarker.Position = new CLLocationCoordinate2D(startlat, startlng);
            currentLocationMarker.TracksInfoWindowChanges = true;
            currentLocationMarker.Map = googleMap;

            //Pickup Marker
            var pickupMarker = new Marker()
            {
                Title = "Pick up Location",
                Position = new CLLocationCoordinate2D(endlat, endlng),
                Map = googleMap,
                Icon = Marker.MarkerImage(UIColor.Green)
            };


            // Constantly Changing Current Location marker
            positionMarker = new Marker();
            positionMarker.Icon = UIImage.FromBundle("posimarker");
            positionMarker.Title = "Current Location";
            positionMarker.Position = new CLLocationCoordinate2D(startlat, startlng);
            positionMarker.TracksInfoWindowChanges = true;

            CLLocationCoordinate2D southwest = new CLLocationCoordinate2D(directionData.routes[0].bounds.southwest.lat, directionData.routes[0].bounds.southwest.lng);
            CLLocationCoordinate2D northeast = new CLLocationCoordinate2D(directionData.routes[0].bounds.northeast.lat, directionData.routes[0].bounds.northeast.lng);

            CoordinateBounds bounds = new CoordinateBounds(southwest, northeast);
            CameraUpdate cupdates = CameraUpdate.FitBounds(bounds, 100);
            googleMap.SelectedMarker = currentLocationMarker;
            googleMap.Animate(cupdates);

            duration = directionData.routes[0].legs[0].duration.value;
            distance = directionData.routes[0].legs[0].distance.value;
            durationString = directionData.routes[0].legs[0].duration.text;
            distanceString = directionData.routes[0].legs[0].distance.text;

        }

        public void DrawTripToDestination(string json)
        {
            var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);

            var points = directionData.routes[0].overview_polyline.points;

            Google.Maps.Path gmspath = Google.Maps.Path.FromEncodedPath(points);
            Google.Maps.Polyline gmspolyline = Google.Maps.Polyline.FromPath(gmspath);
            gmspolyline.StrokeWidth = 4;
            gmspolyline.StrokeColor = UIColor.FromRGB(6, 144, 193);
            gmspolyline.Geodesic = true;
            gmspolyline.Map = googleMap;


            int pathCount = (int)gmspath.Count;
            for (int i = 0; i < pathCount; i++)
            {
                CLLocationCoordinate2D cPoint = gmspath.CoordinateAtIndex((nuint)i);
                Console.WriteLine("Position " + i.ToString() + " = " + cPoint.Latitude.ToString() + " , " + cPoint.Longitude.ToString());
            }

            double startlat = directionData.routes[0].legs[0].start_location.lat;
            double startlng = directionData.routes[0].legs[0].start_location.lng;
            double endlat = directionData.routes[0].legs[0].end_location.lat;
            double endlng = directionData.routes[0].legs[0].end_location.lng;

            Circle circleLocation = new Circle();
            circleLocation.Position = new CLLocationCoordinate2D(startlat, startlng);
            circleLocation.Radius = 8;
            circleLocation.StrokeColor = UIColor.FromRGB(6, 144, 193);
            circleLocation.FillColor = UIColor.FromRGB(6, 144, 193);
            circleLocation.Map = googleMap;

            Circle circleDestination = new Circle();
            circleDestination.Position = new CLLocationCoordinate2D(endlat, endlng);
            circleDestination.Radius = 8;
            circleDestination.StrokeColor = UIColor.FromRGB(6, 144, 193);
            circleDestination.FillColor = UIColor.FromRGB(6, 144, 193);
            circleDestination.Map = googleMap;

            //My take off position
            Marker currentLocationMarker = new Marker();
            currentLocationMarker.Icon = Marker.MarkerImage(UIColor.Green);
            currentLocationMarker.Title = "Pickup Location";
            currentLocationMarker.Position = new CLLocationCoordinate2D(startlat, startlng);
            currentLocationMarker.TracksInfoWindowChanges = true;
            currentLocationMarker.Map = googleMap;

            //Pickup Marker
            var pickupMarker = new Marker()
            {
                Title = "Destination",
                Position = new CLLocationCoordinate2D(endlat, endlng),
                Map = googleMap,
                Icon = Marker.MarkerImage(UIColor.Red)
            };


            // Constantly Changing Current Location marker
            positionMarker = new Marker();
            positionMarker.Icon = UIImage.FromBundle("posimarker");
            positionMarker.Title = "Current Location";
            positionMarker.Position = new CLLocationCoordinate2D(startlat, startlng);
            positionMarker.TracksInfoWindowChanges = true;

            CLLocationCoordinate2D southwest = new CLLocationCoordinate2D(directionData.routes[0].bounds.southwest.lat, directionData.routes[0].bounds.southwest.lng);
            CLLocationCoordinate2D northeast = new CLLocationCoordinate2D(directionData.routes[0].bounds.northeast.lat, directionData.routes[0].bounds.northeast.lng);

            CoordinateBounds bounds = new CoordinateBounds(southwest, northeast);
            CameraUpdate cupdates = CameraUpdate.FitBounds(bounds, 100);
            googleMap.SelectedMarker = currentLocationMarker;
            googleMap.Animate(cupdates);

            duration = directionData.routes[0].legs[0].duration.value;
            distance = directionData.routes[0].legs[0].distance.value;
            durationString = directionData.routes[0].legs[0].duration.text;
            distanceString = directionData.routes[0].legs[0].distance.text;

        }

        public async void UpdateMovement(CLLocationCoordinate2D myposition, CLLocationCoordinate2D destination, string whereto)
        {
            positionMarker.Map = googleMap;
            positionMarker.Position = myposition;
            CameraPosition cameraPosition = CameraPosition.FromCamera(myposition.Latitude, myposition.Longitude, 15);
            googleMap.Animate(cameraPosition);

            if (!isRequestingDirection)
            {
                isRequestingDirection = true;
                string json = await GetDirectionJsonAsync(myposition, destination);

                // check for null
                if (!string.IsNullOrEmpty(json))
                {
                    var directionData = JsonConvert.DeserializeObject<DirectionParser>(json);
                    string thisDuration = directionData.routes[0].legs[0].duration.text;

                    positionMarker.Title = "Current Location";
                    positionMarker.Snippet = thisDuration + " away from " + whereto;
                    googleMap.SelectedMarker = positionMarker;
                    isRequestingDirection = false;
                }
            }
           
        }

        public double CalculateFares(double durationMins)
        {
            double basefare = 5; //USD
            double distnacefare = 1.5; //USD per kilometer
            double timefare = 3; // USD per minute

            double kmfares = (distance / 1000) * distnacefare;
            double minfares = durationMins * timefare;

            double total = kmfares + minfares + basefare;
            double fares = Math.Round(total / 10) * 10;

            return fares;
        }
    }
}

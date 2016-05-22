using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using Android.Util;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace BusLocation
{
	[Activity (Label = "BusLocation", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity, ILocationListener
	{
		static readonly string TAG = "X:" + typeof (MainActivity).Name;
		TextView _addressText;
		Location _currentLocation;
		LocationManager _locationManager;
		Button btnAddr ;

		string _locationProvider;
		TextView _locationText;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			_addressText = FindViewById<TextView>(Resource.Id.address_text);
			_locationText = FindViewById<TextView>(Resource.Id.location_text);
			btnAddr = FindViewById<TextView> (Resource.Id.get_address_button);
			btnAddr.Click += BtnAddr_Click;
			InitializeLocationManager ();
		}

		async void BtnAddr_Click (object sender, System.EventArgs e)
		{
			if (_currentLocation == null)
			{
				_addressText.Text = "Can't determine the current address. Try again in a few minutes.";
				return;
			}

			Address address = await ReverseGeocodeCurrentLocation();
			DisplayAddress(address);
		}

		async Task<Address> ReverseGeocodeCurrentLocation()
		{
			Geocoder geocoder = new Geocoder(this);
			IList<Address> addressList =
				await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

			Address address = addressList.FirstOrDefault();
			return address;
		}

		void DisplayAddress(Address address)
		{
			if (address != null)
			{
				StringBuilder deviceAddress = new StringBuilder();
				for (int i = 0; i < address.MaxAddressLineIndex; i++)
				{
					deviceAddress.AppendLine(address.GetAddressLine(i));
				}
				// Remove the last comma from the end of the address.
				_addressText.Text = deviceAddress.ToString();
			}
			else
			{
				_addressText.Text = "Unable to determine the address. Try again in a few minutes.";
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}

		void InitializeLocationManager()
		{
			_locationManager = (LocationManager) GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = string.Empty;
			}
			Log.Debug(TAG, "Using " + _locationProvider + ".");
		}

		public void OnLocationChanged (Location location)
		{
			throw new System.NotImplementedException ();
		}

		public void OnProviderDisabled (string provider)
		{
			throw new System.NotImplementedException ();
		}

		public void OnProviderEnabled (string provider)
		{
			throw new System.NotImplementedException ();
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			throw new System.NotImplementedException ();
		}
	}
}



using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace REBGV.CosmosDB
{
	public class Listing
	{
		[JsonProperty(PropertyName = "id")]
		public string id { get; set; }
		public string contractDate { get; set; }
		public string addressUnitNumber { get; set; }
		public string addressHouseNumber { get; set; }
		public string addressStreetName { get; set; }
		public string addressStreetType { get; set; }
		public string addressCityOrTown { get; set; }
		public string area { get; set; }
		public string designatedAgent1 { get; set; }
		public string userId1 { get; set; }
		public string designatedAgent2 { get; set; }
		public string userId2 { get; set; }
		public string[] showingInformation { get; set; }
		public string appointmentPhoneNumber { get; set; }
		public string forAppointmentCall { get; set; }
		public string dwellingClassification { get; set; }
		public string typeOfDwelling { get; set; }
		public string[] styleOfHome { get; set; }
		public string titleToLand { get; set; }
		public string landLeaseExpiryYear { get; set; }
		public string[] sellersInterest { get; set; }
		public string[] occupancy { get; set; }
		public bool propertyDisclosureStatement { get; set; }
		public string propertyDisclosureStatementNotCompletedExplanation { get; set; }
		
		[JsonExtensionData]
		public Dictionary<string, object> extraFields { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
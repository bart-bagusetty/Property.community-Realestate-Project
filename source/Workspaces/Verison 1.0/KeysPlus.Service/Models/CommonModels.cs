using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeysPlus.Service.Models
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }

        public int CountryId { get; set; }

        public string Number { get; set; }

        public string Street { get; set; }

        public string Suburb { get; set; }

        public string Region { get; set; }

        public string District { get; set; }

        public List<string> SuburbList { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

    }
    public class SaveWatchlistModel
    {
        public int Id { get; set; }
        public ItemType ItemType { get; set; }
    }

    public enum ItemType
    {
        RentalListing = 1,
        MarketJob = 2
    }

    public class WatchlistDisplayModel : SearchInputModel
    {
        //public IPagedList PagedItems { get; set;}
        public ItemType ItemType { get; set; }
        //public int Page { get; set; }
        public bool IsProfileComplete { get; set; }
        public string Title { get; set; }

    }

    public class WatctlistItem<T>
    {
        public T Model { get; set; }
        public AddressViewModel Address { get; set; }
        public PropertyViewModel Property { get; set; }
    }
}

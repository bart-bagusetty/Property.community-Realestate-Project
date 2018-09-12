//Sub namespace - ProjectKeys.Address
(function (keys) {

    var self = keys.Address || (keys.Address = {});

    // custom validations Rules

    ko.validation.rules['streetLength'] = {
        validator: function (val, required) {

            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            if (val.length > 100 || val.length < 0) {
                return false;
            } else {
                return true;
            }
        },
        message: 'Invalid entry. Must be between 1-100 characters'
    };
    
    ko.validation.rules['urlLength'] = {
        validator: function (val, required) {

            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            if (val.length > 250) {
                return false;
            } else {
                return true;
            }
        },
        message: 'Invalid entry. Must be between 1-250 characters'
    };
    
    ko.validation.rules['notzero'] = {
        validator: function (val, required) {
            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            if (val == 0) {
                return false;
            } else {
                return true;
            }
        },
        message: 'Invalid entry. Input cannot be zero'
    };
    ko.validation.rules['isNumber'] = {
        validator: function (val, required) {

            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            if( isNaN(val)) {
                return false;
            } else {
                return true;
            }
        },
        message: 'Invalid entry. Please enter a number'
    };
    ko.validation.rules['numberLength'] = {
        validator: function (val, required) {

            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            if (val>99999||val<0) {
                return false;
            } else {
                return true;
            }

        },

        message: 'Invalid entry. Address number must be in 1-99999'
    };
    var AddressFields = function (item)
    {
        this.AddressId = ko.observable(item.AddressId);
        this.CountryId = ko.observable(item.CountryId);

        this.Number = ko.observable(item.Number).extend({
            required:
                {
                    params: true, message: "Please enter your Number"
                 },
            pattern:
                {
                params: "^[0-9][A-Za-z0-9\\s\-\\,\\/\\&\]{0,9}$",
                message: "Please enter a valid Number",
                }
        }),

        this.Street = ko.observable(item.Street).extend({
            required:
                {
                    params: true, message: "Please enter your Street"
                },
            pattern:
                {
                    params: "^[A-Za-z0-9][A-Za-z0-9\\s\-\\,\\/\\&\]{0,100}$",
                    message: "Please enter 1-100 alphanumeric characters only",
                }
        }),
        this.Suburb = ko.observable(item.Suburb).extend({
            required:
                {
                    params: false, message: "Please enter your Suburb"
                },
            pattern:
                {
                    params: "^[A-Za-z\\s]{0,50}$",
                    message: "The suburb field can only accept only 1-50 characters",
                }
        }),
        this.City = ko.observable(item.City).extend
        ({
            required:
                {
                    params: true, message: "Please enter your City"
                },
            pattern:
                {
                    params: "^[A-Za-z\\s]{0,50}$",
                    message: " Please enter a valid City"
                }
        }),
        this.PostCode = ko.observable(item.PostCode).extend
            ({
                required: {
                    params: true,
                    message: "Please enter your Post Code"
                },
                Number: { params: true, },
                pattern:
                    {
                        params: "^[0-9]{1,4}$",
                        message: "Please enter a valid Post Code and maximum 4 characters only.",
                    }
            }),
        this.Latitude = ko.observable(item.Latitude);
        this.Longitude = ko.observable(item.Longitude);
    };

    ko.bindingHandlers.addressAutocomplete = {
       
        init: function (element, boundProp, allProp, data, context) {
            google.maps.event.addDomListener(element, 'keydown', function (e) {
                if (e.keyCode == 13 && $('.pac-container:visible').length) {
                    e.preventDefault();
                }
            });
            var address = ko.unwrap(boundProp());
            var allBindings = ko.unwrap(allProp());
            var autocomplete;
            var options = {};
            var geocoder = new google.maps.Geocoder();
            var nzBounds = new google.maps.LatLngBounds(
                new google.maps.LatLng(-33.884362, 154.711217),
                new google.maps.LatLng(-21.632195, 110.062781));
            var componentForm = {
                street_number: 'short_name',
                route: 'long_name',
           
                administrative_area_level_1: 'short_name',
                administrative_area_level_2: 'short_name',
                //country: 'long_name',
                postal_code: 'short_name'
            };
            var addressForm = {
                street_number: 'Number',
                route: 'Street',
                administrative_area_level_1: 'Suburb',
                administrative_area_level_2: 'City',
                //country : 'Country',
                postal_code: 'PostCode'
            };

            if (allBindings.hasOwnProperty('AutocompleteOptions')) {
                options = allBindings;
            } else {
                options = {
                    bounds: nzBounds,
                    componentRestrictions: { country: "au" },
                    types: ['geocode']
                };
            }

            autocomplete = new google.maps.places.Autocomplete(element, options);
            autocomplete.addListener('place_changed', FillInAddress);
            element.addEventListener('focus', Geolocate, false);

            function FillInAddress() {
               
                var place = autocomplete.getPlace();
                //context.$data.Latitude(place.geometry.location.lat());
                //context.$data.Longitude(place.geometry.location.lng());
                address.Latitude(place.geometry.location.lat());
                address.Longitude(place.geometry.location.lng());
                for (var i = 0; i < place.address_components.length; i++) {
                    var addressType = place.address_components[i].types[0];
                    if (componentForm[addressType]) {
                        var value = place.address_components[i][componentForm[addressType]];
                        if (addressForm.hasOwnProperty(addressType)) {
                            var key = addressForm[addressType];
                            //context.$data[key](value);
                            address[key](value);
                           
                        }
                    }
                }
            }

            function Geolocate() {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        var geolocation = {
                            lat: position.coords.latitude,
                            lng: position.coords.longitude
                        };
                        var circle = new google.maps.Circle({
                            center: geolocation,
                            radius: position.coords.accuracy
                        });
                        autocomplete.setBounds(circle.getBounds());
                    });
                }
            }
        }
    };

    self.addressData = ko.observable();

    self.CreateAddress = function () {

        var newAddress = {
            Number: "",
            Street: "",
            AddressId: 0,
            CountryId: 1,
            City: "",
            Latitude: "",
            Longitude: "",
            PostCode: "",
            Suburb: ""
        };

        var forCreation = new AddressFields(newAddress);
        self.addressData(forCreation);
        return self.addressData();
    };


    self.DeleteAddress = function (address) {
        $.ajax("/Address/Delete", {
            data: ko.toJSON(address),
            type: "post",
            contentType: "application/json",
            success: function () {
                console.log("Address Deleted");
                //Redirection should ideally be to where the Address data was removed (e.g. from Property,Company,Admin)
                window.location.replace("/Admin/Home");
            }
        });
    };

    self.EditAddress = function (address) {
        var forEditing = new AddressFields(address);

        self.addressData(forEditing);
        return self.addressData();
    };

    self.SaveAddress = function (address) {
        if (address.AddressId() === 0) {
            console.log("At Save Create");

            $.ajax("/Address/Create", {
                data: ko.toJSON(address),
                type: "post",
                contentType: "application/json",
                success: function (result) {
                    console.log("Address Created");
                    console.log(result);
                    self.addressData(result);
                }
            });
        }
        else {
            console.log("At Save Edit");

            $.ajax("/Address/Edit", {
                data: ko.toJSON(address),
                type: "post",
                contentType: "application/json",
                success: function (result) {
                    console.log("Address Edited");
                    console.log(result);
                    self.addressData(result);
                }
            });
        }
        return self.addressData();
    };

    self.GetAddress = function (address) {
        console.log("At Get");

        var data = {
            addressId: address.AddressId
        };

        $.getJSON("/Address/Get", data, function (result) {
            console.log("Data from server");
            console.log(result);
            var address = new AddressFields(result);
            self.addressData(address);
        });
        return self.addressData();
    };

})(ProjectKeys = ProjectKeys || (ProjectKeys = {}));
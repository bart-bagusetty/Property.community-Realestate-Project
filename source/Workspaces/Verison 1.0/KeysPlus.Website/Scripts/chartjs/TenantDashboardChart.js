function RentalViewModel(data) {
    var self = this;
    self.PropertyId = ko.observable(data.PropertyId);
    self.Address = ko.observable(data.Address);
    self.Landlordname = ko.observable(data.Landlordname);
    self.RentalPaymentType = ko.observable(data.RentalPaymentType);
    self.TargetRent = ko.observable(data.TargetRent);
    self.PaymentDueDate = ko.observable(data.PaymentDueDate);
    self.PaymentStartDate = ko.observable(data.PaymentStartDate);
    self.IsDate = ko.observable(true);
}


var Rentals = function (data) {
    var self = this;
    self.TenantRentals = ko.observableArray();
    data.TenantRentalDashboardData.forEach(function (item) {
        self.TenantRentals.push(new RentalViewModel(item));
    });
}
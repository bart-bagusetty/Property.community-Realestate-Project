//function PageViewModel(data) {
//    var self = this;
//    self.EnableButtons = ko.observable(true);
//    console.log(data);
function PageViewModel(dataVm) {
    var token = $("input[name = '__RequestVerificationToken']").val();
    var self = this;
    for (var key in dataVm) {
        self[key] = dataVm[key]
    }
    //self.SelectedItem = ko.observable();
    //self.ApplyView = ko.observable();
    //self.SelectedRentalProperty = ko.observable();
    //self.toMain = ko.observable();
    self.Items().forEach(function (item) {
        KeysUtils.injectViewProps(item);
    });

    //self.Items = ko.mapping.fromJS(data);
    //self.Items().forEach(function (item) {
    //    KeysUtils.injectViewProps(item);
    //});

    
    self.MainView = ko.observable(true);
    self.DetailView = ko.observable(false);
    self.SelectedItem = ko.observable();
    self.ApplyView = ko.observable(false);
    self.SelectedRentalProperty = ko.observable();
    self.TenantInfoView = ko.observable(false);
    self.TenantInfo = new TenantInfoVm(KeysExtendsDic.TenantInfo);
    self.ShowMain = function () {
        self.MainView(true);
        self.DetailView(false);
        self.ApplyView(false);
        self.TenantInfoView(false);
    }
    self.ShowDetails = function (data) {
        self.MainView(false);
        self.DetailView(true);
        self.ApplyView(false);
        self.TenantInfoView(false);
        console.log('data',data);
        self.SelectedItem(data);        
    }
        self.HasMapData = ko.computed(function () {
            var item = self.SelectedItem();
            if (!item) return false;
            return item.Address.Latitude && item.Address.Longitude;
        });
        function TenantInfoVm(dic) {
            var self = this;
            self.validFileTypes = ["image/jpeg", "image/png", "image/gif"];
            self.Model = new Entity(dic);
            self.Model.PhotoFile = ko.observable();
            self.ImgData = ko.observable();
            self.Model.DateOfBirth = ko.observable().extend({
                date: true,
                required: {
                    params: true,
                    message: "Please enter a date."
                }
            });
            self.Model.Address = new AddressViewModel();
            self.AddressField = ko.observable().extend({
                required: {
                    params: true,
                    message: "Please enter address."
                }
            });
            self.Errors = ko.validation.group([self.Model, self.AddressField], { deep: true });
            self.IsValid = ko.computed(function () {
                return self.Errors().length == 0;
            });
        }
    }


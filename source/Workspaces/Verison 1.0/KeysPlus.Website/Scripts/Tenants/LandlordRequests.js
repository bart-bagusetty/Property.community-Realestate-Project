function PageViewModel(dataVm) {
    var token = $("input[name = '__RequestVerificationToken']").val();
    var self = this;
    for (var key in dataVm) {
        self[key] = dataVm[key]
    }
    self.Items().forEach(function (item) {
        KeysUtils.injectViewProps(item);
    });
    self.Inspection = ko.observable();
    self.SelectedItem = ko.observable();
    self.MainView = ko.observable(true);
    self.DetailView = ko.observable(false);
    self.AcceptView = ko.observable(false);
    self.DeclineView = ko.observable(false);
    self.ShowMain = function () {
        self.MainView(true);
        self.DetailView(false);
        self.AcceptView(false);
        self.DeclineView(false);
    }
    self.ShowAccept = function (item) {
        self.MainView(false);
        self.DetailView(false);
        self.AcceptView(true);
        self.DeclineView(false);
        var ins = new EntityViewModel(KeysExtendsDic.Inspection);
        ins.Model.PropertyId = item.Model.PropertyId();
        ins.Model.RequestId = item.Model.Id();
        ins.PercentDone = ko.observable(0);
        self.Inspection(ins);
        self.SelectedItem(item);
    }
    
    self.showDecline = function (item) {
        self.MainView(false);
        self.DetailView(false);
        self.AcceptView(false);
        self.DeclineView(true);
        self.SelectedItem(item);
    }

    self.AcceptRequest = function (data) {
        var formData = KeysUtils.toFormData({ requestId: data.Model.Id()}, token);
        var url = '/Tenants/Home/AccepLandlordRequest';
        KeysUtils.post(url, formData, self.postSuccess);
    }

    self.AddInspection = function (data) {
        var formData = KeysUtils.toFormData(data.Model, token);
        var url = '/Tenants/Home/AddInspection';
        KeysUtils.post(url, formData, self.postSuccess);
    }

    self.Decline = function (data) {
        var formData = KeysUtils.toFormData(data.Model, token);
        var url = '/Tenants/Home/DeclineRequest';
        KeysUtils.post(url, formData, self.postSuccess);
    }

    self.postSuccess = function(result) {
        if (result.Success) {
            KeysUtils.notification.show('<p>' + result.Message + '</p>', 'notice', KeysUtils.reload);
        }
        else {
            KeysUtils.notification.showErrorMsg();
        }
    }
}


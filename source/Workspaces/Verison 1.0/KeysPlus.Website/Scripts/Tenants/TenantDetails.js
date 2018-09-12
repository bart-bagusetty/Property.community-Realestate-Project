var tok = typeof token !== "undefined" ? token : $('input[name="__RequestVerificationToken"]').val();

function TenantInfoVm() {
    var token = $("input[name = '__RequestVerificationToken']").val();
    var self = this;
    self.ValidFileTypes = ["image/jpeg", "image/png", "image/gif"];
    self.Model = new Entity(KeysExtendsDic.TenantInfo);
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
    self.SaveInfo = function (data) {
        var formData = KeysUtils.toData(data.Model);
        formData.append("__RequestVerificationToken", token);
        $.ajax({
            type: 'POST',
            url: '/Tenants/Home/Onboarding',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (result) {
                if (result.Success) {
                    KeysUtils.notification.show('<p>Details updated successfully!</p>', 'success');
                    window.location.replace('/Home/Dashboard');
                    return;
                }
                else {
                    KeysUtils.notification.show('<p>Something went wrong please try again later!</p>', 'error');
                }
            }
        });
    }
}
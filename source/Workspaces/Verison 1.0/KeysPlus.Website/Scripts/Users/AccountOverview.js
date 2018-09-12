//ko.bindingHandlers for Country dropdown list
//ko.bindingHandlers.country = {
//    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
//        var $element = $(element);
//        var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
//        // show only when value is provided
//        if (valueUnwrapped && valueUnwrapped.length > 0) {
//            $element.css('display', 'block');
//            $element.data('country', valueUnwrapped);
//            $element.val(valueUnwrapped);

//            var data = $element.data();
//            delete data.bfhcountries;
//            data.country = valueUnwrapped;
//            $element.bfhcountries(data);
//        } else {
//            $element.css('display', 'none');
//        }
//    }
//};

var AddressModel = function (data) {
    var self = this;

    self.errorsAddress = ko.validation.group(self);

    self.Number = ko.observable(data.Number) || ko.observable();
    self.Number.extend(Extender.addressNumber);

    self.Street = ko.observable(data.Street) || ko.observable();
    self.Street.extend(Extender.addressStreet);

    self.Suburb = ko.observable(data.Suburb) || ko.observable();
    self.Suburb.extend(Extender.addressSuburb);

    self.City = ko.observable(data.City) || ko.observable();
    self.City.extend(Extender.addressCity);

    self.PostCode = ko.observable(data.PostCode) || ko.observable();
    self.PostCode.extend(Extender.postcode);

    self.Latitude = ko.observable(data.Latitude) || ko.observable();
    self.Longitude = ko.observable(data.Longitude) || ko.observable();
    self.Errors = ko.validation.group(self);
    self.IsValid = ko.computed(function () {
        return self.Errors().length == 0;
    });

}

var AccountOverviewViewModel = function (comVm) {
    var self = this;
    self.Company = comVm;
    self.Company.Name.extend(Extender.companyName);
    self.Company.Phone.extend(Extender.homePhoneNumber);
    self.Company.Website.extend(Extender.websiteCompany);
    self.Company.PhysicalAddress.Number.extend(Extender.addressNumber);
    self.Company.PhysicalAddress.Street.extend(Extender.addressStreet);
    self.Company.PhysicalAddress.Suburb.extend(Extender.addressSuburb);
    self.Company.PhysicalAddress.City.extend(Extender.addressCity);
    self.Company.PhysicalAddress.PostCode.extend(Extender.postcode);
    self.Company.BillingAddress.Number.extend(Extender.addressNumber);
    self.Company.BillingAddress.Street.extend(Extender.addressStreet);
    self.Company.BillingAddress.Suburb.extend(Extender.addressSuburb);
    self.Company.BillingAddress.City.extend(Extender.addressCity);
    self.Company.BillingAddress.PostCode.extend(Extender.postcode);
    self.Id = ko.observable("");
    self.FirstName = ko.observable();
    self.FirstName.extend(Extender.firstName);
    self.MiddleName = ko.observable();
    self.MiddleName.extend(Extender.middleName);
    self.LastName = ko.observable();
    self.LastName.extend(Extender.lastName);
    //self.PlaceOfBirth = ko.observable("").extend({
    //    required: {
    //        params: true,
    //        message: "Please enter your birth place"
    //    },
    //    pattern: {
    //        params: "^[A-Za-z][A-Za-z\\s\,\\-\.\/\]{1,50}$",
    //        message: "Please enter a valid Place of Birth"
    //    }
    //});
    self.availableRoles = ko.observable(['Service Provider', 'Tenant', 'Property Owner', 'Guest']);
    self.userRole = ko.observable();
    self.PlaceOfBirth = ko.observable();
    self.PhoneNumber = ko.observable();
    self.PhoneNumber = ko.observable().extend(Extender.homePhoneNumber);
    self.Language = ko.observable();
    self.Language.extend(Extender.language);
    self.Occupation = ko.observable();
    self.Occupation.extend(Extender.occupation);
    self.LinkedinUrl = ko.observable();
    self.LinkedinUrl = ko.observable().extend(Extender.linkedinUrl);
    self.PhyAddressSearchBar = ko.observable();
    self.BillAddressSearchBar = ko.observable();
    self.PhysicalAddress = ko.observable(new AddressModel(""));
    self.BillingAddress = ko.observable(new AddressModel(""));
    self.ProfilePhoto = ko.observable("");
    self.MediaFiles = ko.observableArray();
    self.FilesRemoved = ko.observableArray();
    self.MainView = ko.observable(true);
    self.EditUser = ko.observable(false);
    self.EditCompany = ko.observable(false);
    self.isShipSame = ko.observable(false);
    self.isCompShipSame = ko.observable(false);
    self.IsServiceSupplier = ko.observable(false);
    self.isCompanyValid = ko.observable(false);
    self.NewPhoto = ko.observable();
    self.CompanyErrors = ko.validation.group([
        self.Company.Name,
        self.Company.Phone,
        self.Company.Website,
        self.Company.PhysicalAddress,
        self.Company.BillingAddress,
    ])

    self.isCompanyValid = ko.computed(function () {
        return self.CompanyErrors().length == 0;
    });

    self.ShowCompanyEdit = function () {
        self.MainView(false);
        self.EditUser(false);
        self.EditCompany(true);
    }
    // $(".image-preview-clear").show();

    GetAccountOverview();

    self.validFileTypes = [
        "image/gif",
        "image/jpeg",
        "image/png",
        "image/jpg"
    ];

    self.FileWarning = ko.observable(KeysInstrucTion.fileUpLoad);
    self.viewChange = function () {
        self.MainView(false);
        self.EditUser(true);
        self.EditCompany(false);
    };

    self.FullUserName = ko.pureComputed(function () {
        return self.MiddleName() == null ? self.FirstName() + " " + self.LastName() : self.FirstName() + " " + self.MiddleName() + " " + self.LastName();
    });

    $(function () {
        // Clear event
        $('.image-preview-clear').click(function () {

            $('.image-preview-filename').val("");
            $('.image-preview-clear').hide();
            $('.image-preview-input input:file').val("");
            $(".image-preview-input-title").text("Choose File");
        });

    });

    self.checkShipSame = ko.computed(function () {
        if (self.isShipSame()) {
            UpdateAddress(ko.mapping.toJS(self.PhysicalAddress()), self.BillingAddress());
        }
        else {
            self.BillingAddress().Number();
            self.BillingAddress().Street();
            self.BillingAddress().Suburb();
            self.BillingAddress().City();
            self.BillingAddress().PostCode();
        }
    });

    function resetOrientation(srcBase64, srcOrientation, callback) {
        var img = new Image();
        img.onload = function () {
            var width = img.width,
                height = img.height,
                canvas = document.createElement('canvas'),
                ctx = canvas.getContext("2d");
            // set proper canvas dimensions before transform & export
            if (4 < srcOrientation && srcOrientation < 9) {
                canvas.width = height;
                canvas.height = width;
            } else {
                canvas.width = width;
                canvas.height = height;
            }
            // transform context before drawing image
            switch (srcOrientation) {
                case 2: ctx.transform(-1, 0, 0, 1, width, 0); break;
                case 3: ctx.transform(-1, 0, 0, -1, width, height); break;
                case 4: ctx.transform(1, 0, 0, -1, 0, height); break;
                case 5: ctx.transform(0, 1, 1, 0, 0, 0); break;
                case 6: ctx.transform(0, 1, -1, 0, height, 0); break;
                case 7: ctx.transform(0, -1, -1, 0, height, width); break;
                case 8: ctx.transform(0, -1, 1, 0, 0, width); break;
                default: break;
            }
            // draw image
            ctx.drawImage(img, 0, 0);
            // export base64
            callback(canvas.toDataURL());
        };
        img.src = srcBase64;
    }

    self.ImageFile = ko.observable();
    self.fileName = ko.observable();
    self.fileSrc = ko.observable();

    self.fileUpload = function (elemet, event) {
        var files = event.target.files;
        console.log(files);
        if (files && files[0]) {
            var f = files[0];
            if (!f.type.match('image.*')) {
                return;
            }

            //Get the current orientation value of the image
            var originalImage = document.getElementById("editimage");
            var imgOrientation;
            EXIF.getData(originalImage, function () {
                imgOrientation = EXIF.getTag(this, "Orientation");
            });
            console.log("Orientation: " + imgOrientation);

            var reader = new FileReader();
            reader.onload = function (e) {
                var photo = {
                    File: ko.observable(f),
                    Data: ko.observable(e.target.result),
                    OldFileName: ko.observable(f.name),
                    Status: "add",
                }
                self.ProfilePhoto(photo);
            }
            reader.readAsDataURL(f);
            //originalImage = document.getElementById("editimage");
            //Set proper image orientation
            //resetOrientation(originalImage.src, imgOrientation, function (resetBase64Image) {
            //    originalImage.src = resetBase64Image;
            //});
        }
        
    };
    self.removePhoto = function (file) {
        $('#editimage').attr('src', '/images/icon-user-default.png');
        //self.removeFiles(file);
    };
    self.removeFiles = function (file) {
        if (file.Status === 'load') {
            self.FilesRemoved.push(file.Id);
        }
        self.MediaFiles.remove(file);
    }
    self.errors = ko.validation.group([
        self.FirstName,
        self.MiddleName,
        self.LastName,
        self.PhoneNumber,
        self.Language,
        self.Occupation,
        self.LinkedinUrl,
        self.PhysicalAddress,
        self.BillingAddress,

    ]);
    //self.AddressErrors = ko.computed(function () {
    //    return self.Company.PhysicalAddress.IsValid() && self.Company.BillingAddress.IsValid();
    //});
    self.isValid = ko.computed(function () {
        return self.errors().length == 0;
    });


    self.edit = function (data) {
        var formData = new FormData();
        formData.append('Id', data.Id());
        formData.append('FirstName', data.FirstName());
        formData.append('MiddleName', data.MiddleName() == null ? "" : data.MiddleName());
        formData.append('LastName', data.LastName());
        formData.append('userRole', data.userRole());
        formData.append('PlaceOfBirth', data.PlaceOfBirth());
        formData.append('Language', data.Language() == null ? "" : data.Language());
        formData.append('Occupation', data.Occupation() == null ? "" : data.Occupation());
        formData.append('LinkedinUrl', data.LinkedinUrl() == null ? "" : data.LinkedinUrl());
        formData.append('PhoneNumber', data.PhoneNumber() == null ? "" : data.PhoneNumber());

        if (data.ProfilePhoto().Status == 'add') {
            formData.append('NewPhoto', data.ProfilePhoto().File());
        }


        if ($('#editcompanyimage').attr('src') == "/images/icon-user-default.png") {
            formData.append('ProfilePhoto().Status', "removed");
        }

        formData.append('PhysicalAddress.Number', data.PhysicalAddress().Number());
        formData.append('PhysicalAddress.Street', data.PhysicalAddress().Street());
        formData.append('PhysicalAddress.Suburb', data.PhysicalAddress().Suburb());
        formData.append('PhysicalAddress.City', data.PhysicalAddress().City());
        formData.append('PhysicalAddress.PostCode', data.PhysicalAddress().PostCode());
        if (isShipSame.checked == false) {
            console.log("different billing");
            formData.append('BillingAddress.Number', data.BillingAddress().Number());
            formData.append('BillingAddress.Street', data.BillingAddress().Street());
            formData.append('BillingAddress.Suburb', data.BillingAddress().Suburb());
            formData.append('BillingAddress.City', data.BillingAddress().City());
            formData.append('BillingAddress.PostCode', data.BillingAddress().PostCode());
        }
        else {
            console.log("same billing");
            formData.append('BillingAddress.Number', data.PhysicalAddress().Number());
            formData.append('BillingAddress.Street', data.PhysicalAddress().Street());
            formData.append('BillingAddress.Suburb', data.PhysicalAddress().Suburb());
            formData.append('BillingAddress.City', data.PhysicalAddress().City());
            formData.append('BillingAddress.PostCode', data.PhysicalAddress().PostCode());
        }

        //formData.append('FilesRemoved', data.FilesRemoved());
        var removedData = data.FilesRemoved();

        for (var i = 0; i < removedData.length; i++) {
            formData.append('FilesRemoved', removedData[i]);
        }
        var addedPhoto = data.MediaFiles();
        for (var i = 0; i < addedPhoto.length; i++) {
            formData.append('MediaFiles' + i, addedPhoto[i].File);
        }

        if (self.isValid()) {
            var url = "/Account/Edit/" + self.Id();
            $.ajax({
                type: "Post",
                url: url,
                data: formData,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.success) {
                        KeysUtils.notification.show('<p>Changes saved successfully!</p>', 'success', viewChangeBack);
                        GetAccountOverview();
                    }

                },
                error: function (error) {

                    alert(error.status + "<!----!>" + error.statusText);
                },
                fail: function () { }

            });

        }
        else {
            alert("Please check your submission. Please fill all required fields");
        }
    };

    self.editCompany = function (data) {
        debugger;
        var formData = new FormData();
        formData.append('Name', data.Name());
        formData.append('Phone', data.Phone());
        formData.append('Website', data.Website());

        if (data.ProfilePhoto().Status() == 'add') {
            formData.append('NewCompPhoto', data.ProfilePhoto().File());
        }

        if ($('#editimage').attr('src') == "/images/icon-user-default.png") {
            formData.append('ProfilePhoto.Status', "removed");
        }

        formData.append('PhysicalAddress.Number', data.PhysicalAddress.Number());
        formData.append('PhysicalAddress.Street', data.PhysicalAddress.Street());
        formData.append('PhysicalAddress.Suburb', data.PhysicalAddress.Suburb());
        formData.append('PhysicalAddress.City', data.PhysicalAddress.City());
        formData.append('PhysicalAddress.PostCode', data.PhysicalAddress.PostCode());

        if (isCompShipSame.checked == false) {
            console.log("different billing");
            formData.append('BillingAddress.Number', data.BillingAddress.Number());
            formData.append('BillingAddress.Street', data.BillingAddress.Street());
            formData.append('BillingAddress.Suburb', data.BillingAddress.Suburb());
            formData.append('BillingAddress.City', data.BillingAddress.City());
            formData.append('BillingAddress.PostCode', data.BillingAddress.PostCode());
        }
        else {
            console.log("same billing");
            formData.append('BillingAddress.Number', data.PhysicalAddress.Number());
            formData.append('BillingAddress.Street', data.PhysicalAddress.Street());
            formData.append('BillingAddress.Suburb', data.PhysicalAddress.Suburb());
            formData.append('BillingAddress.City', data.PhysicalAddress.City());
            formData.append('BillingAddress.PostCode', data.PhysicalAddress.PostCode());
        }
        //var removedData = data.FilesRemoved();

        //for (var i = 0; i < removedData.length; i++) {
        //    formData.append('FilesRemoved', removedData[i]);
        //}
        //var addedPhoto = data.MediaFiles();
        //for (var i = 0; i < addedPhoto.length; i++) {
        //    formData.append('MediaFiles' + i, addedPhoto[i].File);
        //}

        if (self.isCompanyValid()) {
            var url = "/Account/EditCompany/" + self.Id();
            $.ajax({
                type: "Post", // type:"PUT"
                url: url,
                data: formData,//ko.toJSON(newjob), //
                // dataType: 'json',
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.success) {
                        KeysUtils.notification.show('<p>Changes saved successfully!</p>', 'success', viewChangeBack);
                        //$("#SaveModal").modal("show");
                        //var saveModal = document.getElementById('saveModal');
                        GetAccountOverview();
                        //self.FirstView(true);
                        //self.SecondView(false);
                    }

                },
                error: function (error) {

                    alert(error.status + "<!----!>" + error.statusText);
                },
                fail: function () { }

            });                                                                                                                        
        }
        else {
            alert("Please check your submission. Please fill all required fields");
        }
    };

    function viewChangeBack() {
        self.MainView(true);
        self.EditUser(false);
        self.EditCompany(false);
    }

    function GetAccountOverview() {
        $.ajax({
            type: "GET",
            url: "/Account/FetchAccountOverview",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.Id(data.Id);
                self.FirstName(data.FirstName);
                self.MiddleName(data.MiddleName);
                self.LastName(data.LastName);
                self.PlaceOfBirth(data.PlaceOfBirth);
                data.Language ? self.Language(data.Language) : 1;
                self.Occupation(data.Occupation);
                self.LinkedinUrl(data.LinkedinUrl);
                self.PhoneNumber(data.PhoneNumber);
                self.IsServiceSupplier(data.IsServiceSupplier);
                UpdateAddress(data.PhysicalAddress, self.PhysicalAddress());
                UpdateAddress(data.BillingAddress, self.BillingAddress());
                self.ProfilePhoto(data.ProfilePhoto);
                self.MediaFiles(data.MediaFiles);
            },
            error: function (error) {
                //   alert(error.status + "<--and--> " + error.statusText);
                console.log("error data");
            }
        });
    };

    function UpdateAddress(data, address) {
        data.Number ? address.Number(data.Number) : 1;
        data.Street ? address.Street(data.Street) : 1;
        data.Suburb ? address.Suburb(data.Suburb) : 1;
        data.City ? address.City(data.City) : 1;
        data.PostCode ? address.PostCode(data.PostCode) : 1;
        data.Latitude ? address.Latitude(data.Latitude) : 1;
        data.Longitude ? address.Longitude(data.Longitude) : 1;
    }
}//end of view modal
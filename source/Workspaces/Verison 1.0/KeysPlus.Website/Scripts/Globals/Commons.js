var map;
Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}
function AddressViewModel() {
    var self = this;
    self.Number = ko.observable().extend({
        required: {
            params: true,
            message: "Please include your property's number."
        },
        pattern: {
            params: "^[A-Za-z0-9][A-Za-z0-9\\d\\s\\-,_\\//]{0,50}$",
            message: "The Number field must be alphanumeric"
        }
    });
    self.Street = ko.observable().extend({
        required: {
            params: true,
            message: "Please include your Street."
        },
        pattern: {
            params: "^[A-Za-z0-9][A-Za-z0-9\\s\-\\,\\/\\&\]{0,100}$",
            message: "The Street field Must be between 1-100 characters."
        }
    });
    self.Suburb = ko.observable().extend({
        pattern: {
            params: "^[A-Za-z ]{1,50}$",
            message: "The suburb field can only accept only 1-50 characters."
        }
    });
    self.City = ko.observable().extend({
        required: {
            params: true,
            message: "Please include your City."
        },
        pattern: {
            params: "^[A-Za-z ]{1,50}$",
            message: "The City field must be alphanumeric and betweeen 1-50 characters."
        }
    }),
    self.PostCode = ko.observable().extend({
        required: {
            params: true,
            message: "Please include your Post Code."
        },
        pattern: {
            params: "^[0-9 ]{0,4}$",
            message: "The Post Code field must be numeric characters and must be between 1-4 characters."
        }
    });
    self.Region = ko.observable();
    self.Latitude = ko.observable();
    self.Longitude = ko.observable();
    self.Country = ko.observable();
    self.Errors = ko.validation.group(self);
    self.IsValid = ko.computed(function () {
        return self.Errors().length == 0;
    });

}
var KeysPattern = {};
KeysPattern.numeric = /^[0-9]+(.[0-9]{1,9})?$/;
KeysPattern.email = /^[a-z0-9][a-zA-Z0-9_]*(\.[a-zA-Z][a-zA-Z0-9_]*)?@[a-z][a-zA-Z-0-9]*\.[a-z]+(\.[a-z]+)?$/;
KeysPattern.website = /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi;
KeysPattern.date = /^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.]((19|20)\\d\\d)$/;

var KeysFiles = {};
KeysFiles.getTotalSize = function (files) {
    var size = 0;
    if (ko.isObservable(files)) files = files();
    files.forEach(function (fileModel) {
        size += (fileModel.File ? fileModel.File.size : null) || (ko.isObservable(fileModel.Size) ? fileModel.Size() : fileModel.Size);
    });
    return size;
}
KeysFiles.validFileTypes = ["application/pdf", "application/msword", "image/gif", "image/jpeg", "image/png", "image/.jpg", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 'docx', 'doc'];
KeysFiles.validDocFiles = ["application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 'docx', 'doc'];
KeysFiles.validImgFiles = ["image/gif", "image/jpeg", "image/png", "image/jpg"];
KeysFiles.isFileValid = function (validFiles, file) {
    var ext = file.name.split('.').pop();
    var type = file.type || ext;
    return ~validFiles.indexOf(type);
}
KeysFiles.isImg = function (mediaFile) {

    var mediaType = mediaFile.MediaType ?  ko.utils.unwrapObservable(mediaFile.MediaType) : 0;
    var fileType = mediaFile.type
    var notImg = !~KeysFiles.validImgFiles.indexOf(fileType);
    return !notImg || mediaType == 1;
}
KeysFiles.isDoc = function (mediaFile) {
    var mediaType = mediaFile.MediaType || 0;
    var fileType = mediaFile.type
    var notDoc = !~KeysFiles.validDocFiles.indexOf(fileType);
    return !notDoc || mediaType == 2;
}
KeysFiles.getType = function (mediaFile) {
    return KeysFiles.isImg(mediaFile) ? 1 : 2;
}
KeysFiles.getFirstPhoto = function (mediaFiles) {
    var files = ko.isObservable(mediaFiles) ? mediaFiles() : mediaFiles;
    for (var i = 0; i < files.length; i++) {
        if (KeysFiles.isImg(files[i])) {
            return files[i];
        }
    }
    return new FileModel({ Data: '/images/no-prop-img.png' })
}
//KeysFiles.getFirstPhoto = function (mediaFiles) {
//    return ko.computed(function () {
//        for (var i = 0; i < mediaFiles.length; i++) {
//            if (KeysFiles.isImg(mediaFiles[i])) {
//                return mediaFiles[i];
//                break;
//            }
//        }
//        return new FileModel({ Data: '/images/no-prop-img.png' })
//    }, this);
//}
var KeysInstrucTion = {};
KeysInstrucTion.fileUpLoad = 'You can upload up to 5 photos. Total max size is 5 MB and supported file types are *.jpg, *.jpeg, *.png, *.gif & *.pdf';

var KeysWarning = {};
KeysWarning.maxFileSize = 'File Size is too big. Maximum allowed size is up to 5MB';
KeysWarning.fileType = 'Supported file types are *.jpg, *.jpeg, *.png, *.gif';
KeysWarning.maxNumberFiles = function (maxNumberFiles) { return "Please upload no more than " + maxNumberFiles + " items" }

var Rules = {};
Rules.require = function (text) {
    return {
        params: true,
        message: text
    }
}
Rules.textLength = function (key, length, text) {
    return { params: length, message: key + " must consist at least " + length + " characters" };
}

var Extender = {};
Extender.tenantNumber = {
    required: Rules.require('Please enter number of tenants'),
    number: {
        params: true,
        message: "Please enter number only."
    },
    
};
Extender.tenantNote = {
    required: Rules.require('Please enter note'),
    minLength: { params: 10, message: "Note must consist at least 10 characters" },
    maxLength: { params: 500, message: "Note should not exceed 500 characters" }
}
Extender.companyName = {
    required: Rules.require('Please enter a company name'),
    pattern: {
        params: "^[A-Za-z]+$",
        message: "The company name must be composed of alphabets."
    }
}
Extender.firstName = {
    required: Rules.require('Please enter first name'),
    pattern: {
        params: "^[A-Z a-z_  ]+$",
        message: "The first name must be composed of alphabets.ttt"
    }
};
Extender.middleName = {
    pattern: {
        params: "^[A-Z a-z_  ]+$",
        message: "Please enter a valid Middle Name"
    }
}
Extender.lastName = {
    required: Rules.require('Please enter last name'),
    pattern: {
        params: "^[A-Z a-z_  ]+$",
        message: "The last name must be composed of alphabets."
    }
};
Extender.language = {
    required: {
        params: true,
        message: "Please enter a language"
    },
    pattern: {
        params: "^[A-Za-z][A-Za-z\\s\,\]{1,19}$",
        message: "Please enter a valid language less than 20 characters"
    }
}
Extender.occupation = {
    pattern: {
        params: "^[A-Za-z][A-Za-z\\s\,\]{1,39}$",
        message: "Occupation must be less than 40 characters"
    }
}
Extender.postcode = {
    required: {
        params: true,
        message: "Please enter your Postcode."
    },
    Number: {
        params: true

    },
    pattern: {
        //params: "^[0-9 ]{0,4}$",
        params: "^[0-9]{4}$",
        message: "The Postcode field must be a 4-digit number."
    },
    notzero: true
}
Extender.yearBuilt = {
    required: {
        params: true,
        message: "Please enter the Year Built."
    },
    pattern: {
        params: "^(19[0-9]{2}|200[0-9]|201[0-8])$",
        message: "Please enter a valid year from 1900 to present"
    }
}
Extender.bedroom = {
    required: {
        params: true,
        message: "Please include the number of bedrooms."
    },
    Number: {
        params: true

    },
    pattern: {
        params: "^[0-9]{0,2}?$",
        message: " This field must be a number from 0 to 99."
    }
}
Extender.bathroom = {
    required: {
        params: true,
        message: "Please include the number of bathrooms."
    },
    Number: {
        params: true

    },
    pattern: {
        params: "^[0-9]{0,2}$",
        message: " This field must be a number from 0 to 99."
    }
}
Extender.parkingSpace = {
    required: {
        params: true,
        message: "Please include the number of carparks."
    },
    Number: {
        params: true

    },
    pattern: {
        params: "^[0-9 ]{0,2}$",
        message: "This field must be a number from 0 to 99."
    }
}
Extender.decimalNumeric = function (maxNum, isQuote) {
    var extend = {
        required: {
            params: true,
            message: "Please enter an amount."
        },
        Number: {
            params: true,
            message: "Please enter a number."
        },
        pattern: {
            params: /^[0-9]+(.[0-9]{1,9})?$/,
            //params: /^\d(?:\.\d)?$/,
            // params: /^\d+(\.\d+)?$/,
            message: "Please enter a valid numeric amount upto 2 decimal places."
        }
    };
    if (maxNum) {
        isQuote ? extend.maxQuote = maxNum : extend.maxValue = maxNum;
    }
    else extend.keysMax = true;
    extend.keysMin = true;
    return extend;
}
Extender.decimalMortgage = function (maxNum, isQuote) {
    var extend = {
        required: {
            params: true,
            message: "Please enter an amount."
        },
        Number: {
            params: true,
            message: "Please enter a number."
        },
        pattern: {
            params: /^[0-9]+(.[0-9]{1,9})?$/,
            message: "Please enter a valid numeric amount upto 2 decimal places."
        }
    };
    if (maxNum) {
        isQuote ? extend.maxQuote = maxNum : extend.maxValue = maxNum;
    }
    else extend.mortgageMax = true;
    extend.mortgageMin = true;
    return extend;
}
Extender.commonDecimal = {
    required: {
        params: true,
        message: "Please enter an amount."
    },
    number: {
        params: true,
        message: "Please enter a number."
    },
    pattern: {
        params: "^[0-9]+(.[0-9]{1,2})?$",
        message: "Please enter a valid numeric amount upto 2 decimal places."
    }
}
Extender.integer = function (maxNum) {
    var extend = {
        required: {
            params: true,
            message: "Please enter an amount."
        },
        Number: {
            params: true,
        },
        pattern: {
            params: /^[0-9]+$/,
            message: "Please enter a valid numeric amount."
        }
    };
    if (maxNum) {
        extend.maxValue = maxNum;
    };
    return extend;
}
Extender.textArea = {
    requried : Rules.require("Please enter a Job Description .")
    
};
Extender.title = {
    required: {
        params: true,
        message: "Please enter title."
    },
    minLength: { params: 10, message: "Title must consist at least 10 characters" },
    maxLength: { params: 200, message: "Title should not exceed 200 characters" }
}
Extender.note = {
    required: {
        params: true,
        message: "Please enter note."
    },
    minLength: { params: 10, message: "Note must consist at least 10 characters" },
    maxLength: { params: 500, message: "Note should not exceed 500 characters" }
}
Extender.message = {
    required: {
        params: true,
        message: "Please enter message."
    },
    minLength: { params: 10, message: "Message must consist at least 10 characters" },
    maxLength: { params: 500, message: "Message should not exceed 1000 characters" }
}
Extender.reason = {
    minLength: { params: 10, message: "Reason must consist at least 10 characters" },
    maxLength: { params: 500, message: "Reason should not exceed 500 characters" }
}
Extender.description = {
    required: {
        params: true,
        message: "Please enter description."
    },
    minLength: { params: 10, message: "Description must consist at least 10 characters" },
    maxLength: { params: 500, message: "Description should not exceed 500 characters" }
}
Extender.descriptionRentalListing = {
    required: {
        params: true,
        message: "Please enter description."
    },
    minLength: { params: 10, message: "Description must consist at least 10 characters" },
    maxLength: { params: 1500, message: "Description should not exceed 1500 characters" }
}
Extender.furnishing = {
    requried: Rules.require("Please enter furnishing .(eg. Kitchen,dining etc)"),
    minLength: { params: 1, message: "Furnishing must consist at least 1 characters" },
    maxLength: { params: 100, message: "Furnishing should not exceed 100 characters" }
}
Extender.idealTenant = {
    requried: Rules.require("Please enter ideal tenant .(eg. Non-smoker,tidy etc)"),
    minLength: { params: 1, message: "Ideal Tenant must consist at least 1 characters" },
    maxLength: { params: 100, message: "Ideal Tenant should not exceed 100 characters" }
}
Extender.email =  {
   
        required: {
            params: true,
            message: "Please enter the Email"
        },
        pattern: {
            params: /^[a-z0-9][a-zA-Z0-9_]*(\.[a-zA-Z][a-zA-Z0-9_]*)?@[a-z][a-zA-Z-0-9]*\.[a-z]+(\.[a-z]+)?$/,
            message: "Please enter a valid Email."
        },
        maxLength: { params: 50, message: "Maximum 50 characters only" }
   
    
}
Extender.addressNumber = {
    required: {
        params: true,
        message: "Please enter a number."
    },
    pattern:
    {
        params: "^[0-9][A-Za-z0-9\\s\-\\,\\/\\&\]{0,9}$",
        message: "Please enter a valid Number"
    }
}
Extender.addressStreet = {
    required: {
        params: true,
        message: "Please enter street."
    },
    pattern:
         {
              params: "^[A-Za-z0-9][A-Za-z0-9\\s\-\\,\\/\\&\]{0,100}$",
             message: "Please enter 1-100 alphanumeric characters only"
         }
}
Extender.addressSuburb = {
    required:
    {
        params: true, message: "Please enter Suburb"
    },
    pattern:
    {
        params: "^[A-Za-z\\s]{0,50}$",
        message: "The suburb field can only accept only 1-50 characters"
    }
}
Extender.addressCity = {
    required:
    {
        params: true,
        message: "Please enter your City"
    },
    pattern:
    {
        params: "^[A-Za-z\\s]{0,50}$",
        message: " Please enter a valid City"
    }
};
Extender.phoneNumber = {
    required: {
        params: true,
        message: "Please enter a phone number"
    },
    Number: {
        params: true,
        message: "This field must be numerical"
    },
    pattern: {
        params: "^\\d{8,10}$",
        message: "Please enter a valid phone number"
    }
}
Extender.homePhoneNumber = {
    required: {
        params: true,
        message: "Please enter a phone number."
    },
    Number: {
        params: true,
    },
    pattern: {
        params: "^[0-9]{9,12}$",
        message: "Please enter valid phone number."
    }
}
Extender.propertyName = {
    required: {
        params: true,
        message: "Please enter your property's name."
    },
    pattern: {
        params: "^[\\w\\d\\s]{4,30}$",
        message: "The Property Name field must be between 4-30 alphanumeric characters."
    }
}
Extender.addressName = {
    required: {
        params: true,
        message: "Please enter an address.",
    }
}
Extender.mobilePhoneNumber = {
    required: {
        params: true,
        message: "Please enter mobile phone number."
    },
    Number: {
        params: true,
    },
    pattern: {
        //params: /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/,
        params: "^[0-9]{10,12}$",
        message: "Please enter valid mobile number."
    }
}
Extender.website = function (options) {
    var extend = {
        pattern: {
            params: /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi,
            message: "Please enter a valid website."
        },
        maxLength: { params: 50, message: "Maximum 50 characters only" }
    };
    if (options && options.required) {
        extend.required = {
            params: true,
            message: "Please enter the website"
        };
    }
    return extend;
}
Extender.websiteCompany = {
    pattern: {
        params: /[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?/gi,
        message: "Please enter a valid website."
    },    
    maxLength: {
        params: 50, message: "Maximum 50 characters only"
    },
    required: {
        params: true,
        message: "Please enter a website"
    }
};
Extender.linkedinUrl = {
    pattern: {
        params: "^https?://((www|\w\w)\.)?linkedin.com/((in/[^/]+/?)|(pub/[^/]+/((\w|\d)+/?){3}))$",
        message: "Please enter a valid Linkedin URL."
    }
};
Extender.targetRent = {
    required: {
        params: true,
        message: "Please enter target rent."
    },
    number: {
        params: true,
        message: "Rent amount must be a number."
    },
    pattern: {
        params: "^[0-9]+(.[0-9]{1,2})?$",
        message: "Rent amount must be a number and can be upto two decimal places."
    }
}
Extender.landArea = {
   
    number: {
        params: true,
        message: "Land area must be a number."
    },
    pattern: {
        params: "^[0-9]{0,6}$",
        message: "Land area must be a number and musn't exceed six digits."
    }
}
Extender.floorArea = {
    number: {
        params: true,
        message: "Floor area must be a number."
    },
    pattern: {
        params: "^[0-9]{0,6}$",
        message: "Floor area must be a number and musn't exceed six digits."
    }
}
Extender.dateOfBirth = function(){

}
Extender.date = function (options) {
    var extend = {
        pattern:{
            params: "^(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.]((19|20)\\d\\d)$",
            message:"Please enter dd/mm/yyyy format"
        }
    };
    if (options.isDate && options.isDate == true) {
        extend.date = true;
    }
    if (options.required && options.required == true) {
        extend.required = {
            params: true,
            message: "Please enter a date."
        }
    }

    if (options.maxDate) {
        extend.maxDate = moment(options.maxDate);
    }
    if (options.minDate) {
        extend.minDate = moment(options.minDate);
    }
    return extend;
}
Extender.occupantCount = {
    required: {
        params: true,
        message: "Please enter the count of occupants."
    },
    Number: {
        params: true,
    },
    pattern: {
        params: /^[0-9]{0,5}$/,
        message: "Please enter a valid numeric amount."
    }
}
Extender.movingCost = {
    required: {
        message: "Please enter a moving cost."
    },
    Number: {
        params: true,
    },
    pattern: {
        params: /^[0-9]{1,5}(\.[0-9]{1,2})?$/,
        message: "Please enter a valid numeric amount upto 2 decimal places"
    }
}
Extender.availableDate = {
    required: {
        params: true, message: "Please enter a date."
    }
}
Extender.petsAllowed = { required: { param: true, message: "Please select a choice." } }
var KeysUtils = {};
KeysUtils.injectMediaProps = function (fvm) {
    if (fvm.Name) {
        fvm.OldFileName = ko.observable(fvm.Name);
    }
}
KeysUtils.injectExtends = function (model, extendDic) {
    for (var key in extendDic) {
        if (typeof extendDic[key] === 'object') {
            KeysUtils.injectExtends(model[key], extendDic[key])
        }
        else if (extendDic[key] instanceof Function) {
            model[key].extend(extendDic[key]());
        }
        else {
            model[key].extend(Extender[extendDic[key]]);
        }
    }
}
KeysUtils.injectViewProps = function (viewModel) {
    var addr = viewModel.Address || viewModel.Model.Address;
    if (addr) { 
        viewModel.StreetAddress = KeysUtils.toAddressStreet(addr);
        viewModel.CitySub = KeysUtils.toCitySub(addr);
    }
    if (viewModel.Model.LandlordMedia || viewModel.LandlordMedia) {
        viewModel.ImgFiles1 = ko.computed(function () {
            var files = viewModel.Model.LandlordMedia || viewModel.LandlordMedia;
            return files().filter(function (element) {
                var type = ko.utils.unwrapObservable(element.MediaType);
                return type == 1;
            });
        });
    }
    if (viewModel.Model.MediaFiles || viewModel.MediaFiles) {
        viewModel.FirstPhoto = ko.computed(function () {
            var files = viewModel.Model.MediaFiles || viewModel.MediaFiles;
            return KeysFiles.getFirstPhoto(files);
        });
        viewModel.ImgFiles = ko.computed(function () {
            var files = viewModel.Model.MediaFiles || viewModel.MediaFiles;
            return files().filter(function (element) {
                var type = ko.utils.unwrapObservable(element.MediaType);
                return type == 1;
            });
        });
        viewModel.DocFiles = ko.computed(function () {
            var files = viewModel.Model.MediaFiles || viewModel.MediaFiles;
            return files().filter(function (element) {
                var type = ko.utils.unwrapObservable(element.MediaType);
                return type == 2;
            });
        });
    }
    viewModel.Errors = ko.validation.group(viewModel.Model, { deep: true, observable: true, live: true });
    viewModel.IsValid = ko.computed(function () {
        var errors = viewModel.Errors().length;
        return viewModel.Errors().length == 0;
    });
}
KeysUtils.injectOpProps = function (viewModel, editUrl, deleteUrl) {
    var token =  $("input[name = '__RequestVerificationToken']").val();
    function reload()
    {
        location.reload();
    }
    viewModel.MainView = ko.observable(true);
    viewModel.DetailView = ko.observable(false);
    viewModel.EditView = ko.observable(false);
    viewModel.DeleteView = ko.observable(false);
    viewModel.SelectedItem = ko.observable();
    viewModel.SelectedItemCopy = ko.observable();
    viewModel.ShowDetail = function (item) {
        viewModel.SelectedItem(item);
        viewModel.DetailView(true);
        viewModel.MainView(false);
        viewModel.EditView(false);
        viewModel.DeleteView(false);
    }
    viewModel.ShowEdit = function (item) {
        viewModel.SelectedItem(item);
        var itemCopy = ko.mapping.fromJS(ko.toJS(item));
        KeysUtils.injectExtends(itemCopy.Model, viewModel.ExtendDic);
        KeysUtils.injectViewProps(itemCopy);
        viewModel.SelectedItemCopy(itemCopy);
        viewModel.EditView(true);
        viewModel.MainView(false);
        viewModel.DetailView(false);
        viewModel.DeleteView(false);
        console.log( viewModel.SelectedItemCopy())
    }
    viewModel.ShowDelete = function (item) {
        viewModel.SelectedItem(item);
        viewModel.DeleteView(true);
        viewModel.MainView(false);
        viewModel.EditView(false);
        viewModel.DetailView(false);
    }
    viewModel.ShowMain = function () {
        viewModel.MainView(true);
        viewModel.DetailView(false);
        viewModel.EditView(false);
        viewModel.DeleteView(false);
    }
    viewModel.RemoveFile = function (file) {
        if (file.Status() == 'load') {
            viewModel.SelectedItemCopy().Model.FilesRemoved.push(file.Id());
        }
        viewModel.SelectedItemCopy().Model.MediaFiles.remove(file);
    }
    viewModel.EditSuccess = function (result) {
        if (result.Success || result.success) {
            //viewModel.SelectedItem().Model.MediaFiles(ko.mapping.fromJS(result.MediaFiles)());
            KeysUtils.notification.show('<p>Item edited successfully</p>', 'notice', reload);
        }
        else {
            KeysUtils.notification.showErrorMsg();
        }
    }
    viewModel.DeleteSuccess = function (result) {
        if (result.Success) {
            viewModel.Items.remove(viewModel.SelectedItem());
            KeysUtils.notification.show('<p>Item deleted successfully</p>', 'notice', viewModel.ShowMain);
        }
        else {
            KeysUtils.notification.showErrorMsg();
        }
        
    }
    viewModel.Edit = function (data) {
        var formData = KeysUtils.toData(data.Model);
        formData.append('__RequestVerificationToken', token);
        var url = viewModel.EditUrl() || editUrl;
        if (url) KeysUtils.post(url, formData, viewModel.EditSuccess);
    }
    viewModel.Delete = function (data) {
        var url = viewModel.DeleteUrl() || deleteUrl;
        //debugger;
        if (url) KeysUtils.postDelete(url, data.Model.Id(), viewModel.DeleteSuccess);
    }
}

KeysUtils.postDelete = function (url, id, successCallback, errorCallback) {
    var data = { id: id, __RequestVerificationToken : $("input[name = '__RequestVerificationToken']").val() };
    $.ajax({
        type: "POST",
        url: url,
        data: data,
        success: function (result) {
            if (successCallback)
                successCallback(result);
        },
        error: function (result) {
            if (errorCallback)
                errorCallback(result);

        },
    });
}
KeysUtils.post = function (url, formData, successCallback, errorCallback) {
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {
            if (successCallback)
                successCallback(result);
        },
        error: function (result) {
            if (errorCallback)
                errorCallback(result);

        },
    });
}

KeysUtils.toData = function (obj, form, namespace) {
    var fd = form || new FormData();
    var formKey;
    for (var property in obj) {
        if (obj.hasOwnProperty(property)) {
            var val = ko.utils.unwrapObservable(obj[property]);
            if (namespace) {
                formKey = namespace + '[' + property + ']';
            } else {
                formKey = property;
            }
            if (property == 'MediaFiles') {
                val.forEach(function (element) {
                    var status = ko.utils.unwrapObservable(element.Status);
                    if (status == 'add') {
                        fd.append("Files", element.File);
                    }
                });
            }
            else if (property == 'FilesRemoved') {
                val.forEach(function (element) {
                    fd.append("FilesRemoved", element);
                });
            }
            else if (val instanceof Date) {
                fd.append(formKey, val.toUTCString());
            }
            else if (typeof obj[property] === 'object' && !(obj[property] instanceof File)) {
                KeysUtils.toData(obj[property], fd, property);
            } else {
                val = typeof val != 'undefined' ? val : null;
                fd.append(formKey, val);
            }
        }
    }
    return fd;
};

KeysUtils.toFormData = function (viewModel, token) {
    var formData = new FormData();
    for (var key in viewModel) {
        var val = ko.utils.unwrapObservable(viewModel[key]);
        //var val = ko.isObservable(viewModel[key]) ? viewModel[key]() : viewModel[key];
        if (key == 'MediaFiles') {
            val.forEach(function (element) {
                var status = ko.utils.unwrapObservable(element.Status);
                if (status == 'add') {
                    formData.append("Files", element.File);
                }
            });
        }
        else if (key == 'FilesRemoved') {
            val.forEach(function (element) {
                formData.append("FilesRemoved", element);
            });
        }
        else if (val instanceof Date) {
            formData.append(key, val.toUTCString());
        }
        else {

            formData.append(key, val);
        }

    }
    if (token) {
        formData.append("__RequestVerificationToken", token);
    }
    return formData;
}
KeysUtils.postSuccess = function (result) {
    if (result.Success) {
        viewModel.Items.remove(viewModel.SelectedItem());
        KeysUtils.notification.show('<p>' + result.Message + '</p>', 'notice', KeysUtils.reload);
    }
    else {
        KeysUtils.notification.showErrorMsg();
    }
}
KeysUtils.reload = function () {
    window.location.reload();
}
KeysUtils.goPage = function (url) {
    window.location = url;
}
KeysUtils.clearObservable = function (vm) {
    for (var index in vm) {
        if (ko.isObservable(vm[index])) {
            vm[index](null);
        }
    }
};
KeysUtils.copyObservable = function (a, b) {
    for (var key in a) {
        if (b[key]) {
            if (ko.isObservable(a[key])) {
                b[key](a[key]());
            }
            else b[key](a[key]);
        }
        
    }
}
KeysUtils.parseJsonDate = function(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}
KeysUtils.toDotnetDate = function (date) {
    if (!date) return null;
    var dt = moment(date, 'DD/MM/YYYY h:mm A');
    return moment(dt).format('DD/MM/YYYY h:mm A');
}
KeysUtils.moveFormNext = function (element) {
    var current_fs, next_fs, previous_fs; //fieldsets
    var left, opacity, scale; //fieldset properties which we will animate
    var animating; //flag to prevent quick multi-click glitches
    if (animating) return false;
    animating = true;

    current_fs = element.parent();
    next_fs = element.parent().next();

    //activate next step on progressbar using the index of next_fs
    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

    //show the next fieldset
    next_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale current_fs down to 80%
            scale = 1 - (1 - now) * 0.2;
            //2. bring next_fs from the right(50%)
            left = (now * 50) + "%";
            //3. increase opacity of next_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'transform': 'scale(' + scale + ')' });
            next_fs.css({ 'left': left, 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
}
KeysUtils.toAddressStreet = function (address) {
    var val = '';
    var num = ko.isObservable(address.Number) ? address.Number() : address.Number;

    if (num != null){
        val = val + num.trim() + " ";
    }
    var st = ko.isObservable(address.Street) ? address.Street() : address.Street;
    if (st != null) {
        val = val + st.trim() + ",  ";
    }
    return val;
}
KeysUtils.toCitySub = function (address) {
    var val = '';
    var sb = ko.isObservable(address.Suburb) ? address.Suburb() : address.Suburb;
    //debugger;
    if (!(!sb || 0 === sb.length)) {
        val = val + sb.trim() + ", ";
    }
    var city = ko.isObservable(address.City) ? address.City() : address.City;
    if (city == sb) {
        val = val + " ";
    }
    else if (city != null) {
        val = val + city.trim();
    }
    //var pc = ko.isObservable(address.PostCode) ? address.PostCode() : address.PostCode;
    //if (pc != null) {
    //    val = val + pc.trim() + " ";
    //}
    val = val.trim();
    val = val[val.length - 1] == ',' ? val.slice(0, -1) : val;
    return val;
}
KeysUtils.fullAddress = function (address) {
    var num = ko.isObservable(address.Number) ? address.Number() : address.Number;
    if (num != null){
        var val = (address.Number).trim() + " ";
    }
    var st = ko.isObservable(address.Street) ? address.Street() : address.Street;
    if(st != null){
        val = val + (address.Street).trim() + ",  ";
    }
    var sb = ko.isObservable(address.Suburb) ? address.Suburb() : address.Suburb;
    if (sb !=null) {
        val = val + (address.Suburb).trim() + ", ";
    }
    var city = ko.isObservable(address.City) ? address.City() : address.City;
    if (city == sb) {
        val = val + " ";
    }
    else if(city != null){
        val = val + (address.City).trim() + ", ";
    }
    var pc = ko.isObservable(address.PostCode) ? address.PostCode() : address.PostCode;
    if(pc != null){
        val = val + (address.PostCode).trim() + " ";
    }
    //if (address.Number != null) {
    //    if (address.Number != null)
    //    { var val = (address.Number).trim() + " "; }
    //    if (address.Street != null)
    //    { val = val + (address.Street).trim() + ",  "; }
    //    if (address.Suburb != null)
    //    { val = val + (address.Suburb).trim() + ", "; }
    //    if (address.City == address.Suburb)
    //    { val = val + " "; }
    //    else if (address.City != null)
    //    { val = val + (address.City).trim() + ", "; }
    //    if (address.PostCode != null)
    //    { val = val + (address.PostCode).trim() + " "; }
    //}
    return val;
}
KeysUtils.notification = {}
KeysUtils.notification.errorMsg = '<p>Something went wrong, please try again later.</p>';
KeysUtils.notification.showErrorMsg = function () {
  
    KeysUtils.notification.show(KeysUtils.notification.errorMsg, 'error');
}
KeysUtils.notification.show = function (message, type, onCloseCallback, openCallback) {
    var notification = new NotificationFx({
        wrapper: document.getElementById('main-content') || document.body,
        message: message,
        //layout: 'bar',
        //effect: 'slidetop',
        layout: 'growl',
        effect: 'jelly',
        type: type,
        ttl: 2000,
        //closeTime: 5000,
        onClose: function () {
            if (onCloseCallback) onCloseCallback();
            else return false;
        },
        onOpen: function () {
            if (openCallback) openCallback();
            else return false;
        }
    });

    notification.show();
}
KeysUtils.getPartialText = function (param) {
    var text = ko.isObservable(param) ? param() : param;
    if (text == null || text == "" || text == undefined)
        return;

    if (text.length < 120)
        return text;

    var part = "";
    for (var i = 0; i < 120; i++) {
        part += text[i];
    }

    return part += "...";
}
KeysUtils.findNzDistrictAndSuburb = function (city, suburb, locatities) {
    var foundReg;
    var foundDistrict;
    for (var i = 0; i < locatities.length; i++) {
        var reg  = locatities[i];
        var dists = reg.Districts;
        for (var j = 0; j < dists.length; j++) {
            var dist = dists[j]
            var subs = dist.Suburbs
            for (var k = 0; k < subs.length; k++) {
                var sub = subs[k];
                if (sub.Name.indexOf(suburb) > -1) {
                    foundDistrict = dist.Name;
                    foundReg = reg;
                }
            }
        }
    }
    debugger;
}
var Validator = {};
Validator.dateAfter = function (val, otherVal) {
    var d1 = new Date(val),
        d2 = new Date(otherVal);
    if (!isNaN(d1.getTime())) {
        return d1 > d2;
    }
    
    return true;
}
Validator.dateBefore = function (val, otherVal) {
    var d1 = new Date(val),
        d2 = new Date(otherVal);
    return d1 < d2;
}
ko.validation.rules['dateRequired'] = {
    validator: function (val, otherVal) {
        debugger;
        if (!val && otherVal) return false;
        return true;
    },
    message: 'Please enter a date'
}
ko.validation.rules['datePickerBefore'] = {
    validator: function (val, otherVal) {
        debugger;
        var date = moment.isDate(val) ? val : new Date(parseFloat(val.replace(/[^0-9]/g, '')));
        if (!moment.isDate(date)) return false;
        return date <= otherVal;
    },
    message: 'Date must not be before end date'
}

ko.validation.rules['datePickerAfter'] = {
    validator: function (val, otherVal) {
        if (val) {
            var date = moment.isDate(val) ? val : new Date(parseFloat(val.replace(/[^0-9]/g, '')));
            if (!moment.isDate(date)) return false;
            var valid = date >= otherVal;
            return valid;
        }
        
        return true;
    },
    message: 'End date must be after start date'
}
ko.validation.rules['datePickerPaymentDate'] = {
    validator: function (val, otherVal) {
        debugger;
        if (val) {
            var date = moment.isDate(val) ? val : new Date(parseFloat(val.replace(/[^0-9]/g, '')));
            if (!moment.isDate(date)) return false;
            var valid = date >= otherVal;
            return valid;
        }

        return true;
    },
    message: 'Payment start date must be after start date'
}
ko.validation.rules['datePickerAfterYear'] = {
    validator: function (val, otherVal) {
        var date = moment.isDate(val) ? val : new Date(parseFloat(val.replace(/[^0-9]/g, '')));
        if (!moment.isDate(date)) return false;
        var year = date.getFullYear();
        return year >= otherVal;
    },
    message: 'Date must not be before {0}'
}
ko.validation.rules['dateAfterYearBuilt'] = {
    validator: function (val, otherVal) {
        var d1 = new Date(val).getFullYear();
        return d1 >= otherVal;
    },
    message: 'Date must not be before {0}'
}

ko.validation.rules['dateBefore'] = {
    validator: Validator.dateBefore,
    message: 'The start date must be before the end date'
}
ko.validation.rules['dateAfterStart'] = {
    validator: Validator.dateAfter,
    message: 'End date must be after start date'
}
ko.validation.rules['maxDate'] = {
    validator: function (val, otherVal) {
        var inputDate = moment(val, 'DD-MM-YYYY');
        var otherDate = moment(otherVal, 'DD-MM-YYYY');
        var compare = inputDate < otherDate;
        return inputDate < otherDate;
    },
    message: 'Invalid date, should not be future date'
}
ko.validation.rules['minDate'] = {
    validator: function (val, otherVal) {
        var inputDate = moment(val);
        var otherDate = moment(otherVal);
        debugger;
        return inputDate > otherDate;
    },
    message: 'Invalid past date'
}
ko.validation.rules['numeric'] = {
    validator: function (val, required) {
        val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
        var res = val.match(/^[1-9]\d*(\.\d*)?$/);
        if (res == null) {
            return false;
        } else {
            return true;
        }
    },
    message: 'Please enter valid Amount'
};
var isLessThan = function (val, otherVal) {
    if (KeysPattern.numeric.test(val)){
        return parseFloat(val) <= parseFloat(otherVal);
    }
    return null;
};
ko.validation.rules['maxValue'] = {
    validator: isLessThan,
    message: 'The field must less than or equal to {0}'
};
ko.validation.rules['maxQuote'] = {
    validator: isLessThan,
    message: 'The field must not exceed maximum budget'
};
ko.validation.rules['keysMax'] = {
    validator: function (val, required) {
        if (typeof val == "string") {
            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            val = parseFloat(val);
        }
        if (val > 9999999) {
            return false;
        } else {
            return true;
        }
    },
    message: 'Enter amount between $[1-9999999]'
};
ko.validation.rules['keysMin'] = {
    validator: function (val, required) {
        if (typeof val == "string") {
            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            val = parseFloat(val);
        }
        if (val < 1) {
            return false;
        } else {
            return true;
        }
    },
    message: 'Enter amount between $[1-9999999]'
};
ko.validation.rules['mortgageMax'] = {
    validator: function (val, required) {
        if (typeof val == "string") {
            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            val = parseFloat(val);
        }
        if (val > 9999999) {
            return false;
        } else {
            return true;
        }
    },
    message: 'Enter amount between $[0-9999999]'
};
ko.validation.rules['mortgageMin'] = {
    validator: function (val, required) {
        if (typeof val == "string") {
            val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
            val = parseFloat(val);
        }
        if (val < 0) {
            return false;
        } else {
            return true;
        }
    },
    message: 'Enter amount between $[0-9999999]'
};
function decimalPlaces(num) {
    var match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);

    if (!match) { return 0; }
    return Math.max(
         0,
         // Number of digits right of decimal point.
         (match[1] ? match[1].length : 0)
         // Adjust for scientific notation.
         - (match[2] ? +match[2] : 0));
};
ko.validation.rules['dp'] = {
   
    validator: function (val, required) {
        debugger;
        var dp = decimalPlaces(val)
        if (dp > 2) {
            return false;
        } else {
            return true;
        }
    },
    message: ''
};

ko.validation.rules['existingEmail'] = {
    validator: function (val, otherVal) {
       return otherVal == false ;
    },
    message: 'This tenant already exist within this property.'
};

ko.validation.rules['SupplierOrOwner'] = {
    validator: function (val, otherVal) {
        return otherVal == false;
    },
    message: 'This email already exist as a Supplier or Owner.'
};

ko.validation.registerExtenders();

ko.bindingHandlers.numericKeyOnly = {
    init: function (element) {
        $(element).on("keydown", function (event) {
            // Allow: backspace, delete, tab, escape, and enter
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: . ,
                (event.keyCode == 188 || event.keyCode == 190 || event.keyCode == 110) ||
                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            }
            else {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });
    }
};
ko.bindingHandlers.toFixed2 = {
    init: function (element, valueAccessor, allBindings) {
        var value = valueAccessor();
        var valueUnwrapped = ko.unwrap(value);
        value(valueUnwrapped.toFixed(2));
    },
}
ko.bindingHandlers.trimTo2dp = {
    init: function (element, valueAccessor, allBindings) {
        var valueObservable = valueAccessor();
        ko.utils.registerEventHandler(element, "change", function () {
            if (KeysPattern.numeric.test($(element).val())) {
                var num = parseFloat($(element).val()).toFixed(2);
                if (!isNaN(num)) {
                    $(element).val(num);
                }
            }
        });
    },

    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        if (KeysPattern.numeric.test($(element).val())) {
            var num = parseFloat($(element).val()).toFixed(2);
            if (!isNaN(num)) {
                $(element).val(num);
            }
        }
    }
};

ko.bindingHandlers.toNumberFormat = {

    init: function (element, valueAccessor, allBindings) {
        var valueObservable = valueAccessor();

        var num = parseFloat($(element).val());
        if (!isNaN(num)) {
            $(element).val(num.toLocaleString());
        }
        ko.utils.registerEventHandler(element, "change", function () {
            if (KeysPattern.numeric.test($(element).val())) {
                var num = parseFloat($(element).val());
                debugger;
                if (!isNaN(num)) {
                    $(element).val(num.toLocaleString());
                }
            }
        });
    },

    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        if (KeysPattern.numeric.test($(element).val())) {
            var num = parseFloat($(element).val());
            debugger;
            if (!isNaN(num)) {
                $(element).val(num.toLocaleString());
            }
        }
    }
};
ko.bindingHandlers.numericValue = {
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var allBindings = ko.utils.unwrapObservable(allBindings());

        if (allBindings.value()) {
            // use informed precision or default value
            var precision = allBindings.numericValue.precision || 1;

            // prevent rounding
            var regex = new RegExp('^-?\\d+(?:\.\\d{0,' + precision + '})?');

            // update observable
            allBindings.value(parseFloat(allBindings.value().match(regex)[0]).toFixed(precision));
        }
    }
};
ko.bindingHandlers.stopBinding = {
    init: function () {
        return { controlsDescendantBindings: true };
    }
};
function FileModel(f) {
    if (f.Id) this.Id = f.Id;
    if (f.File) this.File = f.File;
    if (f.Data) this.Data = f.Data;
    if (f.OldFileName) this.OldFileName = f.OldFileName;
    if (f.Name) {
        this.Name = f.Name;
        this.OldFileName = f.Name;
    }
    if (f.NewFileName) this.NewFileName = f.NewFileName;
    this.Status = f.Status;
    this.MediaType = f.MediaType;
}
ko.bindingHandlers.clickRemoveMediaFile = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var name = valueAccessor();
        $(element).on("click", function () {
            debugger;
            var data = bindingContext.$parent.Data || bindingContext.$parent.Model;
            var mediaFiles = data[name]
            var filesRemoved = bindingContext.$parent.Data.FilesRemoved || bindingContext.$parent.Model[name];
            if (viewModel.Status == 'load') {
                if (filesRemoved) filesRemoved.push(viewModel.Id);
            }
            mediaFiles.remove(viewModel);
            
        });
        
    },
}
ko.bindingHandlers.datePicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().dateTimePickerOptions || {};
        $(element).datetimepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            var currVal = $(element).val();
            if (!currVal) {
                value(currVal);
                return;
            }
            
            var d = value();
            if (ko.isObservable(value)) {
                if (event.date != null && !(event.date instanceof Date)) {
                    if (event.date != false && 'toDate' in event.date) {
                        value(event.date.toDate());
                    }
                } else {
                    value(event.date);
                }
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("DateTimePicker");
            if (picker) {
                picker.destroy();
            }
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

        var picker = $(element).data("DateTimePicker");
        //when the view model is updated, update the widget
        if (picker) {
            var koDate = ko.utils.unwrapObservable(valueAccessor());
            if (ko.isObservable(koDate)) koDate = koDate();
            if (koDate) {
                //in case return from server datetime i am get in this form for example /Date(93989393)/ then fomat this
                koDate = (typeof (koDate) !== 'object') ? new Date(parseFloat(koDate.replace(/[^0-9]/g, ''))) : koDate;
                picker.date(koDate);
            }

        }
    }
};
ko.validation.makeBindingHandlerValidatable('datePicker');
ko.bindingHandlers.uploadFile = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) { },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var fileOb = valueAccessor();
        var allBinds = allBindings();
        var maxFileSize = allBindings.get('totalMaxSize') || 2000000;
        var validFiles = allBindings.get('validFileTypes') || KeysFiles.validFileTypes;
        var display = allBindings.get('display');
        $(element).change(function (event) {
            var files = event.target.files;
            if (files && files[0]) {
                var f = files[0];
                if (!f.type.match('image.*')) {
                    KeysUtils.notification.show('<p>File not supported!</p>', 'error');
                    element.value = '';
                    return;
                }
                if (f.size >= 2000000) {
                    KeysUtils.notification.show('<p>File size must less than 2MB!</p>', 'error');
                    element.value = '';
                    return;
                }
                else {
                    var reader = new FileReader();
                    reader.onloadend = function (e) {
                        var result = e.target.result;
                        var fileData = {
                            Data: result,
                            File : f,
                        };
                        fileOb(f);
                        display(result);
                    }
                    reader.readAsDataURL(f);
                }
            }
        });
    }
}
ko.bindingHandlers.removeFile = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) { },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var fileOb = valueAccessor();
        var allBinds = allBindings();
        var maxFileSize = allBindings.get('totalMaxSize') || 2000000;
        var validFiles = allBindings.get('validFileTypes') || KeysFiles.validFileTypes;
        var elName = allBindings.get('element')
        var display = allBindings.get('display');
        $(element).click(function () {
            fileOb(null);
            display(null);
            var el = document.getElementById(elName);
            el.value = '';
        });
    }
}
ko.bindingHandlers.uploadFiles = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) { },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var mediaFiles = valueAccessor();
        var allBinds = allBindings();
        var maxFileSize = allBindings.get('totalMaxSize') || 5000000;
        var validFiles = allBindings.get('validFileTypes') || KeysFiles.validFileTypes;
        var fileWarning = allBindings.get('fileWarning') || ko.observable();
        var maxNumberFiles = allBindings.get('maxFiles') || 5;
        $(element).change(function (event) {
            var files = this.files;
            var length = files.length;
            if (files.length + mediaFiles().length > maxNumberFiles) {
                fileWarning(KeysWarning.maxNumberFiles(maxNumberFiles));
                element.value = '';
                return;
            }
            for (var i = 0; i < files.length; i++) {
                if (!KeysFiles.isFileValid(validFiles, files[i])) {
                    fileWarning(KeysWarning.fileType);
                    KeysUtils.notification.show('<p>Selected File Not Supported!</p>', 'error');
                    element.value = '';
                    return;
                }
                else if (typeof (maxFileSize) != "undefined" && files[i].size + KeysFiles.getTotalSize(mediaFiles()) >= maxFileSize) {
                    var total = files[i].size + KeysFiles.getTotalSize(mediaFiles());
                    fileWarning(KeysWarning.maxFileSize);
                    element.value = '';
                    break;
                }
                else {
                    (function (file) {
                        var reader = new FileReader();
                        reader.onloadend = function (e) {
                            var result = e.target.result;
                            var photo = {
                                Data: result,
                                File: file,
                                Name: file.name,
                                Status: "add",
                                MediaType: KeysFiles.getType(file),
                            };
                            var res = mediaFiles();
                            var item = ko.mapping.fromJS(photo);
                            item.File = file;
                            KeysUtils.injectMediaProps(item);
                            mediaFiles.push(item);
                        };
                        fileWarning('');
                        reader.readAsDataURL(file);
                    })(files[i]);
                }
            }

            element.value = '';
        });
    }
};
ko.bindingHandlers.filesUpload = {
    
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {},
    
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
      
        var mediaFiles = valueAccessor();
        var allBinds = allBindings();
        var maxFileSize = allBindings.get('totalMaxSize') || 5000000;
        var validFiles = allBindings.get('validFileTypes') || KeysFiles.validFileTypes;
        var fileWarning = allBindings.get('fileWarning') || ko.observable();
        var maxNumberFiles = allBindings.get('maxFiles') || 5;
        $(element).change(function (event) {
            
            var files = this.files;
            var length = files.length;
            if (files.length + mediaFiles().length > maxNumberFiles) {
                fileWarning(KeysWarning.maxNumberFiles(maxNumberFiles));
                KeysUtils.notification.show('<p>Cannot Upload More Than ' + maxNumberFiles+' Files!</p>', 'error');
                element.value = '';
                return;
            }
            for (var i = 0; i < files.length; i++) {
                if (!KeysFiles.isFileValid(validFiles, files[i])) {
                    fileWarning(KeysWarning.fileType);
                    KeysUtils.notification.show('<p>Selected File Not Supported!</p>', 'error');
                    element.value = '';
                }
                else if (typeof (maxFileSize) != "undefined" && files[i].size + KeysFiles.getTotalSize(mediaFiles()) >= maxFileSize) {
                    var total = files[i].size + KeysFiles.getTotalSize(mediaFiles());
                    fileWarning(KeysWarning.maxFileSize);
                    KeysUtils.notification.show('<p>Selected File Is Too Big!</p>', 'error');
                    element.value = '';
                    break;
                }
                else {
                    (function (file) {
                        var reader = new FileReader();
                        reader.onloadend = function (e) {
                            var result = e.target.result;
                            var photo = {
                                Data: result,
                                File: file,
                                Name: file.name,
                                Status: "add",
                                MediaType: KeysFiles.getType(file),
                            };
                            var res = mediaFiles();
                            mediaFiles.push(new FileModel(photo));
                        };
                        fileWarning('');
                        reader.readAsDataURL(file);
                    })(files[i]);
                }
            }

            element.value = '';
        });
    }
};
ko.bindingHandlers.inViewport = {
    init: function (element, valueAccessor) {
        var opts = valueAccessor();

        if (!opts) throw new Error('inViewport requires an argument.')

        if (!opts.viewport)
            throw new Error('viewport option is required.');
        var viewportNode = document.getElementById(opts.viewport);

        if (!opts.observable)
            throw new Error('observable option is required.');
        // Store a reference to the setter on the element, we'll 
        // use this later in the updateVisibility callback.
        element.___visibilityObservable = opts.observable;

        // Ensure a state object on the viewportNode
        // that is shared between the bindings.
        var shared = viewportNode.__vsscrollshared ||
                     (viewportNode.__vsscrollshared = {});

        if (shared.candidates) { // Already initialized
            shared.candidates.push(element);
        } else { // Not initialized

            // All elements that can be visible are stored in candidates
            shared.candidates = [element];

            var isInViewport = function (element) {
                var elementTop = element.offsetTop,
                    elementBottom = elementTop + element.offsetHeight,
                    viewportTop = viewportNode.scrollTop,
                    viewportBottom = viewportTop + viewportNode.offsetHeight;
                return elementBottom > viewportTop && elementTop < viewportBottom;
            };

            var updateVisibility = function () {
                shared.candidates.forEach(function (c) {
                    c.___visibilityObservable(isInViewport(c));
                });
            };

            var onViewportChanged = function () {
                // Update visibility when viewport has changed, 
                // but throttle it slightly so we don't get a hojillion
                // kabillion events per second.
                clearTimeout(shared.throttleHandle);
                shared.throttleHandle = setTimeout(updateVisibility, 250);
            }

            viewportNode.addEventListener('scroll', onViewportChanged);
            viewportNode.addEventListener('resize', onViewportChanged);

            // Trigger initial visiblity update, but defer execution
            // so that all elements are allowed to render first.
            setTimeout(updateVisibility, 0);
        }

    }
};
ko.bindingHandlers.numeric = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var positions = ko.utils.unwrapObservable(allBindingsAccessor().positions) || ko.bindingHandlers.numeric.defaultPositions;
        var formattedValue = parseFloat(value).toFixed(positions);
        var finalFormatted = ko.bindingHandlers.numeric.withCommas(formattedValue);

        ko.bindingHandlers.text.update(element, function () { return finalFormatted; });
    },
    defaultPositions: 2,
    withCommas: function (original) {
        original += '';
        x = original.split(',');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }
};
var KeysMap = {};
KeysMap.makeInfoWindow = function(){
    return new google.maps.InfoWindow();
}
KeysMap.googleMapMarkers = [];
KeysMap.intitialiseMap = function (pos) {
    map = new google.maps.Map(document.getElementById('map'), {
        center: pos,
        zoom: 15
    });
    var marker = new google.maps.Marker({ map: map, position: pos, icon: KeysMap.makeMarkerImage() });
}
KeysMap.makeMarkerImage = function () {
    return new google.maps.MarkerImage(
    "/images/map-marker.png",
    new google.maps.Size(71, 71),
    new google.maps.Point(0, 0),
    new google.maps.Point(17, 34),
    new google.maps.Size(30, 30));
}
KeysMap.createInfoMarker = function (place, infoWindow) {
    var placeLoc = { lat: place.lat, lng: place.lng };
    var marker = new google.maps.Marker({
            map: map,
            position: placeLoc
    });

    google.maps.event.addListener(marker, 'click', function () {
        infoWindow.setContent(place.content);
        infoWindow.open(map, this);
    });

    return marker;
}
KeysMap.setMarkers = function (map) {
    for (var i = 0; i < KeysMap.googleMapMarkers.length; i++) {
        KeysMap.googleMapMarkers[i].setMap(map);
    }
}
KeysMap.clearMarkers = function (markers) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    };
    markers = [];
}
ko.bindingHandlers.showMap = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var address = ko.isObservable(value) ? value() : value;
        var lat = ko.isObservable(address.Latitude) ? address.Latitude() : address.Latitude;
        var lng = ko.isObservable(address.Longitude) ? address.Longitude() : address.Longitude;
        if (!lat || !lng) {
            return;
        }
        var pos = { lat: parseFloat(lat), lng: parseFloat(lng) };
        KeysMap.intitialiseMap(pos);
        marker = new google.maps.Marker({ map: map, position: pos, icon: KeysMap.makeMarkerImage() });
    }
};

ko.bindingHandlers.clickShowSchoolsNearBy = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var address = ko.isObservable(value) ? value() : value;
        var lat = ko.isObservable(address.Latitude) ? address.Latitude() : address.Latitude;
        var lng = ko.isObservable(address.Longitude) ? address.Longitude() : address.Longitude;
        if (!lat || !lng) {
            return;
        }
        var pos = { lat: parseFloat(lat), lng: parseFloat(lng) };
        
        $(element).on("click", function () {
            KeysMap.intitialiseMap(pos);
            KeysMap.clearMarkers(KeysMap.googleMapMarkers);
            infowindow = new google.maps.InfoWindow();
            var service = new google.maps.places.PlacesService(map);
            service.nearbySearch({
                location: pos,
                radius: 2000,
                type: ['school']
            }, callback);

            function callback(results, status) {
                if (status === google.maps.places.PlacesServiceStatus.OK) {
                    for (var i = 0; i < results.length; i++) {
                        createMarker(results[i]);
                    }
                }
            }

            function createMarker(place) {
                var placeLoc = place.geometry.location;
                var marker = new google.maps.Marker({
                    map: map,
                    position: place.geometry.location
                });

                google.maps.event.addListener(marker, 'click', function () {
                    infowindow.setContent(place.name);
                    infowindow.open(map, this);
                });
            }
            //$.ajax({
            //        type: "GET",
            //        url: '/Rental/Rental/GetSchoolInfo',
            //        data: { DestLat: lat, DestLon: lng },
            //        dataType: 'json',
            //        success: function (data) {
            //            data.forEach(function (item) {
            //            item.lat = item.SchoolLat;
            //            item.lng = item.SchoolLon;
            //            item.content = '<div>' + item.SchoolName + '</div><br>' + '<div>' + item.DistanceBoundingInKM.toFixed(2) + ' km</div>'
            //            var marker = KeysMap.createInfoMarker(item, KeysMap.makeInfoWindow());
            //            KeysMap.googleMapMarkers.push(marker)
            //        });
            //    }
            //});

        });
    }
};

ko.bindingHandlers.clickShowMap = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var address = ko.isObservable(value) ? value() : value;
        var lat = ko.isObservable(address.Latitude) ? address.Latitude() : address.Latitude;
        var lng = ko.isObservable(address.Longitude) ? address.Longitude() : address.Longitude;
        if (!lat || !lng) {
            return;
        }
        var pos = { lat: parseFloat(lat), lng: parseFloat(lng) };
        $(element).on("click", function () {;
            KeysMap.clearMarkers(KeysMap.googleMapMarkers);
            KeysMap.intitialiseMap(pos)
        });
        
    }
}

ko.bindingHandlers.clickShowTransport = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var address = ko.isObservable(value) ? value() : value;
        var lat = ko.isObservable(address.Latitude) ? address.Latitude() : address.Latitude;
        var lng = ko.isObservable(address.Longitude) ? address.Longitude() : address.Longitude;
        if (!lat || !lng) {
            return;
        }
        var pos = { lat: parseFloat(lat), lng: parseFloat(lng) };
        $(element).on("click", function () {
            KeysMap.clearMarkers(KeysMap.googleMapMarkers);
            KeysMap.intitialiseMap(pos);
            $.ajax({
                type: "GET",
                url: '/Rental/Rental/GetTransportInfo',
                data: { DestLat: lat, DestLon: lng },
                dataType: 'json',
                success: function (data) {
                    if (!data) return;
                    data.forEach(function (item) {
                        item.lat = item.StopLat;
                        item.lng = item.StopLon;
                        item.content = '<div>' + item.StopName + '</div><br>' + '<div>' + item.DistanceBoundingInKM.toFixed(2) + ' km</div>'
                        var marker = KeysMap.createInfoMarker(item, KeysMap.makeInfoWindow());
                        KeysMap.googleMapMarkers.push(marker)
                    });
                }
            });
        });

    }
}
function Entity(dic) {
    var self = this;
    self.Id = ko.observable(0);
    for (var key in dic) {
        if (dic[key]  instanceof Function) {
            self[key] = ko.observable().extend(dic[key]());
        }
        else if (typeof dic[key] === 'string') {
            self[key] = ko.observable().extend(Extender[dic[key]]);
        }
        else {
            self[key] = ko.observable().extend(dic[key]);
        }
    }
    self.MediaFiles = ko.observableArray();
}
function EntityViewModel(dic) {
    var self = this;
    self.Model = new Entity(dic);
    self.RemoveFile = function (file) {
        if (file.Status() == 'load') {
            if (self.Model.FilesRemoved)  self.Model.FilesRemoved.push(file.Id());
        }
        self.Model.MediaFiles.remove(file);
    }
    self.Errors = ko.validation.group(self.Model, {deep : true, live : true});
    self.IsValid = ko.computed(function () {
        return self.Errors().length == 0;
    });
}
var KeysExtendsDic = {};
KeysExtendsDic.Request = {
    RequestMessage: 'description'
};
KeysExtendsDic.RequestReply = {
    Reason: 'description'
};
KeysExtendsDic.RentalApp = {
    TenantsCount: 'tenantNumber',
    Note : 'tenantNote'
}
KeysExtendsDic.RentalListing = {
    Title: 'title',
    OccupantCount: 'occupantCount',
    Description: 'descriptionRentalListing',
    MovingCost: 'movingCost',
    TargetRent: 'targetRent',
    AvailableDate: 'availableDate',
    Furnishing: 'furnishing',
    IdealTenant: 'idealTenant',
    PetsAllowed: 'petsAllowed'
};
KeysExtendsDic.Job = {
    JobDescription: 'description',
    Note : 'note'
};
KeysExtendsDic.NewJob = {
    JobDescription: 'description',
};
KeysExtendsDic.Inspection = {
    Message: 'message'
}

KeysExtendsDic.TenantInfo = {
    HomePhoneNumber: 'homePhoneNumber',
    MobilePhoneNumber: 'mobilePhoneNumber',
}
KeysExtendsDic.JobQuote = function (maxBudget) {
    var dic = {
        Amount: Extender.decimalNumeric(maxBudget, true),
        Note: 'description'
    }
    return dic;
}
KeysExtendsDic.EditQuote = {
    Note: 'description'
}
KeysExtendsDic.SupplierInfo = {
    Name: 'name',
    PhoneNumber: 'phoneNumber',
    Website: Extender.website()
}
KeysExtendsDic.Property = {
    Name: 'propertyName',
    Description: 'description',
    YearBuilt: 'yearBuilt',
    TargetRent: 'targetRent',
    LandSqm: 'landArea',
    FloorArea: 'floorArea',
    Bedroom: 'bedroom',
    Bathroom: 'bathroom',
    ParkingSpace: 'parkingSpace',
    Address: {
        Number: 'addressNumber',
        Street: 'addressStreet',
        City: 'addressCity',
        Suburb: 'addressSuburb',
        PostCode : 'postCode'
    }
}
KeysExtendsDic.MarketJob = {
    Title: 'title',
    MaxBudget: Extender.decimalNumeric,
    JobDescription: 'description'
}
KeysExtendsDic.Eval = {
    Reason: 'reason'
}
KeysExtendsDic.CompanyOverView = {
    Name: 'companyName',
}


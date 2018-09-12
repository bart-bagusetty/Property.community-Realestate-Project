function addDatePicker() {
    //$('.payment-start-date').datepicker({
    //    dateFormat: 'mm/dd/yy'
    //}).datepicker();
    //$('.payment-end-date').datepicker({
    //    dateFormat: 'mm/dd/yy'
    //}).datepicker();
    //$('.expense-date').datepicker({
    //    dateFormat: 'yy-MM-dd'
    //    //dateFormat: 'mm/dd/yy'
    //}).datepicker();

}



var actualTotalRepayment = 0;
var ExpenseModel = function (expenses) {
    var self = this;
    selfFromExpense = this;
     // self = this;
    self.expenses = ko.observableArray(expenses);
    //// to add new expense row
    self.addExpense = function () {
        console.log(actualTotalRepayment);
        console.log(newProperty.NewPropertyId);
        self.expenses.push({
            propertyId: newProperty.NewPropertyId,
            amount: ko.observable("").extend({ required: { params: false, message: "Please enter the Expense" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } }),
            description: ko.observable("").extend({ required: { params: false, } }),
            ExpenseDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter a date." } }),
          
        });
        addDatePicker();
    };
    // to remove a row
    self.removeExpense = function (expense) {
        self.expenses.remove(expense);
    };

    // saving the expenses of the property
    self.saveExpense = function (expenses) {
        debugger;
        var forSaving = ko.toJSON(self.expenses);
        console.log("expenses");
        console.log(forSaving);
        $.ajax({
            url: '/PropertyOwners/Onboarding/AddExpense',
            data: forSaving,
            method: "POST",
            contentType: 'application/json',
        }).done(function (data) {
            if (data.success) {
                console.log(data);
            }
        });
    };
};

var expenseModel = new ExpenseModel(
    [
        {
            propertyId: newProperty.NewPropertyId,
            amount: ko.observable("").extend({ required: { params: false, message: "Please enter the Expense" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } }),
            description: ko.observable("").extend({ required: { params: false, } }),
            ExpenseDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter a date." } }),
        }
    ]
     
);



var RepaymentModel = function (repayments) {
    var self = this;
    selfFromRpayments = this;
    self.repayments = ko.observableArray(repayments);

    self.addRepayment = function () {
        console.log("repay push");
        
        self.repayments.push({
            propertyId: newProperty.NewPropertyId,
            amount: ko.observable("").extend({ required: { params: false, message: "Please enter the Repayment Amount" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } }),
            FrequencyType: ko.observable("").extend({ required: { params: false, message: "Please enter the frequency" } }),
            startDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter start date." } }),
            //endDate: ko.observable("").extend({ date: true, min: this.startDate, message: "In valid date!" }),
            endDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter end date." } }),
        });
        addDatePicker();
    };

    self.removeRepayment = function (repayment) {
        self.repayments.remove(repayment);
        console.log("remove repayment");
    };

    // saving the repayments of the property
    self.saveRepayments = function (repayments) {
        debugger;
        var forSaving = ko.toJSON(self.repayments);
        console.log(forSaving);
        $.ajax({
            url: '/PropertyOwners/Onboarding/AddRepayment',
            data: forSaving,
            method: "POST",
            contentType: 'application/json',
        }).done(function (data) {
            console.log(data);
            if (data.Success) {
                actualTotalRepayment = data.ActualTotalRepayment;
                console.log(data);
            }
        });
    };

};


var repaymentModel = new RepaymentModel(
    [
        {
            propertyId: newProperty.NewPropertyId,
            amount: ko.observable("").extend({ required: { params: false, message: "Please enter the Repayment Amount" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } }),
            FrequencyType: ko.observable("").extend({ required: { params: false, message: "Please enter the frequency" } }),
            startDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter start date." } }),
            //endDate: ko.observable("").extend({ date: true, min: this.startDate, message: "In valid date!" }),
            endDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter end date." } }),

        }
    ]
);


var rentalPaymentModel = function (payment) {
    var self = this;
    selfFromRental = this;
    // self = this;
    self.payment = ko.observableArray(payment);
    //// to add new payment row
    self.AddRental = function () {
   //     console.log(actualTotalRepayment);
  //      console.log(newProperty.NewPropertyId);
        self.payment.push({
            propertyId: newProperty.NewPropertyId,
            amount: ko.observable("").extend({ required: { params: false, message: "Please enter the Amount" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } }),
            FrequencyType: ko.observable("").extend({ required: { params: false, } }),
            ExpenseDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter a date." } }),

        });
        addDatePicker();
    };
    // to remove a row
    self.removeRental = function (payment) {
        self.amount.remove(payment);
    };

    // saving the expenses of the property
    self.saveRental = function (payment) {
        debugger;
        var forSaving = ko.toJSON(self.payment);
        console.log("rent");
        console.log(forSaving);
        $.ajax({
            url: '/PropertyOwners/Onboarding/AddRentalPayment',
            data: forSaving,
            method: "POST",
            contentType: 'application/json',
        }).done(function (data) {
            if (data.success) {
                console.log(data);
            }
        });
    };
};

var rentalPaymentModel = new rentalPaymentModel(
    [
        {
            propertyId: newProperty.NewPropertyId,
            amount: ko.observable("").extend({ required: { params: false, message: "Please enter the Amount" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } }),
            FrequencyType: ko.observable("").extend({ required: { params: false, message: "Please enter the frequency" } }),
            ExpenseDate: ko.observable("").extend({ date: true, required: { params: true, message: "Please enter a date." } }),
        }
    ]
);

var FinanceModel = function () {
    var self = this;
    self.propertyid = ko.observable(prop.NewPropertyId);
    self.TotalRepayment = ko.observable(actualTotalRepayment);
    self.PurchasePrice = ko.observable().extend({ required: { params: false, message: "Please enter the Purchase Price" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter number only", } });
    self.Mortgage = ko.observable().extend({ required: { params: false, message: "Please enter the Mortgage value" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } });
    self.CurrentHomeValue = ko.observable().extend({ required: { params: false, message: "Please enter the Home value" }, pattern: { params: "^[0-9]+(.[0-9]{1,2})?$", message: "Please enter a valid Amount", } });
    self.HomeValueType = ko.observable("");

    $('#addFinance').click(function () {
        var Financials = {};
        Financials.propertyid = newProperty.NewPropertyId;
        Financials.PurchasePrice = self.PurchasePrice;
        Financials.Mortgage = self.Mortgage;
        Financials.CurrentHomeValue = self.CurrentHomeValue;
        Financials.HomeValueType = self.HomeValueType;
        Financials.TotalRepayment = actualTotalRepayment;
        var forSaving = ko.toJSON(Financials);
        selfFromExpense.saveExpense();
        selfFromRpayments.saveRepayments();
        console.log(forSaving);
        $.ajax({
            url: '/PropertyOwners/Onboarding/AddFinancial',
            data: ko.toJSON(Financials),
            method: "POST",
            contentType: 'application/json',
        }).done(function (data) {
            if (data.success) {
                console.log(data);
            }
            });
        debugger;
    });

};


// trying to use the same save function to do all the saving 
function save(url, data) {
    $.ajax({
        url: url,
        data: ko.toJSON(data),
        method: "POST",
        contentType: 'application/json',
    }).done(function (data) {
        if (data.success) {
        }
    });
}


ko.bindingHandlers.stopBinding = {
    init: function () {
        return { controlsDescendantBindings: true };
    }
};


ko.applyBindings(new FinanceModel(), document.getElementById('propertyFinancialSection'));
ko.applyBindings(expenseModel, document.getElementById('expenseSection'));
ko.applyBindings(repaymentModel, document.getElementById('repaymentSection'));
ko.applyBindings(rentalPaymentModel, document.getElementById('rentalSection'));




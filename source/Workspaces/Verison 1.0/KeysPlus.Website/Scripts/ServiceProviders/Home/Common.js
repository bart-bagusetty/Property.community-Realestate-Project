//Namespace
var ServiceProviders = ServiceProviders || {};

ServiceProviders.createJob = function (job, jobList) {
    $.post("ServiceProviders/Home/Save", job, function () {
        ServiceProviders.fetchDataOfCurrentPage(jobList);
    });
}

ServiceProviders.editJob = function (job, jobList) {
    // After editing, fetch the data of current page from server again
    $.post("ServiceProviders/Home/Save", job, function () {
        ServiceProviders.fetchDataOfCurrentPage(jobList);
    });
}

ServiceProviders.fetchDataOfCurrentPage = function (jobList) {
    var aTag = $(".pagination a[href]");
    var url = aTag.prop("href");
    var page = url != null ? url.substring(url.indexOf("=") + 1) : null;
    if (page != null) {
        if (aTag.prop("rel") == "next") {
            page = (parseInt(page) - 1);
        } else {
            page = (parseInt(page) + 1);
        }
        url = url.substring(0, url.indexOf("=") + 1) + page;
    } else {
        url = "ServiceProviders/Home/Index";
    }
    $.get(url, null, function (response) {
        ServiceProviders.reloadData(response, jobList);
    });

};

ServiceProviders.reloadData = function (htmlPage, jobList) {
    var jobListData = $("#jobListData").val();
    $("#pagination").html(htmlPage);
    ko.mapping.fromJSON(jobListData, jobList);
    //ServiceProviders.removeServerData();
    ServiceProviders.formatDate(jobList());
};

ServiceProviders.loadProperty = function () {
    var propertyData;
    $.ajax({
        url: "ServiceProviders/Home/GetPropertyByUserId",
        data: { userId: 1 },
        async: false,
        method: "POST",
        success: function (data) {
            propertyData = data;
        }
    });
    return propertyData;
};

ServiceProviders.formatDate = function (jobList) {
    jobList.forEach(function (job) {
        job.JobStartDate(moment(job.JobStartDate()).format().split('T')[0]);
        if (job.JobEndDate() != null) {
            job.JobEndDate(moment(job.JobEndDate()).format().split('T')[0]);
        }
    });
};

ServiceProviders.assignAction = function (action) {
    return {
        managerManageJob: action == 1,
        providerManageJob: action == 2,
        providerApplyJob: action == 3
    };
};

ServiceProviders.removeServerData = function () {
    $("#serverDataDiv").remove();
};
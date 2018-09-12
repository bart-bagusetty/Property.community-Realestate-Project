
function DrawDashBoard(data) {
    var propData = chartData.makeData(data.PropDashboardData);
    var propOptions = chartOptions.makeOptions(data.PropDashboardData);
    var rentData = chartData.makeData(data.RentalDashboardData);
    var rentOptions = chartOptions.makeOptions(data.RentalDashboardData);
    var maintenanceData = chartData.makeData(data.MaintenanceDashboardData);
    var maintenanceOptions = chartOptions.makeOptions(data.MaintenanceDashboardData);
    var tenantData = chartData.makeData(data.TenantDashboardData);
    var tenantOptions = chartOptions.makeOptions(data.TenantDashboardData);
    var jobsData = chartData.makeData(data.JobsDashboardData);
    var jobsOptions = chartOptions.makeOptions(data.JobsDashboardData);

    var propChart = KeysChart.drawDoughnut('property-chart', propData, propOptions);
    document.getElementById('prop-chart-legends').innerHTML = propChart.generateLegend();

    var rentChart = KeysChart.drawDoughnut('rental-chart', rentData, rentOptions);
    document.getElementById('rent-chart-legends').innerHTML = rentChart.generateLegend();

    var maintenanceChart = KeysChart.drawDoughnut('maintenance-chart', maintenanceData, maintenanceOptions);
    document.getElementById('maintenance-chart-legends').innerHTML = maintenanceChart.generateLegend();

    var tenantChart = KeysChart.drawDoughnut('tenant-chart', tenantData, tenantOptions);
    document.getElementById('tenant-chart-legends').innerHTML = tenantChart.generateLegend();

    var jobChart = KeysChart.drawDoughnut('jobs-chart', jobsData, jobsOptions);
    document.getElementById('jobs-chart-legends').innerHTML = jobChart.generateLegend();


}


$(function () {
    //Widgets count
    $('.count-to').countTo();

    //Sales count to
    $('.sales-count-to').countTo({
        formatter: function (value, options) {
            return '$' + value.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, ' ').replace('.', ',');
        }
    });
    initSlaMonthlyChart();
});
var realtime = 'on';

var data = [], totalPoints = 110;
function getRandomData() {
    if (data.length > 0) data = data.slice(1);
    while (data.length < totalPoints) {
        var prev = data.length > 0 ? data[data.length - 1] : 50, y = prev + Math.random() * 10 - 5;
        if (y < 0) { y = 0; } else if (y > 100) { y = 100; }

        data.push(y);
    }
    var res = [];
    for (var i = 0; i < data.length; ++i) {
        res.push([i, data[i]]);
    }
    return res;
}

function SlaMonthlyChart(mems) {
    let aData = mems;
    let aLabels = aData.result[0];
    let aDatasetnegatif = aData.result[2];
    let aDatasetpozitif = aData.result[1];
    dataT = {
        labels: aLabels,
        datasets: [
            {
                label: "Sla sağlayanlar (%)",
                data: aDatasetpozitif,
                stack: 'Stack 0',
                fill: false,
                backgroundColor: "rgba(54, 162, 235, 0.2)",
                borderColor: "rgb(54, 162, 235)",
                borderWidth: 1
            },
            {
                label: "Sla geçenler (%)",
                data: aDatasetnegatif,
                stack: 'Stack 0',
                fill: false,
                backgroundColor: "rgba(255, 99, 132, 0.2)",

                borderColor: "rgb(255, 99, 132)",
                borderWidth: 1
            }
        ]
    };
    let ctx = $("#bar_chart").get(0).getContext("2d");
    myNewChart = new Chart(ctx, {
        type: 'bar',
        data: dataT,
        options: {
            responsive: true,
            title: { display: true, text: 'Aylık Sla Yüzdesi' },
            legend: { position: 'bottom' },
            scales: {
                xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' } }],
                yAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { stepSize: 50, beginAtZero: true } }]
            }
        }
    });
}

function initSlaMonthlyChart() {
    $.ajax({
        type: "POST",
        url: "/Home/SlaMonthlyChart?projectsName=" + 0,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (mems) {
            let aData = mems;
            let aLabels = aData.result[0];
            let aDatasetnegatif = aData.result[1];
            let aDatasetpozitif = aData.result[2];
            let dataT = {
                labels: aLabels,
                datasets: [
                    {
                        label: "Sla geçenler (%)",
                        data: aDatasetnegatif,
                        stack: 'Stack 0',
                        fill: false,
                        backgroundColor: "rgba(255, 99, 132, 0.2)",//kırmızı
                        borderColor: "rgb(255, 99, 132)",
                        borderWidth: 1
                    }, {
                        label: "Sla sağlayanlar (%)",
                        data: aDatasetpozitif,
                        stack: 'Stack 0',
                        fill: false,
                        backgroundColor: "rgba(54, 162, 235, 0.2)",//mavi
                        borderColor: "rgb(54, 162, 235)",
                        borderWidth: 1
                    }
                ]
            };
            let ctx = $("#bar_chart").get(0).getContext("2d");
            myNewChart = new Chart(ctx, {
                type: 'bar',
                data: dataT,
                options: {
                    responsive: true,
                    title: { display: true, text: 'Aylık Sla Yüzdesi' },
                    legend: { position: 'bottom' },
                    scales: {
                        xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' } }],
                        yAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { stepSize: 50, beginAtZero: true } }]
                    }
                }
            });
        }
    });
}

function getProjects() {
    var projectsName = [];
    var project = $('#app');
    var innerHTML = project[0].childNodes[0].childNodes[1].innerHTML;
    var indexStart = innerHTML.indexOf("vue-treeselect__multi-value-label");
    while (parseInt(indexStart) !== -1) {
        innerHTML = innerHTML.substring(indexStart + 35);
        indexEnd = innerHTML.indexOf("</span>");
        projectsName.push(innerHTML.substring(0, indexEnd));
        indexStart = innerHTML.indexOf("vue-treeselect__multi-value-label");
    }
    console.log(projectsName);
    var projects = "";
    for (var i = 0; i < projectsName.length; i++) {
        projects += projectsName[i] + ",";
    }
    if (projects.length > 0) {
        projects = projects.substring(0, projects.length - 1);
    }
    return projects;
}

$(document).ready(function () {
    $(function () {// Loading projects into combobox
        AjaxCall('/Home/GetProjectsTreeList', null).done(function (response) {
            Vue.component('treeselect', VueTreeselect.Treeselect);
            new Vue({
                //name: 'app',   // sonradan eklendi
                el: '#app',
                data: response.result
            });
        }).fail(function (error) {
            alert(error.StatusText + ' Projeler yüklenemedi');
        });

        $('#showValue').click(function () {// update chart when project selection

            myNewChart.destroy();
            $.ajax({
                url: "/Home/SlaMonthlyChart?projectsName=" + getProjects(),
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    SlaMonthlyChart(data)
                }
            }).done(function (response) {

                myNewChart.update();

            }).fail(function (error) {
                alert(error.StatusText);
            });
        });
    });

    function AjaxCall(url, data, type) {
        return $.ajax({
            url: url,
            type: type ? type : 'GET',
            data: data,
            contentType: 'application/json'
        });
    }

    document.getElementById("bar_chart").onclick = function (evt) {
        let activePoints = myNewChart.getElementsAtEvent(evt);
        let negetifSla = activePoints[0];
        let secilenAy = myNewChart.data.labels[negetifSla._index];
        let year = secilenAy.split("-")[0];
        let month = secilenAy.split("-")[1];

        var table = $('#slaMonthlyDetailTable').DataTable();

        $('#slaMonthlyDetailTable').dataTable().fnClearTable();
        $.ajax({
            url: "/Home/SlaMonthlyChartDetailTable?projects=" + getProjects() + " &month=" + month + " &year=" + year,
            dataType: 'json',
            type: 'post',
            success: function (data) {
                $.each(data.result.data, function (a, b) {
                    table.row.add([
                        b.redmine_link,
                        '---------------',
                        b.created_on_str,
                        b.closed_on_str,
                        b.success_rate
                    ]).draw(false);
                });
            }
        }).fail(function (error) {
            alert(error.StatusText);
        });
    };
});

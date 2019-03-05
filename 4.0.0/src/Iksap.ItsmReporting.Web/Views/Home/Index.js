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
    get_label_name();
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
    let aLabelsDown = aData.result[3];
    let aLabelTop = aData.result[3][2];
    dataT = {
        labels: aLabels,
        datasets: [
            {
                label: aLabelsDown[0],
                data: aDatasetpozitif,
                stack: 'Stack 0',
                fill: false,
                backgroundColor: "rgba(54, 162, 235, 0.2)",
                borderColor: "rgb(54, 162, 235)",
                borderWidth: 1
            },
            {
                label: aLabelsDown[1],
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
            title: { display: true, text: aLabelTop },
            legend: { position: 'bottom' },
            scales: {
                xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' } }],
                yAxes: [{ gridLines: { display: true }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { stepSize: 10, beginAtZero: true } }]
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
            let aLabelsDown = aData.result[3];
            let aLabelTop = aData.result[3][2];
            let dataT = {
                labels: aLabels,
                datasets: [
                    {
                        //label: "Sla sağlayanlar (%)",
                        label: aLabelsDown[0],
                        data: aDatasetpozitif,
                        stack: 'Stack 0',
                        fill: false,
                        backgroundColor: "rgba(54, 162, 235, 0.2)",
                        borderColor: "rgb(54, 162, 235)",
                        borderWidth: 1
                    },
                    {
                        //label: "Sla geçenler (%)",
                        label: aLabelsDown[1],
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
                    title: { display: true, text: aLabelTop },
                    legend: { position: 'bottom' },
                    scales: {
                        xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' } }],
                        yAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { stepSize: 100, beginAtZero: true } }]
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
    var projects = "";
    for (var i = 0; i < projectsName.length; i++) {
        projects += projectsName[i] + ",";
    }
    if (projects.length > 0) {
        projects = projects.substring(0, projects.length - 1);
    }
    return projects;
}

function setLanguage(language_url) {
    $('#slaMonthlyDetailTable').DataTable({
        "language": {
            "url": language_url
        }
    });
}

function result_click(id) {
    // burada veri çekilecek
    $.ajax({
        url: "/Home/TicketDetail?id=" + id,
        dataType: 'json',
        type: 'post',
        success: function (data) {
            body_message = "";
            header_message = "<button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>";
            header_message += "<h2><u>TicNo: " + data.result.id + "</u></h2>";
            //header_message = "<h2><u>TicNo: " + data.result.id + "</u> <u>SLA: <strong>%" + data.result.success_rate + "</strong></u> <u>süresi: <strong>" + data.result.rate.time_limit + "</strong></u></h2>";
            body_message = "<h5>" + data.result.project_name + " : " + data.result.subject + "</h5>";
            //body_message += "<h5>Ticket'ta kişiye özel toplam geçen süre</h5>";
            //body_message += "<tr style=\"background-color: black; color: white;\"><th>İsim Soyisim</th><th>SLA Süresi</th><th>Ticketın Son Durumu</th><th>Başlangıç Zamanı</th><th>Bitiş Zamanı</th></tr>";
            body_message += "<table>";
            body_message += "<br/><h3>Kişiye özel süre dağılımı</h3><br/>";
            if (data.result.success_rate >= 100) {
                body_message += "<tr style=\"background-color: #fa6a86; color: white;\"><th>İsim Soyisim</th><th>Toplam SLA Süresi</th></tr>";
            }
            else {
                body_message += "<tr style=\"background-color: #56aeed; color: white;\"><th>İsim Soyisim</th><th>Toplam SLA Süresi</th></tr>";
            }
            for (i = 0; i < data.result.singleUsers.length; i++) {
                if (data.result.singleUsers[i].iksapUser === 0) {
                    body_message += "<tr style=\"background-color: #eee; text-align: right;\">";
                }
                else {
                    body_message += "<tr>";
                }
                body_message += "<td>" + data.result.singleUsers[i].firstname + " " + data.result.singleUsers[i].lastname + "</td>";
                body_message += "<td>" + data.result.singleUsers[i].sla_time_hour + "s " + data.result.singleUsers[i].sla_time_minute + "dk " + data.result.singleUsers[i].sla_time_second + "sn" + "</td>";
                body_message += "</tr>";
            }
            body_message += "</table>";
            body_message += "<br/><h3>Ticket ayrıntılı süre dağılımı</h3><br/>";
            body_message += "<table>";
            if (data.result.success_rate >= 100) {
                body_message += "<tr style=\"background-color: #fa6a86; color: white;\"><th>İsim Soyisim</th><th>SLA Süresi</th><th>Ticketın Son Durumu</th><th>Başlangıç Zamanı</th><th>Bitiş Zamanı</th></tr>";
            }
            else {
                body_message += "<tr style=\"background-color: #56aeed; color: white;\"><th>İsim Soyisim</th><th>SLA Süresi</th><th>Ticketın Son Durumu</th><th>Başlangıç Zamanı</th><th>Bitiş Zamanı</th></tr>";
            }
            for (i = 0; i < data.result.users.length; i++) {
                if (data.result.users[i].iksapUser === 0) {
                    body_message += "<tr style=\"background-color: #eee; text-align: right;\">";
                }
                else {
                    body_message += "<tr>";
                }
                body_message += "<td>" + data.result.users[i].firstname + " " + data.result.users[i].lastname + "</td>";
                body_message += "<td>" + data.result.users[i].sla_time_hour + "s " + data.result.users[i].sla_time_minute + "dk " + data.result.users[i].sla_time_second + "sn" + "</td>";
                body_message += "<td>" + data.result.users[i].value_name + "</td>";
                body_message += "<td>" + data.result.users[i].start_time_str + "</td>";
                body_message += "<td>" + data.result.users[i].end_time_str + "</td>";
                body_message += "</tr>";
            }
            modal_header = document.getElementById("modal_header");
            modal_header.innerHTML = header_message;
            modal_body = document.getElementById("modal_body");
            modal_body.innerHTML = body_message;
        }
    }).fail(function (error) {
        alert(error.StatusText);
    });
}

$(document).ready(function () {
    let language_url;
    AjaxCall('/Home/getCurrentLanguage', null).done(function (response) {
        language_url = response;
        setLanguage(language_url);
    }).fail(function (error) {
        setLanguage("//cdn.datatables.net/plug-ins/1.10.19/i18n/English.json");
    });


    $(function () {// Loading projects into combobox
        AjaxCall('/Home/GetProjectsTreeList', null).done(function (response) {
            Vue.component('treeselect', VueTreeselect.Treeselect);
            new Vue({
                el: '#app',
                data: response.result,
                //methods: {
                //    greet: function () {
                //        //alert('Hello ' + this.name + '!');
                //    }
                //}
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
                    change_label_content(data.result[4], data.result[5]);
                    SlaMonthlyChart(data);
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
                for (i = 0; i < data.result.data.length; i++) {
                    table.row.add([
                        data.result.data[i].redmine_link,
                        data.result.data[i].created_on_str,
                        data.result.data[i].closed_on_str,
                        "%" + data.result.data[i].success_rate,
                        "<button id=\"" + data.result.data[i].id + "\" onclick=\"result_click(this.id)\" type=\"button\" class=\"btn btn-primary\" data-toggle=\"modal\" data-target=\"#myModal\">Ticket Detayları</button>"
                    ]).draw(false);
                }
            }
        }).done(function (response) {
            location.href = "#sladetail";
        }).fail(function (error) {
            alert(error.StatusText);
        });
       
    };
});

function change_label_content(lbl1, lbl2) {
    var lbl1_content = document.getElementById("lbl1_content");
    lbl1_content.innerHTML = lbl1;
    var lbl2_content = document.getElementById("lbl2_content");
    lbl2_content.innerHTML = lbl2;
}

// form yüklenince üstteki label'ların ismini düzeltmek için kullanılıyor.
function get_label_name() {
    $.ajax({
        url: "/Home/GetLabelNames",
        dataType: 'json',
        type: 'post',
        success: function (data) {
            var lbl1_name = document.getElementById("lbl1_name");
            lbl1_name.innerHTML = data.result[0];
            var lbl2_name = document.getElementById("lbl2_name");
            lbl2_name.innerHTML = data.result[1];
        }
    }).fail(function (error) {
        alert(error.StatusText);
    });
}
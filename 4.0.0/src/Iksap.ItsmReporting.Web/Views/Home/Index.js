﻿$(function () {
    //Widgets count
    $('.count-to').countTo();

    //Sales count to
    $('.sales-count-to').countTo({
        formatter: function (value, options) {
            return '$' + value.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, ' ').replace('.', ',');
        }
    });

    //initRealTimeChart();
    initDonutChart();
   // initSparkline();
    initSlaMonthlyChart();


});

var realtime = 'on';
function initRealTimeChart() {
    //Real time ==========================================================================================
    var plot = $.plot('#real_time_chart', [getRandomData()], {
        series: {
            shadowSize: 0,
            color: 'rgb(0, 188, 212)'
        },
        grid: {
            borderColor: '#f3f3f3',
            borderWidth: 1,
            tickColor: '#f3f3f3'
        },
        lines: {
            fill: true
        },
        yaxis: {
            min: 0,
            max: 100
        },
        xaxis: {
            min: 0,
            max: 100
        }
    });

    function updateRealTime() {
        plot.setData([getRandomData()]);
        plot.draw();

        var timeout;
        if (realtime === 'on') {
            timeout = setTimeout(updateRealTime, 320);
        } else {
            clearTimeout(timeout);
        }
    }

    updateRealTime();

    $('#realtime').on('change', function () {
        realtime = this.checked ? 'on' : 'off';
        updateRealTime();
    });
    //====================================================================================================
}

function initSparkline() {
    $(".sparkline").each(function () {
        var $this = $(this);
        $this.sparkline('html', $this.data());
    });
}

function initDonutChart() {
    Morris.Donut({
        element: 'donut_chart',
        data: [{
                label: 'Chrome',
                value: 37
            }, {
                label: 'Firefox',
                value: 30
            }, {
                label: 'Safari',
                value: 18
            }, {
                label: 'Opera',
                value: 12
            },
            {
                label: 'Other',
                value: 3
            }],
        colors: ['rgb(233, 30, 99)', 'rgb(0, 188, 212)', 'rgb(255, 152, 0)', 'rgb(0, 150, 136)', 'rgb(96, 125, 139)'],
        formatter: function (y) {
            return y + '%'
        }
    });
}

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

function initSlaMonthlyChart() {
    $.ajax({
        type: "POST",
        url: "Home/SlaMonthlyChart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (mems) {
            console.log(mems);
            var aData = mems;
            var aLabels = aData.result[0];
            var aDatasetnegatif = aData.result[1];
            var aDatasetpozitif = aData.result[2];
            var dataT = {
                //labels: aLabels,
                labels: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran","Temmuz","Ağustoz","Eylul","Ekim","Kasım","Aralık"],
                datasets: [{
                    label: "Negatif",
                    data: aDatasetnegatif,
                    stack: 'Stack 0',
                    fill: false,
                    backgroundColor: "rgba(54, 162, 235, 0.2)",
                    borderColor: "rgb(54, 162, 235)",
                    borderWidth: 1
                },
                {
                    label: "Pozitif",
                    data: aDatasetpozitif,
                    stack: 'Stack 0',
                    fill: false,
                    backgroundColor: "rgba(255, 99, 132, 0.2)",
                    borderColor: "rgb(255, 99, 132)",
                    borderWidth: 1
                }

                ]
            };
            var ctx = $("#bar_chart").get(0).getContext("2d");
            var myNewChart = new Chart(ctx, {
                type: 'bar',
                data: dataT,
                options: {
                    responsive: true,
                    title: { display: true, text: 'Aylık' },
                    legend: { position: 'bottom' },
                    scales: {
                        xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' } }],
                        yAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { stepSize: 50, beginAtZero: true } }]
                    },
                }
            });

        }
    });

}

function initSlaDetay3() {

    $('.dataTables_filter input').attr('placeholder', 'Search...').hide();

    
    var table = $("#myTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "language": {
            "processing": "Yükleniyor... Lütfen Bekleyin",
            "url": "//cdn.datatables.net/plug-ins/1.10.12/i18n/Turkish.json"
        },
        "ajax": {
            "url": "/Home/getPeople",
            "type": "GET",
            "dataType": "json",
            "data": data.result,
            "success": function (veri) {
                console.log(veri.result);
                console.log(veri);
                //data = veri.result[0];
            }
        },
        "columns": [
            { "data": "firstname", "name": "firstname", "autoWidth": true },
 
            { "data": "lastname", "name": "lastname", "autoWidth": true },
            { "data": "login", "name": "login", "autoWidth": true },
         
        ]
       
    });
  

    $('.search-input').on('keyup change', function () {
        var index = $(this).attr('data-column'),
            val = $(this).val();
        table.columns(index).search(val.trim()).draw();
    });

}


function initSlaDetay22() {

    $('.dataTables_filter input').attr('placeholder', 'Search...').hide();


    var table = $("#peopleTable").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "ajax": {
            "url": "/Home/getPeople",
            "type": "POST",
            "dataType": "json",
            "success": function (veri) {
                console.log("veri",veri);
                var tdata = veri.result;
                data: tdata;

            }
        },
        "columns": [
            { "data": "firstname", "name": "firstname", "autoWidth": true },
            { "data": "lastname", "name": "lastname", "autoWidth": true },
            { "data": "login", "name": "login", "autoWidth": true },
        ]

    });


    $('.search-input').on('keyup change', function () {
        var index = $(this).attr('data-column'),
            val = $(this).val();
        table.columns(index).search(val.trim()).draw();
    });

}

function initSlaDetay2() {

    $('.dataTables_filter input').attr('placeholder', 'Search...').hide();
        $.ajax({
        url: "/Home/getPeople",
            type: 'POST',
            dataType: 'json',
            success: function (data) {
            console.log("yeni", data.result);
             var exampleTable  = $('#peopleTable')
                .DataTable({
                    data: data.result,
                    //'aaSorting': [[1, 'asc']],
                    processing: true, // for show progress bar
                    //serverSide: true,
                    filter: true, // this is for disable filter (search box)
                    orderMulti: false, // for disable multiple column at once

                    //dom: "<'row'<'col-md-6 text-left'T><'col-md-6 text-right'f>>" +
                    //    "<'row'<'col-md-12't>>" +
                    //    "<'row'<'col-md-6'i><'col-md-6'p>>",
                    //'columnDefs': [
                    //    { 'width': '25px', 'targets': [0] },
                    //    { 'sortable': false, 'targets': [0] }
                    //],
                    'columns': [
                        { "data": "firstname", "name": "firstname", "autoWidth": true },
                        { "data": "lastname", "name": "lastname", "autoWidth": true },
                        { "data": "login", "name": "login", "autoWidth": true },
                       
                    ]
                });
        }
    });
    $('.search-input').on('keyup change', function () {
        var index = $(this).attr('data-column'),
            val = $(this).val();
        exampleTable.columns(index).search(val.trim()).draw();
    });

}
function initSlaDetay() {
            $.ajax({
                url: 'https://jsonplaceholder.typicode.com/users',
                method: 'get',
                dataType: 'json',
                success: function (data) {
                    console.log(data);
                    var exampleTable = $('#example')
                        .DataTable({
                            data: data,
                            'aaSorting': [[1, 'asc']],
                            dom: "<'row'<'col-md-6 text-left'T><'col-md-6 text-right'f>>" +
                                "<'row'<'col-md-12't>>" +
                                "<'row'<'col-md-6'i><'col-md-6'p>>",
                            'columnDefs': [
                                { 'width': '25px', 'targets': [0] },
                                { 'sortable': false, 'targets': [0] }
                            ],
                            'columns': [
                                {
                                    'data': 'id',
                                    'render': function (data, type, full, meta) {
                                        return '<button class="btn btn-primary btn-xs" id="btnOne"><i class="fa fa-edit"></i></button>';
                                    }
                                },
                                { 'data': 'name' },
                                { 'data': 'username' },
                                {
                                    //'data': 'email',
                                    'render': function (data, type, full, meta) {
                                        return '<a href="mailto:' + full.email + '?">E-Mail</a>';
                                    }
                                },
                                { 'data': 'phone' },
                                {
                                    //'data': 'email',
                                    'render': function (data, type, full, meta) {
                                        return '<a href="http://' + full.website + '"target=_blank">Website</a>';
                                    }
                                },
                            ]
                        });
                }
            });

}
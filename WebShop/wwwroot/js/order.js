var dataTable;
var connectionOrder = new signalR.HubConnectionBuilder().withUrl("/hubs/order").build();


$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
        loadDataTableAdmin("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
            loadDataTableAdmin("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
                loadDataTableAdnin("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                    loadDataTableAdmin("approved");
                }
                else {
                    if (url.includes("cancelled")) {
                        loadDataTable("cancelled");
                        loadDataTableAdmin("cancelled");
                    }
                    else {
                        loadDataTable("all");
                        loadDataTableAdmin("all");
                    }
                }
                
            }
        }
    }

});

function loadDataTable(status) {
    dataTable = $('#tblDataUser').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'id', "width": "3%" },
            { data: 'carrier', "width": "20%" },
            { data: 'trackingNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderDate', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "7%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-1"> <i class="bi bi-pencil-square"></i></a>                    
                       
                    </div>`
                },
                "width": "10%"
            }
        ]
    });
}


function loadDataTableAdmin(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'id', "width": "3%" },
            { data: 'trackingNumber', "width": "20%" },
            { data: 'carrier', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderDate', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "7%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-1"> <i class="bi bi-pencil-square"></i></a>                    
                     <a onClick=Delete('/admin/order/delete?orderId=${data}')  class="btn btn-danger mx-1">  <i class="bi bi-trash"></i></a>    
                    </div>`
                }, 
                "width": "10%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "YOU WON'T BE ABLE TO REVERT THIS!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}
connectionOrder.on("newOrder", () => {
    dataTable.ajax.reload();
    toastr.success("New order recived");
})
function fulfilled() {

}
function rejected() {

}



connectionOrder.start().then(fulfilled, rejected);
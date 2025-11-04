$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    $('#companiesTable').DataTable({
        "ajax": {
            "url": "/Admin/Companies/GetAll"
        },
        "columns": [
            { "data": "name" },
            { "data": "tinNumber" },
            { "data": "streetAdress" },
            { "data": "city" },
            { "data": "region" },
            { "data": "postalCode" },
            { "data": "country" },
            { "data": "phone" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                                <a href="/Admin/Companies/Upsert/${data}" class="btn btn-primary btn-sm me-2">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                                <a href="javascript:void(0)" onClick="deleteCompany('/Admin/Companies/Delete/${data}')" class="btn btn-danger btn-sm">
                                    <i class="bi bi-trash"></i> Delete
                                </a>
                            `;
                }
            }
        ]
    });
};

function deleteCompany(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // read antiforgery token rendered on the page
            var token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: url,
                type: "POST", // controller expects POST (ActionName("Delete") + [HttpPost])
                headers: {
                    "RequestVerificationToken": token
                },
                success: function () {
                    Swal.fire({
                        title: "Deleted!",
                        text: "Company has been deleted.",
                        icon: "success"
                    }).then(function () {
                        // reload datatable after user closes the success dialog
                        $('#companiesTable').DataTable().ajax.reload();
                    });
                },
                error: function (xhr) {
                    Swal.fire({
                        title: "Error",
                        text: "Delete failed. " + (xhr.responseText || ""),
                        icon: "error"
                    });
                }
            });
        }
    });
}
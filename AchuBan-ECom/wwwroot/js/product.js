$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    $('#productsTable').DataTable({
        "ajax": {
            "url": "/Admin/Products/GetAll"
        },
        "columns": [
            {
                "data": "imageUrl",
                "render": function (data) {
                    // hide image cell when there's no image
                    if (!data) return "";
                    return `<img src="${data}" style="max-height:100px;" />`;
                }
            },
            { "data": "name" },
            { "data": "isbn" },
            { "data": "author" },
            { "data": "listPrice" },
            { "data": "price50" },
            { "data": "price100" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                                <a href="/Admin/Products/Upsert/${data}" class="btn btn-primary btn-sm me-2">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a>
                                <a href="javascript:void(0)" onClick="deleteProduct('/Admin/Products/Delete/${data}')" class="btn btn-danger btn-sm">
                                    <i class="bi bi-trash"></i> Delete
                                </a>
                            `;
                }
            }
        ]
    });
};

function deleteProduct(url) {
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
                        text: "Product has been deleted.",
                        icon: "success"
                    }).then(function () {
                        // reload datatable after user closes the success dialog
                        $('#productsTable').DataTable().ajax.reload();
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
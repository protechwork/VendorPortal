

//alert();
function deleteCompany(ID) {
    // Send the AJAX request
    $.ajax({
        type: "POST",
        url: "HOME/delete_company.php",
        data: {
            id: ID
        },
        dataType: "json",
        success: function (response) {
            // Handle the response from the server
            if (response.status === "success") {
                // Display success message
                alert("Database updated successfully.");
                location.reload(true);
            } else if (response.status === "exist") {
                // Display success message
                alert("Record Already Found in Contact.");
            } else {
                // Display error message
                alert("Failed to update Database.");
            }
        },
        error: function () {
            // Display error message
            alert("An error occurred while updating Database.");
        }
    });
}
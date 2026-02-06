function showAddMedicineModal() {
    $('#modalTitle').html('Add New Medicine');
    $('#btnSaveUpdate').text('Save Medicine');
    $('#currentMedicineId').val('');
    $('#addMedicineModal').show();
    resetForm();
}
function editMedicine(medicineId) {
    $('#modalTitle').html('Edit Medicine');
    $('#btnSaveUpdate').text('Update Medicine');
    $('#modalMode').val('edit');
    $('#currentMedicineId').val(medicineId);
    fetchMedicineData(medicineId);
    $('#addMedicineModal').show();
}
function validateForm() {
    let isValid = true;
    clearErrors();
    let medName = $('#txtMedName').val().trim();
    if (medName === '') { $('#nameError').text('Medicine name is required').show(); isValid = false; }
    let batchValue = $('#txtBatch').val().trim();
    if (batchValue === '') { $('#batchError').text('Batch number is required').show(); isValid = false; }
    let expiryValue = $('#txtExpiry').val();
    if (expiryValue === '') { $('#expiryError').text('Expiry date is required').show(); isValid = false; }
    let stockValue = $('#txtStock').val().trim();
    if (stockValue === '' || Number(stockValue) < 0) { $('#stockError').text('Stock must be 0 or greater').show(); isValid = false; }
    let priceValue = $('#txtPrice').val().trim();
    if (priceValue === '' || Number(priceValue) <= 0) { $('#priceError').text('Price must be greater than 0').show(); isValid = false; }
    return isValid;
}
function saveMedicine() {
    if (!validateForm()) return;
    var medicine = buildMedicineObject();
    $.ajax({
        url: 'Inventory.aspx/AddMedicine',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ medicine: medicine }),
        dataType: 'json',
        success: function (response) {
            const result = response.d;
            if (result && result.Message === "Session Expired") {
                showToast(result.Message, true);
                return;
            }
            if (result && result.Success) {
                localStorage.setItem("toastMessage", "Medicine saved successfully!");
                localStorage.setItem("toastType", "success");
                closeModal();
                location.reload();
            } else {
                showToast(result.Message || 'Failed to save medicine', true);
            }
        },
        error: function (xhr, status, error) {
            showToast('Error: ' + error, true);
        }
    });
}
function updateMedicine() {
    if (!validateForm()) return;
    var medicine = buildMedicineObject();
    $.ajax({
        type: "POST",
        url: "Inventory.aspx/UpdateMedicine",
        data: JSON.stringify({ medicine: medicine }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const result = response.d;
            if (result && result.Message === "Session Expired") {
                showToast(result.Message, true);
                return;
            }
            if (result && result.Success) {
                localStorage.setItem("toastMessage", "Medicine updated successfully!");
                localStorage.setItem("toastType", "success");
                closeModal();
                location.reload();
            } else {
                showToast(result.Message || 'Failed to update medicine', true);
            }
        },
        error: function (xhr, status, error) {
            showToast('Error: ' + error, true); 
        }
    });
}
function deleteMedicine(medicineId) {
    if (!confirm("Are you sure you want to delete this medicine?")) return;
    $.ajax({
        url: 'Inventory.aspx/DeleteMedicine',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ medicineId: medicineId }),
        dataType: 'json',
        success: function (response) {
            const result = response.d;
            if (result && result.Message === "Session Expired") {
                showToast(result.Message, true);
                return;
            }
            if (result && result.Success) {
                localStorage.setItem("toastMessage", "Medicine deleted successfully!");
                localStorage.setItem("toastType", "success");
                location.reload();
            } else {
                showToast(result.Message || "Failed to delete!", true);
            }
        },
        error: function (xhr, status, error) {
            showToast('Error: ' + error, true);
        }
    });
}
function buildMedicineObject() {
    var medicine = {
        Name: $('#txtMedName').val().trim(),
        BatchNumber: $('#txtBatch').val().trim(),
        ExpiryDate: $('#txtExpiry').val(),
        Stock: parseInt($('#txtStock').val()) || 0,
        UnitPrice: parseFloat($('#txtPrice').val())
    };

    if ($('#modalMode').val() === 'edit') {
        medicine.Id = $('#currentMedicineId').val();
    }

    return medicine;
}
function fetchMedicineData(medicineId) {
    $.ajax({
        type: "POST",
        url: "Inventory.aspx/GetMedicineById",
        data: JSON.stringify({ medicineId: medicineId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var medicine = response.d;
            if (medicine) {
                populateModalWithData(medicine);
            } else {
                showToast('Medicine not found!', true); 
            }
        },
        error: function (xhr, status, error) {
            showToast('Error: ' + error, true); 
        }
    });
}
function saveOrUpdateMedicines() {
    var mode = $('#modalMode').val();
    if (mode === 'create') {
        saveMedicine();
    } else {
        updateMedicine();
    }
}
function populateModalWithData(medicine) {
    $('#txtMedName').val(medicine.Name);
    $('#txtBatch').val(medicine.BatchNumber);
    if (medicine.ExpiryDate) {
        var expiryDate = new Date(medicine.ExpiryDate);
        $('#txtExpiry').val(formatDateForInput(expiryDate));
    }
    $('#txtStock').val(medicine.Stock);
    $('#txtPrice').val(medicine.UnitPrice);
}
function formatDateForInput(date) {
    var year = date.getFullYear();
    var month = ('0' + (date.getMonth() + 1)).slice(-2);
    var day = ('0' + date.getDate()).slice(-2);
    return year + '-' + month + '-' + day;
}
function closeModal() {
    $('#addMedicineModal').hide();
    resetForm();
}
function resetForm() {
    $('#txtMedName').val('');
    $('#txtBatch').val('');
    $('#txtExpiry').val('');
    $('#txtStock').val('0');
    $('#txtPrice').val('');
    $('#modalMode').val('create');
    $('#currentMedicineId').val('');
    clearErrors();
}
function clearErrors() {
    $('.error-message').hide().text('');
}

window.onclick = function (event) {
    if (event.target.id === 'addMedicineModal') {
        closeModal();
    }
};

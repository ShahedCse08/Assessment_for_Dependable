var medicines = [];

$(document).ready(function () {
    loadMedicines();
});
function loadMedicines() {
    $.ajax({
        type: "POST",
        url: "Billing.aspx/GetMedicines",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            medicines = res.d || [];
        },
        error: function (xhr, status, error) {
            showToast('Failed to load medicines. Please try again.', true);
        }
    });
}
function openCreateModal() {
    resetInvoiceForm();
    addMedicineRow();
    calculateModalTotal();
    fetchInvoiceNumber()
    $('#createInvoiceModal').show();
}
function openEditModal(invoiceId) {
    $('#modalTitle').html('Edit Invoice');
    $('#btnSaveText').text('Update Invoice');
    $('#modalMode').val('edit');
    $('#currentInvoiceId').val(invoiceId);
    fetchInvoiceData(invoiceId); 
    $('#createInvoiceModal').show();
}
function fetchInvoiceNumber() {
    $.ajax({
        url: 'Billing.aspx/GenerateInvoiceNumber',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.d) {
                $('#modalInvoiceNo').text(response.d);
            }
        },
        error: function () {
            showToast('Failed to generate invoice number', true);
        }
    });
}
function fetchInvoiceData(invoiceId) {
    $.ajax({
        type: "POST",
        url: "Billing.aspx/GetInvoiceData",
        data: JSON.stringify({ invoiceId: invoiceId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (res) {
            var invoiceData = res.d;
            if (!invoiceData) {
                showToast('Invoice not found', true);
                return;
            }
            populateModalWithData(invoiceData);
        },
        error: function () {
            showToast('Failed to load invoice data', true);
        }
    });
}
function populateModalWithData(invoiceData) {
    $('#modalMode').val("edit");
    $('#currentInvoiceId').val(invoiceData.Id);
    $('#modalInvoiceNo').text(invoiceData.InvoiceNumber);
    $('#modalCustomerName').val(invoiceData.CustomerName || "");
    $('#modalContact').val(invoiceData.Contact || "");
    if (invoiceData.Date) {
        var inviceDate = new Date(invoiceData.Date);
        $('#invoiceDate').val(formatDateForInput(inviceDate));
    }
    $('#modalBillingTableBody').empty();
    invoiceData.Details.forEach(function (detail, index) {
        addMedicineRowFromData(detail, index);
    });
    $('#modalSubTotal').text(invoiceData.SubTotal.toFixed(2));
    $('#modalDiscount').val(invoiceData.Discount);
    calculateModalTotal();
}
function addMedicineRowFromData(detail, rowIndex) {
    const tbody = $('#modalBillingTableBody');
    const rowId = 'modalRow_' + rowIndex;
    let medicineOptions = '<option value="">Select Medicine</option>';
    medicines.forEach(med => {
        const isSelected = med.Id == detail.MedicineId;
        medicineOptions += `<option value="${med.Id}" 
            data-batch="${med.BatchNumber}" 
            data-expiry="${med.ExpiryDate}" 
            data-price="${med.UnitPrice}"
            ${isSelected ? 'selected' : ''}>${med.Name}</option>`;
    });
    let formattedExpiry = "";
    if (detail.ExpiryDate) {
        const expiryDate = new Date(detail.ExpiryDate);
        if (!isNaN(expiryDate.getTime())) {
            formattedExpiry = expiryDate.toLocaleDateString('en-GB', {
                day: '2-digit',
                month: 'short',
                year: 'numeric'
            });
        }
    }
    const newRow = `
    <tr id="${rowId}">
        <td>
            <select class="medicine-select form-control" onchange="updateModalRowDetails('${rowId}', this)">
                ${medicineOptions}
            </select>
        </td>
        <td>
            <input type="text" class="batch-input form-control" value="${detail.BatchNumber}" readonly />
        </td>
        <td>
            <input type="text" class="expiry-input form-control" value="${formattedExpiry}" readonly />
        </td>
        <td>
            <input type="number" class="quantity-input form-control" 
                   value="${detail.Quantity}" min="1" 
                   onchange="calculateModalRowTotal('${rowId}')" 
                   onkeyup="calculateModalRowTotal('${rowId}')" />
        </td>
        <td>
            <input type="number" class="price-input form-control" 
                   value="${detail.UnitPrice}" step="0.01" readonly />
        </td>
        <td>
            <span class="line-total">${detail.LineTotal.toFixed(2)}</span>
        </td>
        <td>
            <button type="button" class="remove-btn" onclick="removeModalRow('${rowId}')">
                <i class="fas fa-trash"></i>
            </button>
        </td>
    </tr>
    `;

    tbody.append(newRow);
    calculateModalTotal();
}
function formatDateForInput(date) {
    if (!date) return "";
    if (typeof date === 'string' && date.startsWith("/Date(")) {
        date = parseInt(date.match(/\d+/)[0]);
    }
    const d = new Date(date);
    if (isNaN(d)) return "";
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${year}-${month}-${day}`; 
}
function saveOrUpdateInvoice() {
    var mode = $('#modalMode').val();
    if (mode === 'create') {
        saveInvoice();
    } else {
        updateInvoice();
    }
}
function closeModal() {
    $('#createInvoiceModal').hide();
    resetInvoiceForm();
}
function resetInvoiceForm() {
    $('#modalMode').val('create');
    $('#currentInvoiceId').val('');
    $('#modalCustomerName').val('');
    $('#modalContact').val('');
    $('#invoiceDate').val(new Date().toISOString().split('T')[0]);
    $('#modalBillingTableBody').empty(); 
    $('#modalSubTotal').text('0.00');
    $('#modalDiscount').val('0');
    $('#modalDiscountAmount').text('0.00');
    $('#modalGrandTotal').text('0.00');
    $('#modalTitle').html('<i class="fas fa-file-invoice-dollar"></i> Create New Invoice');
    $('#btnSaveText').text('Save Invoice');
}
function addMedicineRow() {
    const tbody = $('#modalBillingTableBody');
    const rowCount = tbody.find('tr').length;
    const rowId = 'modalRow_' + rowCount;
    let medicineOptions = '<option value="">Select Medicine</option>';
    medicines.forEach(med => {
        medicineOptions += `<option value="${med.Id}" 
                           data-batch="${med.BatchNumber}" 
                           data-expiry="${med.ExpiryDate}" 
                           data-price="${med.UnitPrice}">
                        ${med.Name}
                      </option>`;
    });

    const newRow = `
                <tr id="${rowId}">
                    <td>
                        <select class="medicine-select form-control" onchange="updateModalRowDetails('${rowId}', this)">
                            ${medicineOptions}
                        </select>
                    </td>
                    <td>
                        <input type="text" class="batch-input form-control" readonly />
                    </td>
                    <td>
                        <input type="text" class="expiry-input form-control" readonly />
                    </td>
                    <td>
                        <input type="number" class="quantity-input form-control" 
                               value="1" min="1" 
                               onchange="calculateModalRowTotal('${rowId}')" 
                               onkeyup="calculateModalRowTotal('${rowId}')" />
                    </td>
                    <td>
                        <input type="number" class="price-input form-control" step="0.01" readonly />
                    </td>
                    <td>
                        <span class="line-total">0.00</span>
                    </td>
                    <td>
                        <button type="button" class="remove-btn" onclick="removeModalRow('${rowId}')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;

    tbody.append(newRow);
    calculateModalTotal();
}
function updateModalRowDetails(rowId, selectElement) {
    const row = $('#' + rowId);
    const selectedOption = selectElement.options[selectElement.selectedIndex];
    if (selectedOption.value) {
        const batch = selectedOption.getAttribute('data-batch');
        const expiry = selectedOption.getAttribute('data-expiry');
        const price = selectedOption.getAttribute('data-price');
        const expiryDate = new Date(expiry);
        const formattedExpiry = expiryDate.toLocaleDateString('en-GB', {
            day: '2-digit',
            month: 'short',
            year: 'numeric'
        });
        row.find('.batch-input').val(batch);
        row.find('.expiry-input').val(formattedExpiry);
        row.find('.price-input').val(price);
        calculateModalRowTotal(rowId);
    } else {
        row.find('.batch-input').val('');
        row.find('.expiry-input').val('');
        row.find('.price-input').val('');
        row.find('.line-total').text('0.00');
    }
    calculateModalTotal();
}
function calculateModalRowTotal(rowId) {
    const row = $('#' + rowId);
    const quantity = parseFloat(row.find('.quantity-input').val()) || 0;
    const price = parseFloat(row.find('.price-input').val()) || 0;
    const total = quantity * price;
    row.find('.line-total').text(total.toFixed(2));
    calculateModalTotal();
}
function calculateModalTotal() {
    let subtotal = 0;
    $('.line-total').each(function () {
        subtotal += parseFloat($(this).text()) || 0;
    });
    const discountPercent = parseFloat($('#modalDiscount').val()) || 0;
    const discountAmount = subtotal * (discountPercent / 100);
    const grandTotal = subtotal - discountAmount;
    $('#modalSubTotal').text(subtotal.toFixed(2));
    $('#modalDiscountAmount').text(discountAmount.toFixed(2));
    $('#modalGrandTotal').text(grandTotal.toFixed(2));
}
function removeModalRow(rowId) {
    $('#' + rowId).remove();
    calculateModalTotal();
    if ($('#modalBillingTableBody tr').length === 0) {
        addMedicineRow();
    }
}
function validateInvoiceModal() {
    let isValid = true;
    $('.error-message').hide();
    const customerName = $('#modalCustomerName').val().trim();
    if (!customerName) {
        $('#nameError').text('Customer name is required').show();
        isValid = false;
    }

    let hasMedicine = false;
    $('.medicine-select').each(function () {
        if ($(this).val()) hasMedicine = true;
    });
    if (!hasMedicine) {
        $('#itemError').text('Please add at least one medicine to the invoice').show();
        isValid = false;
    }
    return isValid; 
}
function getInvoiceDtoFromModal(existingInvoiceId = 0) {
    const masterDto = {
        Id: existingInvoiceId,
        InvoiceNumber: $('#modalInvoiceNo').text().trim(),
        CustomerName: $('#modalCustomerName').val().trim(),
        Contact: $('#modalContact').val().trim(),
        Date: new Date($('#invoiceDate').val()).toISOString().split('T')[0],
        Discount: parseFloat($('#modalDiscount').val()) || 0,
        SubTotal: 0,
        GrandTotal: 0,
        Details: []
    };

    $('.medicine-select').each(function () {
        const row = $(this).closest('tr');
        if (!$(this).val()) return;

        const lineTotal = parseFloat(row.find('.line-total').text().replace('$', '').replace(',', '')) || 0;

        masterDto.Details.push({
            MedicineId: parseInt($(this).val()),
            MedicineName: $(this).find('option:selected').text(),
            BatchNumber: row.find('.batch-input').val(),
            ExpiryDate: row.find('.expiry-input').val()
                ? new Date(row.find('.expiry-input').val()).toISOString().split('T')[0]
                : null,
            Quantity: parseInt(row.find('.quantity-input').val()) || 0,
            UnitPrice: parseFloat(row.find('.price-input').val()) || 0,
            LineTotal: lineTotal
        });

        masterDto.SubTotal += lineTotal;
    });

    masterDto.GrandTotal = masterDto.SubTotal - (masterDto.SubTotal * (masterDto.Discount / 100));
    return masterDto;
}
function saveInvoice() {
    if (!validateInvoiceModal()) return;

    const masterDto = getInvoiceDtoFromModal();

    $.ajax({
        url: 'Billing.aspx/CreateInvoice',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ dto: masterDto }),
        dataType: 'json',
        beforeSend: function () {
            $('#btnSaveText').prop('disabled', true).html('Saving...');
        },
        success: function (response) {
            const result = response.d;
            if (result && result.Message === "Session Expired") {
                showToast(result.Message, true);
                return;
            }
            if (result && result.Success) {
                localStorage.setItem("toastMessage", "Invoice saved successfully!");
                localStorage.setItem("toastType", "success");
                closeModal();
                location.reload();
            } else {
                showToast(result.Message || 'Failed to save invoice', true);
            }
            $('#btnSaveText').prop('disabled', false).html('Save Invoice');
        },
        error: function (xhr, status, error) {
            showToast('Error: ' + error, true);
            $('#btnSaveText').prop('disabled', false).html('Save Invoice');
        }
    });
}
function updateInvoice() {
    const invoiceId = parseInt($('#currentInvoiceId').val()) || 0;
    if (invoiceId <= 0) {
        showToast('Invalid invoice id', true);
        return;
    }
    if (!validateInvoiceModal()) return;
    const masterDto = getInvoiceDtoFromModal(invoiceId);
    $.ajax({
        url: 'Billing.aspx/UpdateInvoice',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ dto: masterDto }),
        dataType: 'json',
        beforeSend: function () {
            $('#btnSaveText').prop('disabled', true).html('Updating...');
        },
        success: function (response) {
            const result = response.d;
            if (result && result.Message === "Session Expired") {
                showToast(result.Message, true);
                return;
            }
            if (result && result.Success) {
                localStorage.setItem("toastMessage", "Invoice updated successfully!");
                localStorage.setItem("toastType", "success");
                closeModal();
                location.reload();
            } else {
                showToast(result.Message || 'Failed to update invoice', true);
            }
            $('#btnSaveText').prop('disabled', false).html('Update Invoice');
        },
        error: function (xhr, status, error) {
            showToast('Error: ' + error, true);
            $('#btnSaveText').prop('disabled', false).html('Update Invoice');
        }
    });
}
function deleteInvoice(id) {
    if (!confirm("Are you sure you want to delete this invoice?")) return;
    $.ajax({
        url: 'Billing.aspx/DeleteInvoice',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ invoiceId: id }),
        dataType: 'json',
        success: function (response) {
            const result = response.d;
            if (result && result.Message === "Session Expired") {
                showToast(result.Message, true);
                return;
            }
            if (result && result.Success) {
                localStorage.setItem("toastMessage", "Invoice deleted successfully!");
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

$(window).click(function (event) {
    if (event.target.id === 'createInvoiceModal') {
        closeModal();
    }
});

$(document).ready(function () {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });

    const successMsg = $('#tempSuccess').val();
    const errorMsg = $('#tempError').val();

    if (successMsg && successMsg.trim() !== '') {
        Toast.fire({
            icon: 'success',
            title: successMsg
        });
    }

    if (errorMsg && errorMsg.trim() !== '') {
        Toast.fire({
            icon: 'error',
            title: errorMsg
        });
    }
});
function changePage(page) {
    if (page < 1) return;
    const form = document.getElementById('filterForm');
    if (!form) return;

    let pageInput = form.querySelector('input[name="page"]');
    if (!pageInput) {
        pageInput = document.createElement('input');
        pageInput.type = 'hidden';
        pageInput.name = 'page';
        form.appendChild(pageInput);
    }
    pageInput.value = page;
    form.submit();
}
function DeleteTransaction(transactionId) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Dữ liệu giao dịch này sẽ không thể khôi phục!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e91e63',
        cancelButtonColor: '#7b809a',
        confirmButtonText: 'Đồng ý xóa',
        cancelButtonText: 'Hủy',
        customClass: { popup: 'border-radius-xl' }
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Transaction/DeleteTransaction?id=${transactionId}`;
        }
    });
}
function DeleteWallet(walletId) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Dữ liệu ví này sẽ không thể khôi phục!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e91e63',
        cancelButtonColor: '#7b809a',
        confirmButtonText: 'Đồng ý xóa',
        cancelButtonText: 'Hủy',
        customClass: { popup: 'border-radius-xl' }
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Wallet/DeleteWallet?id=${walletId}`;
        }
    });
}
function SetDefaultWallet(walletId) {
    window.location.href = `/Wallet/DefaultWallet?id=${walletId}`;
}
function validateAndSubmit() {
    const form = document.querySelector('#modalAddWallet form');
    const walletName = form.querySelector('input[name="WalletName"]').value.trim();
    const bankId = form.querySelector('select[name="BankId"]').value;
    const cardNumber = form.querySelector('input[name="CardNumber"]').value.trim();
    const cardHolderName = form.querySelector('input[name="CardHolderName"]').value.trim();

    const nameRegex = /^[\p{L}\s]+$/u;
    const numberRegex = /^[0-9]+$/;

    let isValid = true;

    clearAllErrors();

    if (!walletName) {
        showError('WalletName', 'Vui lòng nhập tên ví.');
        isValid = false;
    }

    if (!bankId || bankId === "") {
        showError('Bank', 'Vui lòng chọn ngân hàng liên kết.');
        isValid = false;
    }

    if (!cardNumber) {
        showError('CardNumber', 'Vui lòng nhập số thẻ.');
        isValid = false;
    } else if (!numberRegex.test(cardNumber)) {
        showError('CardNumber', 'Số thẻ chỉ được chứa chữ số.');
        isValid = false;
    } else if (cardNumber.length < 10) {
        showError('CardNumber', 'Số thẻ phải có ít nhất 10 ký tự.');
        isValid = false;
    }

    if (!cardHolderName) {
        showError('CardHolderName', 'Vui lòng nhập tên chủ thẻ.');
        isValid = false;
    } else if (!nameRegex.test(cardHolderName)) {
        showError('CardHolderName', 'Tên chủ thẻ không hợp lệ (chỉ chứa chữ cái).');
        isValid = false;
    }

    if (isValid) {
        form.querySelector('input[name="CardHolderName"]').value = cardHolderName.toUpperCase();
        form.submit();
    }
}

function showError(fieldName, message) {
    const errorSpan = document.getElementById(`error-${fieldName}`);
    if (errorSpan) {
        errorSpan.innerText = message;
      
        const group = errorSpan.closest('.input-group');
        if (group) {
            group.classList.add('is-invalid');
        }
    }
}

function clearAllErrors() {
    const errorSpans = document.querySelectorAll('[id^="error-"]');
    errorSpans.forEach(span => {
        span.innerText = '';
        const group = span.closest('.input-group');
        if (group) {
            group.classList.remove('is-invalid');
        }
    });
}
var myModal = document.getElementById('modalAddWallet')
myModal.addEventListener('shown.bs.modal', function () {
    if (typeof (md) !== "undefined") {
        md.initFormConrtols();
    }
})

function updateBankLogo() {
    const select = document.getElementById('bankSelect');
    const img = document.getElementById('selectedBankLogo');

    const selectedOption = select.options[select.selectedIndex];
    const logoUrl = selectedOption.getAttribute('data-logo');
    console.log("Logo URL:", logoUrl);

    if (logoUrl && logoUrl.trim() !== "") {
        img.src = logoUrl;
        img.style.display = 'block';
    } else {
        img.src = "";
        img.style.display = 'none';
    }
}
document.addEventListener("DOMContentLoaded", updateBankLogo);

function transferFunds(element) {
    const id = element.dataset.id;
    const name = element.dataset.name;
    const balance = parseFloat(element.dataset.balance);
    const bankLogo = element.dataset.logo;

    document.getElementById('displayBankLogo').src = bankLogo;
    document.getElementById('displayWalletName').innerText = name;

    const displayBalanceEl = document.getElementById('displayBalance');
    displayBalanceEl.innerText = balance.toLocaleString('it-IT') + " VNĐ";
    displayBalanceEl.dataset.rawBalance = balance;

    document.getElementById('sentWalletId').value = id;
    document.getElementById('transferAmount').value = "";

    const selectReceived = document.getElementById('receivedWalletId');
    selectReceived.value = ""; 

    Array.from(selectReceived.options).forEach(option => {
        if (option.value === id) {
            option.disabled = true;
            option.style.display = "none";
        } else {
            option.disabled = false;
            option.style.display = "block";
        }
    });

    var myModal = new bootstrap.Modal(document.getElementById('modalTransferWallet'));
    myModal.show();
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('#modalTransferWallet form');
    const transferAmount = document.getElementById('transferAmount');
    const errorAmount = document.getElementById('error-amount');
    const errorReceived = document.getElementById('error-received');

    function getRawBalance() {
        return parseFloat(document.getElementById('displayBalance').dataset.rawBalance) || 0;
    }

    transferAmount.addEventListener('input', function () {
        if (this.value < 0) {
            this.value = 0;
        }
        const amount = parseFloat(this.value);
        const balance = getRawBalance();

        if (amount > balance) {
            errorAmount.innerText = "Số tiền vượt quá số dư khả dụng!";
            errorAmount.style.display = 'block';
        } else if (this.value !== "" && amount < 1000) {
            errorAmount.innerText = "Số tiền tối thiểu là 1.000 VNĐ";
            errorAmount.style.display = 'block';
        } else {
            errorAmount.style.display = 'none';
        }
    });

    form.addEventListener('submit', function (e) {
        let isValid = true;
        const amount = parseFloat(transferAmount.value);
        const balance = getRawBalance();
        const receivedWallet = document.getElementById('receivedWalletId');

        if (!receivedWallet.value) {
            errorReceived.innerText = "Vui lòng chọn ví nhận";
            errorReceived.style.display = 'block';
            isValid = false;
        }

        if (isNaN(amount) || amount < 1000) {
            errorAmount.innerText = "Vui lòng nhập số tiền từ 1.000đ trở lên";
            errorAmount.style.display = 'block';
            isValid = false;
        } else if (amount > balance) {
            errorAmount.innerText = "Số tiền vượt quá số dư khả dụng!";
            errorAmount.style.display = 'block';
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
        }
    });

    const modalIncome = document.getElementById('modalIncome');
    const modalExpense = document.getElementById('modalExpense');

    modalIncome.addEventListener('show.bs.modal', loadCategories);
    modalExpense.addEventListener('show.bs.modal', loadCategories);

    async function loadCategories() {

        try {

            const response = await fetch('/Category/GetCategoriesForTransaction');

            if (response.ok) {

                const categories = await response.json();

                const incomeSelect = document.getElementById('incomeCategorySelect');
                const expenseSelect = document.getElementById('expenseCategorySelect');
                incomeSelect.innerHTML = '<option value="" disabled selected>-- Chọn danh mục thu --</option>';
                expenseSelect.innerHTML = '<option value="" disabled selected>-- Chọn danh mục chi --</option>';
                categories.forEach(cat => {

                    const option = document.createElement('option');

                    option.value = cat.categoryId;

                    option.textContent = cat.categoryName;

                    if (cat.type === "Income") {

                        incomeSelect.appendChild(option);

                    } else if (cat.type === "Expense") {

                        expenseSelect.appendChild(option);
                    }

                });

            }

        } catch (error) {

            console.error('Lỗi khi load category:', error);

        }

    }
});

['formIncome', 'formExpense'].forEach(formId => {
    const form = document.getElementById(formId);
    if (!form) return;

    const amountInput = form.querySelector('input[name="Amount"]');
    amountInput.addEventListener('input', function () {
        if (this.value < 0) this.value = 0;
        clearError(form, 'Amount');
    });

    form.querySelector('select[name="CategoryId"]').addEventListener('change', () => clearError(form, 'CategoryId'));
    form.querySelector('textarea[name="Note"]').addEventListener('input', () => clearError(form, 'Note'));

    form.addEventListener('submit', function (e) {
        let hasError = false;
        const amount = this.elements['Amount'].value;
        const category = this.elements['CategoryId'].value;
        const note = this.elements['Note'].value.trim();

        this.querySelectorAll('.error-msg').forEach(el => el.innerText = '');
        this.querySelectorAll('.input-group').forEach(el => el.classList.remove('border', 'border-danger'));

        // Validate Số tiền
        if (!amount || amount <= 0) {
            showValidationErr(this, 'Amount', 'Vui lòng nhập số tiền hợp lệ');
            hasError = true;
        }

        // Validate Danh mục
        if (!category) {
            const label = formId === 'formIncome' ? 'thu nhập' : 'chi tiêu';
            showValidationErr(this, 'CategoryId', `Vui lòng chọn danh mục ${label}`);
            hasError = true;
        }

        // Validate Ghi chú
        if (!note) {
            showValidationErr(this, 'Note', 'Nội dung ghi chú không được để trống');
            hasError = true;
        } else if (note.length > 200) {
            showValidationErr(this, 'Note', 'Ghi chú tối đa 200 ký tự');
            hasError = true;
        }

        if (hasError) {
            e.preventDefault();
        }
    });
});

function showValidationErr(formElement, fieldName, message) {
    const errorSpan = formElement.querySelector(`#err-${fieldName}`);
    if (errorSpan) errorSpan.innerText = message;
}

function clearError(formElement, fieldName) {
    const errorSpan = formElement.querySelector(`#err-${fieldName}`);
    if (errorSpan) errorSpan.innerText = '';
}
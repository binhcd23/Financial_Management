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

$(document).ready(function () {
    // Cấu hình mẫu Toast của SweetAlert2
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

    // 1. Lấy thông báo từ hidden inputs
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

    if ($('.field-validation-error').length > 0 || $('.validation-summary-errors').length > 0) {
        const modalElem = document.getElementById('modalAddGoal');
        if (modalElem) {
            const myModal = new bootstrap.Modal(modalElem);
            myModal.show();
        }
    }
    const dateInput = document.getElementById('TargetDate');
    if (dateInput) {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);

        const year = tomorrow.getFullYear();
        const month = String(tomorrow.getMonth() + 1).padStart(2, '0');
        const day = String(tomorrow.getDate()).padStart(2, '0');

        const minDate = `${year}-${month}-${day}`;
        dateInput.setAttribute('min', minDate);
    }
});
function validateAndSubmit() {
    // 1. Lấy giá trị
    const name = $('#GoalName').val().trim();
    const amount = $('#TargetAmount').val();
    const dateValue = $('#TargetDate').val();
    let isValid = true;

    // 2. Reset lỗi cũ
    $('.text-danger').text('');

    // 3. Kiểm tra Tên
    if (name === "") {
        $('#error-GoalName').text('Tên mục tiêu không được để trống');
        isValid = false;
    } else if (name.length > 100) {
        $('#error-GoalName').text('Tên không quá 100 ký tự');
        isValid = false;
    }

    // 4. Kiểm tra Số tiền
    if (!amount || amount < 1000) {
        $('#error-TargetAmount').text('Số tiền phải lớn hơn 1,000 VNĐ');
        isValid = false;
    }

    // 5. Kiểm tra Ngày
    if (!dateValue) {
        $('#error-TargetDate').text('Vui lòng chọn ngày kết thúc');
        isValid = false;
    } else {
        const selectedDate = new Date(dateValue);
        selectedDate.setHours(0, 0, 0, 0);

        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        tomorrow.setHours(0, 0, 0, 0);

        if (selectedDate < tomorrow) {
            $('#error-TargetDate').text('Ngày kết thúc phải từ ngày mai trở đi');
            isValid = false;
        }
    }

    // 6. Nếu tất cả OK thì mới Submit form
    if (isValid) {
        document.getElementById('createGoalForm').submit();
    } else {
        // Thông báo nhẹ nhàng cho người dùng
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 2000
        });
        Toast.fire({ icon: 'warning', title: 'Vui lòng kiểm tra lại thông tin!' });
    }
}
// 4. Các hàm xử lý hành động (Giữ nguyên logic cũ)
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

async function updateGoalProgress(goalId) {
    const input = document.getElementById(`amount-${goalId}`);
    const amount = input ? input.value : 0;

    if (!amount || amount <= 0) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'warning',
            title: 'Vui lòng nhập số tiền hợp lệ!',
            showConfirmButton: false,
            timer: 2000
        });
        return;
    }

    const params = new URLSearchParams();
    params.append('goalId', goalId);
    params.append('addedValue', amount);

    try {
        const response = await fetch('/Finance/UpdateProgress', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: params
        });

        if (response.ok) {
            window.location.reload();
        } else {
            Swal.fire({ icon: 'error', title: 'Lỗi', text: 'Không thể cập nhật tiến độ' });
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

function confirmDelete(goalId) {
    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Dữ liệu mục tiêu này sẽ không thể khôi phục!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e91e63',
        cancelButtonColor: '#7b809a',
        confirmButtonText: 'Đồng ý xóa',
        cancelButtonText: 'Hủy',
        customClass: { popup: 'border-radius-xl' }
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Finance/DeleteGoal?id=${goalId}`;
        }
    });
}
const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true
});
// 1. Mở Modal chính
function openConfigModal() {
    renderMainMenu();
    $('#mainConfigModal').modal('show');
}

// 2. Giao diện Menu chính
function renderMainMenu() {
    $('#btnBackConfig').hide();
    $('#configModalTitle').text("Cài đặt hệ thống");
    $('#configModalFooter').hide();

    let html = `
        <div class="list-group">
            <button onclick="renderSavingForm()" class="list-group-item list-group-item-action border-0 d-flex align-items-center mb-2 shadow-sm border-radius-lg">
                <div class="icon icon-shape icon-sm bg-gradient-primary shadow text-center me-3 d-flex align-items-center justify-content-center">
                    <i class="fa fa-piggy-bank opacity-10"></i>
                </div>
                <div class="d-flex flex-column">
                    <h6 class="mb-0 text-lg font-weight-bold">Thiết lập tiết kiệm</h6>
                    <p class="mb-0 text-secondary">Cấu hình tỷ lệ trích tiền tự động</p>
                </div>
                <i class="fa fa-chevron-right ms-auto text-xs text-secondary"></i>
            </button>
            
            <button onclick="renderBudgetList()" class="list-group-item list-group-item-action border-0 d-flex align-items-center shadow-sm border-radius-lg">
                <div class="icon icon-shape icon-sm bg-gradient-info shadow text-center me-3 d-flex align-items-center justify-content-center">
                    <i class="fa fa-bullseye opacity-10"></i>
                </div>
                <div class="d-flex flex-column">
                    <h6 class="mb-0 text-lg font-weight-bold">Giới hạn ngân sách</h6>
                    <p class="mb-0 text-secondary">Quản lý hạn mức chi tiêu theo danh mục</p>
                </div>
                <i class="fa fa-chevron-right ms-auto text-xs text-secondary"></i>
            </button>
        </div>
    `;
    $('#configModalBody').html(html);
}

// 3. Giao diện Form Tiết Kiệm (Saving Config)
function renderSavingForm() {
    $('#btnBackConfig').show().attr('onclick', 'renderMainMenu()');
    $('#configModalTitle').text("Thiết lập tiết kiệm");
    $('#configModalFooter').hide();
    $('#configModalBody').html('<p class="text-center">Đang tải dữ liệu...</p>');

    // Sử dụng Promise.all để đợi cả 2 request cùng xong
    const getWallets = $.get(`/Wallet/GetSavingWallets`);
    const getProfile = $.get(`/Usertaxprofile/GetSavingProfile`);

    $.when(getWallets, getProfile)
        .done(function (walletsRes, profileRes) {
            const wallets = walletsRes[0];
            const profile = profileRes[0];
            renderSavingUI(wallets, profile, true);
        })
        .fail(function (xhr) {
            if (xhr.status === 404) {
                getWallets.done(function (wallets) {
                    renderSavingUI(wallets, { savingRate: 0, walletId: 0 }, false);
                }).fail(function () {
                    Toast.fire({ icon: 'error', title: 'Không thể tải danh sách ví' });
                });
            } else {
                Toast.fire({ icon: 'error', title: 'Lỗi hệ thống' });
            }
        });
}

// Hàm phụ trợ để vẽ giao diện tránh lặp code
function renderSavingUI(wallets, profile, isEdit) {
    let options = "";
    if (wallets && wallets.length > 0) {
        options = wallets.map(w =>
            `<option value="${w.walletId}" ${w.walletId == profile.walletId ? 'selected' : ''}>${w.walletName} - ${w.bankName}</option>`
        ).join('');
    } else {
        options = `<option value="">-- Bạn không có ví tiết kiệm nào --</option>`;
    }

    let html = `
        <div class="form-group">
            <label class="form-label">Ví tiết kiệm mục tiêu</label>
            <select class="form-select border p-2" id="walletSelect">
                ${options}
            </select>
            <small id="errorWallet" class="text-danger" style="display:none"></small>
        </div>
        <div class="form-group mt-3">
            <label class="form-label">Tỷ lệ trích tiết kiệm (%)</label>
            <input type="number" class="form-control border p-2" id="rateInput" 
                   value="${profile.savingRate || 0}" min="0" max="100">
            <small id="errorRate" class="text-danger" style="display:none"></small>
        </div>
    `;
    $('#configModalBody').html(html);

    if (isEdit) {
        $('#configModalFooter').show().html(`
            <button class="btn bg-gradient-info w-100" onclick="saveSavingConfig('PUT')">
                <i class="fa fa-save me-2"></i> Cập nhật thiết lập
            </button>
        `);
    } else {
        $('#configModalFooter').show().html(`
            <div class="text-center mb-2"><small class="text-info">Bạn đang tạo thiết lập mới cho hệ thống</small></div>
            <button class="btn bg-gradient-primary w-100" onclick="saveSavingConfig('POST')">
                <i class="fa fa-plus me-2"></i> Tạo thiết lập mới
            </button>
        `);
    }
}
function saveSavingConfig(method) {
    $('#errorWallet, #errorRate').hide().text('');
    let isValid = true;

    const walletId = $('#walletSelect').val();
    const savingRate = parseFloat($('#rateInput').val());

    if (!walletId) {
        $('#errorWallet').text('Vui lòng chọn một ví tiết kiệm mục tiêu').show();
        isValid = false;
    }

    if (isNaN(savingRate)) {
        $('#errorRate').text('Vui lòng nhập tỷ lệ tiết kiệm').show();
        isValid = false;
    } else if (savingRate < 0 || savingRate > 100) {
        $('#errorRate').text('Tỷ lệ tiết kiệm phải nằm trong khoảng từ 0% đến 100%').show();
        isValid = false;
    }

    if (!isValid) return;

    const dto = {
        walletId: parseInt(walletId),
        savingRate: savingRate
    };

    const url = method === 'POST' ? '/Usertaxprofile/SaveConfig' : '/Usertaxprofile/EditConfig';

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: function (response) {
            Toast.fire({ icon: 'success', title: response.message });
            renderSavingForm();
        },
        error: function (xhr) {
            const msg = xhr.responseJSON ? xhr.responseJSON.message : "Có lỗi xảy ra";
            Toast.fire({ icon: 'error', title: msg });
        }
    });
}
// 4. Giao diện Danh sách Ngân sách (Budget List)
function renderBudgetList() {
    $('#btnBackConfig').show().attr('onclick', 'renderMainMenu()');
    $('#configModalTitle').text("Giới hạn ngân sách");
    $('#configModalFooter').show().html(`
        <button class="btn bg-gradient-success w-100 mb-0" onclick="renderCreateBudgetForm()">
            <i class="fa fa-plus me-2"></i>Thêm ngân sách mới
        </button>
    `);

    $.get(`/Budget/GetList`, function (data) {
        let html = '<div class="list-group list-group-flush">';
        if (data.length === 0) html += '<p class="text-center">Chưa có ngân sách nào.</p>';

        data.forEach(item => {
            html += `
                <button onclick="renderBudgetDetail(${item.categoryId})" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center border-0 border-bottom">
                    <div>
                        <span class="d-block font-weight-bold">Danh mục: ${item.categoryName}</span>
                        <small class="text-secondary">Hạn mức: ${item.amountLimit.toLocaleString()} đ</small>
                    </div>
                   <i class="fa fa-trash text-danger p-2"
                       onclick="confirmDeleteBudget(event, ${item.budgetId})"></i>
                </button>
            `;
        });
        html += '</div>';
        $('#configModalBody').html(html);
    });
}
function getNextDayStr(dateStr) {
    const d = new Date(dateStr);
    d.setDate(d.getDate() + 1);
    return d.toISOString().split('T')[0];
}

// giao diện tạo budget mơ
function renderCreateBudgetForm() {
    $('#btnBackConfig').show().attr('onclick', 'renderBudgetList()');
    $('#configModalTitle').text("Thiết lập ngân sách mới");

    $('#configModalFooter').show().html(`
        <div class="d-flex justify-content-between w-100">
            <button class="btn btn-link text-secondary mb-0" onclick="renderBudgetList()">Hủy</button>
            <button class="btn bg-gradient-success mb-0" onclick="saveNewBudget()">Lưu thiết lập</button>
        </div>
    `);

    // --- PHẦN TÍNH TOÁN NGÀY CHUẨN ---
    const now = new Date();
    const today = now.toISOString().split('T')[0];

    const tomorrowObj = new Date();
    tomorrowObj.setDate(now.getDate() + 1);
    const tomorrow = tomorrowObj.toISOString().split('T')[0];
    // --------------------------------

    $.get('/Category/GetCategories', function (categories) {
        let options = categories.map(c => `<option value="${c.categoryId}">${c.categoryName}</option>`).join('');

        let html = `
            <div class="form-group">
                <label>Chọn danh mục chi tiêu</label>
                <select class="form-select border p-2" id="selectCategoryId">
                    ${options}
                </select>
            </div>
            <div class="form-group mt-3">
                <label>Hạn mức (đ)</label>
                <input type="number" class="form-control border p-2" id="inputAmountLimit" placeholder="VD: 5000000">
                <small id="errorAmountLimit" class="text-danger" style="display:none"></small>
            </div>
            <div class="row mt-3">
                <div class="col-6">
                    <label>Ngày bắt đầu</label>
                    <input type="date" class="form-control border p-2" id="inputStartDate"
                           min="${today}" value="${today}">
                    <small id="errorStartDate" class="text-danger" style="display:none"></small>
                </div>
                <div class="col-6">
                    <label>Ngày kết thúc</label>
                    <input type="date" class="form-control border p-2" id="inputEndDate"
                           min="${tomorrow}">
                    <small id="errorEndDate" class="text-danger" style="display:none"></small>
                </div>
            </div>
        `;
        $('#configModalBody').html(html);

        // --- XỬ LÝ SỰ KIỆN CHANGE ---
        $(document).off('change', '#inputStartDate').on('change', '#inputStartDate', function () {
            const selectedStartDate = $(this).val();
            if (!selectedStartDate) return;

            const minEndDate = getNextDayStr(selectedStartDate);

            // Cập nhật min cho End Date
            $('#inputEndDate').attr('min', minEndDate);

            // Kiểm tra nếu giá trị hiện tại của End Date không còn hợp lệ thì xóa đi
            if ($('#inputEndDate').val() && $('#inputEndDate').val() < minEndDate) {
                $('#inputEndDate').val('');
                $('#errorEndDate').text("Ngày kết thúc đã được đặt lại do ngày bắt đầu thay đổi").show();
            } else {
                $('#errorEndDate').hide();
            }

            $('#errorStartDate').hide(); // Ẩn lỗi khi người dùng đã chọn lại
        });
    });
}
// 5. Giao diện Chi tiết Budget (Budget Detail Form)
function renderBudgetDetail(categoryId) {
    $('#btnBackConfig').show().attr('onclick', 'renderBudgetList()');
    $('#configModalTitle').text("Chi tiết ngân sách");
    $('#configModalFooter').show().html(`
        <button class="btn bg-gradient-info w-100" onclick="saveBudgetUpdate()">Cập nhật ngân sách</button>
    `);

    $.get(`/Budget/GetDetail?categoryId=${categoryId}`, function (data) {
        const minEndDate = getNextDayStr(data.startDate);
        const today = new Date().toISOString().split('T')[0];

        let html = `
            <input type="hidden" id="editCategoryId" value="${data.categoryId}">
            <div class="form-group">
                <label>Hạn mức chi tiêu</label>
                <input type="number" class="form-control border p-2" id="editAmountLimit" value="${data.amountLimit}">
                <small id="errorEditAmount" class="text-danger" style="display:none"></small>
            </div>
            <div class="row mt-3">
                <div class="col-6">
                    <label>Ngày bắt đầu</label>
                    <input type="date" class="form-control border p-2" id="editStartDate" value="${data.startDate}" min="${today}">
                    <small id="errorEditStart" class="text-danger" style="display:none"></small>
                </div>
                <div class="col-6">
                    <label>Ngày kết thúc</label>
                    <input type="date" class="form-control border p-2" id="editEndDate" value="${data.endDate}" min="${minEndDate}">
                    <small id="errorEditEnd" class="text-danger" style="display:none"></small>
                </div>
            </div>
        `;
        $('#configModalBody').html(html);
        $(document).off('change', '#editStartDate').on('change', '#editStartDate', function () {
            const selectedStart = $(this).val();
            if (!selectedStart) return;

            const nextMinEnd = getNextDayStr(selectedStart);
            $('#editEndDate').attr('min', nextMinEnd);

            if ($('#editEndDate').val() <= selectedStart) {
                $('#editEndDate').val('');
                $('#errorEditEnd').text("Ngày kết thúc đã được đặt lại").show();
            } else {
                $('#errorEditEnd').hide();
            }
            $('#errorEditStart').hide();
        });
    });
}
function saveNewBudget() {
    // Reset lại các thông báo lỗi trước khi kiểm tra
    $('.text-danger').hide();
    let isValid = true;

    // Lấy dữ liệu
    const categoryId = $('#selectCategoryId').val();
    const amountLimit = parseFloat($('#inputAmountLimit').val());
    const startDate = $('#inputStartDate').val();
    const endDate = $('#inputEndDate').val();
    const today = new Date().toISOString().split('T')[0];

    // 1. Kiểm tra Hạn mức
    if (!amountLimit || amountLimit <= 0) {
        $('#errorAmountLimit').text("Hạn mức phải lớn hơn 0").show();
        isValid = false;
    }

    // 2. Kiểm tra Ngày bắt đầu
    if (!startDate) {
        $('#errorStartDate').text("Vui lòng chọn ngày bắt đầu").show();
        isValid = false;
    } else if (startDate < today) {
        $('#errorStartDate').text("Không được chọn ngày quá khứ").show();
        isValid = false;
    }

    // 3. Kiểm tra Ngày kết thúc
    if (!endDate) {
        $('#errorEndDate').text("Vui lòng chọn ngày kết thúc").show();
        isValid = false;
    } else if (endDate <= startDate) {
        $('#errorEndDate').text("Ngày kết thúc phải lớn hơn ngày bắt đầu").show();
        isValid = false;
    }

    if (!isValid) return;

    const budgetData = {
        categoryId: parseInt(categoryId),
        amountLimit: amountLimit,
        startDate: startDate,
        endDate: endDate
    };

    $.ajax({
        url: '/Budget/CreateBudget',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(budgetData),
        success: function (response) {
            if (response.success) {
                Toast.fire({ icon: 'success', title: response.message });
                renderBudgetList();
            }
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON ? xhr.responseJSON.message : "Lỗi hệ thống";
            Toast.fire({ icon: 'error', title: errorMsg });
        }
    });
}
function saveBudgetUpdate() {
    $('.text-danger').hide();
    let isValid = true;

    // 2. Lấy dữ liệu từ form
    const categoryId = $('#editCategoryId').val();
    const amountLimit = parseFloat($('#editAmountLimit').val());
    const startDate = $('#editStartDate').val();
    const endDate = $('#editEndDate').val();

    // 3. Validate các trường
    if (!amountLimit || amountLimit <= 0) {
        $('#errorEditAmount').text("Hạn mức phải lớn hơn 0").show();
        isValid = false;
    }

    if (!startDate) {
        $('#errorEditStart').text("Vui lòng chọn ngày bắt đầu").show();
        isValid = false;
    }

    if (!endDate) {
        $('#errorEditEnd').text("Vui lòng chọn ngày kết thúc").show();
        isValid = false;
    } else if (endDate <= startDate) {
        $('#errorEditEnd').text("Ngày kết thúc phải lớn hơn ngày bắt đầu").show();
        isValid = false;
    }

    // Nếu không hợp lệ thì dừng lại
    if (!isValid) return;

    // 4. Gửi dữ liệu cập nhật
    const budgetData = {
        categoryId: parseInt(categoryId),
        amountLimit: amountLimit,
        startDate: startDate,
        endDate: endDate
    };

    $.ajax({
        url: '/Budget/UpdateBudget',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(budgetData),
        success: function (response) {
            if (response.success) {
                Toast.fire({ icon: 'success', title: response.message });
                renderBudgetList();
            }
        },
        error: function (xhr) {
            const errorMsg = xhr.responseJSON ? xhr.responseJSON.message : "Có lỗi xảy ra";
            Toast.fire({ icon: 'warning', title: errorMsg });
        }
    });
}
function confirmDeleteBudget(event, budgetId) {
    // Ngăn việc kích hoạt hàm renderBudgetDetail của thẻ cha
    event.stopPropagation();

    Swal.fire({
        title: 'Xác nhận xóa?',
        text: "Bạn sẽ không thể khôi phục thiết lập ngân sách này!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ea0606',
        cancelButtonColor: '#8392ab',
        confirmButtonText: 'Đồng ý xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            $.get(`/Budget/DeleteBudget?id=${budgetId}`, function (response) {
                if (response.success) {
                    Toast.fire({
                        icon: 'success',
                        title: response.message 
                    });
                    renderBudgetList();
                }
            }).fail(function () {
                Toast.fire({
                    icon: 'error',
                    title: 'Không thể xóa thiết lập này'
                });
            });
        }
    });
}
document.addEventListener('DOMContentLoaded', function () {
    const otpBoxes = document.querySelectorAll('.otp-box');
    const storageKey = 'otp_countdown_@email';
    const countdownElement = document.getElementById('countdown');
    const resendForm = document.getElementById('resend-form');
    const countdownContainer = document.getElementById('countdown-container');

    let timeLeft = 60;
    let countdownInterval = null;

    // Khôi phục thời gian từ sessionStorage nếu có
    const savedTime = sessionStorage.getItem(storageKey);
    if (savedTime) {
        const startTime = parseInt(savedTime);
        const elapsed = Math.floor((Date.now() - startTime) / 1000);
        timeLeft = Math.max(0, 60 - elapsed);
    } else {
        sessionStorage.setItem(storageKey, Date.now().toString());
    }

    // Cập nhật hiển thị ngay lập tức trước khi start countdown
    if (countdownElement) {
        countdownElement.textContent = timeLeft;
    }

    // Bắt đầu đếm ngược
    function startCountdown() {
        countdownInterval = setInterval(function () {
            timeLeft--;
            if (countdownElement) {
                countdownElement.textContent = timeLeft;
            }

            if (timeLeft <= 0) {
                clearInterval(countdownInterval);
                sessionStorage.removeItem(storageKey);
                otpBoxes.forEach(box => {
                    box.disabled = true;
                    box.classList.add('disabled');
                });
                if (countdownContainer) countdownContainer.style.display = 'none';
                if (resendForm) resendForm.style.display = 'inline';
            }
        }, 1000);
    }

    // Bắt đầu countdown khi trang load
    startCountdown();

    // Focus vào ô đầu tiên khi trang load (chỉ nếu chưa hết thời gian)
    if (otpBoxes.length > 0 && timeLeft > 0) {
        otpBoxes[0].focus();
    }

    otpBoxes.forEach((box, index) => {
        box.addEventListener('input', function (e) {
            // Kiểm tra nếu đã hết thời gian
            if (timeLeft <= 0 || box.disabled) {
                e.preventDefault();
                return;
            }

            const value = e.target.value;

            // Chỉ cho phép số
            if (!/^\d$/.test(value)) {
                e.target.value = '';
                return;
            }

            // Chuyển sang ô tiếp theo
            if (value && index < otpBoxes.length - 1) {
                otpBoxes[index + 1].focus();
            }

            // Cập nhật hidden input
            updateOTPCode();
        });

        box.addEventListener('keydown', function (e) {
            // Xử lý phím Backspace
            if (e.key === 'Backspace') {
                if (!e.target.value && index > 0) {
                    otpBoxes[index - 1].focus();
                }
            }

            // Xử lý phím Arrow
            if (e.key === 'ArrowLeft' && index > 0) {
                otpBoxes[index - 1].focus();
            }
            if (e.key === 'ArrowRight' && index < otpBoxes.length - 1) {
                otpBoxes[index + 1].focus();
            }
        });

        // Xử lý paste
        box.addEventListener('paste', function (e) {
            // Kiểm tra nếu đã hết thời gian
            if (timeLeft <= 0) {
                e.preventDefault();
                return;
            }

            e.preventDefault();
            const pastedData = e.clipboardData.getData('text');
            const numbers = pastedData.replace(/\D/g, '').slice(0, 6);

            numbers.split('').forEach((num, i) => {
                if (i < otpBoxes.length && !otpBoxes[i].disabled) {
                    otpBoxes[i].value = num;
                }
            });

            // Focus vào ô cuối cùng được điền
            const lastFilledIndex = Math.min(numbers.length - 1, otpBoxes.length - 1);
            if (lastFilledIndex >= 0 && !otpBoxes[lastFilledIndex].disabled) {
                otpBoxes[lastFilledIndex].focus();
            }

            updateOTPCode();
        });
    });

    // Cập nhật hidden input khi OTP thay đổi
    function updateOTPCode() {
        const otpCode = Array.from(otpBoxes).map(box => box.value).join('');
        const hiddenInput = document.getElementById('otpCode');
        if (hiddenInput) {
            hiddenInput.value = otpCode;
        }
    }

    // Kiểm tra nếu có thông báo lỗi từ server khi trang load
    const serverError = document.querySelector('.error-message');
    if (serverError && serverError.style.display !== 'none') {
        // Có lỗi từ server, thêm hiệu ứng shake
        setTimeout(() => {
            shakeOTPBoxes();
        }, 100);
    }

    function shakeOTPBoxes() {
        otpBoxes.forEach(box => {
            if (!box.disabled) {
                box.classList.add('error');
                setTimeout(() => {
                    box.classList.remove('error');
                }, 500);
            }
        });
    }

    // Reset countdown khi gửi lại mã
    if (resendForm) {
        resendForm.addEventListener('submit', function () {
            sessionStorage.removeItem(storageKey);
        });
    }
});
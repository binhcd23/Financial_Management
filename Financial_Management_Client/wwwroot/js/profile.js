$(document).ready(function () {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });

    if (window.flashMessages) {
        if (window.flashMessages.success && window.flashMessages.success !== '') {
            Toast.fire({
                icon: 'success',
                title: window.flashMessages.success
            });
        }

        if (window.flashMessages.error && window.flashMessages.error !== '') {
            Toast.fire({
                icon: 'error',
                title: window.flashMessages.error
            });
        }

        if (window.flashMessages.isPasswordModalInvalid === true) {
            const modalElem = document.getElementById('changePasswordModal');
            if (modalElem) {
                const myModal = new bootstrap.Modal(modalElem);
                myModal.show();
            }
        }
    }
});
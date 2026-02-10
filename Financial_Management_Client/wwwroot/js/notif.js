function showNotificationDetail(id) {
    console.log("Đang gọi ID:", id);
    fetch(`/Notification/GetDetail?id=${id}`)
        .then(response => {
            if (!response.ok) throw new Error("Không lấy được dữ liệu");
            return response.json();
        })
        .then(data => {
  
            document.getElementById('notifTitle').innerText = data.title;
            document.getElementById('notifMessage').innerText = data.message;
            document.getElementById('notifTime').innerText = data.timeAgo;

            const modalElement = document.getElementById('notifModal');
            const myModal = new bootstrap.Modal(modalElement);

            myModal.show();
            markAsRead(id);
        })
        .catch(error => console.error('Error:', error));
}

function markAsRead(id) {
    fetch(`/Notification/MarkAsRead?notificationId=${id}`, { method: 'POST' })
        .then(res => res.json())
        .then(data => console.log("Trạng thái update:", data.success));
}
function loadNotifications() {
    $.get('/Notification/GetLatestNotifications', function (data) {
        updateNotificationUI(data);
    });
}

let lastUnreadCount = 0;
let isFirstLoad = true;
function updateNotificationUI(notifications) {
    const container = $('#notificationList');
    const iconContainer = $('#notificationIconContainer');

    const unreadCount = (notifications || []).filter(n => n.isRead === false).length;

    if (unreadCount > lastUnreadCount) {

        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true
        });

        Toast.fire({
            icon: 'info',
            title: isFirstLoad ? 'Bạn có thông báo chưa đọc' : 'Bạn có thông báo mới!',
            text: isFirstLoad ? `${unreadCount} tin chưa xem.` : `Hệ thống vừa gửi cho bạn một thông báo mới.`
        });
    }

    lastUnreadCount = unreadCount;
    isFirstLoad = false;

    let badge = iconContainer.find('.badge');

    if (unreadCount > 0) {
        if (badge.length === 0) {
            iconContainer.append(`
                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" 
                      style="padding: 5px; 
                             min-width: 10px; 
                             min-height: 10px; 
                             border: 2px solid #fff; 
                             z-index: 1000; 
                             display: block !important;">
                </span>`);
        }
    } else {
        badge.remove();
    }

    let html = '';
    if (notifications && notifications.length > 0) {
        notifications.forEach(item => {
            const id = item.notificationId;
            const title = item.title;
            const type = item.type;
            const time = item.timeAgo;
            const isRead = item.isRead;

            const unreadStyle = (isRead === false) ? 'style="background-color: rgba(0,0,0,0.03); font-weight: bold;"' : '';

            html += `
                <li class="mb-2" ${unreadStyle}>
                    <a class="dropdown-item border-radius-md" href="javascript:;" onclick="showNotificationDetail(${id})">
                        <div class="d-flex py-1">
                            <div class="my-auto">
                                <i class="fa-solid ${type === 'Warning' ? 'fa-triangle-exclamation text-warning' : 'fa-envelope text-primary'} me-3"></i>
                            </div>
                            <div class="d-flex flex-column justify-content-center">
                                <h6 class="text-sm font-weight-normal mb-1">
                                    <span>${title}</span>
                                </h6>
                                <p class="text-xs text-secondary mb-0">
                                    <i class="fa fa-clock me-1"></i> ${time}
                                </p>
                            </div>
                        </div>
                    </a>
                </li>`;
        });
    } else {
        html = '<li class="text-center p-2"><p class="text-xs mb-0">Không có thông báo mới</p></li>';
    }
    container.html(html);
}

setInterval(loadNotifications, 60000);

$(document).ready(function () {
    loadNotifications();
});
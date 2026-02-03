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
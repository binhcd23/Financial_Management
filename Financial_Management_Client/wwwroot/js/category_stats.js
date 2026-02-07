document.addEventListener("DOMContentLoaded", function () {
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');

    if (startDateInput && endDateInput) {
        const updateMinDate = () => {
            const startDateValue = startDateInput.value;
            if (startDateValue) {
                // Tạo một đối tượng Date từ giá trị ngày bắt đầu
                let minEnd = new Date(startDateValue);
                // Cộng thêm 1 ngày
                minEnd.setDate(minEnd.getDate() + 1);

                // Chuyển định dạng về YYYY-MM-DD để gán vào thuộc tính min
                const minEndStr = minEnd.toISOString().split('T')[0];

                // Thiết lập: Ngày kết thúc phải lớn hơn ngày bắt đầu ít nhất 1 ngày
                endDateInput.setAttribute('min', minEndStr);

                // Nếu ngày kết thúc hiện tại nhỏ hơn hoặc BẰNG ngày bắt đầu, tự động nhảy lên minEndStr
                if (endDateInput.value && endDateInput.value <= startDateValue) {
                    endDateInput.value = minEndStr;
                }
            }
        };

        // Chạy lần đầu khi trang load (để giữ logic nếu đã có dữ liệu từ Query)
        updateMinDate();

        // Lắng nghe sự kiện thay đổi trên ô ngày bắt đầu
        startDateInput.addEventListener('change', updateMinDate);
    }

    if (!window.chartData) {
        console.error("Dữ liệu biểu đồ chưa được khởi tạo từ View!");
        return;
    }

    const data = window.chartData;

    // --- BIỂU ĐỒ CỘT (CHI TIÊU) ---
    const ctxBar = document.getElementById('expenseBarChart');
    if (ctxBar) {
        new Chart(ctxBar.getContext('2d'), {
            type: 'bar',
            data: {
                labels: data.categoryLabels,
                datasets: [{
                    label: 'Số tiền chi (VNĐ)',
                    data: data.categoryValues,
                    backgroundColor: '#e91e63',
                    borderRadius: 5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return value.toLocaleString() + ' đ';
                            }
                        }
                    }
                }
            }
        });
    }

    // --- BIỂU ĐỒ TRÒN (MỤC TIÊU) ---
    const ctxPie = document.getElementById('goalPieChart');
    if (ctxPie) {
        new Chart(ctxPie.getContext('2d'), {
            type: 'doughnut',
            data: {
                labels: data.goalLabels,
                datasets: [{
                    data: data.goalValues,
                    backgroundColor: [
                        '#4caf50', // Completed
                        '#fb8c00', // Active
                        '#f44336', // Cancelled
                        '#03a9f4'  // Default/Info
                    ],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            usePointStyle: true,
                            padding: 20,
                            font: { size: 12 }
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return ` ${context.label}: ${context.raw}%`;
                            }
                        }
                    }
                }
            }
        });
    }
});
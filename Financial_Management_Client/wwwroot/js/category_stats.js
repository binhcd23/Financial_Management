var ctx = document.getElementById("expenseBarChart").getContext("2d");

// Dữ liệu từ Model C# truyền sang JS
var categoryLabels = @Html.Raw(Json.Serialize(Model.CategoryStats.Select(x => x.Name)));
var categoryData = @Html.Raw(Json.Serialize(Model.CategoryStats.Select(x => x.Amount)));

new Chart(ctx, {
    type: "bar",
    data: {
        labels: categoryLabels,
        datasets: [{
            label: "Số tiền đã chi",
            tension: 0.4,
            borderWidth: 0,
            borderRadius: 4,
            borderSkipped: false,
            backgroundColor: "#e91e63",
            data: categoryData,
            maxBarLength: 50
        }],
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { display: false }
        },
        scales: {
            y: {
                grid: { drawBorder: false, display: true, drawOnChartArea: true, drawTicks: false, borderDash: [5, 5] },
                ticks: { display: true, padding: 10, color: '#b2b9bf', font: { size: 11 } }
            },
            x: {
                grid: { drawBorder: false, display: false, drawOnChartArea: false, drawTicks: false },
                ticks: { display: true, color: '#b2b9bf', padding: 10, font: { size: 11 } }
            }
        }
    }
});
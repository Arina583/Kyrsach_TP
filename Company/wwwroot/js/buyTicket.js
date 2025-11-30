document.addEventListener('DOMContentLoaded', function () {
    let seats = document.querySelectorAll('.seat');
    let seatNumberInput = document.getElementById('seatNumber');

    seats.forEach(seat => {
        seat.addEventListener('click', function () {
            // Снять выделение со всех мест
            seats.forEach(s => s.classList.remove('selected'));
            // Выделить текущее место
            this.classList.add('selected');
            // Обновить значение скрытого поля
            seatNumberInput.value = this.dataset.seatnumber;
        });
    });
});
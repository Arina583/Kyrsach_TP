document.addEventListener('DOMContentLoaded', function () {
    let selectedSeat = null;

    const seats = document.querySelectorAll('.seat');
    seats.forEach(seat => {
        seat.addEventListener('click', function () {
            seats.forEach(s => s.classList.remove('selected'));
            this.classList.add('selected');
            selectedSeat = this.dataset.seatnumber;
            document.getElementById('seatNumber').value = selectedSeat;
        });
    });
    const reserveButton = document.getElementById('reserveButton');
    const emailModal = document.getElementById('emailModal');
    const modalOverlay = document.getElementById('modalOverlay');
    const confirmReserve = document.getElementById('confirmReserve');
    const cancelReserve = document.getElementById('cancelReserve');
    const emailInput = document.getElementById('emailInput');

    reserveButton.addEventListener('click', function () {
        if (selectedSeat) {
            emailModal.style.display = 'block';
            modalOverlay.style.display = 'block';
        } else {
            alert('Пожалуйста, выберите место.');
        }
    });

    cancelReserve.addEventListener('click', function () {
        emailModal.style.display = 'none';
        modalOverlay.style.display = 'none';
    });

    confirmReserve.addEventListener('click', function () {
        const email = emailInput.value;
        if (email) {
            document.getElementById('email').value = email;
            document.getElementById('actionType').value = 'reserve';
            document.getElementById('reservationForm').submit();
        } else {
            alert('Пожалуйста, введите email.');
        }
    });

    // Добавляем класс 'reserved' для забронированных мест
    const reservedSeats = document.querySelectorAll('.seat.reserved');
    reservedSeats.forEach(seat => {
        seat.classList.add('disabled'); // Добавляем класс disabled
    });
    seats.forEach(seat => {
        seat.addEventListener('click', function () {
            if (!this.classList.contains('disabled')) {
                seats.forEach(s => s.classList.remove('selected'));
                this.classList.add('selected');
                selectedSeat = this.dataset.seatnumber;
                document.getElementById('seatNumber').value = selectedSeat;
            }
        });
});
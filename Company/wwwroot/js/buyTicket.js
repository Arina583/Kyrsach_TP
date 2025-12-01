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

document.addEventListener('DOMContentLoaded', function () {
    let selectedSeat = null;
    const seats = document.querySelectorAll('.seat');
    const seatNumberInput = document.getElementById('seatNumber');
    const reserveBtn = document.querySelector('button[value="reserve"]');
    const buyBtn = document.querySelector('button[value="buy"]');
    const emailModal = document.getElementById('emailModal');
    const modalOverlay = document.getElementById('modalOverlay');
    const emailInput = document.getElementById('emailInput');
    const confirmReserve = document.getElementById('confirmReserve');
    const cancelReserve = document.getElementById('cancelReserve');

    // Клик по месту — выбор
    seats.forEach(s => {
        s.onclick = () => {
            if (s.classList.contains('reserved')) {
                alert('Это место уже забронировано');
                return;
            }
            seats.forEach(seat => seat.classList.remove('selected'));
            s.classList.add('selected');
            selectedSeat = s.getAttribute('data-seatnumber');
            seatNumberInput.value = selectedSeat;
        };
    });

    // Кнопка "Забронировать" — открываем окно ввода почты
    reserveBtn.onclick = function (e) {
        e.preventDefault();
        if (!selectedSeat) {
            alert('Пожалуйста, выберите место для бронирования');
            return;
        }
        emailInput.value = '';
        emailModal.style.display = 'block';
        modalOverlay.style.display = 'block';
    };

    // Подтверждение в модальном окне
    confirmReserve.onclick = function () {
        if (!emailInput.checkValidity()) {
            alert('Введите корректный email');
            return;
        }
        // Отправляем форму с бронированием через fetch или можно через форму с hidden полями
        // Здесь сделаем простой POST с fetch
        fetch('/Flight/ReserveSeat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ flightId: '@Model.Id', seatNumber: selectedSeat, email: emailInput.value })
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    alert('Место забронировано! На вашу почту отправлены реквизиты для оплаты.');
                    // Помечаем место забронированным визуально:
                    let seatDiv = document.querySelector(`.seat[data-seatnumber="${selectedSeat}"]`);
                    seatDiv.classList.remove('selected');
                    seatDiv.classList.add('reserved');
                    seatDiv.onclick = null; // блокируем выбор
                    seatNumberInput.value = '';
                    selectedSeat = null;
                    emailModal.style.display = 'none';
                    modalOverlay.style.display = 'none';

                    // Добавим ссылку на оплату (можно открыть в новом окне)
                    if (data.paymentUrl) {
                        if (confirm('Перейти к оплате?')) {
                            window.open(data.paymentUrl, '_blank');
                        }
                    }
                } else {
                    alert('Ошибка бронирования: ' + data.message);
                }
            })
            .catch(() => alert('Ошибка связи с сервером'));
    };

    cancelReserve.onclick = function () {
        emailModal.style.display = 'none';
        modalOverlay.style.display = 'none';
    };
});
// جلب العناصر من الصفحة
const popup = document.getElementById("popup");
const bookingButton = document.querySelector(".booking-button");

// عند الضغط على زر الحجز
bookingButton.addEventListener("click", () => {
    popup.style.display = "block";
});

// إغلاق النافذة المنبثقة
function closePopup() {
    popup.style.display = "none";
}

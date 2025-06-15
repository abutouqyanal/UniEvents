// جلب جميع النجوم
const stars = document.querySelectorAll('.star');
let selectedRating = 0;

// عند المرور بالماوس (Hover) على نجمة
stars.forEach((star, index) => {
    star.addEventListener('mouseover', () => {
        resetStars();
        for (let i = 0; i <= index; i++) {
            stars[i].classList.add('hovered');
        }
    });

    // عند مغادرة النجوم
    star.addEventListener('mouseout', () => {
        resetStars();
        updateStars(selectedRating);
    });

    // عند الضغط على نجمة (اختيار التقييم)
    star.addEventListener('click', () => {
        selectedRating = index + 1;
        updateStars(selectedRating);
        console.log(`تم اختيار ${selectedRating} نجوم`);
    });
});

// دالة لإعادة تعيين النجوم
function resetStars() {
    stars.forEach(star => {
        star.classList.remove('selected');
        star.classList.remove('hovered');
    });
}

// دالة لتحديث النجوم حسب التقييم
function updateStars(rating) {
    for (let i = 0; i < rating; i++) {
        stars[i].classList.add('selected');
    }
}
